using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShoeShop.Models.EF
{
    [Table("tb_ProductSize")]
    public class ProductSize
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Size { get; set; }
        public virtual Product Product { get; set; }
    }
}