using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }  // foreign key to User

        [Required]
        public int CategoryId { get; set; }  // foreign key to Category

        [Required]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public User User { get; set; }

        public Category Category { get; set; }
    }
}
