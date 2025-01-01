using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sldc.Model
{
    public static class HexValidator
    {
        public static bool ValidateHexCode(string input)
        {
            string pattern = "^[0-9A-F]*$";
            bool isValid = Regex.IsMatch(input.Substring(1), pattern);

            if (!input.Contains("#") || (input.Length != 7 && input.Length > 1) || !isValid)
                return false;
            return true;
        }
    }
}
