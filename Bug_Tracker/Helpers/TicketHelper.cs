using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models
{
    public class TicketHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserRoleHelper roleHelper = new UserRoleHelper();
        public List<Ticket> ListMyTickets()
        {
            var myTickets = new List<Ticket>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Admin":
                case "DemoAdmin":
                    myTickets.AddRange(db.Tickets);
                    break;
                case "Project Manager":
                    myTickets.AddRange(user.Projects.SelectMany(p => p.Tickets));
                    break;
                case "Developer":
                    myTickets.AddRange(db.Tickets.Where(t => t.AssignedToUserId == userId));
                    break;
                case "Submitter":
                    myTickets.AddRange(db.Tickets.Where(t => t.OwnerUserId == userId));
                    break;
            }

            return myTickets;
        }

        public List<Ticket> ListTicketsForUser(string id)
        {
            var myTickets = new List<Ticket>();
            var user = db.Users.Find(id);

            var myRole = roleHelper.ListUserRoles(id).FirstOrDefault();

            switch (myRole)
            {
                case "Admin":
                case "DemoAdmin":
                    myTickets.AddRange(db.Tickets);
                    break;
                case "Project Manager":
                    myTickets.AddRange(user.Projects.SelectMany(p => p.Tickets));
                    break;
                case "Developer":
                    myTickets.AddRange(db.Tickets.Where(t => t.AssignedToUserId == id));
                    break;
                case "Submitter":
                    myTickets.AddRange(db.Tickets.Where(t => t.OwnerUserId == id));
                    break;
            }

            return myTickets;
        }

        public void AssignDeveloperToTicket(string userId, int ticketId)
        {
            var user = db.Users.Find(userId);
            var ticket = db.Tickets.Find(ticketId);
            ticket.AssignedToUser = user;
            db.SaveChanges();
        }

    }
}