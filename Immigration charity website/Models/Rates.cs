using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Immigration_charity_website.Models
{
    public class Rates
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 确保设置了默认值

        // Navigation property to link to ApplicationUser
        public virtual ApplicationUser User { get; set; }
    }

    public class AverageRatingViewModel
    {
        public int RatingValue { get; set; } // 用于下拉菜单选择的评分值
        public string UserRole { get; set; }
        public List<Rates> Ratings { get; set; } // 用于显示的评分列表
        public double AverageRating { get; set; } // 计算的平均评分
        public IEnumerable<SelectListItem> RatingOptions { get; set; } // 下拉菜单选项
    }

}
