using DreamAnalyzer2.Domain.Enums;
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
        public RoleType Role { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;
        private readonly List<Dream> _dreams = new();
        public IReadOnlyCollection<Dream> Dreams => _dreams.AsReadOnly();

        protected User() { }
        public User(string username, string email, RoleType roleType)
        {
            Username = username;
            Email = email;
            CreatedAt = DateTime.UtcNow;
            Role = roleType;
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

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
                throw new ArgumentNullException("Password hash can't be empty");
            PasswordHash = passwordHash;
        }
    }
}
