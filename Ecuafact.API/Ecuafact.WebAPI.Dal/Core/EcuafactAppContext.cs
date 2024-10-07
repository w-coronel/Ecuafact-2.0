﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ecuafact.WebAPI.Dal.Core
{
    using Ecuafact.WebAPI.Domain.Entities.Engine;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using Ecuafact.WebAPI.Domain.Entities.SRI;
    using Ecuafact.WebAPI.Domain.Entities;
    using Ecuafact.WebAPI.Domain.Entities.App;
    using Ecuafact.WebAPI.Domain.Dal.Core;

    public partial class EcuafactAppContext : DbContext
    {
        #region Entities Set
        public virtual DbSet<AdditionalInfo> AdditionalInfo { get; set; }
        public virtual DbSet<DocumentInfoDetail> DocumentInfoDetail { get; set; }
        public virtual DbSet<SupplierDocument> SupplierDocument { get; set; }
        public virtual DbSet<DetailTaxInfo> DetailTaxInfo { get; set; }
        public virtual DbSet<DocumentInfo> DocumentInfo { get; set; }
        //public virtual DbSet<Emisor> Emisor { get; set; }
        public virtual DbSet<VistaRetenciones> VistaRetenciones { get; set; }
        //public virtual DbSet<VistaFacturas> VistaFacturas { get; set; }
        public virtual DbSet<vwFacturas> vwFacturas { get; set; }
        public virtual DbSet<TipoSustento> TipoSustento { get; set; }
        public virtual DbSet<ApiV3_DocumentosData> ApiV3_DocumentosData { get; set; }        

        #endregion
        public EcuafactAppContext()
            : base("EcuafactAppServiceDb")
        {
            System.Data.Entity.Database.SetInitializer<EcuafactAppContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();          
            base.OnModelCreating(modelBuilder);
        }
    }

        
    public partial class EcuafactSRIContext : DbContext
    {
        #region Entities Set
        public virtual DbSet<INFORUC> InfoRUC { get; set; }
        public virtual DbSet<INFOREGIMEN> INFOREGIMEN { get; set; }

        #endregion
        public EcuafactSRIContext()
            : base("EcuafactSRIServiceDb")
        {
            System.Data.Entity.Database.SetInitializer<EcuafactSRIContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}