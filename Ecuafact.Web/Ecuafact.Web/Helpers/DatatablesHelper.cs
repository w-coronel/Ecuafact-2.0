using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Web;

namespace Ecuafact.Web
{
    public static class DatatablesHelper
    {
        public delegate TType LoadDataFunc<TType>(string search, int page, int pagesize, string sortColumn = "") where TType : IEnumerable;

        public static DataInfo<TType> PaginateData<TType>(this HttpRequestBase request, LoadDataFunc<TType> predicate) 
            where TType: IEnumerable
        {
            try
            {
                var draw = request.Form.GetValues("draw").FirstOrDefault();
                var start = request.Form.GetValues("start").FirstOrDefault();
                var length = request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = request.Form.GetValues("columns[" + request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 10;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                int page = 1;
                if (skip > 0)
                {
                    page = Convert.ToInt32(decimal.Round(skip / pageSize, 0) + 1);
                }

                var data = predicate(searchValue, page, pageSize, sortColumn);

                //Returning Json Data    
                return new DataInfo<TType>
                {
                    Draw = draw,
                    RecordsFiltered = recordsTotal,
                    RecordsTotal = recordsTotal,
                    Data = data
                };
                 
            }
            catch (Exception)
            {
                //throw;
            }

            return new DataInfo<TType>
            {
                Draw = "1",
                RecordsFiltered = 100,
                RecordsTotal = 10,
                Data = default(TType)
            };

        }

    }


    public class DataInfo<T>
    {
        [JsonProperty("draw")]
        public string Draw { get; set; }

        [JsonProperty("recordsFiltered")]
        public int RecordsFiltered { get; set; }
         
        [JsonProperty("recordsTotal")]
        public int RecordsTotal { get; set; }


        [JsonProperty("data")]
        public T Data { get; set; }
    }


}



