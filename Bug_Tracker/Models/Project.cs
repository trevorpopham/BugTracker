using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models
{
    public class Project
    {
        private UserRoleHelper roleHelper = new UserRoleHelper();
        public Project() 
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<ApplicationUser>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public ICollection<ApplicationUser> GetManagersInProject()
        {

            var list = Users.Where(u => roleHelper.ListUserRoles(u.Id).FirstOrDefault() == "Project Manager").ToList();
            return list;
        }
        public ICollection<ApplicationUser> GetDevelopersInProject()
        {
            var list = Users.Where(u => roleHelper.ListUserRoles(u.Id).FirstOrDefault() == "Developer").ToList();
            return list;
        }
        public ICollection<ApplicationUser> GetSubmittersInProject()
        {
            var list = Users.Where(u => roleHelper.ListUserRoles(u.Id).FirstOrDefault() == "Submitter").ToList();
            return list;
        }

    }
}