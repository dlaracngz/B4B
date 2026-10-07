using System.Text.RegularExpressions;

namespace B4B.Application.Security
{
    public class PasswordValidator
    {
        public bool IsValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8)
                return false;

            if (!Regex.IsMatch(password, "[A-Z]"))
                return false;

            if (!Regex.IsMatch(password, "[a-z]"))
                return false;

            if (!Regex.IsMatch(password, "[0-9]"))
                return false;

            if (!Regex.IsMatch(password, @"[\W_]"))
                return false;

            return true;
        }
    }
}