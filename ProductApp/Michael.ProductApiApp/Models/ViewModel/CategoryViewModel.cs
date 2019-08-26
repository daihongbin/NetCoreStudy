using System.ComponentModel.DataAnnotations.Schema;

namespace Michael.ProductApiApp.Models.ViewModel
{
    [NotMapped]
    public class CategoryViewModel
    {
        /// <summary>
        /// 分类id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 父类编号
        /// </summary>
        public int ParentId { get; set; }

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

    }
}
