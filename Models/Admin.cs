using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingPlatform.Models
{
    public class Admin
    {
        [Key]
        [DisplayName("Matrikelnummer")]
        public string MatrikelNr { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Erstellungsdatum")]
        public DateTime DateJoined { get; set; }= DateTime.Now;
    }
}
