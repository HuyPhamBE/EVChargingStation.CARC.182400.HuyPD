using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVChargingStation.CARC.Domain.HuyPD.DTOs.AuthDTO
{
    public class LoginDTO
    {
        [DefaultValue("huy@admin.com")]
        public string Email { get; set; }
        [DefaultValue("123456")]
        public string Password { get; set; }    
    }
}
