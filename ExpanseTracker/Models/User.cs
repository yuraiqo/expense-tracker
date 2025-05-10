using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public DateTime createdAt { get; set; }

        public User(string username, string passwordHash, string email)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
