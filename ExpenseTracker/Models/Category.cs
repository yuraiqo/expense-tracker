using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }  // foreign key to User

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Color { get; set; }

        public User User { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
