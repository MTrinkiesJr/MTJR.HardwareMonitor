using System;

namespace MTJR.HardwareMonitor.Model
{
    /// <summary>
    /// Describes a connection to <see cref="EventHub"/>
    /// </summary>
    public class EventHubConnection
    {
        /// <summary>
        /// The connection id of the connected client
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        /// The ip address of the connected client
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// The connection time of the client
        /// </summary>
        public DateTime ConnectedSince { get; set; }

        /// <summary>
        /// Constructor to create fulfilled <see cref="EventHubConnection"/>
        /// </summary>
        /// <param name="connectionId">Connection id</param>
        /// <param name="ipAddress">IP Address</param>
        public EventHubConnection(string connectionId, string ipAddress)
        {
            ConnectionId = connectionId;
            IpAddress = ipAddress;
            ConnectedSince = DateTime.Now;
        }
    }
}
