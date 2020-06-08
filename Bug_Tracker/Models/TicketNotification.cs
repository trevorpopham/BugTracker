using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? TicketId { get; set; }
        public bool IsRead { get; set; }
        public string NotificationBody { get; set; }
        public DateTime Sent { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}