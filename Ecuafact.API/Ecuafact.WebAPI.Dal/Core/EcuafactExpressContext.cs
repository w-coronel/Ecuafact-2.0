using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Dal.Core
{
    public class EcuafactExpressContext : DbContext
    {
        #region Entities Set
        
        public DbSet<Issuer> Issuers { get; set; }
        /// <summary> Productos del Emisor/// </summary>
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        /// <summary>Contribuyentes del Emisor</summary>
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<ContributorType> ContributorTypes { get; set; }
        public DbSet<IdentificationType> IdentificationTypes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<DocumentType> ElectronicDocumentTypes { get; set; }
        public DbSet<VatRate> IvaRates { get; set; }
        public DbSet<IceRate> IceRates { get; set; }
        public DbSet<RequestSession> RequestSessions { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentDetail> DocumentDetails { get; set; }
        public DbSet<DocumentSequential> DocumentSequentials { get; set; }
        public DbSet<InvoiceInfo> InvoiceInfos { get; set; }
        public DbSet<SettlementInfo> SettlementInfos { get; set; }
        public DbSet<RetentionInfo> RetentionInfos { get; set; }
        public DbSet<RetentionDetail> RetentionDetails { get; set; }
        public DbSet<RetentionTax> RetentionTaxes { get; set; }
        public DbSet<RetentionRate> RetentionRates { get; set; }
        public DbSet<TaxType> TaxTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<TotalTax> TotalTaxes { get; set; }
        public DbSet<AdditionalField> AdditionalFields { get; set; }
        public DbSet<UserPermissions> UserPermissions { get; set; }
        public DbSet<SalesReport> SalesReport { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<PurchaseReport> PurchaseReport { get; set; }       
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<PurchasePayment> PurchasePayment { get; set; }
        public DbSet<PurchaseLog> PurchaseLog { get; set; }
        public DbSet<ElectronicSign> ElectronicSign { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<Subscriber> Subscriber { get; set; }
        public DbSet<ReferredUser> ReferredUser { get; set; }
        public DbSet<Subscription> Subscription { get; set; }
        public DbSet<PurchaseSubscription> PurchaseSubscription { get; set; }
        public DbSet<Agreement> Agreement { get; set; }
        public DbSet<Beneficiary> Beneficiary { get; set; }
        public DbSet<ReferenceCodes> ReferenceCodes { get; set; }
        public DbSet<BeneficiaryReferenceCode> BeneficiaryReferenceCode { get; set; }
        public DbSet<DebitNoteDetail> DebitNoteDetail { get; set; }
        public DbSet<Establishments> Establishments { get; set; }
        public DbSet<IssuePoint> IssuePoint { get; set; }
        public DbSet<LicenceType> LicenceType { get; set; }
        public DbSet<MediaCampaigns> MediaCampaigns { get; set; }
        public DbSet<UserCampaigns> UserCampaigns { get; set; }
        public DbSet<UserPayment> UserPayment { get; set; }
        public DbSet<ECommerce> ECommerce { get; set; }
        public DbSet<Purchases> Purchases { get; set; }
        public DbSet<SupportType> SupportType { get; set; }
        public DbSet<SubscriptionLog> SubscriptionLog { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<IdentificationSupplierType> IdentificationSupplierType { get; set; }      


        #endregion

        public EcuafactExpressContext() : base("EcuafactExpressApiDb")
        {
            //Database.SetInitializer(new MyDbContextDropCreateDatabaseAlways());
            //Database.SetInitializer(new MyDbContextCreateDatabaseIfNotExists());
            Database.SetInitializer<EcuafactExpressContext>(null);
        }      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<DocumentDetail>().Property(x => x.Amount).HasPrecision(18, 6);
            modelBuilder.Entity<DocumentDetail>().Property(x => x.UnitPrice).HasPrecision(18, 6);
            modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasPrecision(18, 6);
            base.OnModelCreating(modelBuilder);
        }
    }
}
