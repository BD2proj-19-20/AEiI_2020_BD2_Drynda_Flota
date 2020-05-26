//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestConsole
{
    using System;
    using System.Collections.Generic;
    
    public partial class Car
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Car()
        {
            this.Lends = new HashSet<Lend>();
            this.Reservations = new HashSet<Reservation>();
            this.CarExtras = new HashSet<CarExtra>();
        }
    
        public int id { get; set; }
        public string Registration { get; set; }
        public short engineCapacity { get; set; }
        public System.DateTime purchaseDate { get; set; }
        public Nullable<System.DateTime> saleDate { get; set; }
        public System.DateTime inspectionValidUntil { get; set; }
        public bool onService { get; set; }
        public int modelId { get; set; }
        public int destinationId { get; set; }
    
        public virtual CarDestination CarDestination { get; set; }
        public virtual CarModel CarModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lend> Lends { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reservation> Reservations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CarExtra> CarExtras { get; set; }
    }
}
