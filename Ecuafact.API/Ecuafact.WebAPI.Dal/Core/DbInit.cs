using System;
using System.Data.Entity;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Dal.Core
{
    public class MyDbContextDropCreateDatabaseAlways : DropCreateDatabaseAlways<EcuafactExpressContext>
    {
        protected override void Seed(EcuafactExpressContext context)
        {
            MyDbContextSeeder.Seed(context);
        }
    }

    public class MyDbContextCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<EcuafactExpressContext>
    {
        protected override void Seed(EcuafactExpressContext context)
        {
            MyDbContextSeeder.Seed(context);
        }
    }

    public static class MyDbContextSeeder
    {
        public static void Seed(EcuafactExpressContext ctx)
        {
            InitIssuer(ctx);
            InitPaymentTypes(ctx);
            InitProductTypes(ctx);
            InitContributorTypes(ctx);
            InitIdentificationTypes(ctx);
            InitIVARates(ctx);
            InitICERates(ctx);
            InitElectronicDocumentTypes(ctx);
            ctx.SaveChanges();
        }

        static void InitIssuer(EcuafactExpressContext dbContext)
        {
            var issuer1 = new Issuer
            {
                IsEnabled = true,
                RUC = "0992882549001",
                BussinesName = "Consultora Informatica Ecuadorian Nexus - Ecuanexus IT",
                MainAddress = "Av. del Periodista 420 entre Calle Olimpo y Calle 10ma",
                TradeName = "Ecuanexus",
                IsAccountingRequired = true,
                EstablishmentCode = "001",
                IssuePointCode = "001"
            };
            dbContext.Issuers.Add(issuer1);

            var issuer2 = new Issuer
            {
                IsEnabled = true,
                RUC = "0919821850001",
                BussinesName = "MORANTE ARREAGA CHRISTIAN ANDRES",
                TradeName = "MORANTE ARREAGA CHRISTIAN ANDRES",
                MainAddress = "GUAYAQUIL",
                IsAccountingRequired = false,
                EstablishmentCode = "001",
                IssuePointCode = "001"
            };
            dbContext.Issuers.Add(issuer2);
            InitRequestSessions(dbContext, issuer1, issuer2);
        }
        
        static void InitPaymentTypes(EcuafactExpressContext dbContext)
        {
            dbContext.PaymentMethods.Add(new PaymentMethod { SriCode = "01",Name = "SIN UTILIZACION DEL SISTEMA FINANCIERO", IsEnabled = true });
            dbContext.PaymentMethods.Add(new PaymentMethod { SriCode = "15",Name = "COMPENSACION DE DEUDAS", IsEnabled = true });
            dbContext.PaymentMethods.Add(new PaymentMethod { SriCode = "16",Name = "TARJETAS DE DEBITO", IsEnabled = true });
            dbContext.PaymentMethods.Add(new PaymentMethod { SriCode = "17",Name = "DINERO ELECTRONICO", IsEnabled = true });
            dbContext.PaymentMethods.Add(new PaymentMethod { SriCode = "18",Name = "TARJETA PREPAGO", IsEnabled = true });
            dbContext.PaymentMethods.Add(new PaymentMethod { SriCode = "19",Name = "TARJETA DE CREDITO", IsEnabled = true });
            dbContext.PaymentMethods.Add(new PaymentMethod { SriCode = "20",Name = "OTROS CON UTILIZACION DEL SISTEMA FINANCIERO", IsEnabled = true });
            dbContext.PaymentMethods.Add(new PaymentMethod { SriCode = "21",Name = "ENDOSO DE TITULOS", IsEnabled = true });
        }

        static void InitProductTypes(EcuafactExpressContext dbContext)
        {
            dbContext.ProductTypes.Add(new ProductType(){ Name = "BIEN", IsEnabled = true});
            dbContext.ProductTypes.Add(new ProductType(){ Name = "SERVICIO", IsEnabled = true});
        }

        static void InitContributorTypes(EcuafactExpressContext dbContext)
        {
            dbContext.ContributorTypes.Add(new ContributorType() { Name = "CLIENTE", IsEnabled = true });
            dbContext.ContributorTypes.Add(new ContributorType() { Name = "SUJETO RETENIDO", IsEnabled = true });
            dbContext.ContributorTypes.Add(new ContributorType() { Name = "DESTINATARIO", IsEnabled = true });
        }

        static void InitIdentificationTypes(EcuafactExpressContext dbContext)
        {
            dbContext.IdentificationTypes.Add(new IdentificationType { Name = "CEDULA", SriCode = "05", IsEnabled = true });
            dbContext.IdentificationTypes.Add(new IdentificationType { Name = "RUC", SriCode = "04", IsEnabled = true });
            dbContext.IdentificationTypes.Add(new IdentificationType { Name = "PASAPORTE", SriCode = "06", IsEnabled = true });
            dbContext.IdentificationTypes.Add(new IdentificationType { Name = "CONSUMIDOR FINAL", SriCode = "07", IsEnabled = true });
            dbContext.IdentificationTypes.Add(new IdentificationType { Name = "IDENTIFICACION DEL EXTERIOR", SriCode = "08", IsEnabled = true });
            dbContext.IdentificationTypes.Add(new IdentificationType { Name = "PLACA", SriCode = "09", IsEnabled = true });
        }

        static void InitIVARates(EcuafactExpressContext dbContext)
        {
            dbContext.IvaRates.Add(new VatRate { Name = "0%", RateValue = 0M, SriCode = "0", IsEnabled = true });
            dbContext.IvaRates.Add(new VatRate { Name = "12%", RateValue = 12M, SriCode = "2", IsEnabled = true });
            dbContext.IvaRates.Add(new VatRate { Name = "14%", RateValue = 14M, SriCode = "3", IsEnabled = true });
            dbContext.IvaRates.Add(new VatRate { Name = "NO OBJETO DE IMPUESTO", RateValue = 0M, SriCode = "6", IsEnabled = true });
            dbContext.IvaRates.Add(new VatRate { Name = "EXENTO DE IVA", RateValue = 0M, SriCode = "7", IsEnabled = true });
        }

        static void InitICERates(EcuafactExpressContext dbContext)
        {
            dbContext.IceRates.Add(new IceRate { Name = "NO APLICA", SriCode = "0", IsEnabled = true, Rate = 0 });
            dbContext.IceRates.Add(new IceRate { Name = "Cocinas, calefones y otros de uso doméstico a gas SRI", SriCode = "3670", IsEnabled = true, Rate = 100.00M });
            dbContext.IceRates.Add(new IceRate { Name = "Focos incandescentes excepto aquellos utilizados como insumos automotrices", SriCode = "3640", IsEnabled = true, Rate = 100.00M });
            dbContext.IceRates.Add(new IceRate { Name = "Servicios de televisión pagada", SriCode = "3092", IsEnabled = true, Rate = 15M });
        }

        static void InitElectronicDocumentTypes(EcuafactExpressContext dbContext)
        {
            dbContext.ElectronicDocumentTypes.Add(new DocumentType { Name ="FACTURA", SriCode = "01", IsEnabled = true });
            dbContext.ElectronicDocumentTypes.Add(new DocumentType { Name ="NOTA DE CREDITO", SriCode = "04", IsEnabled = true });
            dbContext.ElectronicDocumentTypes.Add(new DocumentType { Name ="NOTA DE DEBITO", SriCode = "05", IsEnabled = true });
            dbContext.ElectronicDocumentTypes.Add(new DocumentType { Name ="GUIA DE REMISION", SriCode = "06", IsEnabled = true });
            dbContext.ElectronicDocumentTypes.Add(new DocumentType { Name ="COMPROBANTE DE RETENCION", SriCode = "07", IsEnabled = true });
            dbContext.ElectronicDocumentTypes.Add(new DocumentType { Name = "LIQUIDACION DE COMPRA", SriCode = "03", IsEnabled = true });
        }

        static void InitRequestSessions(EcuafactExpressContext dbContext, Issuer issuer1, Issuer issuer2)
        {
            dbContext.RequestSessions.Add(new RequestSession{ Issuer = issuer1, Token = "33A08B05-0E2C-47F6-B0A1-D0F50F7A7850", IsEnabled = true, CreatedOn = DateTime.Now, Username = "TESTUSER" });
            dbContext.RequestSessions.Add(new RequestSession{ Issuer= issuer2, Token = "E82A4349-6130-4890-A2FF-D0302EFD0B4C", IsEnabled = true, CreatedOn = DateTime.Now, Username = "TESTUSER" });
        }
    }
}
