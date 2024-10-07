using AutoMapper;
using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Domain.Dal.Core;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.App;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecuafact.WebAPI.Models
{
    public static class DocumentReceivedExtensions
    {
        internal static DocumentReceived ToDocumentReceivedDto(this ApiV3_DocumentosData doc)
        {
            return new DocumentReceived
            {
                accessKey =  doc.ClaveAcceso ?? "DOC NO VÁLIDO. NO TIENE CLAVE DE ACCESO EN XML",
                authorizationDate = doc.FechaAut,
                authorizationNumber = doc.Numeroautorizacion ?? "SIN NÚMERO DE AUTORIZACION EN EL XML",
                codTypeDoc = doc.CodTipoDoc,
                date = doc.Fecha.Value.ToString("dd/MM/yyyy"),              
                identificationNumber = doc.NumeroIdentificacionEmisor,
                name = doc.RazonSocial,               
                pk = doc.Pk.ToString(),
                sequence = doc.Secuencia,
                state = doc.Estado.Value.ToString(),
                total = "$ " + doc.Total.ToString("0.00"),
                typeDoc = GetTipoDocumento(doc.CodTipoDoc),
                deductibleId = doc.IdDeducible,
                SupportTypeCode = doc.CodSustento                 
            };
        }
        internal static DocumentReceived ToDocumentReceived(this SupplierDocument doc)
        {
            return new DocumentReceived
            {
                accessKey = doc.AccessKey ?? "DOC NO VÁLIDO. NO TIENE CLAVE DE ACCESO EN XML",
                authorizationDate = doc.AuthorizationDate,
                authorizationNumber = doc.AuthorizationNumber ?? "SIN NÚMERO DE AUTORIZACION EN EL XML",
                codTypeDoc = doc.DocumentTypeCode,
                date = doc.AuthorizationDate,
                identificationNumber = doc.DocumentInfo.ToList().FirstOrDefault().Identification,               
                name = doc.DocumentInfo.ToList().FirstOrDefault().BusinessName,
                pk = doc.DocumentPk.ToString(),
                sequence = doc.Sequential,
                state = doc.Status.ToString(),
                total = "$ " + doc.DocumentInfo.ToList().FirstOrDefault().Total.ToString("0.00"),
                typeDoc = GetTipoDocumento(doc.DocumentTypeCode),
                deductibleId = 0,
                SupportTypeCode = doc.SupportTypeCode
            };
        }
        internal static SupplierDocument ToSupplierDocument(this SalesNoteRequestModel model)
        {
            var documentReceived = new SupplierDocument
            {
                DocumentId = Guid.NewGuid().ToString(),
                LastProcessSequential = 1,
                Environment = "2",
                IssueType = "1",
                BusinessName = model.IssuingBussinesName,
                TradeName = model.IssuingBussinesName,
                RUC = model.IssuingRuc,
                MainAddress = model.IssuingAddress,
                DocumentTypeCode = model.DocumentTypeCode,
                EstablishmentCode = model.EstablishmentCode,
                IssuePointCode = model.IssuePointCode,
                Sequential = model.Sequential,
                AuthorizationNumber = model.AuthorizationNumber,
                Status = (Int16)model.Status,
                AuthorizationDate = model.IssuedOn,
                CreatedOn = DateTime.Now,
                Emails = model.IssuingEmailAddresses,
                ProcessStatus = 5,
                PDF = $"{model.IssuingRuc}_{model.DocumentTypeCode}_{model.EstablishmentCode}_{model.IssuePointCode}_{model.Sequential}.{model.FileType}",
                IDMAIL = "1",
                AuthorizeNow = false,
                CloudMigration = false,
                MigracionRegion = false,
                Offline = true,
                Version ="",
                Id =""
            };
            documentReceived.DocumentInfo.Add(model.ToDocumentInfo());
            documentReceived.DocumentInfo.ToList().ForEach(doc => doc.DocumentPk = documentReceived.DocumentPk);
            documentReceived.Details.AddRange(model.Details.Select(det => det.ToDocumentInfoDetail()));
            var item = 1;
            documentReceived.Details.ToList().ForEach(det => { det.Sequence = $"{item}"; item++; });
            documentReceived.Details.ToList().ForEach(doc => doc.DocumentPk = documentReceived.DocumentPk);
            documentReceived.Payments.AddRange(model.Payments.Select(pay => pay.ToPaymentInfo()));
            documentReceived.Payments.ToList().ForEach(doc => doc.DocumentPk = documentReceived.DocumentPk);
            if (model.AdditionalFields != null && model.AdditionalFields.Any())
            {
                documentReceived.AdditionalFields.AddRange(model.AdditionalFields?.Select(ad => ad.ToAdditionalInfo()));
            }
            else
            {
                documentReceived.AdditionalFields = new List<AdditionalInfo> 
                {
                    new AdditionalInfo
                    {
                        Name = "Cliente",
                        Value = model.ContributorName,
                        LineNumber = 1
                    },
                    new AdditionalInfo
                    {
                        Name = "Email",
                        Value = model.EmailAddresses,
                        LineNumber = 2
                    },
                    new AdditionalInfo
                    {
                        Name = "Teléfono",
                        Value = model.Phone,
                        LineNumber = 3
                    },
                };
            }
            documentReceived.AdditionalFields.ToList().ForEach(doc => doc.DocumentPk = documentReceived.DocumentPk);
            return documentReceived;
        }
        internal static DocumentInfo ToDocumentInfo(this SalesNoteRequestModel model)
        {
            return new  DocumentInfo
            {                
                IssuedOn = model.IssuedOn,
                EstablishmentAddress = model.Address,
                IsSpecialContributor = model.IsSpecialContributor,
                IsAccountingRequired = model.IsAccountingRequired ? "SI" : "NO",
                IdentificationType = model.IdentificationType,
                Identification = model.Identification,
                BusinessName = model.ContributorName,
                Address = model.Address,
                ReferralGuide = model.ReferralGuide,
                Subtotal = model.Subtotal,
                TotalDiscount = model.TotalDiscount,
                Tip = model.Tip,
                Total = model.Total,
                SubtotalVat = model.SubtotalVat,
                SubtotalVatZero = model.SubtotalVatZero,
                Currency = model.Currency,
                Reason = model.Reason
            };

        }
        internal static PaymentInfo ToPaymentInfo(this PaymentModel paymentModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<PaymentModel, PaymentInfo>());
            var mapper = conf.CreateMapper();
            var payment = mapper.Map<PaymentInfo>(paymentModel);
            return payment;
        }
        internal static DocumentInfoDetail ToDocumentInfoDetail(this DocumentDetailModel model)
        {
            return new DocumentInfoDetail
            {
                MainCode = model.MainCode,
                AuxCode = model.AuxCode,
                Description = model.Description,
                Amount = model.Amount,
                UnitPrice = model.UnitPrice,
                Discount = model.Discount,
                Subtotal = model.SubTotal                
            };

        }
        internal static AdditionalInfo ToAdditionalInfo(this AdditionalFieldModel model)
        {
            return new AdditionalInfo
            {
                Name = model.Name,
                Value = model.Value,
                LineNumber = model.LineNumber                
            };

        }

        internal static DocumentReceived ToDocumentResponseDetail(this SupplierDocument doc)
        {

            var ivas = doc.DocumentInfo.FirstOrDefault().TotalTaxes.GroupBy(g => new { g.Code, g.PercentageCode }).Select(s=> s.First()).ToList() ;
            var _docReceived = new DocumentReceived
            {
                accessKey = doc.AccessKey ?? "DOC NO VÁLIDO. NO TIENE CLAVE DE ACCESO EN XML",
                authorizationDate = doc.AuthorizationDate,
                authorizationNumber = doc.AuthorizationNumber ?? "SIN NÚMERO DE AUTORIZACION EN EL XML",
                codTypeDoc = doc.DocumentTypeCode,
                date = doc.CreatedOn.Value.ToString("dd/MM/yyyy"),
                identificationNumber = doc.RUC,
                name = !String.IsNullOrEmpty(doc.TradeName) ? doc.TradeName : doc.BusinessName,
                pk = doc.DocumentPk.ToString(),
                sequence = string.Format("{0}-{1}-{2}", doc.EstablishmentCode, doc.IssuePointCode, doc.Sequential),
                state = doc.Status.Value.ToString(),
                typeDoc = doc.DocumentTypeCode,
                total = "$ " + doc.DocumentInfo.FirstOrDefault().Total.ToString("0.00"),
                discount = "$ " + doc.DocumentInfo.FirstOrDefault().TotalDiscount != null ? doc.DocumentInfo.FirstOrDefault().TotalDiscount.ToString("0.00") : "0.00",
                subTotal0 = "$ " + (doc.DocumentInfo.FirstOrDefault().SubtotalVatZero.HasValue ?  doc.DocumentInfo.FirstOrDefault().SubtotalVatZero.Value.ToString("0.00"): "0.00"),
                subTotal12 = "$ " + (doc.DocumentInfo.FirstOrDefault().SubtotalVat.HasValue ? doc.DocumentInfo.FirstOrDefault().SubtotalVat.Value.ToString("0.00") : "0.00"),
                subTotal15 = "$ " + (doc.DocumentInfo.FirstOrDefault().SubtotalVat15.HasValue ? doc.DocumentInfo.FirstOrDefault().SubtotalVat15.Value.ToString("0.00") : "0.00"),
                deductibleId =(doc.DeductibleId ?? 0),
                iva = (doc.DocumentInfo.FirstOrDefault().ValueAddedTax.HasValue ? doc.DocumentInfo.FirstOrDefault().ValueAddedTax.Value.ToString("0.00") : "0.00")
                
            };

            if (doc.DocumentInfo.FirstOrDefault().SubtotalVat.HasValue)
            {
                if(doc.DocumentInfo.FirstOrDefault().SubtotalVat.Value > 0)
                {
                    _docReceived.ivaType = "2";
                }
                
            }
            else if (doc.DocumentInfo.FirstOrDefault().SubtotalVat15.HasValue)
            {
                _docReceived.ivaType = "4";
            }
               


            if (_docReceived.codTypeDoc == "04" || _docReceived.codTypeDoc == "05")
            {
                _docReceived.documentSupport = doc.DocumentInfo.FirstOrDefault().ModifiedDocumentCode;
                _docReceived.dateDocumentSupport = doc.DocumentInfo.FirstOrDefault().SupportDocumentIssueDate;
                _docReceived.sequenceDocumentSupport = doc.DocumentInfo.FirstOrDefault().ModifiedDocumentNumber;
                _docReceived.reason = doc.DocumentInfo.FirstOrDefault().Reason;

            }
            else if (_docReceived.codTypeDoc == "06")
            {
                _docReceived.identificationNumber = doc.DocumentInfo.FirstOrDefault().Identification;
                _docReceived.name = doc.DocumentInfo.FirstOrDefault().BusinessName;
                _docReceived.plate = doc.DocumentInfo.FirstOrDefault().CarPlate;
                _docReceived.startPoint = doc.DocumentInfo.FirstOrDefault().OriginAddress;
                _docReceived.startDate = doc.DocumentInfo.FirstOrDefault().ShippingStartDate;
                _docReceived.endDate = doc.DocumentInfo.FirstOrDefault().ShippingEndDate;
                _docReceived.endPoint = doc.DocumentInfo.FirstOrDefault().RecipientAddress;

                _docReceived.documentSupport = doc.DocumentInfo.FirstOrDefault().ModifiedDocumentCode;
                _docReceived.dateDocumentSupport = doc.DocumentInfo.FirstOrDefault().SupportDocumentIssueDate;
                _docReceived.sequenceDocumentSupport = doc.DocumentInfo.FirstOrDefault().ModifiedDocumentNumber;
                _docReceived.authorizationDocumentDocumentSupport = doc.DocumentInfo.FirstOrDefault().ReferenceDocumentAuth;
                _docReceived.reason = doc.DocumentInfo.FirstOrDefault().Reason;
                _docReceived.receiverIdentificationNumber = doc.DocumentInfo.FirstOrDefault().RecipientIdentificationType;
                _docReceived.receiverName = doc.DocumentInfo.FirstOrDefault().RecipientName;
                _docReceived.customsDocument = ""; //documento aduanero
                _docReceived.receiverEstablishmentCode = "";
                _docReceived.route = doc.StatusMsg;
            }

            if (doc.DocumentInfo.FirstOrDefault().Total >= 0)           
                _docReceived.total = "$ " + doc.DocumentInfo.FirstOrDefault().Total.ToString("0.00");           
            else         
                _docReceived.total = "$ 0.00";

            _docReceived.details = new List<details>();
            if (_docReceived.codTypeDoc == "07")
            {
                if(doc.DocumentInfo?.FirstOrDefault().TotalTaxes?.Count > 0)
                {
                    doc.DocumentInfo.FirstOrDefault().TotalTaxes.ForEach(det => {
                        _docReceived.details.Add(new details {
                            typeDoc = GetTipoDocumento(det.ReferenceDocumentCode),
                            sequence = det.ReferenceDocumentNumber,
                            date = det.ReferenceDocumentDate,
                            fiscalYear = det.ReferenceDocumentDate.Substring(3),
                            taxable = ConverToMoney(det.TaxableBase != null ? det.TaxableBase.Value:0),
                            taxType = det.Code == "1" ? "RENTA" : "IVA",
                            taxCode = det.PercentageCode,
                            percentage = ConverToPercentage(det.Rate != null ? det.Rate.Value:0),
                            value = ConverToMoney(det.TaxValue != null ? det.TaxValue.Value:0),
                            pk = det.Id.ToString()
                        });
                    });
                }

            }
            else if (_docReceived.codTypeDoc == "05")
            {
                if (doc.Reason?.Count > 0)
                {
                    doc.Reason.ForEach(mot => {
                        _docReceived.details.Add(new details
                        {
                            reason = mot.Reason,
                            value = ConverToMoney((decimal)mot.Value),
                            pk = mot.Id.ToString()
                        });
                    });
                }
            }
            else if (_docReceived.codTypeDoc == "01" || _docReceived.codTypeDoc == "04" || _docReceived.codTypeDoc == "03" || _docReceived.codTypeDoc == "02")
            {
                if(doc.Details?.Count > 0)
                {
                    doc.Details.ForEach(det => {
                        _docReceived.details.Add(new details
                        {
                            code = det.MainCode,
                            code2 = det.AuxCode,
                            description = det.Description,
                            discount = "$ " + det.Discount.ToString("0.00"),
                            price = "$ " + det.UnitPrice.ToString("0.00"),
                            quantity = det.Amount.ToString("0"),
                            subTotal = "$ " + det.Subtotal.ToString("0.00"),
                            pk = det.Id.ToString(),
                            deductible = det.IsDeductible != null ? (det.IsDeductible.Value) ? doc.DeductibleId.Value : 0 : 0
                        });
                    });
                }
            }
            else if (_docReceived.codTypeDoc == "06")
            {
                if (doc.Details?.Count > 0)
                {
                    doc.Details.ForEach(det => {
                        _docReceived.details.Add(new details
                        {
                            code = det.MainCode,
                            code2 = det.AuxCode,
                            description = det.Description,                            
                            quantity = det.Amount.ToString("0"),                          
                            pk = det.Id.ToString(),
                            deductible = det.IsDeductible != null ? (det.IsDeductible.Value) ? doc.DeductibleId.Value : 0 : 0
                        });
                    });
                }
            }

            _docReceived.PaymentTypes = new List<PaymentType>();
            if(doc.Payments?.Count > 0)
            {
                doc.Payments.ForEach(p=> {
                    _docReceived.PaymentTypes.Add(new PaymentType {
                        code = p.PaymentMethodCode,
                        term = p.Term != null ? p.Term.Value:0,
                        total = p.Total != null ? p.Total.Value.ToString("0.00"):"0.00",
                        unitTime = p.TimeUnit
                    });
                 });
            }

            _docReceived.totalTaxs = new List<totalTax>();
            if (doc.DocumentInfo?.FirstOrDefault().TotalTaxes?.Count > 0)
            {
                doc.DocumentInfo.FirstOrDefault().TotalTaxes.ForEach(tt => {
                    _docReceived.totalTaxs.Add(new totalTax
                    {
                        taxCode = tt.Code,
                        percentageTaxCode = tt.PercentageCode,
                        taxableBase = tt.TaxableBase != null ? tt.TaxableBase.Value:0,
                        taxValue = tt.TaxValue != null ? tt.TaxValue.Value:0,
                        taxRate = tt.Rate != null ? tt.Rate.Value:0,
                        id = tt.Id
                    });
                });
            }

            return _docReceived;
        }

        private static string ConverToMoney(decimal value)
        {
            return "$ " + value.ToString("0.00");
        }
        private static string ConverToPercentage(decimal value)
        {
            return value.ToString("0.00") + "%";
        }
        private static string GetTipoDocumento(string codDoc)
        {
            switch (codDoc)
            {
                case "01":
                    return "Factura";
                    break;
                case "03":
                    return "Liquidación de Compra";
                    break;
                case "04":
                    return "Nota Credito";
                    break;
                case "05":
                    return "Nota de Débito";
                    break;
                case "06":
                    return "Guia de Remisión";
                    break;
                case "07":
                    return "Retención";
                    break;
                default:
                    break;
            }
            return codDoc + " - Desconocido";

        }
    }
}