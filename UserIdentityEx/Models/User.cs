using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserIdentityEx.Models
{
    public class User : IdentityUser
    {
        public int Age { get; set; }
        public string Code { get; set; }
    }
}
