namespace Suppl.Library
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Unterrichtsstunden")]
    public partial class Unterrichtsstunden
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Unterrichtsstunden()
        {
            Vertretungens = new HashSet<Vertretungen>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UntisId { get; set; }

        public int Wochentag { get; set; }

        public int Stunde { get; set; }

        [Required]
        [StringLength(10)]
        public string Klasse { get; set; }

        [Required]
        [StringLength(10)]
        public string Lehrer { get; set; }

        [Required]
        [StringLength(25)]
        public string Fach { get; set; }

        [StringLength(25)]
        public string Raum { get; set; }

        public virtual Lehrer Lehrer1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vertretungen> Vertretungens { get; set; }
    }
}
