namespace Fahrtenbuch.Library
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Vehicle")]
    public partial class Vehicle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehicle()
        {
            Reservations = new HashSet<Reservation>();
            Trips = new HashSet<Trip>();
        }

        public Guid ID { get; set; }

        [Required]
        [StringLength(25)]
        public string Type { get; set; }

        [Required]
        [StringLength(32)]
        public string Brand { get; set; }

        [Required]
        [StringLength(25)]
        public string Model { get; set; }

        [Required]
        [StringLength(50)]
        public string NumberPlate { get; set; }

        public int SeatNr { get; set; }

        public decimal Km { get; set; }

        public decimal BasicPrice { get; set; }

        public int? IncludedKm { get; set; }

        public decimal? PricePer100Km { get; set; }

        public decimal? PenaltyPerDay { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reservation> Reservations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
