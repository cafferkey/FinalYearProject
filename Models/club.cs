//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FYP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class club
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public club()
        {
            this.clubbooks = new HashSet<clubbook>();
            this.meetings = new HashSet<meeting>();
            this.members = new HashSet<member>();
        }
    
        public int clubid { get; set; }
        public string clubname { get; set; }
        public Nullable<int> creator { get; set; }
        public Nullable<int> admins { get; set; }
        public string description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<clubbook> clubbooks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<meeting> meetings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<member> members { get; set; }
    }
}