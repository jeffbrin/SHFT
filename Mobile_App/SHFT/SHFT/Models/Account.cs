// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// Account Class
// This class represents an account which stores all the user's
// UserCredential details for firebase, along with the user's
// account type.

using Firebase.Auth;

namespace SHFT.Models
{
    /// <summary>
    /// An account which stores all the user's UserCredential details for firebase, along with the user's account type.
    /// </summary>
    class Account : UserCredential
    {
        /// <summary>
        /// The type of account this user has.
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class with the specified user, authentication credential, operation type, and account type.
        /// </summary>
        /// <param name="user">The <see cref="User"/> associated with this account.</param>
        /// <param name="authCredential">The <see cref="AuthCredential"/> associated with this account.</param>
        /// <param name="op">The <see cref="OperationType"/> associated with this account.</param>
        /// <param name="accountType">The <see cref="AccountType"/> of this account.</param>
        /// <returns>
        /// An instance of the <see cref="Account"/> class with the specified 
        /// user, authentication credential, operation type, and account type.
        /// </returns>
        public Account(User user, AuthCredential authCredential, OperationType op, AccountType accountType)
            : base(user, authCredential, op)
        {
            AccountType = accountType;
        }
    }
}
