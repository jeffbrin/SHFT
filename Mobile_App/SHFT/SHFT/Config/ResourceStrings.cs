// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// ResourceStrings: This class contains static resource strings that are used for connecting to servers. These strings are specifically used for Firebase authentication and database connection.

namespace SHFT.Config
{
    /// <summary>
    /// Contains static resource strings that are used for connecting to servers. These strings are specifically used for Firebase authentication and database connection.
    /// </summary>
    internal static class ResourceStrings
    {
        /// <summary>
        /// Firebase Authentication domain used for user authentication.
        /// </summary>
        public const string FIREBASE_AUTHDOMAIN = "shft-d7e11.firebaseapp.com";

        /// <summary>
        /// Firebase API key used to access Firebase services.
        /// </summary>
        public const string FIREBASE_APIKEY = "AIzaSyB9-bpIOihxfs8mPaSRg7szHiwItJnrYkM";

        /// <summary>
        /// Firebase Realtime Database URL to connect and access the Firebase Database.
        /// </summary>
        public const string FIREBASE_DATABASEURL = "https://shft-d7e11-default-rtdb.firebaseio.com/";

        /// <summary>
        /// Syncfusion license key used for the Syncfusion controls library.
        /// </summary>
        internal const string SYNCFUSION_LICENSE_KEY = "MjA5MzQyMkAzMjMxMmUzMjJlMzNGRlpHdU5HZ00xZW9LWGNERGFBMXZjK1BoOG5WRGEvZzF0ZDVrZ3NkTXVJPQ==";
    }
}
