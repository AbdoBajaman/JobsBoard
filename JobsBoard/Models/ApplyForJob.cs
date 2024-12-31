using JobsBoard.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobsBoard.Models
{
    public class ApplyForJob
    {


        public int Id { get; set; }
   
        [DisplayName(" خطاب التوظيف")]
        public string Message { get; set; }

        [DisplayName("السيرة الذاتيه")]
        public string CVPath { get; set; }
        public DateTime ApplayDate { get; set; }

        public int JobId { get; set; }


        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null!;

        public string UserId { get; set; }


        [ForeignKey("UserId")]
        public virtual JobsBoardUser User { get; set; } = null!;
    }
}
