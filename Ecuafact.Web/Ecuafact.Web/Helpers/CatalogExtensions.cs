using Ecuafact.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Ecuafact.Web
{
    public static class CatalogExtensions
    {
        public static CatalogBaseDto FindById(this List<CatalogBaseDto> list, int id)
        {
            return FindById<CatalogBaseDto>(list, id);
        }

        public static ProductTypeDto FindById(this List<ProductTypeDto> list, int id)
        {
            return FindById<ProductTypeDto>(list, id);
        }

        public static IceRate FindById(this List<IceRate> list, int id)
        {
            return FindById<IceRate>(list, id);
        }

        public static IvaRatesDto FindById(this List<IvaRatesDto> list, int id)
        {
            return FindById<IvaRatesDto>(list, id);
        }


        public static T FindById<T>(this List<T> list, int id)
            where T : CatalogBaseDto
        {
            try
            {
                return list.Find(o => o.Id == id);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

    }

}


namespace System
{ 
    public static class MyExtensions
    {
        public static string Commarize(this double value)
        {
            if (value > 0)
            {
                var units = new string[] { "MIL", "MILLON", "BILLON", "TRILLON" };

                var order = Math.Floor(Math.Log(value) / Math.Log(1000));

                if (order > 0)
                {
                    var unitname = units[Convert.ToInt32(order - 1)];
                    var num = Math.Floor(value / 1000 * order);

                    // output number remainder + unitname
                    return $"{num:0} {unitname}";
                }
            }

            return $"{value:0.00}";
        }

        public static string Commarize(this decimal value)
        {
            return Convert.ToDouble(value).Commarize();
        }

        /// <summary>
        /// Acorta los valores decimales de un valor
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static string ToFixed(this decimal value)
        {
            value = decimal.Round(value, 6);
            var intValue = decimal.Round(value, 0);
            var decValue = value - intValue;

            if (decValue==0)
            {

            }

            var text = value.ToString("0.000000", CultureInfo.GetCultureInfo("en-US"));
            var dec =   text.Split('.');
            
            if (dec.Length > 1)
            {
                var decimals = dec[1];
                if (decimals.All(m=>m == '0'))
                {
                    return dec[0];
                }
                else if (decimals.EndsWith("0000"))
                {

                }
            }

            return text;
        }

        public static bool IsNaturalIdentity(this string cedula)
        { 
            if (string.IsNullOrEmpty(cedula))
            {
                return false;
            }

            cedula = cedula.Trim();
            
            if (cedula.Length > 10)
            {
                cedula = cedula.Substring(0, 10);
            }

            /**
             * Algoritmo para validar cedulas de Ecuador
             * @Author : Victor Diaz De La Gasca.
             * @Fecha  : Quito, 15 de Marzo del 2013
             * @Email  : vicmandlagasca@gmail.com
             * @Pasos  del algoritmo
             * 1.- Se debe validar que tenga 10 numeros
             * 2.- Se extrae los dos primero digitos de la izquierda y compruebo que existan las regiones
             * 3.- Extraigo el ultimo digito de la cedula
             * 4.- Extraigo Todos los pares y los sumo
             * 5.- Extraigo Los impares los multiplico x 2 si el numero resultante es mayor a 9 le restamos 9 al resultante
             * 6.- Extraigo el primer Digito de la suma (sumaPares + sumaImpares)
             * 7.- Conseguimos la decena inmediata del digito extraido del paso 6 (digito + 1) * 10
             * 8.- restamos la decena inmediata - suma / si la suma nos resulta 10, el decimo digito es cero
             * 9.- Paso 9 Comparamos el digito resultante con el ultimo digito de la cedula si son iguales todo OK sino existe error.
             */


            //Preguntamos si la cedula consta de 10 digitos
            if (cedula.Length == 10)
            {

                //Obtenemos el digito de la region que sonlos dos primeros digitos
                var digito_region = Convert.ToInt32(cedula.Substring(0, 2));

                //Pregunto si la region existe ecuador se divide en 24 regiones
                if (digito_region >= 1 && digito_region <= 24)
                { 
                    // Extraigo el ultimo digito
                    var ultimo_digito = Convert.ToInt32(cedula.Substring(9, 1));

                    //Agrupo todos los pares y los sumo
                    var pares = Convert.ToInt32(cedula.Substring(1, 1)) + Convert.ToInt32(cedula.Substring(3, 1)) + Convert.ToInt32(cedula.Substring(5, 1)) + Convert.ToInt32(cedula.Substring(7, 1));

                    //Agrupo los impares, los multiplico por un factor de 2, si la resultante es > que 9 le restamos el 9 a la resultante
                    var numero1 = Convert.ToInt32(cedula.Substring(0, 1));
                    numero1 = (numero1 * 2);

                    if (numero1 > 9)
                    {
                        numero1 = (numero1 - 9);
                    }

                    var numero3 = Convert.ToInt32(cedula.Substring(2, 1));
                    numero3 = (numero3 * 2);

                    if (numero3 > 9)
                    {
                        numero3 = (numero3 - 9);
                    }

                    var numero5 = Convert.ToInt32(cedula.Substring(4, 1));
                    numero5 = (numero5 * 2);
                    if (numero5 > 9)
                    {
                        numero5 = (numero5 - 9);
                    }

                    var numero7 = Convert.ToInt32(cedula.Substring(6, 1));
                    numero7 = (numero7 * 2);
                    if (numero7 > 9)
                    {
                        numero7 = (numero7 - 9);
                    }

                    var numero9 = Convert.ToInt32(cedula.Substring(8, 1));
                    numero9 = (numero9 * 2);
                    if (numero9 > 9)
                    {
                        numero9 = (numero9 - 9);
                    }

                    var impares = numero1 + numero3 + numero5 + numero7 + numero9;

                    //Suma total
                    var suma_total = (pares + impares);

                    //extraemos el primero digito
                    var primer_digito_suma = Convert.ToString(suma_total).Substring(0, 1);

                    //Obtenemos la decena inmediata
                    var decena = (Convert.ToInt32(primer_digito_suma) + 1) * 10;

                    //Obtenemos la resta de la decena inmediata - la suma_total esto nos da el digito validador
                    var digito_validador = decena - suma_total;

                    //Si el digito validador es = a 10 toma el valor de 0
                    if (digito_validador == 10)
                        digito_validador = 0;

                    //Validamos que el digito validador sea igual al de la cedula
                    return (digito_validador == ultimo_digito);
                }
                else
                {
                    // imprimimos en consola si la region no pertenece
                    // console.log('Esta cedula no pertenece a ninguna region');
                    return false;
                }
            }
            else
            {
                //imprimimos en consola si la cedula tiene mas o menos de 10 digitos
                // console.log('Esta cedula tiene menos de 10 Digitos');
                return false;
            }
             
        }

    }
}