// Jeffrey Bringolf 2075606
// W-2023 April 24th 2023
// App Dev 3
// This class provides access to Firebase authentication. It is also used to store User details.

using Firebase.Auth;
using Firebase.Auth.Providers;
using SHFT.Config;
using SHFT.Models;

namespace SHFT.Services
{
    /// <summary>
    /// A class which provides access to the firebase authentication database and servers.
    /// </summary>
    internal class AuthService
    {
        private static readonly FirebaseAuthConfig config = new()
        {
            ApiKey = ResourceStrings.FIREBASE_APIKEY,
            AuthDomain = ResourceStrings.FIREBASE_AUTHDOMAIN,
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            },
        };

        /// <summary>
        /// The firebase authentication client used to authenticate.
        /// </summary>
        /// <returns>Returns a FirebaseAuthClient instance which provides access to the Firebase authentication database and servers.</returns>
        public static FirebaseAuthClient Client { get; } = new FirebaseAuthClient(config);

        /// <summary>
        /// A user Account object which stores a user's account information while they're signed in.
        /// </summary>
        public static Account UserAccount { get; set; }
    }
}
