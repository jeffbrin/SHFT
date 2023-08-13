// SHFT - H
// Winter 2023
// April 26th 2023
// Application Development III
// This class will be used to set up the connection to the IOT Hub.

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Primitives;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json.Linq;
using SHFT.Controllers;
using SHFT.Interfaces;
using SHFT.Models;
using SHFT.Repos;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;

// SHFT - H
// Winter 2023
// May 12th 2023
// Application Development III
// Manages the connection to the cloud.

namespace SHFT.Services
{
    public class ConnectionManager
    {
        private static ConnectionManager _instance;
        private static BlobContainerClient _blobContainerClient;
        private static EventProcessorClient _eventProcessClient;
        private string _deviceId;
        private readonly ITelemetryRepo _telemetryRepo;
        private readonly ServiceClient serviceClient;
        private readonly RegistryManager registryManager;
        private readonly ConcurrentDictionary<string, int> partitionEventCount;

        private ConnectionManager(ITelemetryRepo telemetryRepo)
        {

            partitionEventCount = new ConcurrentDictionary<string, int>();

            _blobContainerClient = new BlobContainerClient(App.connectionsApp.StorageConnectionString,
                App.connectionsApp.BlobContainerName);

            _eventProcessClient = new EventProcessorClient(_blobContainerClient,
                App.connectionsApp.ConsumerGroup,
                App.connectionsApp.EventHubConnectionString,
                App.connectionsApp.EvenHubName);

            _telemetryRepo = telemetryRepo;

            serviceClient = ServiceClient.CreateFromConnectionString(App.connectionsApp.HubConnectionString);
            registryManager = RegistryManager.CreateFromConnectionString(App.connectionsApp.HubConnectionString);

        }


        public async Task SetDeviceTwin(string property, string value)
        {
            var twin = await registryManager.GetTwinAsync(_deviceId);

            Console.WriteLine(twin.Properties.Reported);

            // Temp
            var patch =
                $@"{{
            properties: {{
                desired: {{
                    {property}: {value},
                }}
            }}
        }}";
            await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
            Debug.WriteLine($"{property} set to {value}.");

        }


        public async Task Connect(string deviceId = null)
        {
            if (deviceId is not null)
                this._deviceId = deviceId;

            await _telemetryRepo.Start();

            try
            {

                _eventProcessClient.ProcessEventAsync += ProcessEventHandler;
                _eventProcessClient.ProcessErrorAsync += ProcessErrorHandler;

                try
                {
                    await _eventProcessClient.StartProcessingAsync();
                    await Task.Delay(Timeout.Infinite);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"ERROR while processing event: {e.Message}");
                }
                finally
                {
                    // This may take up to the length of time defined
                    // as part of the configured TryTimeout of the processor;
                    // by default, this is 60 seconds.

                    await _eventProcessClient.StopProcessingAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR occurred: {ex.Message}");
            }
            finally
            {
                _eventProcessClient.ProcessEventAsync -= ProcessEventHandler;
                _eventProcessClient.ProcessErrorAsync -= ProcessErrorHandler;
            }




        }
        public async Task ProcessEventHandler(ProcessEventArgs args)
        {
            try
            { 

                string partition = args.Partition.PartitionId;
                Debug.WriteLine($"{args.Data.EventBody}");

                int eventsSinceLastCheckpoint = partitionEventCount.AddOrUpdate(
                    key: partition,
                    addValue: 1,
                    updateValueFactory: (_, currentCount) => currentCount + 1);

                if (eventsSinceLastCheckpoint >= 50)
                {
                    await args.UpdateCheckpointAsync();
                    partitionEventCount[partition] = 0;
                }
                RouteData(args.Data.EventBody);
                long checkpoint = args.Data.Offset;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"ERROR while processing event: {e.Message}");
            }
        }

        private void RouteData(BinaryData eventBody)
        {


            JObject json = JObject.Parse(eventBody.ToString());
            _telemetryRepo.PassData(json);


        }


        public async Task ProcessErrorHandler(ProcessErrorEventArgs args)
        {
            Debug.WriteLine($"ERROR: {args}");
        }

        public static ConnectionManager GetInstance()
        {
            if (_instance is null)
                _instance = new ConnectionManager(TelemetryRepo.GetInstance());

            return _instance;
        }

        // Invoke the direct method on the device, passing the payload.
        // https://github.com/Azure/azure-iot-sdk-csharp/blob/main/iothub/service/samples/getting%20started/InvokeDeviceMethod/Program.cs
        public async Task<bool> InvokeMethodAsync(string method, string payload)
        {
            int RESPONSE_TIMEOUT = 5;
            var methodInvocation = new CloudToDeviceMethod(method)
            {
                ResponseTimeout = TimeSpan.FromSeconds(RESPONSE_TIMEOUT),
            };
            methodInvocation.SetPayloadJson(payload);

            Debug.WriteLine($"Invoking direct method for device: {_deviceId}");

            // Invoke the direct method asynchronously and get the response from the simulated device.
            CloudToDeviceMethodResult response;
            try
            {
                response = await serviceClient.InvokeDeviceMethodAsync(_deviceId, methodInvocation);
            }
            catch (DeviceNotFoundException ex){
                return false;
            }
            Debug.WriteLine($"Response status: {response.Status}, payload:\n\t{response.GetPayloadAsJson()}");

            // TODO: Return whether it was successful.
            return response.Status == 200;

            }
        }
    }
