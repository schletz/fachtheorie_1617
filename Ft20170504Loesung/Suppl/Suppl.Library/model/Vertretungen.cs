namespace Suppl.Library
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Vertretungen")]
    public partial class Vertretungen
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UntisId { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "date")]
        public DateTime Tag { get; set; }

        [StringLength(10)]
        public string NeuerLehrer { get; set; }

        [StringLength(10)]
        public string NeuerRaum { get; set; }

        [StringLength(10)]
        public string NeuesFach { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NeuerTag { get; set; }

        public int NeueStunde { get; set; }

        public virtual Lehrer Lehrer { get; set; }

        public virtual Unterrichtsstunden Unterrichtsstunden { get; set; }
    }
}
