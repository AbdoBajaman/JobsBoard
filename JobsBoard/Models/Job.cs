using JobsBoard.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobsBoard.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يرجى إدخال عنوان الوظيفة")]
        [DisplayName("عنوان الوظيفه")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "يرجى إدخال وصف الوظيفة")]
        [DisplayName("وصف الوظيف")]
        public string JobContent { get; set; }

        //[DisplayName("تاريخ النشر")]
        //public DateTime Created_At { get; set; }

        [DisplayName("المهام الوظيفيه")]
        public string?  Responsibilities { get; set; }

        [DisplayName("المؤهلات والخبرات")]
        public string? QualificationsANDExperiences { get; set; }

        [DisplayName("مميزات العمل")]
        public string? Benefits { get; set; }



        [DisplayName("صورة الوظيفه")]
        public string? JobImage { get; set; }

        [Required(ErrorMessage = "يرجى تحديد نوع الوظيفة")]
        [DisplayName("نوع الوظيف")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public string? JobStatus { get; set; }

        [DisplayName("اسم الشركة")]
        public string? CompanyName { get; set; }


        public virtual ICollection<ApplyForJob>? ApplyForJobs { get; set; }

        public string? UserId { get; set; }
        public virtual JobsBoardUser? User { get; set; } = null!;



    }
}
