﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AEiI_2020_BD2_Drynda_FlotaEntities : DbContext
    {
        public AEiI_2020_BD2_Drynda_FlotaEntities()
            : base("name=AEiI_2020_BD2_Drynda_FlotaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<CarDestination> CarDestinations { get; set; }
        public virtual DbSet<CarExtra> CarExtras { get; set; }
        public virtual DbSet<CarModel> CarModels { get; set; }
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<CarSupervisor> CarSupervisors { get; set; }
        public virtual DbSet<Contractor> Contractors { get; set; }
        public virtual DbSet<Lend> Lends { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PeoplesPermission> PeoplesPermissions { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }
    }
}
