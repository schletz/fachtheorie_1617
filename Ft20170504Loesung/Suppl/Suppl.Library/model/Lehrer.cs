namespace Suppl.Library
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Lehrer")]
    public partial class Lehrer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lehrer()
        {
            Unterrichtsstundens = new HashSet<Unterrichtsstunden>();
            Vertretungens = new HashSet<Vertretungen>();
        }

        [StringLength(10)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Zuname { get; set; }

        [Required]
        [StringLength(100)]
        public string Vorname { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Unterrichtsstunden> Unterrichtsstundens { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vertretungen> Vertretungens { get; set; }
    }
}
