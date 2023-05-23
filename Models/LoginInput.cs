using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BullDoghs.Models
{
    public class LoginInput
    {
        [Key]
        [Required(ErrorMessage = "Please enter an username")]
        [MaxLength(60)]
        [Display(Name = "User Name", Prompt = "Example Email Address:  ***.***@bdhigh.edu")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [UIHint("password")]
        [MaxLength(50)]
        [Display(Name = "Password")]
        public string? UserPassword { get; set; }

        
        public string? ReturnURL
        {
            get; set;
        }
    }
}
