using Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bug_Tracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRoleHelper RoleHelper = new UserRoleHelper();
        private ProjectsHelper ProjectHelper = new ProjectsHelper();

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Index(DashboardViewModel viewModel)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            viewModel.User = user;
            viewModel.ApplicationUsers = db.Users.ToList();
            viewModel.Managers = RoleHelper.UsersInRole("Project Manager").ToList();
            viewModel.Developers = RoleHelper.UsersInRole("Developer").ToList();
            viewModel.Submitters = RoleHelper.UsersInRole("Submitter").ToList();

            if (RoleHelper.ListUserRoles(user.Id).FirstOrDefault() == "Admin")
            {
                var tickets = db.Tickets.ToList();
                viewModel.Projects = db.Projects.ToList();
                viewModel.Tickets = tickets;
                viewModel.ActiveTicketsCount = tickets.Where(t => t.TicketStatus.Name == "Assigned").Concat(tickets.Where(t => t.TicketStatus.Name == "Unassigned")).Count();
            }
            else if (RoleHelper.ListUserRoles(user.Id).FirstOrDefault() == "Project Manager")
            {
                var tickets = ProjectHelper.ListUserProjects(user.Id).SelectMany(p => p.Tickets).ToList();
                viewModel.Projects = ProjectHelper.ListUserProjects(user.Id);
                viewModel.Tickets = tickets;
                viewModel.ActiveTicketsCount = tickets.Where(t => t.TicketStatus.Name == "Assigned").Concat(tickets.Where(t => t.TicketStatus.Name == "Unassigned")).Count();
            }
            else if (RoleHelper.ListUserRoles(user.Id).FirstOrDefault() == "Developer")
            {
                var tickets = db.Tickets.Where(t => t.AssignedToUserId == user.Id).ToList();
                viewModel.Projects = ProjectHelper.ListUserProjects(user.Id);
                viewModel.Tickets = tickets;
                viewModel.ActiveTicketsCount = tickets.Where(t => t.TicketStatus.Name == "Assigned").Concat(tickets.Where(t => t.TicketStatus.Name == "Unassigned")).Count();
            }
            else if (RoleHelper.ListUserRoles(user.Id).FirstOrDefault() == "Submitter")
            {
                var tickets = db.Tickets.Where(t => t.OwnerUserId == user.Id).ToList();
                viewModel.Projects = ProjectHelper.ListUserProjects(user.Id);
                viewModel.Tickets = tickets;
                viewModel.ActiveTicketsCount = tickets.Where(t => t.TicketStatus.Name == "Assigned").Concat(tickets.Where(t => t.TicketStatus.Name == "Unassigned")).Count();
            }
            else
            {
                var tickets = ProjectHelper.ListUserProjects(user.Id).SelectMany(p => p.Tickets).ToList();
                viewModel.Projects = ProjectHelper.ListUserProjects(user.Id);
                viewModel.Tickets = tickets;
                viewModel.ActiveTicketsCount = tickets.Where(t => t.TicketStatus.Name == "Assigned").Concat(tickets.Where(t => t.TicketStatus.Name == "Unassigned")).Count();
            }

            // load the dashboard viewmodel
            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult About()
        {
            return View();
        }
    }
}