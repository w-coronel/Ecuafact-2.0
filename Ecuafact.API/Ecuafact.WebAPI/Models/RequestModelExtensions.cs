using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using AutoMapper;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Entities.VPOS2;
using Ecuafact.WebAPI.PayMe;

namespace Ecuafact.WebAPI.Models
{
    internal static class RequestModelExtensions
    {
        internal static string GetIPAddress(this HttpRequest Request)
        {
            // The following method returns the client IP address. It is better to use this method than the Request.UserHostAddress() 
            // because UserHostAddress sometimes may capture the IP address of user's proxy.
            // Ref: https://www.c-sharpcorner.com/blogs/get-client-ip-address-in-mvc-30

            string ip = Request?.ServerVariables?["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = Request?.ServerVariables?["REMOTE_ADDR"];
            }

            return ip ?? Request?.UserHostAddress ?? "0.0.0.0";
        }


        internal static Domain.Entities.Payment ToPayment(this PaymentModel paymentModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<PaymentModel, Domain.Entities.Payment>());
            var mapper = conf.CreateMapper();
            var payment = mapper.Map<Domain.Entities.Payment>(paymentModel);
            return payment;
        }

        internal static AdditionalField ToAdditionalField(this AdditionalFieldModel additionalFieldModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<AdditionalFieldModel, AdditionalField>());
            var mapper = conf.CreateMapper();
            var additionalFiel = mapper.Map<AdditionalField>(additionalFieldModel);
            return additionalFiel;
        }

        internal static Tax ToTax(this TaxModel taxModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<TaxModel, Tax>());
            var mapper = conf.CreateMapper();
            var tax = mapper.Map<Tax>(taxModel);
            return tax;
        }

        internal static TotalTax ToTotalTax(this TotalTaxModel totalTaxModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<TotalTaxModel, TotalTax>());
            var mapper = conf.CreateMapper();
            var tax = mapper.Map<TotalTax>(totalTaxModel);
            return tax;
        }

        internal static RetentionDetail ToRetentionDetail(this RetentionDetailModel retentionDetailModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<RetentionDetailModel, RetentionDetail>());
            var mapper = conf.CreateMapper();
            return mapper.Map<RetentionDetail>(retentionDetailModel);
        }
         
        internal static DocumentDetail  ToDocumentDetail(this DocumentDetailModel detailModel)
        {
            return new DocumentDetail
            {
                ProductId = detailModel.ProductId,
                MainCode = detailModel.MainCode,
                AuxCode = detailModel.AuxCode,
                Description = detailModel.Description,
                Amount = detailModel.Amount,
                UnitPrice = detailModel.UnitPrice,
                Discount = detailModel.Discount,
                SubTotal = detailModel.SubTotal,
                Taxes = detailModel.Taxes.ConvertAll(model => model.ToTax()),
                Name1 = detailModel.Name1,
                Name2 = detailModel.Name2,
                Name3 = detailModel.Name3,
                Value1 = detailModel.Value1,
                Value2 = detailModel.Value2,
                Value3 = detailModel.Value3,
            };

        }

        internal static DebitNoteDetail ToDebitNoteDetail(this DebitNoteDetailModel reasonModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<DebitNoteDetailModel, DebitNoteDetail>());
            var mapper = conf.CreateMapper();
            var debitNoteDetail = mapper.Map<DebitNoteDetail>(reasonModel);
            return debitNoteDetail;

        }

        internal static DocumentDetail ToDocumentDetail(this DocumentDetail detail, DocumentDetailModel detailModel)
        {
            detail.ProductId = detailModel.ProductId;
            detail.MainCode = detailModel.MainCode;
            detail.AuxCode = detailModel.AuxCode;
            detail.Description = detailModel.Description;
            detail.Amount = detailModel.Amount;
            detail.UnitPrice = detailModel.UnitPrice;
            detail.Discount = detailModel.Discount;
            detail.SubTotal = detailModel.SubTotal;
            detail.Taxes = detailModel.Taxes.ConvertAll(model => model.ToTax());
            detail.Name1 = detailModel.Name1;
            detail.Name2 = detailModel.Name2;
            detail.Name3 = detailModel.Name3;
            detail.Value1 = detailModel.Value1;
            detail.Value2 = detailModel.Value2;
            detail.Value3 = detailModel.Value3;

            return detail;
        }


        internal static ReferralGuideDetail ToReferralGuideDetail(this ReferralGuideDetailModel detailModel)
        {
            return new ReferralGuideDetail
            {
                ProductId = detailModel.ProductId,
                MainCode = detailModel.MainCode,
                AuxCode = detailModel.AuxCode,
                Description = detailModel.Description,
                Quantity = detailModel.Quantity,
                Name1 = detailModel.Name1,
                Value1 = detailModel.Value1,
                Name2 = detailModel.Name2,
                Value2 = detailModel.Value2,
                Name3 = detailModel.Name3,
                Value3 = detailModel.Value3,
            };

        }


        internal static PurchasePayment UpdatePayment(this PurchasePayment payment, OperationQueryResponse result)
        {
            payment.AuthorizationResult = result.result;
            payment.AuthorizationCode = result.authorizationCode;
            payment.AuthenticationECI = result.authenticationECI;
            payment.BillingCountry = result.billingCountry;
            payment.BillingState = result.billingState;
            payment.CardNumber = result.cardNumber;
            payment.CardType = result.cardType;
            payment.ErrorCode = result.errorCode;
            payment.ErrorMessage = result.errorMessage;
            payment.Language = result.language;
            payment.PurchaseIPAddress = result.purchaseIPAddress ?? HttpContext.Current?.Request?.GetIPAddress();
            payment.Reserved1 = result.reserved1;
            payment.Reserved11 = result.reserved11;
            payment.Reserved12 = result.reserved12;
            payment.Reserved2 = result.reserved2;
            payment.Reserved3 = result.reserved3;
            payment.Reserved4 = result.reserved4;
            payment.Reserved5 = result.reserved5;
            payment.Reserved6 = result.reserved6;
            payment.Reserved9 = result.reserved9;
            payment.ShippingAddress = result.shippingAddress;
            payment.ShippingCity = result.shippingCity;
            payment.ShippingCountry = result.shippingCountry;
            payment.ShippingEMail = result.shippingEMail;
            payment.ShippingFirstName = result.shippingFirstName;
            payment.ShippingLastName = result.shippingLastName;
            payment.ShippingState = result.shippingState;
            payment.ShippingZIP = result.shippingZIP;
            payment.TerminalCode = result.terminalCode;
            payment.Processed = !string.IsNullOrEmpty(result.purchaseIPAddress);

            return payment;
        }

        internal static PurchasePayment UpdatePayment(this PurchasePayment payment, PaymentResultModel result)
        {
            payment.AuthorizationResult = result.AuthorizationResult;
            payment.AuthorizationCode = result.AuthorizationCode;
            payment.BillingCountry = result.ShippingCountry;
            payment.BillingState = result.ShippingState;
            //payment.CardNumber = result.CardNumber;
            //payment.CardType = result.cardType;
            payment.ErrorCode = result.ErrorCode;
            payment.ErrorMessage = result.Message;
            payment.Language = result.Reserved3;
            payment.PurchaseIPAddress = HttpContext.Current?.Request?.GetIPAddress(); 
            payment.Reserved1 = result.Reserved1;
            payment.Reserved11 = result.Reserved11;
            //payment.Reserved12 = result.Reserved12;
            payment.Reserved2 = result.Reserved2;
            payment.Reserved3 = result.Reserved3;
            //payment.Reserved4 = result.Reserved4;
            //payment.Reserved5 = result.Reserved5;
            //payment.Reserved6 = result.Reserved6;
            //payment.Reserved9 = result.Reserved9;
            payment.ShippingAddress = result.ShippingAddress;
            payment.ShippingCity = result.ShippingCity;
            payment.ShippingCountry = result.ShippingCountry;
            payment.ShippingEMail = result.ShippingEmail;
            payment.ShippingFirstName = result.ShippingFirstName;
            payment.ShippingLastName = result.ShippingLastName;
            payment.ShippingState = result.ShippingState;
            payment.ShippingZIP = result.ShippingZIP;
            //payment.TerminalCode = result.TerminalCode;
            payment.Processed = result.AuthorizationResult != null;

            return payment;
        }

        internal static DocumentDetailModel ToDocumentDetailModel(this InvoiceDetailModel invoiceDetailModel)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<InvoiceDetailModel, DocumentDetailModel>());
            var mapper = conf.CreateMapper();
            return mapper.Map<DocumentDetailModel>(invoiceDetailModel);
        }


        //internal static TotalTax ToTotalTax(this TotalTaxModel totalTaxModel)
        //{
        //    var conf = new MapperConfiguration(config => config.CreateMap<TotalTaxModel, TotalTax>());
        //    var mapper = conf.CreateMapper();
        //    var totalTax = mapper.Map<TotalTax>(totalTaxModel);
        //    return totalTax;
        //}



    }
}