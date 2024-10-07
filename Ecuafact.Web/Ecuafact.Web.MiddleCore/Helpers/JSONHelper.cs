using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Entities.SRI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact
{
    public static class JsonHelper
    {
        public static T ReadStreamAsJsonString<T>(this Stream stream)
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamReader sr = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    // deserialize only when there's "{" character in the stream
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        return (T)serializer.Deserialize<T>(reader);
                    }
                }
            }

            return default(T);
        }


        /// <summary>
        /// Envía una solicitud POST al URI especificado como una operación asincrónica.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri">URI al que se envía la solicitud.</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri)
        {
            return client.PostAsync(requestUri, new StringContent(string.Empty));
        }

        public static ContributorDto ToContributor(this SRIContribDto obj)
        {
            if (obj != null)
            {
                var establishment = obj.Establishments?.FirstOrDefault(x => x.EstablishmentNumber == obj.Establishments?.Min(e => e.EstablishmentNumber));

                var contrib = new ContributorDto
                {
                    IdentificationTypeId = 2,
                    IdentificationType = "04",
                    Identification = obj.RUC,
                    BussinesName = obj.BusinessName,
                    TradeName = obj.TradeName,
                    Address = establishment.FullAddress
                };

                return contrib;
            }

            return default;
        }


        public static StringContent ToContent(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
