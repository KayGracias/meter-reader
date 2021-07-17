using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Application.Models.Entities
{
    [Table("AccountMeterReading")]
    public class AccountMeterReading
    {
        [Column("AccountMeterReadingID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int AccountMeterReadingId { get; set; }

        [Column("MeterReadingDateTime")]
        [Required]
        public DateTimeOffset MeterReadingDateTime { get; set; }

        [Column("MeterReadValue")]
        [Required]
        [MaxLength(5)]
        public string MeterReadValue { get; set; }

        [ForeignKey("AccountID")]
        [Required]
        public int AccountID { get; set; }
        public Account Account { get; set; }
    }
}
