using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace Ecuafact.Web.Models
{ 
    public class FeedModel
    {
        public string Title { get; set; }

        public List<MenuOption> Menu { get; set; } = new List<MenuOption>();

        public FeedModel()
        {

        }
        public FeedModel(IEnumerable<FeedItemModel> list)
        {
            this.List = new List<FeedItemModel>(list);
        }

        public List<FeedItemModel> List { get; private set; } = new List<FeedItemModel>();
    }


    public class FeedItemModel
    {
        public FeedItemModel() { }

        public string Icon { get; set; } = "check";
        public string IconStyle { get; set; } = "info";
        public string Description { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;

        public string Time
        {
            get
            {
                return (DateTime.Now - Date).Ago();
            }
        }
    }
}