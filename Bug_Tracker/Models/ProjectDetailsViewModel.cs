using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models
{
    public class ProjectDetailsViewModel
    {
        public string UserRole { get; set; }
        public ICollection<ApplicationUser> Managers { get; set; }
        public ICollection<ApplicationUser> Developers { get; set; }
        public ICollection<ApplicationUser> Submitters { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<ApplicationUser> AllUsers { get; set; }
        public Project Project { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ProjectDetailsViewModel() 
        {
            Managers = new HashSet<ApplicationUser>();
            Developers = new HashSet<ApplicationUser>();
            Submitters = new HashSet<ApplicationUser>();
            Users = new HashSet<ApplicationUser>();
            AllUsers = new HashSet<ApplicationUser>();
            Tickets = new HashSet<Ticket>();
        }
    }
}