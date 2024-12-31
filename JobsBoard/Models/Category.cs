using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobsBoard.Models
{
    public class Category
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "نوع الوظيفه")]
        public string CategoryName { get; set; }


        [DisplayName("وصف النوع")]
        public string CategoryDescription { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
