using System.ComponentModel.DataAnnotations;

namespace RecordHub.IdentityService.Domain.Models
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
