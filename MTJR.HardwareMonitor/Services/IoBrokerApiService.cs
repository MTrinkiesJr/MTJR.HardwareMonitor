using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTJR.HardwareMonitor.Configuration;
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
        /// Datas of the GPU
        /// </summary>
        GPU_Data,
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
        RAM_Data,
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
        /// <param name="itemId">The server id</param>
        /// <param name="itemName">The server name</param>
        /// <param name="info">The current <see cref="HardwareInfo"/> of the <see cref="Server"/></param>
        /// <returns></returns>
        public async Task UpdateStatesAsync(string itemId, string itemName, HardwareInfo info)
        {
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
                    case StateType.GPU_Clock:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Nvidia, ValueType.Clock);
                        break;
                    case StateType.GPU_Power:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Nvidia, ValueType.Power);
                        break;
                    case StateType.GPU_Load:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Nvidia, ValueType.Load);
                        break;
                    case StateType.GPU_Data:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Nvidia, ValueType.Power);
                        break;
                    case StateType.GPU_Temperature:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Nvidia, ValueType.Temperature);
                        break;
                    case StateType.RAM_Load:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Ram, ValueType.Load);
                        break;
                    case StateType.RAM_Data:
                        usedHardwareInfos = info.FindInfosByNestedType(HardwareType.Ram, ValueType.Power);
                        break;
                }

                if (usedHardwareInfos?.Any() == true)
                {
                    await AddStatesAsync(itemId, itemName, usedHardwareInfos, state.StateType);
                }
            }

            //custom handling for HDD infos, because they may appear multiple times, so we creating a json list for this HardwareType

            var hddInfos = new List<HardwareInfo>();
            hddInfos.AddRange(info.FindInfosByNestedType(HardwareType.Hdd, ValueType.Temperature));
            hddInfos.AddRange(info.FindInfosByNestedType(HardwareType.Hdd, ValueType.Load));
            hddInfos.AddRange(info.FindInfosByNestedType(HardwareType.Hdd, ValueType.Power));

            var hdds = hddInfos.GroupBy(a => a.Parent.Parent.Text);

            await CreateObjectAsync($"{itemId}.HDDs", "Json Table for all HDDs" , "state", "json");

            var jsonList = new List<object>();

            foreach (var hddInfo in hdds)
            {
                jsonList.Add(new 
                {
                    name = hddInfo.Key,
                    temperature = hddInfo.FirstOrDefault(a=>a.ValueType == ValueType.Temperature)?.Value ?? "",
                    load = hddInfo.FirstOrDefault(a=>a.ValueType == ValueType.Load)?.Value ?? "",
                    power = hddInfo.FirstOrDefault(a=>a.ValueType == ValueType.Load)?.Value ?? ""
                });
            }

            await UpdateStateAsync($"0_userdata.0.HardwareMonitor.{itemId}.HDDs", JsonConvert.SerializeObject(jsonList));
        }

        /// <summary>
        /// Ensures the base path of an <see cref="Server"/>
        /// all server instances will be created at "0_userdata.0.HardwareMonitor"
        /// </summary>
        /// <param name="itemId">The id of the <see cref="Server"/></param>
        /// <param name="itemName">The name of the <see cref="Server"/></param>
        /// <param name="objectType">The type of the added object</param>
        /// <param name="valueType">The type of the state value</param>
        /// <returns></returns>
        private async Task CreateObjectAsync(string itemId, string itemName, string objectType, string valueType = "")
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
                    Type = valueType
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

            await CreateObjectAsync($"{serverId}.{deviceTypeName}",deviceName , "channel");

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
