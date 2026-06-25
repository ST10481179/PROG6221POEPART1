using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CyberSecurityChatbot
{
    internal class TaskStorageHelper
    {
        private readonly ApplicationDbContext db = new();

        public TaskStorageHelper()
        {
            db.Database.EnsureCreated();
        }

        public List<UserTask> LoadTasks()
        {
            return db.Tasks.OrderBy(t => t.IsComplete).ThenBy(t => t.Id).ToList();
        }

        public UserTask AddTask(string title, string description, string reminder)
        {
            var task = new UserTask
            {
                Title = title,
                Description = description,
                Reminder = reminder,
                IsComplete = false,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };
            db.Tasks.Add(task);
            db.SaveChanges();
            return task;
        }

        public void MarkAsComplete(int id)
        {
            var task = db.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return;
            task.IsComplete = true;
            db.SaveChanges();
        }

        public void UpdateReminder(int id, string reminder)
        {
            var task = db.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return;
            task.Reminder = reminder;
            db.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            var task = db.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return;
            db.Tasks.Remove(task);
            db.SaveChanges();
        }
    }
}
