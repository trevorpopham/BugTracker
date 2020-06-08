using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models
{
    public class ProjectsHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            /*if (db.Projects.AsNoTracking().SelectMany(p => p.Users).Select(u => u.Id).Contains(userId))
            {
                return false;
            }
            */
            if (db.Projects.Find(projectId).Users.Contains(db.Users.Find(userId)))
            {
                return true;
            }
            return false;
        }
        public ICollection<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return projects;
        }

        public ICollection<Project> ListAllProjects()
        {
            var projects = db.Projects.ToList();
            return projects;
        }
        public void AddUserToProject(string userId, int projectId)
        {
            if (!IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);
                proj.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {  
            if (IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var delUser = db.Users.Find(userId);
                proj.Users.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public void ToggleUserOnProject(string userId, int projectId)
        {
            if (!IsUserOnProject(userId, projectId))
            {
                AddUserToProject(userId, projectId);
            }
            else
            {
                RemoveUserFromProject(userId, projectId);
            }
        }
        public ICollection<ApplicationUser> UsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }
        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }
            

    }
}