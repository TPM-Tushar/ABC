﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataAPI.Entity.ECDATA_PENDOCS
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ECDATA_PENDOCSEntities : DbContext
    {
        public ECDATA_PENDOCSEntities()
            : base("name=ECDATA_PENDOCSEntities")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 480;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DocPendingHistory_PENDOCS> DocPendingHistory_PENDOCS { get; set; }
        public virtual DbSet<DocumentMaster_PENDOCS> DocumentMaster_PENDOCS { get; set; }
        public virtual DbSet<NoticeMaster_PENDOCS> NoticeMaster_PENDOCS { get; set; }
        public virtual DbSet<PendingReasonMaster_PENDOCS> PendingReasonMaster_PENDOCS { get; set; }
        public virtual DbSet<SROMaster_PENDOCS> SROMaster_PENDOCS { get; set; }
    }
}