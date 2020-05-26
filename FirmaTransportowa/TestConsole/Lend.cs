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
    
    public partial class Lend
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Lend()
        {
            this.Reservations = new HashSet<Reservation>();
        }
    
        public int id { get; set; }
        public int carId { get; set; }
        public int personId { get; set; }
        public System.DateTime lendDate { get; set; }
        public Nullable<System.DateTime> plannedReturnDate { get; set; }
        public Nullable<System.DateTime> returnDate { get; set; }
        public int startOdometer { get; set; }
        public Nullable<int> endOdometer { get; set; }
        public double startFuel { get; set; }
        public Nullable<double> endFuel { get; set; }
        public bool @private { get; set; }
        public Nullable<int> reservationId { get; set; }
        public string comments { get; set; }
    
        public virtual Car Car { get; set; }
        public virtual Person Person { get; set; }
        public virtual Reservation Reservation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}