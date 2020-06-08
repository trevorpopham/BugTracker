using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bug_Tracker.Helpers;
using Bug_Tracker.Models;
using Microsoft.AspNet.Identity;

namespace Bug_Tracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRoleHelper roleHelper = new UserRoleHelper();
        private HistoryHelper historyHelper = new HistoryHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();
        private TicketHelper ticketHelper = new TicketHelper();
        private NotificationHelper notificationHelper = new NotificationHelper();

        // GET: Tickets
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.ReturnUserRole() == "Admin")
            {
                ViewBag.TicketIndexHeader = "All Tickets";
            }
            else
            {
                ViewBag.TicketIndexHeader = "Your Tickets";
            }

            var tickets = ticketHelper.ListMyTickets();
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            TicketDetailsViewModel viewModel = new TicketDetailsViewModel();
            var user = db.Users.Find(User.Identity.GetUserId());
            viewModel.Ticket = ticket;
            viewModel.User = user;
            viewModel.UserRole = roleHelper.ListUserRoles(User.Identity.GetUserId()).FirstOrDefault();
            viewModel.TicketPriorities = db.Priorities.ToList();
            viewModel.TicketStatuses = db.Statuses.ToList();
            viewModel.TicketTypes = db.Types.ToList();
            viewModel.Developers = roleHelper.UsersInRole("Developer").ToList();

            if (user.ReturnUserRole() == "Developer")
            {
                if (ticket.AssignedToUserId == user.Id)
                {
                    return View(viewModel);
                }
                else
                {
                    RedirectToAction("Index", "Home");
                }
            }
            return View(viewModel);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Histories()
        {
            return View(db.Histories.ToList());
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(int? projectId)
        {
            TicketCreateViewModel viewModel = new TicketCreateViewModel();
            Ticket ticket = new Ticket();
            if (projectId != null)
            {
                viewModel.Project = db.Projects.Find(projectId);
            }
            viewModel.User = db.Users.Find(User.Identity.GetUserId());
            viewModel.Projects = projectHelper.ListUserProjects(viewModel.User.Id);
            viewModel.Ticket = ticket;
            return View(viewModel);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ticket ticket, Project project, string typeName, string priorityName, string userId)
        {
            var owner = db.Users.Find(userId);
            var defaultStatus = db.Statuses.FirstOrDefault(s => s.Name == "Unassigned");
            var type = db.Types.FirstOrDefault(t => t.Name == typeName);
            var priority = db.Priorities.FirstOrDefault(p => p.Name == priorityName);
            ticket.OwnerUser = owner;
            ticket.TicketStatus = defaultStatus;
            ticket.Created = DateTime.Now;
            ticket.ProjectId = project.Id;
            ticket.TicketPriority = priority;
            ticket.TicketType = type;
            db.Tickets.Add(ticket);
            db.SaveChanges();
            if (owner.IsDemoUser)
            {
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            else
            {
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            

           // return View();
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditStatus(int id, int statusId)
        {
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == id);
            var ticket = db.Tickets.Find(id);
            var status = db.Statuses.Find(statusId);
            ticket.Updated = DateTime.Now;
            ticket.TicketStatus = status;
            db.SaveChanges();
            var newTicket = db.Tickets.Find(id);

            TicketDetailsViewModel viewModel = new TicketDetailsViewModel();
            viewModel.Ticket = ticket;
            viewModel.User = db.Users.Find(User.Identity.GetUserId());
            viewModel.UserRole = roleHelper.ListUserRoles(User.Identity.GetUserId()).FirstOrDefault();
            viewModel.TicketPriorities = db.Priorities.ToList();
            viewModel.TicketStatuses = db.Statuses.ToList();
            viewModel.TicketTypes = db.Types.ToList();
            viewModel.Developers = roleHelper.UsersInRole("Developer").ToList();

            historyHelper.RecordHistoricalChanges(oldTicket, newTicket);

            //return RedirectToAction("Details", "Tickets", new { id });
            return PartialView("~/Views/Shared/_TicketHistories.cshtml", viewModel);
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult EditPriority(int id, int priorityId)
        {
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == id);
            var ticket = db.Tickets.Find(id);
            var priority = db.Priorities.Find(priorityId);
            ticket.Updated = DateTime.Now;
            ticket.TicketPriority = priority;
            db.SaveChanges();
            var newTicket = db.Tickets.Find(ticket.Id);

            TicketDetailsViewModel viewModel = new TicketDetailsViewModel();
            viewModel.Ticket = ticket;
            viewModel.User = db.Users.Find(User.Identity.GetUserId());
            viewModel.UserRole = roleHelper.ListUserRoles(User.Identity.GetUserId()).FirstOrDefault();
            viewModel.TicketPriorities = db.Priorities.ToList();
            viewModel.TicketStatuses = db.Statuses.ToList();
            viewModel.TicketTypes = db.Types.ToList();
            viewModel.Developers = roleHelper.UsersInRole("Developer").ToList();

            historyHelper.RecordHistoricalChanges(oldTicket, newTicket);

            //return RedirectToAction("Details", "Tickets", new { id });
            return PartialView("~/Views/Shared/_TicketHistories.cshtml", viewModel);
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult EditType(int id, int typeId)
        {
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == id);
            var ticket = db.Tickets.Find(id);
            var type = db.Types.Find(typeId);
            ticket.Updated = DateTime.Now;
            ticket.TicketType = type;
            db.SaveChanges();
            var newTicket = db.Tickets.Find(ticket.Id);

            TicketDetailsViewModel viewModel = new TicketDetailsViewModel();
            viewModel.Ticket = ticket;
            viewModel.User = db.Users.Find(User.Identity.GetUserId());
            viewModel.UserRole = roleHelper.ListUserRoles(User.Identity.GetUserId()).FirstOrDefault();
            viewModel.TicketPriorities = db.Priorities.ToList();
            viewModel.TicketStatuses = db.Statuses.ToList();
            viewModel.TicketTypes = db.Types.ToList();
            viewModel.Developers = roleHelper.UsersInRole("Developer").ToList();

            historyHelper.RecordHistoricalChanges(oldTicket, newTicket);

            //return RedirectToAction("Details", "Tickets", new { id });
            return PartialView("~/Views/Shared/_TicketHistories.cshtml", viewModel);
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult AssignDeveloperToTicket(int ticketId, string userId)
        {
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticketId);
            ticketHelper.AssignDeveloperToTicket(userId, ticketId);
            var newTicket = db.Tickets.Find(ticketId);
            notificationHelper.SendTicketNotification(oldTicket, newTicket);
            return RedirectToAction("Details", "Tickets", new { id = ticketId });
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult SelectTicketNotification(int notificationId, int ticketId)
        {
            var notification = db.TicketNotifications.Find(notificationId);
            notification.IsRead = true;
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketId });
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult CreateComment(int ticketId, string userId, string commentBody)
        {
            var ticket = db.Tickets.Find(ticketId);
            TicketComment comment = new TicketComment();
            comment.TicketId = ticketId;
            comment.UserId = userId;
            comment.CommentBody = commentBody;
            comment.Created = DateTime.Now;

            ticket.TicketComments.Add(comment);
            db.SaveChanges();

            TicketDetailsViewModel viewModel = new TicketDetailsViewModel();
            viewModel.Ticket = ticket;
            viewModel.User = db.Users.Find(User.Identity.GetUserId());
            viewModel.UserRole = roleHelper.ListUserRoles(User.Identity.GetUserId()).FirstOrDefault();
            viewModel.TicketPriorities = db.Priorities.ToList();
            viewModel.TicketStatuses = db.Statuses.ToList();
            viewModel.TicketTypes = db.Types.ToList();
            viewModel.Developers = roleHelper.UsersInRole("Developer").ToList();

            return PartialView("~/Views/Shared/_TicketComments.cshtml", viewModel);

            // return partial view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult CreateAttachment(HttpPostedFileBase file, string description, int ticketId)
        {
            TicketAttachment attachment = new TicketAttachment();
            if (FileUploadValidator.IsWebFriendlyFile(file) || FileUploadValidator.IsWebFriendlyImage(file))
            {
                var fileName = Path.GetFileName(file.FileName);
                attachment.FileName = fileName;
                var justFileName = Path.GetFileNameWithoutExtension(fileName);
                justFileName = StringUtilities.URLFriendly(justFileName);
                fileName = $"{justFileName}_{DateTime.Now.Ticks}{Path.GetExtension(fileName)}";
                file.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                attachment.FilePath = "/Uploads/" + fileName;
            }
            attachment.Description = description;
            attachment.Created = DateTime.Now;
            attachment.TicketId = ticketId;
            db.Attachments.Add(attachment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketId});
        }

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult EditTicketAndAttachments(int ticketId, string ticketTitle, string ticketDescription, string[] attachmentDescriptions)
        {
            var ticket = db.Tickets.Find(ticketId);
            var attachmentIds = ticket.TicketAttachments.Select(t => t.Id).ToList();
            for (int i = 0; i < attachmentIds.Count(); i++)
            {
                var attachment = ticket.TicketAttachments.FirstOrDefault(a => a.Id == attachmentIds[i]);
                attachment.Description = attachmentDescriptions[i];
            }
            ticket.Title = ticketTitle;
            ticket.Description = ticketDescription;

            db.SaveChanges();

            return RedirectToAction("Details", "Tickets", new { id = ticketId });
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult DeleteAttachment(int id)
        {
            TicketAttachment attachment = db.Attachments.Find(id);
            db.Attachments.Remove(attachment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = attachment.TicketId});
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult EditComment(int id, string commentBody)
        {
            TicketComment comment = db.Comments.Find(id);
            comment.CommentBody = commentBody;
            db.SaveChanges();

            TicketDetailsViewModel viewModel = new TicketDetailsViewModel();
            var ticket = db.Tickets.Find(comment.TicketId);
            viewModel.Ticket = ticket;
            viewModel.User = db.Users.Find(User.Identity.GetUserId());
            viewModel.UserRole = roleHelper.ListUserRoles(User.Identity.GetUserId()).FirstOrDefault();
            viewModel.TicketPriorities = db.Priorities.ToList();
            viewModel.TicketStatuses = db.Statuses.ToList();
            viewModel.TicketTypes = db.Types.ToList();
            viewModel.Developers = roleHelper.UsersInRole("Developer").ToList();

            return PartialView("~/Views/Shared/_TicketComments.cshtml", viewModel);
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult DeleteComment(int id)
        {
            TicketComment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();

            TicketDetailsViewModel viewModel = new TicketDetailsViewModel();
            var ticket = db.Tickets.Find(comment.TicketId);
            viewModel.Ticket = ticket;
            viewModel.User = db.Users.Find(User.Identity.GetUserId());
            viewModel.UserRole = roleHelper.ListUserRoles(User.Identity.GetUserId()).FirstOrDefault();
            viewModel.TicketPriorities = db.Priorities.ToList();
            viewModel.TicketStatuses = db.Statuses.ToList();
            viewModel.TicketTypes = db.Types.ToList();
            viewModel.Developers = roleHelper.UsersInRole("Developer").ToList();

            return PartialView("~/Views/Shared/_TicketComments.cshtml", viewModel);
        }
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult DownloadFile(string filePath)
        {
            string fullName = Server.MapPath("~" + filePath);

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
        }

        byte[] GetFile(string s)
        {
            FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new IOException(s);
            return data;
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
