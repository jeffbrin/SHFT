using SHFT.Config;
using SHFT.Models;
using SHFT.Services;

namespace SHFT.Repos
{
    internal class AccountRepo : DatabaseService<DatabaseAccount>
    {
        private static AccountRepo _instance;

        private AccountRepo(Firebase.Auth.User user, string path, string BaseUrl, string key = "") : base(user, path, BaseUrl, key)
        {
        }

        /// <summary>
        /// Gets the singleton instance of the repo.
        /// </summary>
        /// <returns>The singleton instance of this repo.</returns>
        public static AccountRepo GetInstance()
        {
            return _instance ??= new AccountRepo(AuthService.Client.User, nameof(Account), ResourceStrings.FIREBASE_DATABASEURL, nameof(Account));
        }

        public async void AddAccount(DatabaseAccount account)
        {
            AddItemAsync(account);
        }

        public async Task<AccountType> GetAccountType(string email)
        {
            IEnumerable<DatabaseAccount> accounts = await GetItemsAsync();
            return accounts.Where(a => a.Email == email).Select(e => e.AccountType).First();
        }
    }
}
