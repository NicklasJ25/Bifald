//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bifald.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sager
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sager()
        {
            this.Afsluttede_pladser = new HashSet<Afsluttede_pladser>();
            this.Pladser = new HashSet<Pladser>();
            this.Kasser = new HashSet<Kasser>();
        }
    
        public string Sagsnummer { get; set; }
        public int KundeId { get; set; }
        public System.DateTime Opbevaring_startdato { get; set; }
        public Nullable<System.DateTime> Opbevaring_slutdato { get; set; }
        public bool Afsluttet { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Afsluttede_pladser> Afsluttede_pladser { get; set; }
        public virtual Kunder Kunder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pladser> Pladser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kasser> Kasser { get; set; }
    }
}
