using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AltSourceCore
{
    public static class UserProfileManager
    {
        private static List<UserProfile> userProfiles;

        static UserProfileManager()
        {
            UserProfileManager.userProfiles = new List<UserProfile>();
            UserProfileManager.userProfiles.Add(new UserProfile
                {
                    ID = "system",
                    Password = null
                }
            );
        }

        public static void AddProfile(string userID, string password)
        {
            if (!String.IsNullOrWhiteSpace(userID) && !String.IsNullOrWhiteSpace(password))
            {
                if (UserProfileManager.userProfiles == null)
                {
                    UserProfileManager.userProfiles = new List<UserProfile>();
                }
                if (GetProfile(userID) != null)
                {
                    throw new ApplicationException(String.Format("User ID {0} already exists", userID));
                }
                UserProfileManager.userProfiles.Add(new UserProfile
                    {
                        ID = userID,
                        Password = password
                    }
                );
            }
        }

        public static UserProfile GetProfile(string userID)
        {
            var profile = default(UserProfile);
            if (UserProfileManager.userProfiles != null)
            {
                profile = userProfiles.FirstOrDefault(a => a.ID.Equals(userID, StringComparison.InvariantCultureIgnoreCase));
            }
            return profile;
        }

        public static bool IsPasswordValid(string userID, string password)
        {
            var isValid = false;
            if (UserProfileManager.userProfiles != null)
            {
                var profile = userProfiles.FirstOrDefault(a => a.ID.Equals(userID, StringComparison.InvariantCultureIgnoreCase));
                if (profile != null)
                {
                    if (profile.Password == password)
                    {
                        isValid = true;
                    }
                }
            }
            return isValid;
        }
    }
}
