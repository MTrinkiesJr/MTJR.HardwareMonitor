using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Extensions;
using MTJR.HardwareMonitor.Model;
using Newtonsoft.Json;
using RestSharp;
using ValueType = MTJR.HardwareMonitor.Model.ValueType;

// ReSharper disable InconsistentNaming

namespace MTJR.HardwareMonitor.Services
{
    /// <summary>
    /// Defines a type to send data to Iobroker
    /// </summary>
    public enum StateType
    {
        /// <summary>
        /// Clocks of the CPU
        /// </summary>
        CPU_Clock,
        /// <summary>
        /// Powers of the CPU
        /// </summary>
        CPU_Power,
        /// <summary>
        /// Loads of the CPU
        /// </summary>
        CPU_Load,
        /// <summary>
        /// Temperatures of the CPU
        /// </summary>
        CPU_Temperature,
        /// <summary>
        /// Clocks of the GPU
        /// </summary>
        GPU_Clock,
        /// <summary>
        /// Powers of the GPU
        /// </summary>
        GPU_Power,
        /// <summary>
        /// Loads of the GPU
        /// </summary>
        GPU_Load,
        /// <summary>
        /// Temperatures of the GPU
        /// </summary>
        GPU_Temperature,
        /// <summary>
        /// Load of the memory
        /// </summary>
        RAM_Load,
        /// <summary>
        /// Data of the memory
        /// </summary>
        RAM_Power,
        /// <summary>
        /// Temperatures of the HDDs
        /// </summary>
        HDD_Temperature,
        /// <summary>
        /// Loads of of the HDDs
        /// May not be available for all
        /// </summary>
        HDD_Load,
        /// <summary>
        /// Data of the HDDs
        /// May not be available for all
        /// </summary>
        HDD_Data
    }


