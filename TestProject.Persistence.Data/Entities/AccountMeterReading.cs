using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestProject.Persistence.Data.Entities
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
        public int MeterReadValue { get; set; }

        [ForeignKey("AccountID")]
        [Required]
        public virtual Account Account { get; set; }
    }
}
