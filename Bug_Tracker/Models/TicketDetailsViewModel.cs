using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models
{
    public class TicketDetailsViewModel
    {

        public ApplicationUser User { get; set; }
        public string UserRole { get; set; }
        public Ticket Ticket { get; set; }
        public ICollection<ApplicationUser> Developers { get; set; }
        public ICollection<TicketStatus> TicketStatuses { get; set; }
        public ICollection<TicketPriority> TicketPriorities { get; set; }
        public ICollection<TicketType> TicketTypes { get; set; }

        public TicketDetailsViewModel()
        {
            Developers = new HashSet<ApplicationUser>();
            TicketStatuses = new HashSet<TicketStatus>();
            TicketPriorities = new HashSet<TicketPriority>();
            TicketTypes = new HashSet<TicketType>();
        }
    }
}