    /// <summary>
    /// Service to provide function to send data to IoBroker
    /// </summary>
    public class IoBrokerApiService
    {
        private readonly ConfigurationService _configurationService;
        /// <summary>
        /// Constructor to create fulfilled <see cref="IoBrokerApiService"/>
        /// </summary>
        /// <param name="configurationService"><see cref="ConfigurationService"/> to get the current <see cref="GuiConfiguration"/></param>
        public IoBrokerApiService(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Update all states for a <see cref="Server"/>
        /// </summary>
        /// <param name="serverService">The <see cref="ServerMonitoringService"/> to update the states for</param>
        /// <param name="info">The current <see cref="HardwareInfo"/> of the <see cref="Server"/></param>
        /// <returns></returns>
        public async Task UpdateStatesAsync(ServerMonitoringService serverService, HardwareInfo info)
        {
            var itemId = serverService.Server.Id;
            var itemName = serverService.Server.Name;

            await CreateObjectAsync($"{itemId}.Online", "Online State", "state", "string");
            await UpdateStateAsync($"0_userdata.0.HardwareMonitor.{itemId}.Online", serverService.State.ToString());

            await CreateObjectAsync($"{itemId}.OHMOnline", "Open Hardware Monitor Online State", "state", "string");
            await UpdateStateAsync($"0_userdata.0.HardwareMonitor.{itemId}.OHMOnline",
                serverService.OhmState.ToString());

            var states = _configurationService.GuiConfiguration.IoBrokerStates.Where(a => a.Enabled);

            foreach (var state in states)
            {
                List<HardwareInfo> usedHardwareInfos = new List<HardwareInfo>();
                switch (state.StateType)
                {
                    case StateType.CPU_Clock:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Cpu, ValueType.Clock);
                        break;
                    case StateType.CPU_Power:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Cpu, ValueType.Power);
                        break;
                    case StateType.CPU_Load:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Cpu, ValueType.Load);
                        break;
                    case StateType.CPU_Temperature:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Cpu, ValueType.Temperature);
                        break;
                    case StateType.RAM_Load:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Ram, ValueType.Load);
                        break;
                    case StateType.RAM_Power:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Ram, ValueType.Power);
                        break;
                }

                if (usedHardwareInfos?.Any() == true)
                {
                    await AddStatesAsync(itemId, itemName, usedHardwareInfos, state.StateType);
                }

                await Task.Run(async () => await UpdateGpuInfos(itemId, info));
                await Task.Run(async () => await UpdateHddInfos(itemId, info));
            }
        }

        private async Task UpdateGpuInfos(string itemId, HardwareInfo info)
        {
            var states = _configurationService.GuiConfiguration.IoBrokerStates.Where(a => a.Enabled);

            var nvidiaInfos = info.GetChildrenFlat().FirstOrDefault(a => a.HardwareType == HardwareType.Gpu && a.SubHardwareType == HardwareType.Nvidia);
            var amdInfos = info.GetChildrenFlat().FirstOrDefault(a => a.HardwareType == HardwareType.Gpu && a.SubHardwareType == HardwareType.Amd);

            if (states.Any(a => a.StateType == StateType.GPU_Clock))
            {
                await UpdateGpuInfo(itemId, nvidiaInfos, amdInfos, ValueType.Clock);
            }

            if (states.Any(a => a.StateType == StateType.GPU_Load))
            {
                await UpdateGpuInfo(itemId, nvidiaInfos, amdInfos, ValueType.Load);
            }

            if (states.Any(a => a.StateType == StateType.GPU_Power))
            {
                await UpdateGpuInfo(itemId, nvidiaInfos, amdInfos, ValueType.Power);
            }

            if (states.Any(a => a.StateType == StateType.GPU_Temperature))
            {
                await UpdateGpuInfo(itemId, nvidiaInfos, amdInfos, ValueType.Temperature);
            }
        }

        private async Task UpdateGpuInfo(string itemId, HardwareInfo nvidiaInfos, HardwareInfo amdInfos, ValueType valueType)
        {
            List<HardwareInfo> usedInfos = new List<HardwareInfo>();
            string gpuName = "";
            string gpuType = "";

            if (nvidiaInfos != null)
            {
                usedInfos = nvidiaInfos.GetChildrenFlat().Where(a =>
                        a.HardwareType == HardwareType.Gpu && a.ValueType == valueType &&
                        !string.IsNullOrEmpty(a.Value))
                    .ToList();

                gpuName = nvidiaInfos.Text;
                gpuType = "NVIDIA";
                await CreateObjectAsync($"{itemId}.GPU.{gpuType}", gpuName, "channel");

                foreach (var usedInfo in usedInfos)
                {
                    await CreateObjectAsync($"{itemId}.GPU.{gpuType}.{valueType.ToString()}.{usedInfo.Text}", usedInfo.CompleteName, "state", "number", usedInfo.ValueUnit);
                    await UpdateStateAsync($"0_userdata.0.HardwareMonitor.{itemId}.GPU.{gpuType}.{valueType.ToString()}.{usedInfo.Text}", usedInfo.ParsedValue);
                }
            }

            if (amdInfos != null)
            {
                usedInfos = amdInfos.GetChildrenFlat().Where(a =>
                        a.HardwareType == HardwareType.Gpu && a.ValueType == valueType &&
                        !string.IsNullOrEmpty(a.Value))
                    .ToList();
                gpuName = amdInfos.Text;
                gpuType = "AMD";
                await CreateObjectAsync($"{itemId}.GPU.{gpuType}", gpuName, "channel");

                foreach (var usedInfo in usedInfos)
                {
                    await CreateObjectAsync($"{itemId}.GPU.{gpuType}.{valueType.ToString()}.{usedInfo.Text}", usedInfo.CompleteName, "state", "number", usedInfo.ValueUnit);
                    await UpdateStateAsync($"0_userdata.0.HardwareMonitor.{itemId}.GPU.{gpuType}.{valueType.ToString()}.{usedInfo.Text}", usedInfo.ParsedValue);
                }
            }


        }

        private async Task UpdateHddInfos(string itemId, HardwareInfo info)
        {
            var states = _configurationService.GuiConfiguration.IoBrokerStates.Where(a => a.Enabled);
            //custom handling for HDD infos, because they may appear multiple times, so we creating a json list for this HardwareType

            var hddInfos = new List<HardwareInfo>();

            if (states.Any(a => a.StateType == StateType.HDD_Temperature))
            {
                hddInfos.AddRange(info.FindInfosByNestedType(HardwareType.Hdd, ValueType.Temperature));
            }

            if (states.Any(a => a.StateType == StateType.HDD_Load))
            {
                hddInfos.AddRange(info.FindInfosByNestedType(HardwareType.Hdd, ValueType.Load));
            }

            if (states.Any(a => a.StateType == StateType.HDD_Data))
            {
                hddInfos.AddRange(info.FindInfosByNestedType(HardwareType.Hdd, ValueType.Power));
            }

            var hdds = hddInfos.GroupBy(a => a.Parent.Parent.Text);


            var enumerable = hdds.ToList();

            for (var i = 0; i < enumerable.Count(); i++)
            {
                var hdd = hdds.ElementAt(i);

                await CreateObjectAsync($"{itemId}.HDD.HDD {i}", hdd.Key, "channel");
                var existingLoad = hdd.FirstOrDefault(a => a.ValueType == ValueType.Load);

                if (existingLoad != null)
                {
                    await CreateObjectAsync($"{itemId}.HDD.HDD {i}.Load", existingLoad.CompleteName, "state", "number", "%");
                    await UpdateStateAsync($"0_userdata.0.HardwareMonitor.{itemId}.HDD.HDD {i}.Load", existingLoad.ParsedValue);
                }

                var exisitingData = hdd.FirstOrDefault(a => a.ValueType == ValueType.Power);

                if (exisitingData != null)
                {
                    await CreateObjectAsync($"{itemId}.HDD.HDD {i}.Data", exisitingData.CompleteName, "state", "number", "MB");
                    await UpdateStateAsync($"0_userdata.0.HardwareMonitor.{itemId}.HDD.HDD {i}.Data", exisitingData.ParsedValue);
                }

                var existingTemperature = hdd.FirstOrDefault(a => a.ValueType == ValueType.Temperature);

                if (existingTemperature != null)
                {
                    await CreateObjectAsync($"{itemId}.HDD.HDD {i}.Temperature", existingTemperature.CompleteName, "state", "number", "°C");
                    await UpdateStateAsync($"0_userdata.0.HardwareMonitor.{itemId}.HDD.HDD {i}.Temperature", existingTemperature.ParsedValue);
                }
            }

        }

        /// <summary>
        /// Ensures the base path of an <see cref="Server"/>
        /// all server instances will be created at "0_userdata.0.HardwareMonitor"
        /// </summary>
        /// <param name="itemId">The id of the <see cref="Server"/></param>
        /// <param name="itemName">The name of the <see cref="Server"/></param>
        /// <param name="objectType">The type of the added object</param>
        /// <param name="valueType">The type of the state value</param>
        /// <param name="unit">The optional unit of the state</param>
        /// <returns></returns>
        private async Task CreateObjectAsync(string itemId, string itemName, string objectType, string valueType = "", string unit = "")
        {
            var query = $"/v1/object/0_userdata.0.HardwareMonitor.{itemId}";
            var restClient = new RestClient($"http://{_configurationService.GuiConfiguration.IoBrokerHostname}:{_configurationService.GuiConfiguration.IoBrokerPort}");
            var state = new IoBrokerState()
            {
                Id = $"0_userdata.0.HardwareMonitor.{itemId}",
                Type = objectType,
                Common = new IoBrokerStateCommon()
                {
                    Name = itemName,
                    Type = valueType,
                    Unit = unit
                }
            };
            var restRequest = new RestRequest(query);
            restRequest.AddJsonBody(state);
            restRequest.Timeout = 10000;
            await restClient.ExecuteAsync(restRequest, Method.Post);
        }

        /// <summary>
        /// Add states for given server
        /// </summary>
        /// <param name="serverId">The database id of the <see cref="Server"/></param>
        /// <param name="serverName">The <see cref="Server"/> name</param>
        /// <param name="infos">The resolved <inheritdoc cref="HardwareInfo"/> list of given <see cref="StateType"/></param>
        /// <param name="stateType">The <see cref="StateType"/> for resolved <see cref="HardwareInfo"/>'s</param>
        /// <returns></returns>
        private async Task AddStatesAsync(string serverId, string serverName, List<HardwareInfo> infos, StateType stateType)
        {
            await CreateObjectAsync(serverId, serverName, "device");

            var deviceTypeName = stateType.ToString().Split('_')[0];
            var deviceInfoName = stateType.ToString().Split('_')[1];
            var query = $"/v1/object/0_userdata.0.HardwareMonitor.{serverId}.{deviceTypeName}.{deviceInfoName}";
            var restClient = new RestClient($"http://{_configurationService.GuiConfiguration.IoBrokerHostname}:{_configurationService.GuiConfiguration.IoBrokerPort}");

            //Name of parent would be 'Clock', 'Temperature' etc., cause of this we're using 'Parent.Parent' here!
            var deviceName = infos.FirstOrDefault().Parent.Parent.ResolvedText;

            await CreateObjectAsync($"{serverId}.{deviceTypeName}", deviceName, "channel");

            foreach (var hardwareInfo in infos)
            {
                var stateId = $"0_userdata.0.HardwareMonitor.{serverId}.{deviceTypeName}.{deviceInfoName}.{hardwareInfo.ResolvedText}";
                var state = new IoBrokerState()
                {
                    Id = stateId,
                    Type = "state",
                    Common = new IoBrokerStateCommon()
                    {
                        Name = hardwareInfo.CompleteName,
                        Type = "number",
                        Unit = hardwareInfo.ValueUnit,
                        Min = 0,
                        Max = 9999,
                        Role = "value"
                    }
                };

                var stateQuery = query + $".{hardwareInfo.ResolvedText}";
                var restRequest = new RestRequest(stateQuery);
                restRequest.AddJsonBody(state);
                restRequest.Timeout = 10000;
                await restClient.ExecuteAsync(restRequest, Method.Post);

                await UpdateStateAsync(stateId, hardwareInfo.ParsedValue);
            }


        }

        /// <summary>
        /// Patches the device for all server and states to Jarvis
        /// </summary>
        /// <param name="serverList">List of all servers</param>
        /// <returns></returns>
        public async Task<bool> ImportJarvisDevices(List<ServerMonitoringService> serverList)
        {
            var restClient =
                new RestClient(
                    $"http://{_configurationService.GuiConfiguration.IoBrokerHostname}:{_configurationService.GuiConfiguration.IoBrokerPort}");

            var request = new RestRequest("/v1/state/jarvis.0.devices?withInfo=false");

            var response = await restClient.ExecuteAsync(request, Method.Get);

            JarvisDevices devices = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var state = JsonConvert.DeserializeObject<IoBrokerStateValue>(response.Content);
                devices = JsonConvert.DeserializeObject<JarvisDevices>(state.Val.ToString());
            }
            else
            {
                return false;
            }

            foreach (var server in serverList)
            {
                if (devices.Devices.Any(a => a.Key.ToLower() == $"hardwaremonitor_{server.Server.Id}"))
                {
                    devices.Devices.Remove($"hardwaremonitor_{server.Server.Id}");
                }

                var device = new JarvisDevice()
                {
                    Name = server.Server.Name,
                    Function = "server",
                    Label = server.Server.Name,
                };

                var states = new Dictionary<string, JarvisDeviceState>();


                var stateTypes = Enum.GetValues(typeof(StateType)).Cast<StateType>().Where(a => !a.ToString().StartsWith("HDD") && !a.ToString().StartsWith("GPU"));
                foreach (var stateType in stateTypes)
                {
                    var hardwareType = (HardwareType)Enum.Parse(typeof(HardwareType), stateType.ToString().Split("_")[0], true);
                    var valueType = (ValueType)Enum.Parse(typeof(ValueType), stateType.ToString().Split("_")[1], true);

                    var infos = server.HardwareInfo.FindInfosByNestedType(hardwareType, valueType);

                    foreach (var hardwareInfo in infos)
                    {
                        states.Add($"{hardwareType}.{valueType}.{hardwareInfo.ResolvedText}", new JarvisDeviceState()
                        {
                            State = $"0_userdata.0.HardwareMonitor.{server.Server.Id}.{hardwareType.ToString().ToUpper()}.{valueType}.{hardwareInfo.ResolvedText}",
                            StateKey = $"0_userdata.0.HardwareMonitor.{server.Server.Id}.{hardwareType.ToString().ToUpper()}.{valueType}.{hardwareInfo.ResolvedText}",
                            Label = hardwareInfo.ResolvedText,
                            Unit = hardwareInfo.ValueUnit
                        });
                    }
                }

                device.States = states;
            }

            return true;
        }


        /// <summary>
        /// Updates value of a <see cref="IoBrokerState"/>
        /// </summary>
        /// <param name="stateId">The id of the <see cref="IoBrokerState"/></param>
        /// <param name="value">The value to be set</param>
        /// <returns><see cref="Task"/></returns>
        private async Task UpdateStateAsync(string stateId, object value)
        {
            var restClient =
                new RestClient(
                    $"http://{_configurationService.GuiConfiguration.IoBrokerHostname}:{_configurationService.GuiConfiguration.IoBrokerPort}");

            var restRequest = new RestRequest($"/v1/state/{stateId}");
            restRequest.AddJsonBody(new IoBrokerStateValue()
            {
                Ack = true,
                Val = value
            });
            restRequest.Timeout = 10000;
            await restClient.ExecuteAsync(restRequest, Method.Patch);

        }
    }
}
