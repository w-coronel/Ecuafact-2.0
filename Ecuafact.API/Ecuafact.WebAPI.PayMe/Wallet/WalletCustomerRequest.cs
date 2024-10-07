namespace Ecuafact.WebAPI.PayMe
{
    public class WalletCustomerRequest
    {
        /// <summary>
        /// Codigo del Cliente (Cedula)
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// Nombres del Cliente
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Apellidos del Cliente
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Correo Electronico del Cliente
        /// </summary>
        public string Email { get; set; }


    }

}
