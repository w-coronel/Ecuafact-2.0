using Ecuafact.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.Web
{
    public static class DocumentExtensions
    {
        public static string MicroEmpresa => "Contribuyente Régimen Microempresa";
        public static string AgenteRetencion => "Agente de Retención No. Resolución:";

        public static InvoiceRequestModel ToRequest(this InvoiceModel model)
        {
            if (model != null)
            { 
                var result = new InvoiceRequestModel
                {
                    Id = model.Id,
                    DocumentNumber = model.DocumentNumber,
                    Address = model.ContributorAddress,
                    ContributorId = model.ContributorId,
                    ContributorName = model.ContributorName,
                    Currency = model.Currency,
                    DocumentTypeCode = model.DocumentTypeCode,
                    EmailAddresses = model.Emails,
                    Identification = model.ContributorIdentification,
                    IdentificationType = model.ContributorIdentificationType,
                    IssuedOn = model.IssuedOn.ToString("dd/MM/yyyy"),
                    Phone = "9999999",
                    Reason = model.Reason,
                    ReferralGuide = model.InvoiceInfo.ReferralGuide,
                    SpecialConsumTax = model.InvoiceInfo.SpecialConsumTax,
                    Status = model.Status,

                    Subtotal = model.InvoiceInfo.Subtotal,
                    SubtotalExempt = model.InvoiceInfo.SubtotalExempt,
                    SubtotalNotSubject = model.InvoiceInfo.SubtotalNotSubject,
                    SubtotalVat = model.InvoiceInfo.SubtotalVat,
                    SubtotalVatZero = model.InvoiceInfo.SubtotalVatZero,
                    TotalDiscount = model.InvoiceInfo.TotalDiscount,
                    ValueAddedTax = model.InvoiceInfo.ValueAddedTax,
                    Tip = model.InvoiceInfo.Tip,
                    Total = model.InvoiceInfo.Total,

                    AdditionalFields = model.AdditionalFields,
                    Details = model.InvoiceInfo.Details,
                    Payments = model.InvoiceInfo.Payments,
                    Term = model.InvoiceInfo.Payments.FirstOrDefault()?.Term ?? 0,
                    TimeUnit = model.InvoiceInfo.Payments.FirstOrDefault()?.TimeUnit ?? "dias",

                    EstablishmentCode = model.EstablishmentCode,
                    IssuePointCode = model.IssuePointCode,
                    EstablishmentAddress = model.EstablishmentAddress

                };

                if (result.Details!=null)
                {
                    foreach (var detail in result.Details)
                    {
                        if (detail.Taxes != null)
                        {
                            var taxiva = detail.Taxes.FirstOrDefault(x => x.Code == "2");
                            detail.ValueAddedTaxCode = taxiva?.PercentageCode ?? "0";
                            detail.ValueAddedTaxValue = taxiva?.TaxValue ?? 0M;
                        
                            var taxice = detail.Taxes.FirstOrDefault(x => x.Code == "3");
                            detail.SpecialConsumTaxCode = taxice?.PercentageCode ?? "0";
                            detail.SpecialConsumTaxValue = taxice?.TaxValue ?? 0M;
                        }

                    }
                }

                return result;
            }

            //ToRequest
            return default;
        }

        public static SettlementRequestModel ToRequest(this SettlementModel model)
        {
            var request = new SettlementRequestModel
            {
                Id = model.Id,
                DocumentNumber = model.DocumentNumber,
                Address = model.ContributorAddress,
                ContributorId = model.ContributorId,
                ContributorName = model.ContributorName,
                Currency = model.Currency,
                DocumentTypeCode = model.DocumentTypeCode,
                EmailAddresses = model.Emails,
                Identification = model.ContributorIdentification,
                IdentificationType = model.ContributorIdentificationType,
                IssuedOn = model.IssuedOn.ToString("dd/MM/yyyy"),
                Phone = "9999999",
                Reason = model.Reason,
                ReferralGuide = model.SettlementInfo.ReferralGuide,
                SpecialConsumTax = model.SettlementInfo.SpecialConsumTax,
                Status = model.Status,

                Subtotal = model.SettlementInfo.Subtotal,
                SubtotalExempt = model.SettlementInfo.SubtotalExempt,
                SubtotalNotSubject = model.SettlementInfo.SubtotalNotSubject,
                SubtotalVat = model.SettlementInfo.SubtotalVat,
                SubtotalVatZero = model.SettlementInfo.SubtotalVatZero,
                TotalDiscount = model.SettlementInfo.TotalDiscount,
                ValueAddedTax = model.SettlementInfo.ValueAddedTax,
                Tip = model.SettlementInfo.Tip,
                Total = model.SettlementInfo.Total,

                AdditionalFields = model.AdditionalFields,
                Details = model.SettlementInfo.Details,
                Payments = model.SettlementInfo.Payments,


                EstablishmentCode = model.EstablishmentCode,
                IssuePointCode = model.IssuePointCode,
                EstablishmentAddress = model.EstablishmentAddress
            };

            //ToRequest
            return request;
        }

        public static CreditNoteRequestModel ToRequest(this CreditNoteModel model)
        {
            var request = new CreditNoteRequestModel
            {
                Id = model.Id,
                Address = model.ContributorAddress,
                DocumentNumber = model.DocumentNumber,
                ContributorId = model.ContributorId,
                ContributorName = model.ContributorName,
                Currency = model.Currency,
                EmailAddresses = model.Emails,
                Identification = model.ContributorIdentification,
                IdentificationType = model.ContributorIdentificationType,
                IssuedOn = model.IssuedOn.ToString("dd/MM/yyyy"),
                Phone = "9999999", // No se 
                Reason = model.Reason,
                SpecialConsumTax = model.CreditNoteInfo.SpecialConsumTax,
                Status = model.Status,
                Subtotal = model.CreditNoteInfo.Subtotal,
                SubtotalExempt = model.CreditNoteInfo.SubtotalExempt,
                SubtotalNotSubject = model.CreditNoteInfo.SubtotalNotSubject,
                SubtotalVat = model.CreditNoteInfo.SubtotalVat,
                SubtotalVatZero = model.CreditNoteInfo.SubtotalVatZero,
                Tip = model.CreditNoteInfo.Tip,
                Total = model.CreditNoteInfo.Total,
                TotalDiscount = model.CreditNoteInfo.TotalDiscount,
                ValueAddedTax = model.CreditNoteInfo.ValueAddedTax,
                AdditionalFields = model.AdditionalFields,
                Details = model.CreditNoteInfo.Details.ConvertAll<CreditNoteDetailModel>(detail => new CreditNoteDetailModel
                {
                    MainCode = detail.MainCode,
                    AuxCode = detail.AuxCode,
                    Description = detail.Description,
                    Discount = detail.Discount,
                    ProductId = detail.ProductId,
                    SubTotal = detail.SubTotal,
                    Taxes = detail.Taxes,
                    UnitPrice = detail.UnitPrice,
                    ValueAddedTaxCode = detail.Taxes.FirstOrDefault().Code,
                    ValueAddedTaxValue = detail.Taxes.FirstOrDefault().TaxValue,
                    Amount = detail.Amount,
                    Name1 = detail.Name1,
                    Name2 = detail.Name2,
                    Name3 = detail.Name3,
                    Value1 = detail.Value1,
                    Value2 = detail.Value2,
                    Value3 = detail.Value3,

                }),
                ModifiedValue = model.CreditNoteInfo.ModifiedValue,
                ReferenceDocumentAuth = model.CreditNoteInfo.ReferenceDocumentAuth,
                ReferenceDocumentCode = model.CreditNoteInfo.ReferenceDocumentCode,
                ReferenceDocumentDate = model.CreditNoteInfo.ReferenceDocumentDate,
                ReferenceDocumentId = model.CreditNoteInfo.ReferenceDocumentId,
                ReferenceDocumentNumber = model.CreditNoteInfo.ReferenceDocumentNumber,

                EstablishmentCode = model.EstablishmentCode,
                IssuePointCode = model.IssuePointCode,
                EstablishmentAddress = model.EstablishmentAddress
            };

            //ToRequest
            return request;
        }

        public static RetentionRequestModel ToRequest(this RetentionModel model)
        {
            if (model != null && model.Id > 0)
            {
                var request = new RetentionRequestModel
                {
                    Id = model.Id,
                    ContributorId = model.RetentionInfo.ContributorId,
                    ContributorName = model.RetentionInfo.BusinessName,                   
                    Identification = model.RetentionInfo.Identification,
                    IdentificationType = model.RetentionInfo.IdentificationType,                  
                    Status =model.Status,
                    IssuedOn = $"{model.IssuedOn:dd/MM/yyyy}",
                    FiscalPeriod = model.RetentionInfo.FiscalPeriod,
                    Reason = model.Reason,
                    FiscalAmount = model.RetentionInfo.FiscalAmount,
                    Balance =  (Convert.ToDecimal(model.RetentionInfo.ReferenceDocumentAmount) + (Convert.ToDecimal(model.RetentionInfo.ReferenceDocumentVat))) - Convert.ToDecimal(model.RetentionInfo.FiscalAmount),
                    SupportCode = model.RetentionInfo.SupportCode,
                    AccountingRegistrationDate = model.RetentionInfo.AccountingRegistrationDate,
                    PaymentResident = model.RetentionInfo.PaymentResident,
                    RelatedParty = model.RetentionInfo.RelatedParty,
                    Details = model.RetentionInfo.Details.ConvertAll<RetentionDetailModel>(detail => new RetentionDetailModel
                    {
                        ReferenceDocumentCode = detail.ReferenceDocumentCode,
                        ReferenceDocumentDate = detail.ReferenceDocumentDate,
                        ReferenceDocumentNumber = detail.ReferenceDocumentNumber,
                        ReferenceDocumentTotal = detail.ReferenceDocumentTotal,
                        ReferenceDocumentAuth = detail.ReferenceDocumentAuth,
                        RetentionTaxCode = detail.RetentionTaxCode,
                        RetentionTaxId = detail.RetentionTaxId,
                        TaxBase = detail.TaxBase,
                        TaxRate = detail.TaxRate,
                        TaxTypeCode = detail.TaxTypeCode,
                        TaxValue = detail.TaxValue,
                        SupportCode = detail.SupportCode,
                        AccountingRegistrationDate = detail.AccountingRegistrationDate,
                        PaymentResident = detail.PaymentResident

                    }),
                    AdditionalFields = model.AdditionalFields,
                    ReferenceDocumentId = model.RetentionInfo.ReferenceDocumentId ?? 0,
                    ReferenceDocumentCode = model.RetentionInfo.ReferenceDocumentCode,
                    ReferenceDocumentNumber = model.RetentionInfo.ReferenceDocumentNumber,
                    ReferenceDocumentDate = model.RetentionInfo.ReferenceDocumentDate,
                    ReferenceDocumentAuth = model.RetentionInfo.ReferenceDocumentAuth,
                    ReferenceDocumentAmount = model.RetentionInfo.ReferenceDocumentAmount,
                    ReferenceDocumentVat = model.RetentionInfo.ReferenceDocumentVat,
                    ReferenceDocumentTotal = model.RetentionInfo.ReferenceDocumentTotal ?? 0,
                    ReferenceDocumentSubtotalVat = model.RetentionInfo.ReferenceDocumentSubtotalVat ?? 0,
                    ReferenceDocumentSubtotalVatZero = model.RetentionInfo.ReferenceDocumentSubtotalVatZero ?? 0,
                    ReferenceDocumentSubtotalExempt = model.RetentionInfo.ReferenceDocumentSubtotalExempt ?? 0,
                    ReferenceDocumentSubtotalNotSubject = model.RetentionInfo.ReferenceDocumentSubtotalNotSubject ?? 0,
                    RetentionObjectType = model.RetentionInfo.RetentionObjectType,
                    Address = model.ContributorAddress,
                    EmailAddresses = model.Emails,
                    Phone = "9999999",

                    EstablishmentCode = model.EstablishmentCode,
                    IssuePointCode = model.IssuePointCode,
                    EstablishmentAddress = model.EstablishmentAddress,
                    ReferenceDocumentTotalTax = model.RetentionInfo?.TotalTaxes?.ConvertAll<TotalTaxModel>(totalTax => new TotalTaxModel
                    {
                        PercentageTaxCode = totalTax.PercentageTaxCode,
                        TaxableBase = totalTax.TaxableBase,
                        TaxCode = totalTax.TaxCode,
                        TaxRate = totalTax.TaxRate,
                        TaxValue = totalTax.TaxValue                    
                    }),
                    ReferenceDocumentPayments = model.RetentionInfo.Payments,
                };

                return request;
            }


            return default;
        }
         
        public static ReferralGuideRequestModel ToRequest(this ReferralGuideModel model)
        {

            if (model != null && model.Id > 0)
            {
                var result = new ReferralGuideRequestModel
                {
                    Id = model.Id,
                    DocumentNumber = model.DocumentNumber,
                    Address = model.ContributorAddress,
                    ContributorId = model.ContributorId,
                    ContributorName = model.ContributorName,
                    Currency = model.Currency,
                    EmailAddresses = model.Emails,
                    Identification = model.ContributorIdentification,
                    IdentificationType = model.ContributorIdentificationType,
                    IssuedOn = model.IssuedOn.ToString("dd/MM/yyyy"),
                    Phone = "9999999",
                    Reason = model.Reason,
                    Status = model.Status,                   
                    AdditionalFields = model.AdditionalFields,
                    Details = model.ReferralGuideInfo.Details,
                    ReferenceDocumentId = model.ReferralGuideInfo.ReferenceDocumentId,
                    ReferenceDocumentCode = model.ReferralGuideInfo.ReferenceDocumentCode,
                    ReferenceDocumentNumber = model.ReferralGuideInfo.ReferenceDocumentNumber,
                    ReferenceDocumentAuth = model.ReferralGuideInfo.ReferenceDocumentAuth,
                    ReferenceDocumentDate = model.ReferralGuideInfo.ReferenceDocumentDate,
                    CarPlate = model.ReferralGuideInfo.CarPlate,
                    DAU = model.ReferralGuideInfo.DAU,
                    DestinationAddress = model.ReferralGuideInfo.DestinationAddress,
                    DriverId = model.ReferralGuideInfo.DriverId,
                    DriverIdentification = model.ReferralGuideInfo.DriverIdentification,
                    DriverIdentificationType = model.ReferralGuideInfo.DriverIdentificationType,
                    DriverName = model.ReferralGuideInfo.DriverName,
                    OriginAddress = model.ReferralGuideInfo.OriginAddress,
                    RecipientEstablishment = model.ReferralGuideInfo.RecipientEstablishment,
                    RecipientId = model.ReferralGuideInfo.RecipientId,
                    RecipientIdentification = model.ReferralGuideInfo.RecipientIdentification,
                    RecipientIdentificationType = model.ReferralGuideInfo.RecipientIdentificationType,
                    RecipientName = model.ReferralGuideInfo.RecipientName,
                    ShipmentRoute = model.ReferralGuideInfo.ShipmentRoute,
                    ShippingEndDate = model.ReferralGuideInfo.ShippingEndDate,
                    ShippingStartDate = model.ReferralGuideInfo.ShippingStartDate,

                    EstablishmentCode = model.EstablishmentCode,
                    IssuePointCode = model.IssuePointCode,
                    EstablishmentAddress = model.EstablishmentAddress


                };


                return result;
            }

            //ToRequest
            return default;
        }

        public static DebitNoteRequestModel ToRequest(this DebitNoteModel model)
        {
            if (model != null && model.Id > 0)
            {
                var result = new DebitNoteRequestModel
                {
                    Id = model.Id,
                    DocumentNumber = model.DocumentNumber,
                    Address = model.ContributorAddress,
                    ContributorId = model.ContributorId,
                    ContributorName = model.ContributorName,
                    Currency = model.Currency,                   
                    EmailAddresses = model.Emails,
                    Identification = model.ContributorIdentification,
                    IdentificationType = model.ContributorIdentificationType,
                    IssuedOn = model.IssuedOn.ToString("dd/MM/yyyy"),
                    Phone = "9999999",
                    Reason = model.Reason,                  
                    Status = model.Status,
                    Subtotal = model.DebitNoteInfo.Subtotal,
                    SubtotalExempt = model.DebitNoteInfo.SubtotalExempt,
                    SubtotalNotSubject = model.DebitNoteInfo.SubtotalNotSubject,
                    SubtotalVat = model.DebitNoteInfo.SubtotalVat,
                    SubtotalVatZero = model.DebitNoteInfo.SubtotalVatZero,                    
                    ValueAddedTax = model.DebitNoteInfo.ValueAddedTax,                   
                    Total = model.DebitNoteInfo.Total,
                    AdditionalFields = model.AdditionalFields,
                    Details = model.DebitNoteInfo.DebitNoteDetail,
                    Payments = model.DebitNoteInfo.Payments,                
                    ReferenceDocumentId = model.DebitNoteInfo.ReferenceDocumentId,
                    ReferenceDocumentCode = model.DebitNoteInfo.ReferenceDocumentCode,
                    ReferenceDocumentNumber = model.DebitNoteInfo.ReferenceDocumentNumber,
                    ReferenceDocumentAuth = model.DebitNoteInfo.ReferenceDocumentAuth,
                    ReferenceDocumentDate = model.DebitNoteInfo.ReferenceDocumentDate,

                    EstablishmentCode = model.EstablishmentCode,
                    IssuePointCode = model.IssuePointCode,
                    EstablishmentAddress = model.EstablishmentAddress
                };

               
                return result;
            }

            //ToRequest
            return default;
        }

        public static List<AdditionalFieldModel> ToAdditionalField(this List<AdditionalFieldModel> model)
        {
            var _issuer = SessionInfo.UserSession.Issuer;
            if (model?.Count > 0)
            {
                //Artesano calificado       
                if (_issuer.IsSkilledCraftsman)
                {
                    var addArtesano = model.Where(a => a.Name?.ToUpper() == TextoInfoAdicionalEnum.IsSkilledCraftsman.GetDisplayValue().ToUpper())?.FirstOrDefault();
                    if (addArtesano == null)
                    {
                        model.Add(new AdditionalFieldModel
                        {
                            LineNumber = model.Count + 1,
                            Name = TextoInfoAdicionalEnum.IsSkilledCraftsman.GetDisplayValue().ToUpper(),
                            Value = _issuer.SkilledCraftsmanNumber,
                            IsCarrier = 1
                        });
                    }
                }
                else
                {
                    var deleteArtesano = model.Where(a => a.Name?.ToUpper() == TextoInfoAdicionalEnum.IsSkilledCraftsman.GetDisplayValue().ToUpper())?.FirstOrDefault();
                    if (deleteArtesano != null)
                    {
                        model.Remove(deleteArtesano);
                    }
                }
                //regimen general
                if (_issuer.IsGeneralRegime)
                {
                    var addGeeneral = model.Where(a => a.Name?.ToUpper() == TextoInfoAdicionalEnum.IsGeneralRegime.GetDisplayValue().ToUpper()
                    || a.Value?.ToUpper() == TextoInfoAdicionalEnum.IsGeneralRegime.GetValorCore().ToUpper())?.FirstOrDefault();

                    if (addGeeneral == null)
                    {
                        model.Add(new AdditionalFieldModel
                        {
                            LineNumber = model.Count + 1,
                            Name = TextoInfoAdicionalEnum.IsGeneralRegime.GetDisplayValue().ToUpper(),
                            Value = TextoInfoAdicionalEnum.IsGeneralRegime.GetValorCore()
                        });
                    }
                }
                else
                {
                    var deleteGeeneral = model.Where(a => a.Name?.ToUpper() == TextoInfoAdicionalEnum.IsGeneralRegime.GetDisplayValue().ToUpper()
                    || a.Value?.ToUpper() == TextoInfoAdicionalEnum.IsGeneralRegime.GetValorCore().ToUpper())?.FirstOrDefault();
                    if (deleteGeeneral != null)
                    {
                        model.Remove(deleteGeeneral);
                    }
                }
                ///Sociedades Simplificadas

                if (_issuer.IsSimplifiedCompaniesRegime)
                {
                    var addSociedades = model.Where(a => a.Name?.ToUpper() == TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetDisplayValue().ToUpper()
                    || a.Value?.ToUpper() == TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetValorCore().ToUpper())?.FirstOrDefault();

                    if (addSociedades == null)
                    {
                        model.Add(new AdditionalFieldModel
                        {
                            LineNumber = model.Count + 1,
                            Name = TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetDisplayValue().ToUpper(),
                            Value = TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetValorCore()
                        });
                    }
                }
                else
                {
                    var deleteSociedades = model.Where(a => a.Name?.ToUpper() == TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetDisplayValue().ToUpper()
                        || a.Value?.ToUpper() == TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetValorCore().ToUpper())?.FirstOrDefault();
                    if (deleteSociedades != null)
                    {
                        model.Remove(deleteSociedades);
                    }
                }
            }
            else {
                if(model == null) { model = new List<AdditionalFieldModel>();}
                if (_issuer.IsSkilledCraftsman)
                {
                    model.Add(new AdditionalFieldModel
                    {
                        LineNumber = model.Count + 1,
                        Name = TextoInfoAdicionalEnum.IsSkilledCraftsman.GetDisplayValue().ToUpper(),
                        Value = _issuer.SkilledCraftsmanNumber
                    });
                }

                if (_issuer.IsGeneralRegime)
                {
                    model.Add(new AdditionalFieldModel
                    {
                        LineNumber = model.Count + 1,
                        Name = TextoInfoAdicionalEnum.IsGeneralRegime.GetDisplayValue().ToUpper(),
                        Value = TextoInfoAdicionalEnum.IsGeneralRegime.GetValorCore()
                    });
                }
                if (_issuer.IsSimplifiedCompaniesRegime)
                {
                    model.Add(new AdditionalFieldModel
                    {
                        LineNumber = model.Count + 1,
                        Name = TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetDisplayValue().ToUpper(),
                        Value = TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetValorCore()
                    });
                }              
            }

            // cuando el usuario tiene selecciona emisor con rol cooperativa
            if (SessionInfo.UserRole == Ecuafact.Web.Models.SecuritySessionRole.Cooperative)
            {
                if (model?.Count > 0)
                {
                    var placa = model.Where(s => s.Name == "PLACA").FirstOrDefault();
                    if (placa != null)
                    {
                        model.ForEach(s =>
                        {
                            if (s.Name == "PLACA TRANSPORTISTA")
                            {
                                s.Value = placa.Value;
                            }
                        });

                        model.Remove(placa);
                    }
                }
            }
            return model;
        }

    }
}