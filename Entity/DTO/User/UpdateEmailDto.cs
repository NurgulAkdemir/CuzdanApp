using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO.User
{
    public class UpdateEmailDto
    {
        [Required(ErrorMessage = "Yeni e-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string NewEmail { get; set; } = null!;

        [Required(ErrorMessage = "Şifre doğrulaması gereklidir.")]
        public string Password { get; set; } = null!;
    }
}
