using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Linq;

namespace Ecuafact.WebAPI.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Creando Base de Datos");
                InitDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }

        static void InitDatabase()
        {
            using (var db = new EcuafactExpressContext())
            {
                var prods = db.PaymentMethods.ToList();
                Console.WriteLine("Base de Datos Creada");
                Console.WriteLine("Formas de Pago");
                foreach (var paymentMethod in prods)
                {
                    Console.WriteLine($"Id:{paymentMethod.Id}, Nombre: {paymentMethod.Name}, SriCode: {paymentMethod.SriCode}");
                }
                Console.WriteLine("BD Lista");
            }
        }

    }
}
