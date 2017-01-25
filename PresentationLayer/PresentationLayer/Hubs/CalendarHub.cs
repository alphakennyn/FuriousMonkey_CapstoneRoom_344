using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

namespace PresentationLayer.Hubs
{
    public class CalendarHub : Hub
    {
        //Mapping connection to user so a message can be sent to a specific user
        public override Task OnConnected()
        {
            string name = Context.User.Identity.GetUserId();
            if(name!=null) {
                Groups.Add(Context.ConnectionId, name);
            }
            

            return base.OnConnected();
        }
    }
}