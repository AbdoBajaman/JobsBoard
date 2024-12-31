namespace JobsBoard.Models
{
    public class JobViewModel
    {
        public string JobTitle { get; set; }

        public ICollection<ApplyForJob> ApplyForJob { get; set; }
    }
}
