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
    
    public partial class CarSupervisor
    {
        public int personId { get; set; }
        public int carId { get; set; }
        public System.DateTime beginDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
    
        public virtual Person Person { get; set; }
    }
}
