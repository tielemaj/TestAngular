using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [Required, MinLength(5)] 
        public string Password { get; set; }
    }
}