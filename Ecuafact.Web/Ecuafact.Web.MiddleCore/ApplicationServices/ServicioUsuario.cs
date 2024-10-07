using Ecuafact.Web.MiddleCore.NexusApiServices;
using System;
using System.Net.Http;
using Ecuafact.Web.Domain.Entities.API;
using System.Collections.Generic;
using Ecuafact.Web.Domain.Services;
using System.Net;
using System.Threading.Tasks;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioUsuario
    {

        public static LoginResponseModel ValidateToken(string token)
        {
            var errorMsg = "Nombre de usuario o contraseña incorrectos.";

            try
            {
                var client = ClientHelper.GetClient(token);

                var response = client.PostAsync($"{Constants.WebApiUrl}/ValidateToken", new StringContent(string.Empty)).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.GetContent<LoginResponseModel>();
                }

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            return new LoginResponseModel
            {
                Result = new ErrorModel
                {
                    Code = "1001",
                    Message = errorMsg
                }
            };
        }

        public static LoginResponseModel LogIn(LoginModel model)
        {
            var errorMsg = "Nombre de usuario o contraseña incorrectos.";

            try
            {
                var client = ClientHelper.GetClient(model.Username, model.Password);
                var response = client.PostAsync($"{Constants.WebApiUrl}/authenticate", new StringContent(string.Empty)).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.GetContent<LoginResponseModel>();
                }
                else
                {
                    var error = response.GetContent<OperationResult>();
                    errorMsg = error?.UserMessage ?? errorMsg;
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            return new LoginResponseModel
            {
                Result = new ErrorModel
                {
                    Code = "1001",
                    Message = errorMsg
                }
            };
        }

        public static LoginResponseModel UpdateProfile(profileUpdateRequest request)
        {
            // Registrar Usuarios....
            return JsonWebApiHelper.ExecuteJsonWebApi<LoginResponseModel>(
                $"{Constants.ServiceAppUrl}/UpdateProfile", HttpMethod.Post, request);

        }

        public static LoginResponseModel RegisterUser(registerRequest request)
        {
            // Registrar Usuarios....
            return JsonWebApiHelper.ExecuteJsonWebApi<LoginResponseModel>(
                $"{Constants.ServiceAppUrl}/Register", HttpMethod.Post, request);

        }

        public static PasswordChangeResponse ChangePassword(PasswordChangeRequest request)
        {
            // Cambiar contraseña....
            return JsonWebApiHelper.ExecuteJsonWebApi<PasswordChangeResponse>(
                $"{Constants.ServiceAppUrl}/change_password", HttpMethod.Post, request);

        }

        public static PasswordRecoveryResponse ForgotPassword(passwordRecoveryRequest request)
        {
            // Recuperar contraseña....
            return JsonWebApiHelper.ExecuteJsonWebApi<PasswordRecoveryResponse>(
                $"{Constants.ServiceAppUrl}/forgot_password", HttpMethod.Post, request);

        }

        public static loginSRIResponse LoginSri(string mytoken, string ruc, string nuevaClaveSRI)
        {
            // Iniciar sesion en el SRI....
            var request = new loginSRIRequest
            {
                login = new loginSRI
                {
                    token = mytoken,
                    user = ruc,
                    password = nuevaClaveSRI
                }
            };

            return JsonWebApiHelper.ExecuteJsonWebApi<loginSRIResponse>(
                $"{Constants.ServiceAppUrl}/LoginSRI", HttpMethod.Post, request);

        }

        public static async Task<OperationResult<SriLoginResult>> ValidateSRIUser(string token, string ruc, string claveSRI)
        {
            try
            {
                var client = ClientHelper.GetClient(token);
                var request = new { User = ruc, Password = claveSRI };
                var response = client.PostAsync($"{Constants.WebApiUrl}/sri/validate-user", request.ToContent()).Result;
                var result = await response.Content.ReadAsStringAsync();

                return await response.GetContentAsync<OperationResult<SriLoginResult>>();
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new OperationResult<SriLoginResult>(ex, HttpStatusCode.Unauthorized));
            }
        }

        public void ActualizaClaveSri(string ruc, string claveSRI)
        {
            throw new NotImplementedException();
        }

        public static OperationResult UserCampaign(RegisterModel model)
        {
            try
            {
                var client = ClientHelper.GetClient();
                var response = client.PostAsync($"{Constants.WebApiUrl}/usercampaign", model.ToContent()).Result;

                return response.GetContent<OperationResult>();
            }
            catch (Exception ex)
            {
                return new OperationResult(ex, HttpStatusCode.Unauthorized);
            }
        }

        public static async Task<OperationResult<List<LoginIssuerModel>>> DesvincularCuenta(string token, string usuario, string ruc)
        {
            var client = ClientHelper.GetClient(token);
            var request = new { Username = usuario, RUC = ruc };
            var response = client.PostAsync($"{Constants.WebApiUrl}/disassociate", request.ToContent()).Result;            

            return await response.GetContentAsync<OperationResult<List<LoginIssuerModel>>>();

        }


        #region ########################## Codigo Antiguo ########################## 

        //public loginResponse Autenticar(string usuario, string pass)
        //{
        //    using (var client = new ApiAppServiceClient())
        //    {
        //        client.Open();
        //        var userLogin = new login() { user = usuario, password = pass };
        //        var result = client.Login(new loginRequest() { login = userLogin });

        //        if (result != null)
        //        {
        //            return result;
        //        }
        //    }

        //    throw new ApplicationException("Usuario o Contraseña Erronea.");
        //}

        //public List<NDK.eFac.Be.SucursalEntity> ObtenerSucursales(long idUsuario)
        //      //{
        //      //    var sucursales = NDK.eFac.BA.SucursalCollectionBussinesAction.GetSucursalByUsuario(idUsuario);
        //      //    return sucursales;
        //      //}
        //      public EmisorEntity GetEmisor(int idEmisor)
        //      {
        //          using (var client = new ApiAppServiceClient())
        //          {

        //          }


        //              var emisor = NDK.eFac.BA.EmisorBussinesAction.LoadByPK(idEmisor);
        //          if (emisor == null)
        //          {
        //              throw new ApplicationException("El emisor no existe");
        //          }
        //          return emisor;
        //      }
        //      public PtoEmiEntityCollection GetPtoEmibyUser(long id)
        //      {
        //          var ptoemi = NDK.eFac.BA.PtoEmiCollectionBussinesAction.GetPtoEmiByUsuario(id);
        //          if (ptoemi == null)
        //          {
        //              throw new ApplicationException("El emisor no existe");
        //          }
        //          return ptoemi;
        //      }
        //      public PtoEmiEntityCollection GetPtoEmibyEmisor(long id)
        //      {
        //          var ptoemi = NDK.eFac.BA.PtoEmiCollectionBussinesAction.GetPtoEmiByUsuario(id);
        //          if (ptoemi == null)
        //          {
        //              throw new ApplicationException("El emisor no existe");
        //          }
        //          return ptoemi;
        //      }
        //      public UsuarioEmisorEntityCollection GetUsuarioEmisor(long id)
        //      {
        //          var usuarioEmisorCol = NDK.eFac.BA.UsuarioEmisorCollectionBussinesAction.FindByAll(
        //                                  new NDK.eFac.Be.UsuarioEmisorFindParameterEntity()
        //                                  {
        //                                      IdUsuario = id
        //                                  });
        //          if (usuarioEmisorCol == null)
        //          {
        //              throw new ApplicationException("El emisor no existe");
        //          }
        //          return usuarioEmisorCol;
        //      }

        //      public SucursalEntityCollection GetByIdUsuarioEmisor(long id)
        //      {
        //          var sucursal = NDK.eFac.BA.SucursalCollectionBussinesAction.GetByIdUsuarioEmisor(id);
        //          if (sucursal == null)
        //          {
        //              throw new ApplicationException("El emisor no existe");
        //          }
        //          return sucursal;
        //      }

        //      public RolEntity GetRolUsuario(long id)
        //      {
        //          //var rolusuario = NDK.eFac.BA.RolBussinesAction.LoadByPK(id);
        //          //NDK.eFac.Be.UsuarioEntityCollection userCol = NDK.eFac.BA.UsuarioCollectionBussinesAction.GetByUsuario(id.ToString()); //NDK.eFac.BA.UsuarioCollectionBussinesAction.LoginEntidad(txtUser.Text.Trim().ToLower(), passw);
        //          NDK.eFac.Be.RolEntityCollection userRol = NDK.eFac.BA.RolCollectionBussinesAction.GetRolByUsuario(id);
        //	if (userRol == null)
        //          {
        //              throw new ApplicationException("El usuario no tiene rol");
        //          }
        //	//else if (userRol.Count == 0)
        //	//{
        //	//	return new RolEntity();
        //	//}
        //          return userRol[0];
        //      }
        ///*************************************/

        //public int ActualizaClaveSri(string ruc, string clave)
        //{
        //	if (clave.Count() >= 3)
        //	{
        //		NDK.eFac.Be.EmisorEntityCollection emisorList = NDK.eFac.BA.EmisorCollectionBussinesAction.GetByRuc(ruc);
        //		EmisorEntity emisor = emisorList[0];

        //		clave = NDK.eFac.Be.Cryptography.CloudCryptography.EncryptCloudString(clave);

        //		NDK.eFac.BA.EmisorCollectionBussinesAction.ActualizaClaveSRI(emisor.Id, clave);
        //	}
        //	return 0;
        //}
        ///*************************************/
        //public UsuarioEntity GetUsuarioByRuc(string ruc)
        //{
        //	NDK.eFac.Be.UsuarioEntityCollection userRol = NDK.eFac.BA.UsuarioCollectionBussinesAction.FindByAll(
        //		new NDK.eFac.Be.UsuarioFindParameterEntity()
        //		{
        //			Usuario = ruc
        //		});
        //	if (userRol == null)
        //	{
        //		throw new ApplicationException("El usuario no tiene rol");
        //	}
        //	return userRol[0];

        //}
        #endregion


        #region ################# Informacion del Usuario para Pruebas Locales ################# 
#if DEBUG

        private static LoginResponseModel getAdminLoginResponse()
        {
            var token = "792428c9-e4af-4003-9f8b-3fd6f5f17166";
            return new LoginResponseModel
            {
                Result = new ErrorModel { Code = "200", Message = "OK" },
                UserInfo = new ClientModel
                {
                    BusinessName = "Ronald Ramirez",
                    ClientToken = token,
                    ClientType = "1",
                    Email = "rramirez@ecuanexus.com",
                    Username = "ronald",
                    Id = "ronald",
                    Name = "Ronald Ramirez",
                    Address = "Av. Principal"
                },
                Issuers = new List<LoginIssuerModel>{
                    new LoginIssuerModel
                    {
                        RUC = "0992882549001",
                        Name = "Consultora Informatica Ecuadorian Nexus - Ecuanexus IT",
                        BusinessName = "Ecuanexus",
                        ClientToken = token,
                        Email ="info@ecuanexus.com",
                        PK = token
                    }
                }
            };
        }
#endif
        #endregion

    }
}



