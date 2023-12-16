using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shortly.Models
{
    [Table("Stats")]
    public class StatsModels
    {

        public int Id { get; set; }

        [Required]
        public UrlModels Url { get; set; }

        public string BrowserType { get; set; }

        public string IpAddress { get; set; }

        public string Location { get; set; }

        public string DeviceType { get; set; }

        public bool isQR { get; set; } = false;

        public DateTime HitAt { get; set; }

    }
}