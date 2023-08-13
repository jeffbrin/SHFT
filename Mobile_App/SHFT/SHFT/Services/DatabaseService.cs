using Firebase.Database;
using Firebase.Database.Offline;
using SHFT.Interfaces;
using System.Collections.ObjectModel;

namespace SHFT.Services
{
    public class DatabaseService<T> : IDataStore<T> where T : class, IHasUKey
    {

        protected readonly RealtimeDatabase<T> _realtimeDb;

        public DatabaseService(Firebase.Auth.User user, string path, string BaseUrl, string key = "")
        {
            FirebaseOptions options = new FirebaseOptions()
            {
                OfflineDatabaseFactory = (t, s) => new OfflineDatabase(t, s),
                AuthTokenAsyncFactory = async () => await user.GetIdTokenAsync()
            };
            // The offline database filename is named after type T.
            // So, if you have more than one list of type T objects, you need to differentiate it
            // by adding a filename modifier; which is what we're using the "key" parameter for.
            var client = new FirebaseClient(BaseUrl, options);
            _realtimeDb =
                client.Child(path)
                .AsRealtimeDatabase<T>(key, "", StreamingOptions.LatestOnly, InitialPullStrategy.MissingOnly, true);
        }

        private ObservableCollection<T> _items;
        public ObservableCollection<T> Items
        {
            get
            {
                if (_items == null)
                    Task.Run(() => LoadItems()).Wait();
                return _items;
            }
        }

        private async Task LoadItems()
        {
            _items = new ObservableCollection<T>(await GetItemsAsync());
        }

        public async Task<bool> AddItemAsync(T item)
        {
            try
            {
                string key = _realtimeDb.Post(item); //returns the unique key 
                (item).Key = key; //Using the interface IHasKey to save the key to object
                _realtimeDb.Put(key, item); //Update the entry in the database to maintain the key
                Items.Add(item); //place new item in the observable collection for UI display
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(T item)
        {
            try
            {
                string key = item.Key;
                _realtimeDb.Put(key, item);

                // Might be a better version than this.
                Items[Items.IndexOf(item)] = item;
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(T item)
        {
            try
            {
                _realtimeDb.Delete(item.Key);
                Items.Remove(item);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                try
                {
                    await _realtimeDb.PullAsync();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            var result = _realtimeDb.Once().Select(x => x.Object);
            return await Task.FromResult(result);
        }

    }
}
