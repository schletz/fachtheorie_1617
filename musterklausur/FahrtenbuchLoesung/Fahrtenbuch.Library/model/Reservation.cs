namespace Fahrtenbuch.Library
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reservation")]
    public partial class Reservation
    {
        public Guid ID { get; set; }

        public Guid VehicleID { get; set; }

        public Guid EmployeeID { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
