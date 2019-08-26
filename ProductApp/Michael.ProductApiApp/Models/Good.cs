using Newtonsoft.Json;
using System;

namespace Michael.ProductApiApp.Models
{
    public class Goods
    {
        public int GoodsID { get; set; }

        public string GoodsSN { get; set; }

        public string GoodsName { get; set; }

        public int CategoryID { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal SalePrice { get; set; }

        public string Img { get; set; }

        public bool OnSale { get; set; }

        public DateTime InputTime { get; set; }

        public DateTime ModifyTime { get; set; }

        public bool IsReal { get; set; }

        public string BarCode { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }
    }
}