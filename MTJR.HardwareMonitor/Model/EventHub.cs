using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using MTJR.HardwareMonitor.Services;

namespace MTJR.HardwareMonitor.Model
{

    ///<inheritdoc/>
    public class EventHub : Hub
    {
        private readonly EventService _eventService;


        ///<inheritdoc/>
        public EventHub(EventService eventService)
        {
            _eventService = eventService;
        }

        ///<inheritdoc/>
        public override async Task OnConnectedAsync()
        {
            await _eventService.AddAsync(Context.ConnectionId, Context.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString());
            await base.OnConnectedAsync();
            
        }

        ///<inheritdoc/>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _eventService.RemoveAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }


    }
}
