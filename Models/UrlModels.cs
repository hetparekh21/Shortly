using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shortly.Models
{
    [Table("Url")]
    public class UrlModels
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RedirectTo { get; set; }

        [Required]
        [StringLength(10)]
        public string BackHalf { get; set; }

        public bool Qr { get; set; } = false;

        [StringLength(128)]
        public String User_id { get; set; }

        public bool Auth { get; set; } = false;

        public string Password { get; set; }

        public bool Active { get; set; }

    }
}