using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using AutoMapper;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.SRI;

namespace Ecuafact.WebAPI.Models
{
    internal static class IssuerExtensions
    {
        internal static bool SetElectronicSign(this Issuer issuer, IssuerRequestModel model)
        {
            return issuer.SetElectronicSign(model.CertificateFile, model.CertificatePass);
        }
        
        internal static bool SetElectronicSign(this Issuer issuer, byte[] certificateFile, string certificatePass)
        {
            if (certificateFile != null && certificateFile.Length > 0)
            {
                try
                {
                    if (string.IsNullOrEmpty(certificatePass))
                    {
                        throw new CryptographicException("Debe especificar una clave de firma electrónica válida.");
                    }

                    var certificate = new X509Certificate2(certificateFile, certificatePass);

                    // Validamos que el certificado no este caducado
                    if (certificate.NotAfter < DateTime.Now)
                    {
                        throw new CryptographicException($"El certificado digital de la firma electrónica ya no es válido. Fecha de expiración: {certificate.NotAfter.ToString("dd/MM/yyyy")} ");
                    }

                    issuer.IsEnabled = true;
                    issuer.CertificateIssuedTo = certificate.FriendlyName;
                    issuer.CertificateSubject = certificate.Subject;
                    issuer.CertificateExpirationDate = certificate.NotAfter;
                    return true;
                }
                catch (CryptographicException ex)
                {
                    throw new Exception($"Hubo un error al configurar la firma electrónica.", ex);
                }
                catch (Exception)
                {
                    // Si hubo otro error, no hay problema no se autoriza la emision para este usuario.
                }

                return false;
            }

            return false;
        }

        public static void EmailActivation(this Issuer issuer)
        { 
            var email = Emailer.Create("issuer-update", "ECUAFACT: Su cuenta ha sido activada", issuer.Email);
            email.Parameters.Add("USUARIO", issuer.RUC);
            email.Parameters.Add("NOMBRE", issuer.BussinesName);
            email.Send();
        }

