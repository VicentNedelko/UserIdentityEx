using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserIdentityEx.Models.Enums;

namespace UserIdentityEx.Models
{
    public class Doctor : IdentityUser
    {
        public string LName { get; set; }
        public string FName { get; set; }
        public int Age { get; set; }
        public PositionCode Code { get; set; }
        public int Category { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? RetiredDate { get; set; }

        // Department FK
        public int DepartmentId { get; set; }
        public Department Departmnet { get; set; }
    }
}
