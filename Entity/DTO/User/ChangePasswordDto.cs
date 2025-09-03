using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTO.User
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Mevcut şifre gereklidir.")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Yeni şifre gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Şifre onayı gereklidir.")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre ve onayı eşleşmiyor.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