        internal static IssuerDto ToIssuerDto(this Issuer issuer)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<Issuer, IssuerDto>());
            var mapper = conf.CreateMapper();
            var issuerDto = mapper.Map<IssuerDto>(issuer);            
            issuerDto.LogoFile = GetLogoByIssuer(issuer);
            issuerDto.Establishments = issuer.Establishments?.Count == 0 ? issuer.ToEstablishments():issuer.Establishments;
            return issuerDto;
        }

        internal static Issuer ToIssuer(this IssuerDto issuer)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<IssuerDto, Issuer>());
            var mapper = conf.CreateMapper();
            var issuerDto = mapper.Map<Issuer>(issuer);
 
            return issuerDto;
        }


        internal static Issuer ToIssuer(this SRIContrib contrib)
        {
            if (contrib != null)
            {
                var estab = contrib.Establishments.FirstOrDefault();
                int number = 1;
                int.TryParse(estab.EstablishmentNumber, out number);

                return new Issuer
                {
                    RUC = contrib.RUC,
                    BussinesName = contrib.BusinessName,
                    TradeName = string.IsNullOrWhiteSpace(contrib.TradeName) ? contrib.BusinessName:contrib.TradeName,
                    IsAccountingRequired = contrib.AccountingRequired,
                    IsSpecialContributor = (contrib.ContributorClass == SRIContribClass.Special),
                    MainAddress = contrib.MainAddress,
                    Currency = "DOLAR",
                    EnvironmentType = EnvironmentType.Production,
                    EstablishmentCode = number.ToString("D3"),
                    EstablishmentAddress = estab.FullAddress,
                    IssueType = IssueType.Normal,
                    IssuePointCode = "002",
                    Phone = "",
                    CreatedOn = DateTime.Now,
                    IsEnabled = true,
                    Email = "",
                    City = estab.City,
                    Province = estab.Province,
                    Country = "ECUADOR",
                    Establishments = contrib.Establishments?.Select(ad => ad.ToEstablishmentsSRI()).ToList()
                };
            }

            return default;
        }

        internal static Establishments ToEstablishmentsSRI(this Establishment establishmentSri)
        {
            return new Establishments() { 
                Code = Convert.ToInt32(establishmentSri.EstablishmentNumber).ToString("D3"),
                Address = establishmentSri.Street,
                Name = establishmentSri.CommercialName,
                IssuePoint = new List<IssuePoint>() { 
                    new IssuePoint(){ 
                        Code = "001",
                        Name = "Punto emisión 1"
                    }
                }

            };
        }

        internal static List<Establishments> ToEstablishments(this Issuer issuer)
        {
            var establishments = new List<Establishments>() {
                new Establishments() {
                       Code= issuer.EstablishmentCode,
                       Address = issuer.EstablishmentAddress,
                       IssuerId = issuer.Id,
                       IssuePoint = new List<IssuePoint>(){
                          new IssuePoint(){
                            Code = issuer.IssuePointCode
                          }
                       }
                }
            };
            return establishments;
        }

        internal static Issuer Update(this Issuer issuer, IssuerRequestModel issuerRequest)
        {
            var logo = issuer.Logo;
            var certificate = issuer.Certificate;
            issuer.BussinesName = issuerRequest.BussinesName;
            issuer.TradeName = string.IsNullOrWhiteSpace(issuerRequest.TradeName) ? issuerRequest.BussinesName:issuerRequest.TradeName;
            issuer.Phone = issuerRequest.Phone;
            issuer.Email = issuerRequest.Email;
            issuer.MainAddress = issuerRequest.MainAddress;
            issuer.IsAccountingRequired = issuerRequest.IsAccountingRequired;
            issuer.IsRimpe = issuerRequest.IsRimpe;                       
            issuer.IsSpecialContributor = issuerRequest.IsSpecialContributor;
            issuer.AgentResolutionNumber = issuerRequest.AgentResolutionNumber;
            issuer.EstablishmentAddress = issuerRequest.EstablishmentAddress;
            issuer.IsRetentionAgent = issuerRequest.IsRetentionAgent;
            issuer.ResolutionNumber = issuerRequest.ResolutionNumber;
            issuer.Logo = logo;
            issuer.Certificate = certificate;
            issuer.CertificatePass = issuerRequest.CertificatePass;

            issuer.IsGeneralRegime = issuerRequest.IsGeneralRegime;
            issuer.IsSimplifiedCompaniesRegime = issuerRequest.IsSimplifiedCompaniesRegime;
            issuer.IsSkilledCraftsman = issuerRequest.IsSkilledCraftsman;
            issuer.SkilledCraftsmanNumber = issuerRequest.SkilledCraftsmanNumber;
            issuer.IsPopularBusiness = issuerRequest.IsPopularBusiness;
            issuer.IsCarrier = issuerRequest.IsCarrier;

            // Estableciminetos configurados por el emisor
            issuer.Establishments.AddRange(issuerRequest.Establishments.Select(model => model).ToList());
            issuer.Establishments.ForEach(x =>
            {
                x.IssuerId = issuer.Id; x.IssuePoint.ForEach(k =>
                {
                    k.IssuerId = issuer.Id;
                    k.Name = k.Name ?? $"Caja:{k.Code}";
                });
            });

            return issuer;
        }

        internal static Establishments ToUpdateEstablishment(this EstablishmentsModel model, Establishments establishment)
        {
            establishment.Code = model.Code;
            establishment.Name = model.Name;
            establishment.IssuerId = model.IssuerId;
            establishment.Address = model.Address;
            return establishment;
        }

        internal static Establishments ToUpdateEstablishment(this EstablishmentsModel model, long Id)
        {
            return new Establishments
            {
                Id = Id,
                Code = model.Code,
                Name = model.Name,
                Address = model.Address,
                IssuerId = model.IssuerId
            };
        }

        internal static Establishments ToEstablishment(this EstablishmentsModel model)
        {
            var establishments = new Establishments()
            {
                Code = model.Code,
                Name = model.Name,
                IssuerId = model.IssuerId,
                Address = model.Address
            };

            return establishments;
        }

        internal static IssuePoint ToIssuePoint(this IssuePointModel model)
        {
            return new IssuePoint()
            {
                Code = model.Code,
                Name = model.Name,
                EstablishmentsId = model.EstablishmentsId,
                IssuerId = model.IssuerId,
                CarrierRUC = model.CarrierRUC,
                CarPlate = model.CarPlate,
                CarrierEmail = model.CarrierEmail,
                CarrierPhone = model.CarrierPhone,
                CarrierContribuyente = model.CarrierContribuyente,
                CarrierResolutionNumber = model.CarrierResolutionNumber               

            };
        }

        internal static IssuePoint ToUpdateIssuePoint(this IssuePointModel model, IssuePoint issuePoint)
        {
            issuePoint.Code = model.Code;
            issuePoint.Name = model.Name;
            issuePoint.EstablishmentsId = model.EstablishmentsId;
            issuePoint.IssuerId = model.IssuerId;
            issuePoint.CarrierRUC = model.CarrierRUC;
            issuePoint.CarPlate = model.CarPlate;
            issuePoint.CarrierEmail = model.CarrierEmail;
            issuePoint.CarrierPhone = model.CarrierPhone;
            issuePoint.CarrierResolutionNumber = model.CarrierResolutionNumber;
            return issuePoint;
        }

        internal static IssuePoint ToUpdateIssuePoint(this IssuePointModel model, long Id)
        {
            return new IssuePoint()
            {
                Id = Id,
                Code = model.Code,
                Name = model.Name,
                EstablishmentsId = model.EstablishmentsId,
                IssuerId = model.IssuerId,
                CarrierRUC = model.CarrierRUC,
                CarPlate = model.CarPlate,
                CarrierEmail = model.CarrierEmail,
                CarrierPhone = model.CarrierPhone,
                CarrierContribuyente = model.CarrierContribuyente,
                CarrierResolutionNumber = model.CarrierResolutionNumber
                
            };
        }


        //private static List<IssuePoint> GetIssuePoint(List<DocumentDetail> details)
        //{

        //}

        internal static Issuer ToIssuer(this IssuerRequestModel issuerRequest)
        {
            var conf = new MapperConfiguration(config => config.CreateMap<IssuerRequestModel, Issuer>());
            var mapper = conf.CreateMapper();
            var issuer = mapper.Map<Issuer>(issuerRequest);
            return issuer;
        }

        internal static byte[] GetLogoByIssuer(Issuer issuer)
        {
            if (issuer != null)
            { 
                string logo;
                if (!string.IsNullOrEmpty(issuer?.Logo))
                {
                    logo = HttpContext.Current.GetLogoFileName(issuer?.Logo);
                }
                else { logo = HttpContext.Current.GetLogoFileName(issuer?.RUC); }

                return HttpContext.Current.GetFileContent(logo);
            }
            return new byte[0];
        }

        public static void EmailActivationAccount(this Issuer issuer, Subscription subscription, long purchaseOrderId, string proceso = "C")
        {
            try
            {              
                if (proceso == "C")
                {
                    var OrdenPagoUrl = $"{Constants.WebApp}Payment/TypeLicence";
                    var mensaje = $"Le informamos que la suscripci&oacute;n para el modulo de emisi&oacute;n de documentos " +
                           $"electr&oacute;nicos, caducara el {subscription.SubscriptionExpirationDate.Value.ToShortDateString()}.<br><br>" +
                           "Para evitar futuros inconvenientes le recomendamos proceder a renovar su suscripci&oacute;n lo mas pronto posible.";
                    var email = Emailer.Create("account-notification", "ECUAFACT: Suscripción por Caducar", issuer.Email);
                    email.Parameters.Add("Cliente", issuer.BussinesName);
                    email.Parameters.Add("Mensaje", mensaje);                   
                    email.Send();
                }
                else if (proceso == "L")
                {
                    var nombPlan = subscription.LicenceType.Code == Constants.PlanBasic ? "B&Aacute;SICO" : subscription.LicenceType.Name;
                    var asunto = $"Tu plan {subscription.LicenceType.Name} pronto finalizará";
                    var OrdenPagoUrl = $"{Constants.WebApp}Payment/TypeLicence";
                    var mensaje = $"Le informamos que la suscripci&oacute;n de su plan {nombPlan} tiene un saldo de 5 documentos por emitir, para continuar con la emisi&oacute;n de documentos " +
                                  "al finalizar su plan debe  renovar  la suscripci&oacute;n.";

                    if (subscription.Status == SubscriptionStatusEnum.Inactiva)
                    {
                        asunto = "Suscripción Inactiva por limite de documentos emitidos";
                        mensaje = $"Le informamos que la suscripci&oacute;n de su plan {nombPlan} de emisi&oacute;n de documentos ha finalizado .<br><br>" +
                             $"Para evitar futuros inconvenientes le recomendamos proceder a renovar la suscripci&oacute;n de su plan lo m&aacute;s pronto posible. ";
                    }                   

                    var email = Emailer.Create("account-notification", $"ECUAFACT: {asunto}", issuer.Email);
                    email.Parameters.Add("Cliente", issuer.BussinesName);
                    email.Parameters.Add("Mensaje", mensaje);                   
                    email.Send();
                }
                else if (proceso == "A")
                {                    
                    var asunto = $"Activación {subscription?.LicenceType?.Name}.";                    
                    var mensaje = $"Bienvenido a Ecuafact.<br><br>"+ 
                        "Le comunicamos que ha sido activada la suscripción. Para poder emitir documentos electr&oacute;nicos, " +
                        $"por favor ingresar en  <a href='https://app.ecuafact.com/' target='_blank'>app.ecuafact.com</a>. Configure su cuenta y proceder a realizar la solicitud de tu firma electr&oacute;nica";                   

                    var email = Emailer.Create("renewed-account", $"ECUAFACT: {asunto}", issuer.Email);
                    email.Parameters.Add("Cliente", issuer.BussinesName);
                    email.Parameters.Add("Mensaje", mensaje);
                    email.Send();
                }
                else if (proceso == "M")
                {
                    var asunto = $"Activación {subscription?.LicenceType?.Name}.";
                    var mensaje = $"Bienvenido a Ecuafact.<br><br>" +
                        "Le comunicamos que ha sido activada su suscripción. Para poder emitir documentos electr&oacute;nicos, " +
                        $"por favor ingresar en  <a href='https://app.ecuafact.com/'>app.ecuafact.com</a>. Configure su cuenta y proceder a realizar la solicitud de tu firma electr&oacute;nica<br>" +
                        $"Credenciales de ingreso al App: <br>Usuario:{subscription.RUC} <br>Contraseña: {Constants.AppToken} ";

                   var email = Emailer.Create("renewed-account", $"ECUAFACT: {asunto}", issuer.Email);
                        email.Parameters.Add("Cliente", issuer.BussinesName);
                        email.Parameters.Add("mensaje", mensaje);
                        email.Send();
                }
                else
                {                    

                    var email = Emailer.Create("renovacion-activa", "ECUAFACT: Suscripción Renovada", issuer.Email);
                    email.Parameters.Add("Cliente", issuer.BussinesName);
                    email.Parameters.Add("Fecha1", subscription.SubscriptionStartDate.Value.ToShortDateString());
                    email.Parameters.Add("Fecha2", subscription.SubscriptionExpirationDate.Value.ToShortDateString());

                    email.Send();
                }
            }
            catch(Exception ex)
            { }
        }

        public static void EmailEsignNotification(this Issuer issuer, int dias)
        {
            try
            {
                var titulo = "Tu Firma Electr&oacute;nica ha caducado!";                
                if (dias <= 28 && dias >= 1)
                {
                    titulo = $"Tu firma electr&oacute;nica est&aacute; pr&oacute;xima a caducar: {issuer.CertificateExpirationDate.Value:dd/MM/yyyy}!";                   
                }               
                var email = Emailer.Create("esign-notification", "ECUAFACT: Renovación de Firma Electrónica", issuer.Email);
                email.Parameters.Add("Cliente", issuer.BussinesName);
                email.Parameters.Add("Titulo", titulo);                
                email.Send();
            }
            catch (Exception ex)
            { }
        }


        public static void LogoDefault(this Issuer issuer)
        {
            try
            {
                List<string> _BackgroundColours = new List<string> { "B26126", "FFF7F2", "FFE8D8", "74ADB2", "D8FCFF" };
                //var text = from c in issuer.BussinesName.Split(' ') select c.Substring(0, 1);                
                var text = issuer.BussinesName.FirstOrDefault().ToString();
                var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
                var bgColour = _BackgroundColours[randomIndex];
                var bmp = new Bitmap(192, 192);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                var font = new Font("Poppins", 60, FontStyle.Bold, GraphicsUnit.Pixel);
                var graphics = Graphics.FromImage(bmp);

                graphics.Clear((Color)new ColorConverter().ConvertFromString("#" + bgColour));
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.DrawString(text, font, new SolidBrush(Color.WhiteSmoke), new RectangleF(0, 0, 192, 192), sf);
                graphics.Flush();

                string filename = Path.Combine(Constants.EngineLocation, issuer.Logo);
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                }
                bmp.Save(filename, ImageFormat.Jpeg);
                
            }
            catch (Exception ex)
            { 
            }
        }
    } 
}