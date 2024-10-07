using Ecuafact.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.Web
{
    public static class ElectronicSignExtensions
    {
        public static ElectronicSignRequest  ToRequest(this ElectronicSignModel model)
        {
            return new ElectronicSignRequest
            {
                Id = model.Id,
                RUC = model.RUC,
                BusinessName = model.BusinessName,
                BusinessAddress = model.BusinessAddress,
                City = model.City,
                Province = model.Province,
                Country = model.Country,
                Identification = model.Identification,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                ConfirmPhone = model.Phone,                
                ConfirmEmail = model.Email ?? string.Empty,
                Phone2 = model.Phone2,
                Email2 = model.Email2 ?? string.Empty,
                ConfirmPhone2 = model.Phone2,
                ConfirmEmail2 = model.Email2 ?? string.Empty,
                DocumentType = model.DocumentType,
                FileFormat = model.FileFormat,
                SignatureValidy = model.SignatureValidy,
                Skype = model.Skype,
                SignType = model.SignType,
                VerificationType = model.VerificationType,
                Sexo = model.Sexo,
                BirthDate = model.BirthDate,
                WorkPosition = model.WorkPosition,
                FileCedulaBack = !string.IsNullOrEmpty(model.CedulaBackFile),
                FileCedulaFront = !string.IsNullOrEmpty(model.CedulaFrontFile),
                FileSelfie = !string.IsNullOrEmpty(model.SelfieFile),
                FileRuc = !string.IsNullOrEmpty(model.RucFile),
                FileConstitution = !string.IsNullOrEmpty(model.ConstitutionFile),
                FileDesignation = !string.IsNullOrEmpty(model.DesignationFile),
                FileAuthorizationAge = !string.IsNullOrEmpty(model.AuthorizationAgeFile),
                FingerPrintCode = model.FingerPrintCode,
                InvoiceInfo = new ElectronicSignInvoice
                {
                    Identification = model.PurchaseOrder?.Identification,
                    Name = model.PurchaseOrder?.BusinessName,
                    Address = model.PurchaseOrder?.Address,
                    Email = model.PurchaseOrder?.Email,
                    Phone = model.PurchaseOrder?.Phone
                }, 
            };
        }
    }
}