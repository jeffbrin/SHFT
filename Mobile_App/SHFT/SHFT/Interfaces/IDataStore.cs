// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This interface represents a data store for the application,
// containing all CRUD operations and a collection of items.

using System.Collections.ObjectModel;

namespace SHFT.Interfaces
{
    /// <summary>
    /// Defines methods for performing CRUD operations in a data store.
    /// </summary>
    /// <typeparam name="T">The type of items in the data store.</typeparam>
    public interface IDataStore<T>
    {
        /// <summary>
        /// Asynchronously adds an item to the data store.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <returns>
        /// Returns a task that represents the asynchronous 
        /// operation. The task result contains a boolean indicating
        /// if the operation was successful.
        /// </returns>
        Task<bool> AddItemAsync(T item);

        /// <summary>
        /// Asynchronously updates an existing item in the data store.
        /// </summary>
        /// <param name="item">The item to be updated.</param>
        /// <returns>
        /// Returns a task that represents the asynchronous operation. 
        /// The task result contains a boolean indicating if the 
        /// operation was successful.
        /// </returns>
        Task<bool> UpdateItemAsync(T item);

        /// <summary>
        /// Asynchronously deletes an item from the data store.
        /// </summary>
        /// <param name="item">The item to be deleted.</param>
        /// <returns>
        /// Returns a task that represents the asynchronous 
        /// operation. The task result contains a boolean 
        /// indicating if the operation was successful.
        /// </returns>
        Task<bool> DeleteItemAsync(T item);

        /// <summary>
        /// Asynchronously gets all items from the data store.
        /// </summary>
        /// <param name="forceRefresh">If true, the data store is refreshed before retrieving the items. Default is false.</param>
        /// <returns>
        /// Returns a task that represents the asynchronous operation. 
        /// The task result contains an IEnumerable of items from the 
        /// data store.
        /// </returns>
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);

        /// <summary>
        /// Gets the ObservableCollection of items in the data store. 
        /// This can be used for data binding.
        /// </summary>
        public ObservableCollection<T> Items { get; }
    }
}
