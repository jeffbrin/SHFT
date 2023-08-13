// SHFT - H
// Winter 2023
// April 28th 2023
// Application Development III
// This class acts as a container repository which contains a list
// of containers that represent UI elements and relationships
// between the IOT Hub.

using SHFT.Config;
using SHFT.Models;
using SHFT.Services;

namespace SHFT.Repos
{
    internal class ContainerRepo : DatabaseService<Container>
    {
        private const int START_LOOP = 0;
        private const int END_LOOP = 2;
        private const string DUMMY_CON_STRING = "CON_STR";
        private const string DUMMY_NAME_CONTAINER = "CONTAINER";
        private const string DUMMY_DESCRIPTION_CONTAINER = "DESCRIPTION";
        private const string DUMMY_ID = "ID";

        /// <summary>
        /// An list of containers.
        /// </summary>
        public List<Container> _containers;

        /// <summary>
        /// Gets the single instance of the container repo.
        /// </summary>
        /// <returns>A singleton instance of the ContainerRepo class.</returns>
        private static ContainerRepo _instance;

        /// <summary>
        /// Initializes Container Repo and calls to add to the list of container.
        /// </summary>
        private ContainerRepo(Firebase.Auth.User user, string path, string BaseUrl, string key = "") : base(user, path, BaseUrl, key)
        {
        }

        /// <summary>
        /// Creates a single instance of the Container Repo to be used.
        /// </summary>
        /// <returns>An singleton container repo.</returns>
        public static ContainerRepo GetInstance()
        {
            if (_instance is null)
                _instance = new ContainerRepo(AuthService.Client.User, nameof(Container), ResourceStrings.FIREBASE_DATABASEURL, nameof(Container));
            return _instance;
        }

        /// <summary>
        /// Add's to list of container 2 new ones.
        /// </summary>
        public async Task PopulateContainer()
        {
            foreach (var container in await GetItemsAsync())
            {
                await DeleteItemAsync(container);
            }

            if ((await GetItemsAsync()).Count() > 0)
                return;

            _containers = new List<Container>();
            for (int i = START_LOOP; i < END_LOOP; i++)
            {
                Container container = new(DUMMY_CON_STRING, DUMMY_NAME_CONTAINER, DUMMY_DESCRIPTION_CONTAINER, DUMMY_ID);
                container.Name += $"{i}";
                container.DeviceId = App.connectionsApp.DeviceId;
                container.Description += $" {i}";
                await AddItemAsync(container);
            }
        }

        public async Task<IEnumerable<Container>> GetContainers()
        {
            await PopulateContainer();
            IEnumerable<Container> containers = await GetItemsAsync();
            return containers;
        }
    }
}
