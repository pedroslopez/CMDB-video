using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDB.Models
{
    public class VersionValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string[] vals = ((string)value).Split('.');

            var isValid = vals.Length == 3
                && int.TryParse(vals[0], out int major)
                && int.TryParse(vals[1], out int minor)
                && int.TryParse(vals[2], out int patch)
                && major >= 0
                && minor >= 0
                && patch >= 0;

            return isValid;
        }

    }
}
