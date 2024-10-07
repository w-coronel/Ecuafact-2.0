using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Controllers;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models.Dtos;
using Ecuafact.WebAPI.Domain.Reporting;
using Sentry.Protocol;
using Ecuafact.WebAPI.Domain.Entities.Engine;

namespace Ecuafact.WebAPI.Models
{
    internal static class DocumentExtensions
    {
        static IssuerDto __issuer;
        static IssuerDto Issuer
        {
            get
            {
                __issuer = HttpContext.Current.Session.GetAuthenticatedIssuerSession();

                return __issuer;
            }
        }

        internal static Document ToDocument(this RetentionDto retention)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<RetentionDto, Document>());
            var mapper = conf.CreateMapper();

            return mapper.Map<Document>(retention);
        }

        internal static Document ToDocument(this InvoiceDto invoice)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<InvoiceDto, Document>());
            var mapper = conf.CreateMapper();

            return mapper.Map<Document>(invoice);
        }

        internal static Document ToDocument(this CreditNoteDto creditNote)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<CreditNoteDto, Document>());
            var mapper = conf.CreateMapper();

            return mapper.Map<Document>(creditNote);
        }

        internal static Document ToDocument(this ReferralGuideDto guide)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<ReferralGuideDto, Document>());
            var mapper = conf.CreateMapper();

            return mapper.Map<Document>(guide);
        }

        internal static RetentionDto ToRetention(this Document document)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<Document, RetentionDto>());
            var mapper = conf.CreateMapper();
            //return mapper.Map<RetentionDto>(document);
            var result = mapper.Map<RetentionDto>(document);
            if (result.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(result.AuthorizationDate))
            {
                //result.Status = DocumentStatusEnum.Authorized;
                result.StatusMsg = DocumentStatusEnum.Authorized.GetDisplayValue();
            }
            else if (result.Status == DocumentStatusEnum.Authorized)
            {
                result.Status = DocumentStatusEnum.Validated;
            }
            return result;
        }

        internal static InvoiceDto ToInvoice(this Document document)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<Document, InvoiceDto>());
            var mapper = conf.CreateMapper();
            //return mapper.Map<InvoiceDto>(document);
            var result = mapper.Map<InvoiceDto>(document);
            if(result.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(result.AuthorizationDate))
            {
                //modificar caundo este el app acutalizada
                //Status = DocumentStatusEnum.Validated;
                result.StatusMsg = DocumentStatusEnum.Authorized.GetDisplayValue();
            }
            else if (result.Status == DocumentStatusEnum.Authorized)
            {
                result.Status = DocumentStatusEnum.Validated;
            }
            return result;
        }

        internal static SettlementDto ToSettlement(this Document document)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<Document, SettlementDto>());
            var mapper = conf.CreateMapper();
            //return mapper.Map<SettlementDto>(document);
            var result = mapper.Map<SettlementDto>(document);
            if (result.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(result.AuthorizationDate))
            {
                //modificar caundo este el app acutalizada
                // result.Status = DocumentStatusEnum.Authorized;
                result.StatusMsg = DocumentStatusEnum.Authorized.GetDisplayValue();
            }
            else if (result.Status == DocumentStatusEnum.Authorized)
            {
                result.Status = DocumentStatusEnum.Validated;
            }
            return result;
        }

        internal static CreditNoteDto ToCreditNote(this Document document)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<Document, CreditNoteDto>());
            var mapper = conf.CreateMapper();
            //return mapper.Map<CreditNoteDto>(document);
            var result = mapper.Map<CreditNoteDto>(document);
            if (result.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(result.AuthorizationDate))
            {
                //modificar caundo este el app acutalizada
                //result.Status = DocumentStatusEnum.Authorized;
                result.StatusMsg = DocumentStatusEnum.Authorized.GetDisplayValue();
            }
            else if (result.Status == DocumentStatusEnum.Authorized)
            {
                result.Status = DocumentStatusEnum.Validated;
            }
            return result;
           
        }

        internal static DebitNoteDto ToDebitNote(this Document document)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<Document, DebitNoteDto>());
            var mapper = conf.CreateMapper();
            //return mapper.Map<DebitNoteDto>(document);
            var result = mapper.Map<DebitNoteDto>(document);
            if (result.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(result.AuthorizationDate))
            {
                //modificar caundo este el app acutalizada
                //result.Status = DocumentStatusEnum.Authorized;
                result.StatusMsg = DocumentStatusEnum.Authorized.GetDisplayValue();
            }
            else if (result.Status == DocumentStatusEnum.Authorized)
            {
                result.Status = DocumentStatusEnum.Validated;
            }
            return result;
        }      

        internal static ReferralGuideDto ToReferralGuide(this Document document)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<Document, ReferralGuideDto>());
            var mapper = conf.CreateMapper();
            //return mapper.Map<ReferralGuideDto>(document);
            var result = mapper.Map<ReferralGuideDto>(document);
            if (result.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(result.AuthorizationDate))
            {
                //modificar caundo este el app acutalizada
                //result.Status = DocumentStatusEnum.Authorized;
                result.StatusMsg = DocumentStatusEnum.Authorized.GetDisplayValue();
            }
            else if (result.Status == DocumentStatusEnum.Authorized)
            {
                result.Status = DocumentStatusEnum.Validated;
            }
            return result;
        }

        internal static ReportResult GetReportBuilder(this Document document, Issuer issuer, string reportType)
        {
            ReportBase<Document> reportBuilder = default;
            switch (document.GetDocumentType())
            {
                case DocumentTypeEnum.Invoice:
                    if (reportType.ToUpper().Contains("POS"))
                        reportBuilder = new InvoicePosReport(issuer, reportType);
                    else
                        reportBuilder = new InvoiceReport(issuer);                        
                    break;
                case DocumentTypeEnum.CreditNote:
                    reportBuilder = new CreditNoteReport(issuer);
                    break;
                case DocumentTypeEnum.ReferralGuide:
                    reportBuilder = new ReferralGuideReport(issuer);
                    break;
                case DocumentTypeEnum.RetentionReceipt:
                    reportBuilder = new RetentionReport(issuer);
                    break;
                case DocumentTypeEnum.PurchaseSettlement:
                    reportBuilder = new PurchaseSettlementReport(issuer);
                    break;
                case DocumentTypeEnum.DebitNote:
                    reportBuilder = new DebitNoteReport(issuer);
                    break;
            }

            if (reportBuilder != null)
            {              
                return reportBuilder.Render(document.DocumentReporting(), reportType);
            }

            throw new Exception("El documento no tiene configurado un formato de impresión");
        }

        internal static DocumentTypeEnum GetDocumentType(this Document document)
        {
            if (Enum.TryParse(document.DocumentTypeCode, out DocumentTypeEnum docTypeInt))
            {
                return docTypeInt;
            }

            return DocumentTypeEnum.Invoice;
        }

        internal static Document MapTo(this Document document, DocumentRequestBase requestModel, string documentType)
        {
            if (document == null)
            {
                document = new Document();
            }

            document.DocumentTypeCode = documentType;
            document.RUC = Issuer.RUC;
            document.EstablishmentCode = requestModel.EstablishmentCode ?? Issuer.EstablishmentCode;
            document.IssuePointCode = requestModel.IssuePointCode ?? Issuer.IssuePointCode;
            document.BussinesName = Issuer.BussinesName;
            document.TradeName = Issuer.TradeName;
            document.Emails = requestModel.EmailAddresses;
            document.Status = (DocumentStatusEnum)Convert.ToInt16(requestModel.Status);
            document.MainAddress = Issuer.MainAddress; // Es la direccion principal de la empresa
            document.IssuedOn = Convert.ToDateTime(requestModel.IssuedOn, new CultureInfo("es"));
            document.EstablishmentAddress = !string.IsNullOrWhiteSpace(requestModel.EstablishmentAddress) ? requestModel.EstablishmentAddress : Issuer.EstablishmentAddress;// Es la direccion del establecimiento
            document.ContributorId = requestModel.ContributorId;
            document.ContributorIdentificationType = requestModel.IdentificationType;
            document.ContributorIdentification = requestModel.Identification;
            document.ContributorName = requestModel.ContributorName;
            document.ContributorAddress = requestModel.Address;
            document.Currency = requestModel.Currency;
            document.Reason = requestModel.Reason;
            document.Total = requestModel.Total;

            // Los datos adicionales del documento
            if (requestModel.AdditionalFields != null && requestModel.AdditionalFields.Any())
            {
                document.AdditionalFields.AddRange(requestModel.AdditionalFields?.Select(ad => ad.ToAdditionalField()));
            }

            return document;
        }

        internal static Document MapTo2(this Document document, DocumentRequestBase requestModel, Issuer issuer, string documentType)
        {
            if (document == null)
            {
                document = new Document();
            }

            document.DocumentTypeCode = documentType;
            document.RUC = issuer.RUC;
            document.EstablishmentCode = requestModel.EstablishmentCode ?? issuer.EstablishmentCode;
            document.IssuePointCode = requestModel.IssuePointCode ?? issuer.IssuePointCode;
            document.BussinesName = issuer.BussinesName;
            document.TradeName = issuer.TradeName;
            document.Emails = requestModel.EmailAddresses;
            document.Status = (DocumentStatusEnum)Convert.ToInt16(requestModel.Status);
            document.MainAddress = issuer.MainAddress; // Es la direccion principal de la empresa
            document.IssuedOn = Convert.ToDateTime(requestModel.IssuedOn, new CultureInfo("es"));
            document.EstablishmentAddress = !string.IsNullOrWhiteSpace(requestModel.EstablishmentAddress) ? requestModel.EstablishmentAddress : issuer.EstablishmentAddress;// Es la direccion del establecimiento
            document.ContributorId = requestModel.ContributorId;
            document.ContributorIdentificationType = requestModel.IdentificationType;
            document.ContributorIdentification = requestModel.Identification;
            document.ContributorName = requestModel.ContributorName;
            document.ContributorAddress = requestModel.Address;
            document.Currency = requestModel.Currency;
            document.Reason = requestModel.Reason;
            document.Total = requestModel.Total;

            // Los datos adicionales del documento
            if (requestModel.AdditionalFields != null && requestModel.AdditionalFields.Any())
            {
                document.AdditionalFields.AddRange(requestModel.AdditionalFields?.Select(ad => ad.ToAdditionalField()));
            }

            return document;
        }

        internal static Document ToDocument2(this DocumentRequestBase requestModel, Issuer issuer, string documentType)
        {
            var document = new Document();
            return document.MapTo2(requestModel, issuer, documentType);
        }

        internal static Document ToDocument(this DocumentRequestBase requestModel, string documentType)
        {
            var document = new Document();
            return document.MapTo(requestModel, documentType);
        }

        internal static InvoiceInfo Build(this InvoiceInfo info, InvoiceRequestModel requestModel)
        {
            if (info == null)
            {
                info = new InvoiceInfo();
            }

            /************************** DEPRECATED ****************************/
            info.EstablishmentAddress = string.IsNullOrEmpty(Issuer.EstablishmentAddress) ? Issuer.MainAddress : Issuer.EstablishmentAddress;
            info.IdentificationType = requestModel.IdentificationType;
            info.Identification = requestModel.Identification;
            info.BussinesName = requestModel.ContributorName;
            info.Currency = Issuer.Currency;
            /*******************************************************************/

            info.ReferralGuide = requestModel.ReferralGuide;
            info.SubtotalVat = requestModel.SubtotalVat;
            info.SubtotalVatZero = requestModel.SubtotalVatZero;
            info.SubtotalExempt = requestModel.SubtotalExempt;
            info.SubtotalNotSubject = requestModel.SubtotalNotSubject;
            info.Subtotal = requestModel.Subtotal;
            info.TotalDiscount = requestModel.TotalDiscount;
            info.Total = requestModel.Total;
            info.SpecialConsumTax = requestModel.SpecialConsumTax;
            info.ValueAddedTax = requestModel.ValueAddedTax;
            info.Tip = requestModel.Tip;
            info.Address = requestModel.Address;
            info.IssuedOn = requestModel.IssuedOn;

            //Detalle de los productos para la factura            
            info.Details.AddRange(requestModel.Details.Select(model => model.ToDocumentDetail()).ToList());
            info.Details.ForEach(x => x.DocumentId = info.InvoiceInfoId);

            // El detalle de los totales igualmente se autogenera ya no es necesario enviarlo en el requestmodel           
            info.TotalTaxes.AddRange(GetTotalTaxes(info.Details));          
            info.TotalTaxes.ForEach(x => x.DocumentId = info.InvoiceInfoId);


            // Detalle del metodo de pago acordado para la factura:
            info.Payments.AddRange(requestModel.Payments.Select(pay => pay.ToPayment()).ToList());
            info.Payments.ForEach(x => x.DocumentId = info.InvoiceInfoId);

            return info;
        }

        internal static InvoiceInfo Build2(this InvoiceInfo info, Issuer issuer, InvoiceRequestModel requestModel)
        {
            if (info == null)
            {
                info = new InvoiceInfo();
            }

            /************************** DEPRECATED ****************************/
            info.EstablishmentAddress = string.IsNullOrEmpty(issuer.EstablishmentAddress) ? issuer.MainAddress : issuer.EstablishmentAddress;
            info.IdentificationType = requestModel.IdentificationType;
            info.Identification = requestModel.Identification;
            info.BussinesName = requestModel.ContributorName;
            info.Currency = issuer.Currency;
            /*******************************************************************/

            info.ReferralGuide = requestModel.ReferralGuide;
            info.SubtotalVat = requestModel.SubtotalVat;
            info.SubtotalVatZero = requestModel.SubtotalVatZero;
            info.SubtotalExempt = requestModel.SubtotalExempt;
            info.SubtotalNotSubject = requestModel.SubtotalNotSubject;
            info.Subtotal = requestModel.Subtotal;
            info.TotalDiscount = requestModel.TotalDiscount;
            info.Total = requestModel.Total;
            info.SpecialConsumTax = requestModel.SpecialConsumTax;
            info.ValueAddedTax = requestModel.ValueAddedTax;
            info.Tip = requestModel.Tip;
            info.Address = requestModel.Address;
            info.IssuedOn = requestModel.IssuedOn;

            //Detalle de los productos para la factura            
            info.Details.AddRange(requestModel.Details.Select(model => model.ToDocumentDetail()).ToList());
            info.Details.ForEach(x => x.DocumentId = info.InvoiceInfoId);

            // El detalle de los totales igualmente se autogenera ya no es necesario enviarlo en el requestmodel           
            info.TotalTaxes.AddRange(GetTotalTaxes(info.Details));
            info.TotalTaxes.ForEach(x => x.DocumentId = info.InvoiceInfoId);


            // Detalle del metodo de pago acordado para la factura:
            info.Payments.AddRange(requestModel.Payments.Select(pay => pay.ToPayment()).ToList());
            info.Payments.ForEach(x => x.DocumentId = info.InvoiceInfoId);

            return info;
        }

        internal static CreditNoteInfo Build(this CreditNoteInfo info, CreditNoteRequestModel requestModel)
        {
            if (info == null)
            {
                info = new CreditNoteInfo();
            }

            info.IdentificationType = requestModel.IdentificationType;
            info.Identification = requestModel.Identification;
            info.BussinesName = requestModel.ContributorName;
            info.Currency = Issuer.Currency;
            info.SubtotalVat = requestModel.SubtotalVat;
            info.SubtotalVatZero = requestModel.SubtotalVatZero;
            info.SubtotalExempt = requestModel.SubtotalExempt;
            info.SubtotalNotSubject = requestModel.SubtotalNotSubject;
            info.Subtotal = requestModel.Subtotal;
            info.TotalDiscount = requestModel.TotalDiscount;
            info.Total = requestModel.Total;
            info.SpecialConsumTax = requestModel.SpecialConsumTax;
            info.ValueAddedTax = requestModel.ValueAddedTax;
            info.Tip = requestModel.Tip;
            info.Address = requestModel.Address;
            info.IssuedOn = requestModel.IssuedOn;
            info.ReferenceDocumentId = requestModel.ReferenceDocumentId;
            info.ReferenceDocumentCode = requestModel.ReferenceDocumentCode;
            info.ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber;
            info.ReferenceDocumentDate = requestModel.ReferenceDocumentDate.ToDateTimeString();
            info.ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth;
            info.Reason = requestModel.Reason;
            info.ModifiedValue = requestModel.Total;

            // Detalle de los productos para la factura
            info.Details.AddRange(requestModel.Details.Select(model => model.ToDocumentDetail()));
            info.Details.ForEach(x => x.DocumentId = info.CreditNoteInfoId);

            // El detalle de los totales igualmente se autogenera ya no es necesario enviarlo en el requestmodel
            info.TotalTaxes.AddRange(GetTotalTaxes(info.Details));
            info.TotalTaxes.ForEach(x => x.DocumentId = info.CreditNoteInfoId);

            // Detalle del metodo de pago acordado para la factura:
            info.Payments.AddRange(requestModel.Payments.Select(pay => pay.ToPayment()));
            info.Payments.ForEach(x => x.DocumentId = info.CreditNoteInfoId);

            return info;
        }

        internal static DebitNoteInfo Build(this DebitNoteInfo info, DebitNoteRequestModel requestModel)
        {
            if (info == null)
            {
                info = new DebitNoteInfo();
            }

            info.IdentificationType = requestModel.IdentificationType;
            info.Identification = requestModel.Identification;
            info.BussinesName = requestModel.ContributorName;
            info.Currency = Issuer.Currency;
            info.SubtotalVat = requestModel.SubtotalVat;
            info.SubtotalVatZero = requestModel.SubtotalVatZero;
            info.SubtotalExempt = requestModel.SubtotalExempt;
            info.SubtotalNotSubject = requestModel.SubtotalNotSubject;
            info.Subtotal = requestModel.Subtotal;
            info.TotalDiscount = requestModel.TotalDiscount;
            info.Total = requestModel.Total;
            info.SpecialConsumTax = requestModel.SpecialConsumTax;
            info.ValueAddedTax = requestModel.ValueAddedTax;           
            info.Address = requestModel.Address;
            info.IssuedOn = requestModel.IssuedOn;
            info.ReferenceDocumentId = requestModel.ReferenceDocumentId;
            info.ReferenceDocumentCode = requestModel.ReferenceDocumentCode;
            info.ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber;
            info.ReferenceDocumentDate = requestModel.ReferenceDocumentDate.ToDateTimeString();
            info.ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth;           
            info.ModifiedValue = requestModel.Total;
            info.Reason = requestModel.Reason;

            // Detalle de los productos para la nota debito
            info.DebitNoteDetail.AddRange(requestModel.Details.Select(model => model.ToDebitNoteDetail()));
            info.DebitNoteDetail.ForEach(x => x.DebitNoteInfoId = info.DebitNoteInfoId);

            // El detalle de los totales igualmente se autogenera ya no es necesario enviarlo en el requestmodel
            info.TotalTaxes.AddRange(GetTotalTaxes(info.DebitNoteDetail));
            info.TotalTaxes.ForEach(x => x.DocumentId = info.DebitNoteInfoId);

            // Detalle del metodo de pago acordado para la nota debito:
            info.Payments.AddRange(requestModel.Payments.Select(pay => pay.ToPayment()));
            info.Payments.ForEach(x => x.DocumentId = info.DebitNoteInfoId);

            return info;
        }

        internal static SettlementInfo Build(this SettlementInfo info, SettlementRequestModel requestModel)
        {
            if (info == null)
            {
                info = new SettlementInfo();
            }

            info.IdentificationType = requestModel.IdentificationType;
            info.Identification = requestModel.Identification;
            info.BussinesName = requestModel.ContributorName;
            info.Currency = Issuer.Currency;
            info.SubtotalVat = requestModel.SubtotalVat;
            info.SubtotalVatZero = requestModel.SubtotalVatZero;
            info.SubtotalExempt = requestModel.SubtotalExempt;
            info.SubtotalNotSubject = requestModel.SubtotalNotSubject;
            info.Subtotal = requestModel.Subtotal;
            info.TotalDiscount = requestModel.TotalDiscount;
            info.Total = requestModel.Total;
            info.SpecialConsumTax = requestModel.SpecialConsumTax;
            info.ValueAddedTax = requestModel.ValueAddedTax;
            info.Address = requestModel.Address;
            info.IssuedOn = requestModel.IssuedOn;

            // Detalle de los productos para la factura
            info.Details.AddRange(requestModel.Details.Select(model => model.ToDocumentDetail()));
            info.Details.ForEach(x => x.DocumentId = info.SettlementInfoId);

            // El detalle de los totales igualmente se autogenera ya no es necesario enviarlo en el requestmodel
            info.TotalTaxes.AddRange(GetTotalTaxes(info.Details));
            info.TotalTaxes.ForEach(x => x.DocumentId = info.SettlementInfoId);

            // Detalle del metodo de pago acordado para la factura:
            info.Payments.AddRange(requestModel.Payments.Select(pay => pay.ToPayment()));
            info.Payments.ForEach(x => x.DocumentId = info.SettlementInfoId);

            return info;
        }

        internal static RetentionInfo Build(this RetentionInfo info, RetentionRequestModel requestModel)
        {
            if (info == null)
            {
                info = new RetentionInfo();
            }

            info.IdentificationType = requestModel.IdentificationType;
            info.Identification = requestModel.Identification;
            info.BusinessName = requestModel.ContributorName;
            info.Currency = Issuer.Currency;
            info.IssuedOn = requestModel.IssuedOn;
            info.ContributorId = requestModel.ContributorId;
            info.Reason = requestModel.Reason;
            info.FiscalPeriod = requestModel.FiscalPeriod;
            info.FiscalAmount = requestModel.FiscalAmount;

            // Informacion de la referencia
            info.ReferenceDocumentId = requestModel.ReferenceDocumentId;
            info.ReferenceDocumentCode = requestModel.ReferenceDocumentCode;
            info.ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber;
            info.ReferenceDocumentDate = requestModel.ReferenceDocumentDate.ToDateTimeString();
            info.ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth;
            info.ReferenceDocumentAmount = requestModel.ReferenceDocumentAmount;
            info.ReferenceDocumentVat = requestModel.ReferenceDocumentVat;
            info.ReferenceDocumentTotal = requestModel.ReferenceDocumentTotal;
            info.ReferenceDocumentSubtotalVat = requestModel.ReferenceDocumentSubtotalVat;
            info.ReferenceDocumentSubtotalVatZero = requestModel.ReferenceDocumentSubtotalVatZero;
            info.ReferenceDocumentSubtotalNotSubject = requestModel.ReferenceDocumentSubtotalNotSubject;
            info.ReferenceDocumentSubtotalExempt = requestModel.ReferenceDocumentSubtotalExempt;
            // Informacion sustento para el comprobante
            info.RetentionObjectType = requestModel.RetentionObjectType;
            info.SupportCode = requestModel.SupportCode;
            info.AccountingRegistrationDate = requestModel.AccountingRegistrationDate;
            info.PaymentResident = requestModel.PaymentResident ?? "01";
            info.RelatedParty = requestModel.RelatedParty;

            // validacion para los detalles
            if (requestModel.Details != null) {
                info.Details.AddRange(requestModel.Details.Select(model => model.ToRetentionDetail()));
            }

            // validación para los totales impuestos del documento soporte
            if (requestModel?.ReferenceDocumentTotalTax?.Count > 0){
                info.TotalTaxes.AddRange(requestModel.ReferenceDocumentTotalTax.Select(model => model.ToTotalTax()));
                info.TotalTaxes.ForEach(x => x.DocumentId = info.RetentionInfoId);
            }

            // validación para los tipos de pago del documento soporte
            if (requestModel?.ReferenceDocumentPayments?.Count > 0)
            {
                info.Payments.AddRange(requestModel.ReferenceDocumentPayments.Select(model => model.ToPayment()));
                info.Payments.ForEach(x => x.DocumentId = info.RetentionInfoId);
            }

            return info;
        }

        internal static ReferralGuideInfo Build(this ReferralGuideInfo info, ReferralGuideRequestModel requestModel)
        {
            if (info == null)
            {
                info = new ReferralGuideInfo();
            }

            info.IdentificationType = requestModel.IdentificationType;
            info.Identification = requestModel.Identification;
            info.BusinessName = requestModel.ContributorName;
            info.IssuedOn = requestModel.IssuedOn;
            info.ContributorId = requestModel.ContributorId;
            info.SenderAddress = requestModel.Address;
            info.OriginAddress = requestModel.OriginAddress;
            info.DestinationAddress = requestModel.DestinationAddress;
            info.DriverId = requestModel.DriverId;
            info.DriverIdentificationType = requestModel.DriverIdentificationType;
            info.DriverIdentification = requestModel.DriverIdentification;
            info.DriverName = requestModel.DriverName;
            info.CarPlate = requestModel.CarPlate;
            info.ShippingStartDate = requestModel.ShippingStartDate;
            info.ShippingEndDate = requestModel.ShippingEndDate;
            info.RecipientId = requestModel.RecipientId;
            info.RecipientIdentificationType = requestModel.RecipientIdentificationType;
            info.RecipientIdentification = requestModel.RecipientIdentification;
            info.RecipientName = requestModel.RecipientName;
            info.RecipientAddress = requestModel.DestinationAddress;
            info.Reason = requestModel.Reason;
            info.DAU = requestModel.DAU;
            info.RecipientEstablishment = requestModel.RecipientEstablishment;
            info.ShipmentRoute = requestModel.ShipmentRoute;
            info.ReferenceDocumentId = requestModel.ReferenceDocumentId;
            info.ReferenceDocumentCode = requestModel.ReferenceDocumentCode;
            info.ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber;
            info.ReferenceDocumentDate = string.IsNullOrWhiteSpace(requestModel.ReferenceDocumentNumber) ? null: requestModel.ReferenceDocumentDate.ToDateTimeString();
            info.ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth;

            if (requestModel.Details != null)
                info.Details.AddRange(requestModel.Details.ConvertAll(detail => detail.ToReferralGuideDetail()));

            return info;
        }

        internal static Document DocumentReporting(this Document document)
        {
            if (document != null)
            {
                //remover el campo adicional de artesano calificado del emisor
                var skilledCraftsman = document.AdditionalFields.FirstOrDefault(d => (d.Name ?? "").ToUpper().Contains(TextoInfoAdicionalEnum.IsSkilledCraftsman.GetDisplayValue().ToUpper()) 
                                       && d.IsCarrier != 1);                   
                if(skilledCraftsman != null)
                {
                    document.AdditionalFields.Remove(skilledCraftsman);
                }
                //remover el campo adicional de regimen general del emisor
                var isGeneralRegime = document.AdditionalFields.FirstOrDefault(d => ((d.Name ?? "").ToUpper().Contains(TextoInfoAdicionalEnum.IsGeneralRegime.GetDisplayValue().ToUpper())
                   || (d.Value ?? "").ToUpper().Contains(TextoInfoAdicionalEnum.IsGeneralRegime.GetCoreValue().ToUpper()) &&  d.IsCarrier != 1));
                if(isGeneralRegime != null)
                {
                    document.AdditionalFields.Remove(isGeneralRegime);
                }
                //remover el campo adicional de sociedades simplificadas del emisor
                var isSimplifCompRegime = document.AdditionalFields.FirstOrDefault(d => ((d.Name ?? "").ToUpper().Contains(TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetDisplayValue().ToUpper())
                || (d.Value ?? "").ToUpper().Contains(TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetCoreValue().ToUpper()) && d.IsCarrier != 1));
                if (isSimplifCompRegime != null)
                {
                    document.AdditionalFields.Remove(isSimplifCompRegime);
                }
            }
           
            return document;
        }

        /// <summary>
        /// HELPER METHODS
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>

        private static List<TotalTax> GetTotalTaxes(List<DocumentDetail> details)
        {
            // Llenamos el total de impuestos
            var totalTaxes = new List<TotalTax>();
            foreach (var item in details.Select(model => model.Taxes))
            {
                item.ForEach(tax =>
                {
                    // Buscamos el impuesto
                    var taxItem = totalTaxes.Find(x => x.PercentageTaxCode == tax.PercentageCode);

                    // Si existe en la lista general del total de impuestos se suman los valores respectivos
                    if (taxItem != null)
                    {                       
                        taxItem.TaxableBase += tax.TaxableBase;
                        taxItem.TaxValue += (tax.TaxableBase * (tax.Rate / 100));  //tax.TaxValue;
                    }
                    else
                    {
                        // De lo contrario se tiene que agregar una nueva linea del total de impuestos
                        totalTaxes.Add(
                            new TotalTax
                            {
                                PercentageTaxCode = tax.PercentageCode,
                                TaxRate = tax.Rate,
                                TaxCode = tax.Code,
                                TaxableBase = tax.TaxableBase,
                                TaxValue = tax.TaxValue,
                                AditionalDiscount = 0M
                            });
                    }
                });
            }

            return totalTaxes;
        }

        private static List<TotalTax> GetTotalTaxes(List<DebitNoteDetail> details)
        {
            // Llenamos el total de impuestos
            var totalTaxes = new List<TotalTax>();
            details.ForEach(tax =>
            {
                // Buscamos el impuesto
                var taxItem = totalTaxes.Find(x => x.PercentageTaxCode == tax.PercentageCode);

                // Si existe en la lista general del total de impuestos se suman los valores respectivos
                if (taxItem != null)
                {
                    taxItem.TaxableBase += tax.Value;
                    taxItem.TaxValue += tax.TaxValue;
                }
                else
                {
                    // De lo contrario se tiene que agregar una nueva linea del total de impuestos
                    totalTaxes.Add(
                        new TotalTax
                        {
                            PercentageTaxCode = tax.PercentageCode,
                            TaxRate = tax.TaxRate,
                            TaxCode = tax.TaxCode,
                            TaxableBase = tax.Value,
                            TaxValue = tax.TaxValue,
                            AditionalDiscount = 0M
                        });
                }
            });
            return totalTaxes;
        }

        private static List<TotalTax> GetTotalTaxes(List<DocumentDetail> details, List<DocumentDetailModel> detailsModel)
        {
            // Llenamos el total de impuestos
            var totalTaxes = new List<TotalTax>();
            foreach (var item in details.Select(model => model.Taxes))
            {
                item.ForEach(tax =>
                {
                    // Buscamos el impuesto
                    var taxItem = totalTaxes.Find(x => x.PercentageTaxCode == tax.PercentageCode);
                    
                    // Si existe en la lista general del total de impuestos se suman los valores respectivos
                    if (taxItem != null)
                    {                        
                        taxItem.TaxableBase += tax.TaxableBase;
                        taxItem.TaxValue += tax.TaxValue;
                    }
                    else
                    {
                        // De lo contrario se tiene que agregar una nueva linea del total de impuestos
                        totalTaxes.Add(
                            new TotalTax
                            {
                                PercentageTaxCode = tax.PercentageCode,
                                TaxRate = tax.Rate,
                                TaxCode = tax.Code,
                                TaxableBase = tax.TaxableBase,
                                TaxValue = tax.TaxValue,
                                AditionalDiscount = 0M
                            });
                    }
                });
            }

            return totalTaxes;
        }

        private static List<TotalTax> GetTotalTaxes(List<DocumentDetail> details, List<TotalTax> totalTaxes)
        {
            // Llenamos el total de impuestos           
            foreach (var item in details.Select(model => model.Taxes))
            {
                item.ForEach(tax =>
                {
                    // Buscamos el impuesto
                    var taxItem = totalTaxes.Find(x => x.PercentageTaxCode == tax.PercentageCode);

                    // Si existe en la lista general del total de impuestos se suman los valores respectivos
                    if (taxItem != null)
                    {
                        taxItem.TaxableBase += tax.TaxableBase;
                        taxItem.TaxValue += tax.TaxValue;
                    }
                    else
                    {
                        // De lo contrario se tiene que agregar una nueva linea del total de impuestos
                        totalTaxes.Add(
                            new TotalTax
                            {
                                PercentageTaxCode = tax.PercentageCode,
                                TaxRate = tax.Rate,
                                TaxCode = tax.Code,
                                TaxableBase = tax.TaxableBase,
                                TaxValue = tax.TaxValue,
                                AditionalDiscount = 0M
                            });
                    }
                });
            }

            return totalTaxes;
        }
       
    }
}