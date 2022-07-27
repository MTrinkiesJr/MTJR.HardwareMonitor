using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MTJR.HardwareMonitor.Model;

namespace MTJR.HardwareMonitor.Services
{
    /// <summary>
    /// Service to capture connected clients
    /// </summary>
    public class EventService
    {
        private readonly IHubContext<EventHub> _hubContext;
        private readonly List<EventHubConnection> _connections;

        /// <summary>
        /// The current list of <see cref="EventHubConnection"/>
        /// </summary>
        public IReadOnlyList<EventHubConnection> Connections => _connections;


        /// <summary>
        /// Constructor to create fulfilled <see cref="EventService"/>
        /// </summary>
        /// <param name="connections">Injected list of connections</param>
        /// <param name="hubContext">The hub context of <see cref="EventHub"/></param>
        public EventService(List<EventHubConnection> connections, IHubContext<EventHub> hubContext)
        {
            _connections = connections;
            _hubContext = hubContext;
        }


        /// <summary>
        /// Add or update a connection
        /// </summary>
        /// <param name="connectionId">ConnectionId of the client</param>
        /// <param name="ipAddress">IP Address of the client</param>
        /// <returns></returns>
        public async Task AddAsync(string connectionId, string ipAddress)
        {
            var connection = _connections.FirstOrDefault(a => a.ConnectionId == connectionId);

            if (connection != null)
            {
                connection.ConnectedSince = DateTime.Now;
            }

            connection = new EventHubConnection(connectionId, ipAddress);
            _connections.Add(connection);

            await _hubContext.Clients.All.SendCoreAsync("signalr_connected", new object[] { connection });
        }

        /// <summary>
        /// Removes an existing connection
        /// </summary>
        /// <param name="connectionId">The connection id of disconnected client</param>
        /// <returns></returns>
        public async Task RemoveAsync(string connectionId)
        {
            var connection = _connections.FirstOrDefault(a => a.ConnectionId == connectionId);

            if (connection != null)
            {
                _connections.Remove(connection);

                await _hubContext.Clients.All.SendCoreAsync("signalr_disconnected", new object[] { connection });
            }
        }
        
    }
}
