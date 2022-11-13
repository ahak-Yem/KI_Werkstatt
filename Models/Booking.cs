using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingPlatform.Models
{
    public class Booking
    {
        //BuchungsNr wird automatisch genariert(Auto_Increment) : Jede Buchung hat eine unique Nummer
        [Key]
        public int BookingID { get; set; }

        //Wird von text box in Login-Seite in runtime gespeichert, wenn Ldap der Login autorisiert.
        //Wenn eine Buchung gemacht wird, wird dann in der Buchung Tabelle in der DB gespeichert.
        [Required]
        public string MatrikelNr { get; set; }

        //Man legt das fest,wenn man bucht
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        //immer falsch bis des Ablaufsdatum überschritten wird
        public bool WarningEmailState { get; set; } = false;

        //Zustand
        // bei einer neuen Buchung fängt =reserviert an
        // -> wenn abgeholt, dann =gebucht
        // -> wenn zurückgegeben, dann =zurückgegeben
        // -> wenn nicht zurückgegeben, Ablaufsdatum überschritten und gebucht, dann =abgelaufen
        public string BookingCondition { get; set; }

        //Relationship
        public int ResourceID { get; set; }
        [ForeignKey("ResourceID")]
        public Resources Resource { get; set; }

    }
}
