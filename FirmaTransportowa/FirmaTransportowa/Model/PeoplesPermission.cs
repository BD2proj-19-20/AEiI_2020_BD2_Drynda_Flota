//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FirmaTransportowa.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class PeoplesPermission
    {
        public int personId { get; set; }
        public int permissionId { get; set; }
        public System.DateTime grantDate { get; set; }
        public Nullable<System.DateTime> revokeDate { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual Permission Permission { get; set; }
    }
}