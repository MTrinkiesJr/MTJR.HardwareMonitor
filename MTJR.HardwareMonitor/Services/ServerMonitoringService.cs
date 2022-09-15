using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Data;
using MTJR.HardwareMonitor.Model;
using Newtonsoft.Json;
using RestSharp;
using ValueType = MTJR.HardwareMonitor.Model.ValueType;

namespace MTJR.HardwareMonitor.Services
{
    /// <summary>
    /// Available state for the <see cref="Server"/> and Open Hardware Monitor
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The server or Open Hardware monitor is unreachable
        /// </summary>
        Offline,
        /// <summary>
        /// The server or Open Hardware monitor is reachable
        /// </summary>
        Online
    }


    /// <summary>
    /// Service to monitor Open Hardware Monitor
    /// </summary>
    public class ServerMonitoringService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly ConfigurationService _configurationService;
        private readonly IoBrokerApiService _ioBrokerApi;
        private bool _infosLoading;

        /// <summary>
        /// The <see cref="Server"/> reference from <see cref="DataContext"/>
        /// </summary>
        public Server Server { get; set; }
        /// <summary>
        /// Timer to periodically fetch data from Open Hardware Monitor
        /// </summary>
        public Timer Timer { get; private set; }
        /// <summary>
        /// The current state of the server itself
        /// </summary>
        public State State { get; set; }
        /// <summary>
        /// The current state of Open Hardware Monitor connectivity
        /// </summary>
        public State OhmState { get; set; }
        /// <summary>
        /// The current fetched <see cref="HardwareInfo"/> from Open Hardware Monitor
        /// </summary>
        public HardwareInfo HardwareInfo { get; set; }

        /// <summary>
        /// Current resolved cpu temperature with unit
        /// </summary>
        public string CpuTemp { get; set; }

        /// <summary>
        /// Current resolved cpu load with unit
        /// </summary>
        public string CpuLoad { get; set; }

        /// <summary>
        /// Current resolved gpu temperature with unit
        /// </summary>
        public string GpuTemp { get; set; }

        /// <summary>
        /// Current resolved gpu load with unit
        /// </summary>
        public string GpuLoad { get; set; }

        /// <summary>
        /// Constructor to create a new <see cref="Server"/>
        /// </summary>
        public ServerMonitoringService()
        {
            Server = new Server()
            {
                Id = Guid.NewGuid().ToString(),
                Hostname = "192.168.178.xx",
                Port = 8085,
                Interval = 10000
            };
        }

        /// <summary>
        /// Constructor to create fulfilled <see cref="ServerMonitoringService"/> from existing <see cref="Server"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="scopeFactory"></param>
        public ServerMonitoringService(string id, IServiceProvider serviceProvider, IServiceScopeFactory scopeFactory)
        {
            var serviceProvider1 = serviceProvider;
            _scopeFactory = scopeFactory;
            _hubContext = serviceProvider1.GetService<IHubContext<EventHub>>();
            _configurationService = serviceProvider1.GetRequiredService<ConfigurationService>();
            _ioBrokerApi = serviceProvider1.GetRequiredService<IoBrokerApiService>();

            Task.Run(async () => await Init(id));
        }

        /// <summary>
        /// Initialized the <see cref="ServerMonitoringService"/> from existing <see cref="Server"/>
        /// </summary>
        /// <param name="id">The id of the server</param>
        /// <returns></returns>
        private async Task Init(string id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                Server = await dataContext.Servers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            }

            Timer = new Timer(Server.Interval);
            Timer.Elapsed += TimerOnElapsed;
            Timer.Start();
            await GetInfoAsync();
        }

        /// <summary>
        /// Elapsed every Interval
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Arguments passed to the event</param>
        private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (!_infosLoading)
            {
                await GetInfoAsync();
            }
        }


        /// <summary>
        /// Fetches the current <see cref="HardwareInfo"/> from Open Hardware Monitor
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task GetInfoAsync()
        {
            if (!_infosLoading)
            {
                _infosLoading = true;
                if (!await CheckStatusAsync())
                {
                    State = State.Offline;
                    Server.LastFailure = DateTime.Now;

                    await _hubContext.Clients.All.SendCoreAsync("state",
                        new object[] { new { id = Server.Id, state = State.ToString(), name = Server.Name } });

                    await Task.Run(async () => await UpdateDatabaseAsync());
                    return;
                }

                State = State.Online;


                await _hubContext.Clients.All.SendCoreAsync("state",
                    new object[] { new { id = Server.Id, state = State.ToString(), name = Server.Name } });
                var client = new RestClient($"http://{Server.Hostname}:{Server.Port}");

                var request = new RestRequest("/data.json");
                request.Timeout = 5000;

                var response = await client.ExecuteAsync(request, Method.Get);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Server.LastSuccess = DateTime.Now;
                    HardwareInfo = null;
                    if (response.Content != null)
                    {
                        HardwareInfo = JsonConvert.DeserializeObject<HardwareInfo>(response.Content);
                    }

                    HardwareInfo?.ResolveParents(Server.Id);
                    OhmState = State.Online;

                    await _hubContext.Clients.All.SendCoreAsync("ohmstate",
                        new object[] { new { id = Server.Id, state = OhmState.ToString(), name = Server.Name } });

                    CpuTemp = await SendEventForHardware("cpu_temp", HardwareType.Cpu, ValueType.Temperature);
                    CpuLoad = await SendEventForHardware("cpu_load", HardwareType.Cpu, ValueType.Load);
                    GpuTemp = await SendEventForHardware("gpu_temp", HardwareType.Gpu, ValueType.Temperature);
                    GpuLoad = await SendEventForHardware("gpu_load", HardwareType.Gpu, ValueType.Load);
                }
                else
                {
                    Server.LastFailure = DateTime.Now;
                    OhmState = State.Offline;

                    await _hubContext.Clients.All.SendCoreAsync("ohmstate",
                        new object[] { new { id = Server.Id, state = OhmState.ToString(), name = Server.Name } });
                }

                await Task.Run(async () => await UpdateDatabaseAsync());

                if (_configurationService.GuiConfiguration.UseIoBroker)
                {
                    await Task.Run(async () => await _ioBrokerApi.UpdateStatesAsync(this, HardwareInfo));
                }

                _infosLoading = false;
            }
        }

        /// <summary>
        /// Send event for current <see cref="CpuLoad"/>, <see cref="CpuTemp"/>, <see cref="GpuLoad"/> and <see cref="GpuTemp"/>
        /// </summary>
        /// <param name="eventName">The name of the event to send</param>
        /// <param name="hardwareType">The <see cref="HardwareType"/> to find the correct <see cref="HardwareInfo"/></param>
        /// <param name="valueType">The <see cref="ValueType"/> to find the correct <see cref="HardwareInfo"/></param>
        /// <returns><see cref="Task{String}"/></returns>
        private async Task<string> SendEventForHardware(string eventName, HardwareType hardwareType, ValueType valueType)
        {
            var hardwareInfo = HardwareInfo.FindInfoByNestedType(hardwareType, valueType);

            if (hardwareInfo != null)
            {
                await _hubContext.Clients.All.SendCoreAsync(eventName, new object[] { new { id = Server.Id, value = hardwareInfo.Value, parsedValue = hardwareInfo.ParsedValue, unit = hardwareInfo.ValueUnit, name = Server.Name } });
                return hardwareInfo.Value;
            }

            return null;
        }


        /// <summary>
        /// Updates the database entry for current <see cref="Server"/>
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task UpdateDatabaseAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var server = await dataContext.Servers.FirstOrDefaultAsync(a => a.Id == Server.Id);

                    server.LastFailure = Server.LastFailure;
                    server.LastSuccess = Server.LastSuccess;

                    await dataContext.SaveChangesAsync();

                    await _hubContext.Clients.All.SendCoreAsync("updated", new object[] { Server });
                }
                catch (Exception)
                {
                    //ignored
                }
            }
        }


        /// <summary>
        /// Send a ping to the server to check the availability
        /// </summary>
        /// <returns><see cref="Task{Bool}"/></returns>
        private Task<bool> CheckStatusAsync()
        {
            bool retVal = false;
            try
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                // Use the default Ttl value which is 128,
                // but change the fragmentation behavior.
                options.DontFragment = true;
                // Create a buffer of 32 bytes of data to be transmitted.
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 60;

                PingReply reply = pingSender.Send(Server.Hostname, timeout, buffer, options);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                Console.WriteLine(ex.Message);
            }
            return Task.FromResult(retVal);
        }

        /// <summary>
        /// Update the current <see cref="Server"/> from given <see cref="ServerConfiguration"/>
        /// </summary>
        /// <param name="request">The <see cref="ServerConfiguration"/></param>
        /// <returns><see cref="Task{Bool}"/></returns>
        public async Task<bool> UpdateAsync(ServerConfiguration request)
        {
            Timer.Stop();
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                    var server = await dataContext.Servers.FirstOrDefaultAsync(a => a.Id == Server.Id);

                    server.Name = request.Name;
                    server.Hostname = request.Hostname;
                    server.Interval = request.Interval;
                    server.Port = request.Port;

                    dataContext.Servers.Update(server);
                    await dataContext.SaveChangesAsync();

                    Server = server;

                    Timer.Interval = request.Interval;

                    await _hubContext.Clients.All.SendCoreAsync("updated", new object[] { Server });
                }
                catch (Exception)
                {
                    return false;
                }

            }
            Timer.Start();

            return true;
        }
    }
}
