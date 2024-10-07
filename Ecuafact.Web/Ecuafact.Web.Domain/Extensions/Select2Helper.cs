using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System 
{
    public static class Select2Helper
    {
        public static Select2List<TType> ToSelect2<TType>(this IEnumerable<Select2ListItem<TType>> list, bool more = false)
        {
            return new Select2List<TType>(list, more);
        }
    }

    public class Select2List<TType>
    {
        public Select2List()
        {
            results = new List<Select2ListItem<TType>>();
            pagination = new Select2Pagination(false);
        }

        public Select2List(IEnumerable<Select2ListItem<TType>> result, bool more = false)
        {
            results = new List<Select2ListItem<TType>>(result);
            pagination = new Select2Pagination(more);
        }

        public List<Select2ListItem<TType>> results { get; set; }

        public Select2Pagination pagination { get; set; }
    }

    public class Select2Pagination
    {
        public Select2Pagination(bool morepage)
        {
            more = morepage;
        }

        public bool more { get; private set; } = false;
    }

    public class Select2ListItem<T>
    {
        public Select2ListItem(T value)
        {
            this.data = value;
        }

        public object id { get; set; }
        public string text { get; set; }
        public T data { get; set; }
    }
}