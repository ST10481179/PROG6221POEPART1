using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityChatbot
{
    internal class TaskManager
    {
        private readonly TaskStorageHelper _storage;
        private readonly ActivityLogger _logger;

        public TaskManager(ActivityLogger logger)
        {
            _storage = new TaskStorageHelper();
            _logger = logger;
        }

        public UserTask AddTask(string title, string description, string reminder)
        {
            var task = _storage.AddTask(title, description, reminder);
            var note = string.IsNullOrWhiteSpace(reminder)
                ? $"Task added: '{task.Title}'"
                : $"Task added: '{task.Title}' (Reminder set for {reminder})";
            _logger.Log(note);
            return task;
        }

        public List<UserTask> GetAllTasks()
        {
            return _storage.LoadTasks();
        }

        public void MarkAsComplete(int id)
        {
            var task = _storage.LoadTasks().FirstOrDefault(t => t.Id == id);
            if (task == null) return;
            _storage.MarkAsComplete(id);
            _logger.Log($"Task marked complete: '{task.Title}'");
        }

        public void DeleteTask(int id)
        {
            var task = _storage.LoadTasks().FirstOrDefault(t => t.Id == id);
            if (task == null) return;
            _storage.DeleteTask(id);
            _logger.Log($"Task deleted: '{task.Title}'");
        }

        public void SetReminder(int id, string reminder)
        {
            var task = _storage.LoadTasks().FirstOrDefault(t => t.Id == id);
            if (task == null) return;
            _storage.UpdateReminder(id, reminder);
            _logger.Log($"Reminder set: '{task.Title}' on {reminder}");
        }
    }
}
