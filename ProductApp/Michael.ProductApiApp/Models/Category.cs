using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Michael.ProductApiApp.Models
{
    public class Category
    {
        /// <summary>
        /// 分类id
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryID { get; set; }

        /// <summary>
        /// 父类编号
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 显示排序
        /// </summary>
        public int ViewOrder { get; set; }

        /// <summary>
        /// 分类描述
        /// </summary>
        public string Description { get; set; }

        [JsonIgnore]
        public List<Goods> Goodses { get; set; }
    }
}