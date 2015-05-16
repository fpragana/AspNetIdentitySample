using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentitySample.Models
{
    public class GoogleAuthenticatorViewModel
    {
        [Required]
        public string Code { get; set; }

        public string SecretKey { get; set; }

        public string BarcodeUrl { get; set; }
    }
}
