namespace Fahrtenbuch.Library
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Trip")]
    public partial class Trip
    {
        public Guid ID { get; set; }

        public Guid VehicleID { get; set; }

        public Guid EmployeeID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal KmBegin { get; set; }

        public decimal? KmEnd { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Vehicle Vehicle { get; set; }
    }
}
