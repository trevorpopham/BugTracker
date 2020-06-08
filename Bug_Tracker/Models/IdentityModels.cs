using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Bug_Tracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        private TicketHelper ticketHelper = new TicketHelper();
        private UserRoleHelper roleHelper = new UserRoleHelper();

        [Display(Name="First Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage ="First Name must be between 1 and 50 characters")]
        public string FirstName { get; set; }
        [Display(Name="Last Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last Name Name must be between 1 and 50 characters")]
        public string LastName { get; set; }
        [Display(Name="Display Name")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Display Name must be between 1 and 50 characters")]
        public string DisplayName { get; set; }
        public string AvatarPath { get; set; }
        public bool IsDemoUser { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistorys { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }

        [NotMapped]
        public string FullName { 
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public int ReturnNumberOfTickets()
        {
            return ticketHelper.ListTicketsForUser(Id).Count();
        }

        public string ReturnUserRole()
        {
            return roleHelper.ListUserRoles(Id).FirstOrDefault();
        }




        public ApplicationUser()
        {
            TicketComments = new HashSet<TicketComment>();
            Projects = new HashSet<Project>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketHistorys = new HashSet<TicketHistory>();
            TicketNotifications = new HashSet<TicketNotification>();

        }
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TicketPriority> Priorities { get; set; }
        public DbSet<TicketStatus> Statuses { get; set; }
        public DbSet<TicketAttachment> Attachments { get; set; }
        public DbSet<TicketComment> Comments { get; set; }
        public DbSet<TicketHistory> Histories { get; set; }
        public DbSet<TicketType> Types { get; set; }

        public DbSet<TicketNotification> TicketNotifications { get; set; }

        //public System.Data.Entity.DbSet<Bug_Tracker.Models.ApplicationUser> ApplicationUsers { get; set; }
        /*public override int SaveChanges()
        {
            var user = this.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user.IsDemoUser)
            {
                return 0;
            }
            else
            {
                return base.SaveChanges();
            }
        }*/
    }
}