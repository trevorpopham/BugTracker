using Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Bug_Tracker.Helpers
{
    public class NotificationHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void SendTicketNotification(Ticket oldTicket, Ticket newTicket)
        {
            var ticketHasBeenAssigned = oldTicket.AssignedToUserId == null && newTicket.AssignedToUserId != null;
            var ticketHasBeenUnassigned = oldTicket.AssignedToUserId != null && newTicket.AssignedToUser == null;
            var ticketHasBeenReassigned = oldTicket.AssignedToUserId != null && newTicket.AssignedToUserId != null && oldTicket.AssignedToUserId != newTicket.AssignedToUserId;

            var newTicketUser = db.Users.Find(newTicket.AssignedToUserId);
            var oldTicketUser = db.Users.Find(oldTicket.AssignedToUserId);

            if (ticketHasBeenAssigned)
            {
                var notificationMessage = $"You have been assigned to a new ticket named {newTicket.Title}, on project named {newTicket.Project.Name}. It has a {newTicket.TicketPriority.Name} priority.";
                SendNotificationTo(newTicket.AssignedToUserId, notificationMessage, newTicket.Id);

                var ticketLink = $" Click <a href='https://localhost:44301/Tickets/Details/{newTicket.Id}'>here</a> to view the ticket";
                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = "You have been assigned to a new ticket.";
                mailMessage.Body = notificationMessage + ticketLink;
                mailMessage.To.Add(new MailAddress(newTicketUser.Email));
                mailMessage.From = new MailAddress("trevorpopham@gmail.com");
                mailMessage.IsBodyHtml = true;
                SendEmail.Send(mailMessage);
            }
            else if (ticketHasBeenUnassigned)
            {
                var notificationMessage = $"You have been unassigned from a ticket named {newTicket.Title}, on project named {newTicket.Project.Name}.";
                SendNotificationTo(oldTicket.AssignedToUserId, notificationMessage, newTicket.Id);

                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = "You have been unassigned from a ticket.";
                mailMessage.Body = notificationMessage;
                mailMessage.To.Add(new MailAddress(oldTicketUser.Email));
                mailMessage.From = new MailAddress("trevorpopham@gmail.com");
                SendEmail.Send(mailMessage);
            }
            else if (ticketHasBeenReassigned)
            {
                var notificationMessage1 = $"You have been assigned to a new ticket named {newTicket.Title}, on project named {newTicket.Project.Name}. It has a {newTicket.TicketPriority.Name} priority.";
                SendNotificationTo(newTicket.AssignedToUserId, notificationMessage1, newTicket.Id);

                var ticketLink = $" Click <a href='https://localhost:44301/Tickets/Details/{newTicket.Id}'>here</a> to view the ticket";
                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = "You have been assigned to a new ticket.";
                mailMessage.Body = notificationMessage1 + ticketLink;
                mailMessage.To.Add(new MailAddress(newTicketUser.Email));
                mailMessage.From = new MailAddress("trevorpopham@gmail.com");
                mailMessage.IsBodyHtml = true;
                SendEmail.Send(mailMessage);

                var notificationMessage2 = $"You have been unassigned to a new ticket named {newTicket.Title}, on project named {newTicket.Project.Name}.";
                SendNotificationTo(oldTicket.AssignedToUserId, notificationMessage2, newTicket.Id);


                MailMessage mailMessage2 = new MailMessage();
                mailMessage2.Subject = "You have been unassigned from a ticket.";
                mailMessage2.Body = notificationMessage2;
                mailMessage2.To.Add(new MailAddress(oldTicketUser.Email));
                mailMessage2.From = new MailAddress("trevorpopham@gmail.com");
                SendEmail.Send(mailMessage2);
            }

        }

        public void SendNotificationTo(string userId, string message, int? ticketId)
        {
            var notification = new TicketNotification
            {
                NotificationBody = message,
                UserId = userId,
                IsRead = false,
                TicketId = ticketId,
                Sent = DateTime.Now
            };
            db.TicketNotifications.Add(notification);
            db.SaveChanges();
        }
    }
}