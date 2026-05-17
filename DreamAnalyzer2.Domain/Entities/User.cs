using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Entities
{
    public class User : Entity
    {
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;
        private readonly List<Dream> _dreams = new();
        public IReadOnlyCollection<Dream> Dreams => _dreams.AsReadOnly();

        protected User() { }
        public User(string username, string email, string passwordHash)
        {
            Username = username;
            Email = email;
            CreatedAt = DateTime.UtcNow;
            PasswordHash = passwordHash;
        }

        public void AddDream(Dream dream)
        {
            if (dream == null) 
                throw new ArgumentNullException(nameof(dream));
            _dreams.Add(dream);
        }
        public void RemoveDream(Dream dream)
        {
            if (dream == null)
                throw new ArgumentNullException(nameof(dream));
            if (_dreams.Contains(dream))
                _dreams.Remove(dream);
        }
    }
}
