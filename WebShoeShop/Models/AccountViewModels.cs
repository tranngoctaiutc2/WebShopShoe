using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShoeShop.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage ="Tên đăng nhập không được để trống")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class CreateAccountViewModel
    {
   /*     [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }*/
        [Required(ErrorMessage ="Tên tài khoản không được để trống")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải là chữ số và gồm 10 chữ số")]
        public string Phone { get; set; }
        public List<string> Roles { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "Mật khẩu phải có ít nhất 6 kí tự và bao gồm chữ cái, chữ cái in hoa, chữ số và kí tự đặc biệt")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không chính xác")]
        public string ConfirmPassword { get; set; }
    }
    public class EditAccountViewModel
    {
        /*     [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
             public string Id { get; set; }*/
        [Required(ErrorMessage = "Tên tài khoản không được để trống")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải là chữ số và gồm 10 chữ số")]
        public string Phone { get; set; }
        public List<string> Roles { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage ="Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage ="Email không được để trống")]
        [EmailAddress(ErrorMessage ="Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "Mật khẩu phải có ít nhất 6 kí tự và bao gồm chữ cái, chữ cái in hoa, chữ số và kí tự đặc biệt")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage ="Email không được để trống")]
        [EmailAddress(ErrorMessage ="Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu ít nhất {2} kí tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Mật khẩu phải trùng khớp")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
