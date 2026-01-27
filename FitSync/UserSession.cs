using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitSync
{
    public static class UserSession
    {
        public static string CurrentUsername { get; set; }

        public static void Login(string username)
        {
            CurrentUsername = username;
        }

        public static void Logout()
        {
            CurrentUsername = null;
        }

        public static bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(CurrentUsername);
        }
    }
}
