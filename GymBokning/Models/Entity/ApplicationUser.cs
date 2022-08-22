using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GymBokning.Models.Entity
{
    public class ApplicationUser: IdentityUser
    {
        [Required(ErrorMessage = "The Password is required! Minimum length: 5, should contain uppercase and digit. ")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long. ", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public DateTime TimeOfRegistration { get; set; }
        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }

    }
}
