using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class RegisterDto
    {
        [Required,StringLength(100)]
        public string Name { get; set; }=null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;  
        [Required, StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }=null!;
        [Required]
        [StringLength(100, MinimumLength = 8)] // حسب شرط الـ 8 رموز في الـ SRS
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password Should Contain Upper Case and Lowere case letter and special Character ")]
        public string Password { get; set; }=null!;
    }
}
