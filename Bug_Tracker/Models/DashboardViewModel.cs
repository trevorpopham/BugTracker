using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bug_Tracker.Models
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public int ActiveTicketsCount { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public ICollection<ApplicationUser> Managers { get; set; }
        public ICollection<ApplicationUser> Developers { get; set; }
        public ICollection<ApplicationUser> Submitters { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Ticket> Tickets{ get; set; }
        public DashboardViewModel()
        {
            ApplicationUsers = new HashSet<ApplicationUser>();
            Managers = new HashSet<ApplicationUser>();
            Developers = new HashSet<ApplicationUser>();
            Submitters = new HashSet<ApplicationUser>();
            Projects = new HashSet<Project>();
            Tickets = new HashSet<Ticket>();
        }
    }

}