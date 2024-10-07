using System;
using System.Reflection;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

namespace System
{
    public static class HttpClientHelper
    {
        private static readonly JsonSerializer _serializer = new JsonSerializer();

        public static T GetContent<T>(this HttpResponseMessage response)
        {
            using (var stream = response?.Content?.ReadAsStreamAsync()?.Result)
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    using (var json = new JsonTextReader(reader))
                    {
                        return _serializer.Deserialize<T>(json);
                    }
                }
                else return default;
            }
        }

        public async static  Task<T> GetContentAsync<T>(this HttpResponseMessage response)
        {
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            using (var json = new JsonTextReader(reader))
            {

                return _serializer.Deserialize<T>(json);
            } 
        }

        public static HttpClient SetAuthentication(this HttpClient client, string username, string password)
        {
            var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
            var token = Convert.ToBase64String(byteArray);

            return SetAuthentication(client, token);
        }

        public static HttpClient SetAuthentication(this HttpClient client, string token)
        {
            if (client.DefaultRequestHeaders.Authorization?.Parameter != token)
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
            }
            return client;
        }

        static HttpClient __defaultClient;
        public static HttpClient DefaultClient
        {
            get
            {
                if (__defaultClient == null || IsDisposed(__defaultClient))
                {
                    __defaultClient = new HttpClient(ClientHandler);
                }

                return __defaultClient;
            }
        }

        public static HttpClient GetClient()
        {
            var client = HttpClientHelper.DefaultClient;
            // Sets security headers
            client.DefaultRequestHeaders.Clear();
            return client;
        }

        public static HttpClient GetClient(string user, string pass)
        {
            var client = HttpClientHelper.DefaultClient;
            // Sets security headers
            client.DefaultRequestHeaders.Clear();
            client.SetAuthentication(user, pass);
            return client;
        }

        public static HttpClient GetClient(string token)
        {
            var client = HttpClientHelper.DefaultClient;
            // Sets security headers
            client.DefaultRequestHeaders.Clear();
            client.SetAuthentication(token);
            return client;
        }


        static HttpClientHandler __clientHandler;
        public static HttpClientHandler ClientHandler
        {
            get
            {
                if (__clientHandler == null || IsDisposed(__clientHandler))
                {
                    __clientHandler = new HttpClientHandler();
                }

                return __clientHandler;
            }
        }

        public static bool IsDisposed(IDisposable obj)
        {
            try
            {
                if (obj is HttpClient)
                {
                    ((HttpClient)obj).BaseAddress = ((HttpClient)obj).BaseAddress;
                }
                else
                {
                    obj.GetHashCode();
                    obj.GetType();
                    obj.ToString();
                }

                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
