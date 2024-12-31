using System.ComponentModel.DataAnnotations;

namespace JobsBoard.ViewModel
{
    public class Contact
    {
        [Required]
        public string Name { get; set; }


        [Required]
        public string Email { get; set; }

        [Required]
        public string subject { get; set; }

        [Required]
        public string  Message { get; set; }
    }
}
