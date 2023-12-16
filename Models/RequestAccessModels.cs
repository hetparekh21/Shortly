using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shortly.Models
{
    [Table("RequestAccess")]
    public class RequestAccessModels
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public UrlModels Url { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public String Note { get; set; }

        public bool Granted { get; set; } = false;


    }
}