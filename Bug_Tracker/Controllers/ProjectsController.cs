using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bug_Tracker.Models;
using Microsoft.AspNet.Identity;

namespace Bug_Tracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private UserRoleHelper RoleHelper = new UserRoleHelper();
        private ProjectsHelper ProjectHelper = new ProjectsHelper();

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            ProjectDetailsViewModel viewModel = new ProjectDetailsViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            var userRole = RoleHelper.ListUserRoles(user.Id).FirstOrDefault();

            Project project = db.Projects.Find(id);
            viewModel.Project = project;
            //viewModel.Tickets = project.Tickets.ToList();
            viewModel.Managers = project.GetManagersInProject();
            viewModel.Developers = project.GetDevelopersInProject();
            viewModel.Submitters = project.GetSubmittersInProject();
            viewModel.Users = ProjectHelper.UsersOnProject((int)id);
            viewModel.UserRole = userRole;

            if (userRole == "Admin")
            {
                var users = RoleHelper.UsersInRole("Project Manager").Concat(RoleHelper.UsersInRole("Developer")).Concat(RoleHelper.UsersInRole("Submitter"));
                viewModel.AllUsers = users.ToList();
                viewModel.Tickets = project.Tickets.ToList();
            }
            else if(userRole == "Project Manager")
            {
                var users = RoleHelper.UsersInRole("Developer").Concat(RoleHelper.UsersInRole("Submitter"));
                viewModel.AllUsers = users.ToList();
                viewModel.Tickets = project.Tickets.ToList();
            }
            else if (userRole == "Developer")
            {
                viewModel.Tickets = project.Tickets.Where(t => t.AssignedToUserId == user.Id).ToList();
            }
            else if (userRole == "Submitter")
            {
                viewModel.Tickets = project.Tickets.Where(t => t.OwnerUserId == user.Id).ToList();
            }


            if (project == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AddUserToProject(string[] userIds, int projectId)
        {
            var project = db.Projects.FirstOrDefault(p => p.Id == projectId);

            var allUsers = project.Users.ToList();

            if (userIds == null)
            {
                foreach (var user in allUsers)
                {
                    ProjectHelper.RemoveUserFromProject(user.Id, project.Id);
                }
            }
            else 
            {
                foreach (var user in allUsers)
                {
                    ProjectHelper.RemoveUserFromProject(user.Id, project.Id);
                }
                foreach (var userId in userIds)
                {
                    ProjectHelper.AddUserToProject(userId, project.Id);
                }
            }
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }


        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string ProjectTitle, string ProjectDescription)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.Name = ProjectTitle;
                project.Description = ProjectDescription;
                db.Projects.Add(project);
                db.SaveChanges();
                if (db.Users.Find(User.Identity.GetUserId()).IsDemoUser)
                {
                    return RedirectToAction("Details", "Projects", new { id = project.Id });
                }
                else
                {
                    return RedirectToAction("Details", "Projects", new { id = project.Id });
                }
                
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Projects/Edit/5

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string projectTitle, string projectDescription, int id)
        {
            var project = db.Projects.Find(id);
            project.Name = projectTitle;
            project.Description = projectDescription;
            db.SaveChanges();
            return RedirectToAction("Details", "Projects", new { id = id});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
