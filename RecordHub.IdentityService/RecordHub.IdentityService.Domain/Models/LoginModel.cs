using System.ComponentModel.DataAnnotations;

namespace RecordHub.IdentityService.Domain.Models
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
