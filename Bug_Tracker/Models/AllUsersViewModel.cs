using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models
{
    public class AllUsersViewModel
    {
        public ApplicationUser User { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public string SpecialPropertyHeader { get; set; }
        public string Header { get; set; }
        public string Role { get; set; }
        public AllUsersViewModel()
        {
            Users = new HashSet<ApplicationUser>();
        }
    }
}