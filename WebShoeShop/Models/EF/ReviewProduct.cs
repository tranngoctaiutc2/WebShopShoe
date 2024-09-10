using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShoeShop.Models.EF
{
    [Table("tb_Review")]
    public class ReviewProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }  
        public string UserName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string FullName { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }
        public string Content { get; set; } 
        public int Rate { get; set; }   
        public DateTime CreateDate { get; set; }
        public string Avatar { get; set; }
        public virtual Product Product { get; set; }

    }
}