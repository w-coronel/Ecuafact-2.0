﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EcuafactExpress.Dal.Core
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EcuafactAPPEntities : DbContext
    {
        public EcuafactAPPEntities()
            : base("name=EcuafactAPPEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdditionalInfo> campoAdicional { get; set; }
        public virtual DbSet<DocumentInfoDetail> detalle { get; set; }
        public virtual DbSet<SupplierDocument> documento { get; set; }
        public virtual DbSet<DetailTaxInfo> impuesto { get; set; }
        public virtual DbSet<DocumentInfo> infoFactura { get; set; }
    }
}
