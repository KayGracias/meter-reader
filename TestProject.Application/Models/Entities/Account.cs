using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Application.Models.Entities
{
    [Table("Account")]
    public class Account
    {
        [Column("AccountID")]
        [Key]
        [Required]
        public int AccountId { get; set; }

        [Column("FirstName")]
        [Required]
        public string FirstName { get; set; }

        [Column("LastName")]
        [Required]
        public string LastName { get; set; }
    }

}
