using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace password_manager.Services
{
    public class PasswordGenerator
    {
        private static readonly Random rnd = new Random();

        public string Generate(int length, bool upper, bool lower, bool digits)
        {
            string chars = "";
            if (upper) chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_-+=<>?";
            if (lower) chars += "abcdefghijklmnopqrstuvwxyz!@#$%^&*()_-+=<>?";
            if (digits) chars += "0123456789!@#$%^&*()_-+=<>?";

            if (chars.Length == 0) return "";

            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[rnd.Next(chars.Length)])
                .ToArray());
        }
    }
}