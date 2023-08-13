// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// DatabaseAccount Class
// This class represents an account stored in the database and implements
// the IHasUKey interface.

using SHFT.Interfaces;

namespace SHFT.Models
{
    internal class DatabaseAccount : IHasUKey
    {
        /// <summary>
        /// Gets or sets the account type of the database account.
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Gets or sets the email associated with the database account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the key of the database account.
        /// </summary>
        public string Key { get; set; }
    }
}
