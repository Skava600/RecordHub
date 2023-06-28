using System.ComponentModel.DataAnnotations;

namespace RecordHub.IdentityService.Domain.Models
{
    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "Must be between 5 and 40 characters", MinimumLength = 5)]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Name consists only of latin or cyrillic letters")]
        [StringLength(40, ErrorMessage = "Name must be between 2 and 40 characters", MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Surname consists only of latin or cyrillic letters")]
        [StringLength(40, ErrorMessage = "Surname must be between 2 and 40 characters", MinimumLength = 2)]
        public string Surname { get; set; }
        [Required]
        [RegularExpression(@"^\+375\d{9}$", ErrorMessage = "Invalid Belarus phone number")]
        public string PhoneNumber { get; set; }
    }
}
