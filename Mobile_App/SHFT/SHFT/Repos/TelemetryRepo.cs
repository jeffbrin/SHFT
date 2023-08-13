using Newtonsoft.Json.Linq;
using SHFT.Controllers;
using SHFT.Interfaces;
using SHFT.Models;
using SHFT.Services;
using System.Diagnostics;

// SHFT - H
// Winter 2023
// May 12th 2023
// Application Development III
// Routes telemetry data to the controllers.

namespace SHFT.Repos
{
    /// <summary>
    /// The repo which routes telemetry data.
    /// </summary>
    internal class TelemetryRepo : ITelemetryRepo
    {

        const string SECURITY_SUBSYSTEM = "SecuritySubsystem";
        const string PLANT_SUBSYSTEM = "PlantSubsystem";
        const string GEO_LOCATION_SUBSYSTEM = "GeoLocationSubsystem";
        const string TIMESTAMP = "timestamp";
        const string READING_TYPE = "reading_type";
        const string VALUE = "value";
        const string DEGREES = "degrees";
        const string MINUTES = "minutes";
        const string LATITUDE_DIRECTION = "Latitude Direction";
        const string LONGITUDE_DIRECTION = "Longitude Direction";
        const string SOUTH = "S";
        const string WEST = "W";
        const string READING_UNIT = "reading_unit";
        private DateTime _mostRecentReadingTimestamp = DateTime.UnixEpoch;
        public const string TELEMETRY_INTERVAL_TWIN_NAME = "telemetryInterval";

        private static TelemetryRepo _instance;

        private TelemetryRepo()
        {

        }

        public static TelemetryRepo GetInstance()
        {
            if (_instance is null)
                _instance = new TelemetryRepo();

            return _instance;
        }

        public async Task Start()
        {
            // Get the most recent reading timstamp so that we don't process old readings.
            // Event hub processor doesn't have a way of "accepting" a message so it sends he past 24h every time.
            if (_mostRecentReadingTimestamp == DateTime.UnixEpoch)
                _mostRecentReadingTimestamp = await HistoricalPlantDataRepo.GetInstance().GetMostRecentReading();
        }

        /// <summary>
        /// Received data from the connection manager and processes it.
        /// </summary>
        /// <param name="data">The data to be processed.</param>
        public void PassData(JObject data)
        {

            try
            {
                try
                {
                    JArray securityReadings = (JArray)data[SECURITY_SUBSYSTEM];
                    SetSecurityValues(securityReadings);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                try
                {
                    JArray plantReadings = (JArray)data[PLANT_SUBSYSTEM];
                    SetPlantValues(plantReadings);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                try
                {
                    JArray geo_location_Readings = (JArray)data[GEO_LOCATION_SUBSYSTEM];
                    SetGeoLocationValues(geo_location_Readings);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }



            }
            catch (Exception e)
            {

                return;
            }
        }

        /// <summary>
        /// Creates a reading.
        /// </summary>
        /// <typeparam name="T">The type of data the reading stores.</typeparam>
        /// <param name="value">The value of the reading.</param>
        /// <param name="unit">The reading's unit.</param>
        /// <param name="timestamp">The time at which the reading was created.</param>
        /// <param name="type">The type of reading.</param>
        /// <returns>A new reading containing the data passed to this method.</returns>
        private Reading<T> CreateReading<T>(T value, string unit, DateTime timestamp, string type)
        {
            return new Reading<T>() { Value = value, Unit = unit, Timestamp = timestamp, Type = type };
        }

        /// <summary>
        /// Converts a string into a datetime.
        /// </summary>
        /// <param name="value">The string timestamp.</param>
        /// <returns></returns>
        private DateTime StringToTimestamp(string value)
        {
            string[] splitTimestamp = value.Split();
            string date = splitTimestamp[0];
            string time = splitTimestamp[1];
            string[] splitDate = date.Split('-');
            string[] splitTime = time.Split(':');

            int year = int.Parse(splitDate[0]);
            int month = int.Parse(splitDate[1]);
            int day = int.Parse(splitDate[2]);

            int hour = int.Parse(splitTime[0]);
            int minute = int.Parse(splitTime[1]);
            int second = (int)float.Parse(splitTime[2]);

            return new DateTime(year, month, day, hour, minute, second);
        }

        /// <summary>
        /// Sets the geo-location values in the controller.
        /// </summary>
        /// <param name="values">A JArray of telemetry data.</param>
        private void SetGeoLocationValues(JArray values)
        {
            foreach (JToken value in values)
            {
                var parsedValue = JObject.Parse(value.ToString());
                DateTime timestamp = StringToTimestamp(parsedValue[TIMESTAMP].ToString());

                // Ignore old readings
                if (timestamp <= _mostRecentReadingTimestamp)
                    return;

                string type = parsedValue[READING_TYPE].ToString();

                try
                {
                    // value. reading type 
                    switch (type)
                    {
                        case Reading<object>.TypeOptions.GEO_LOCATION:

                            //degrees + (minutes / 60) + (seconds / 3600)
                            JObject latitudeData = JObject.Parse(parsedValue[VALUE][Reading<object>.TypeOptions.LATITUDE].ToString());
                            JObject longitudeData = JObject.Parse(parsedValue[VALUE][Reading<object>.TypeOptions.LONGITUDE].ToString());

                            float decimalLatitude = float.Parse(latitudeData[DEGREES].ToString()) + float.Parse(latitudeData[MINUTES].ToString()) / 60;
                            float decimalLongitude = float.Parse(longitudeData[DEGREES].ToString()) + float.Parse(longitudeData[MINUTES].ToString()) / 60;

                            if (parsedValue[VALUE][LATITUDE_DIRECTION].ToString() == SOUTH)
                                decimalLatitude *= -1;
                            if (parsedValue[VALUE][LONGITUDE_DIRECTION].ToString() == WEST)
                                decimalLongitude *= -1;

                            GeoLocationController.GetInstance().SetLatitude(
                                CreateReading(decimalLatitude,
                                    Reading<object>.UnitOptions.DEGREES,
                                    timestamp,
                                    Reading<object>.TypeOptions.LATITUDE)
                                );
                            GeoLocationController.GetInstance().SetLongitude(
                                CreateReading(decimalLongitude,
                                    Reading<object>.UnitOptions.DEGREES,
                                    timestamp,
                                    Reading<object>.TypeOptions.LONGITUDE)
                                );
                            break;
                        case Reading<object>.TypeOptions.PITCH:
                            GeoLocationController.GetInstance().SetPitch(
                                CreateReading(float.Parse(parsedValue[VALUE].ToString()),
                                    parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type)
                                );
                            break;
                        case Reading<object>.TypeOptions.ROLL:
                            GeoLocationController.GetInstance().SetRoll(
                                CreateReading(float.Parse(parsedValue[VALUE].ToString()),
                                    parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type)
                                );
                            break;
                        case Reading<object>.TypeOptions.VIBRATION:
                            GeoLocationController.GetInstance().SetVibration(
                                CreateReading(float.Parse(parsedValue[VALUE].ToString()),
                                    parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type)
                                );
                            break;
                    }
                }
                // Catch errors caused by old bad readings.
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Sets the plant values in the controller.
        /// </summary>
        /// <param name="values">A JArray of telemetry data.</param>
        private void SetPlantValues(JArray values)
        {
            foreach (JToken value in values)
            {
                var parsedValue = JObject.Parse(value.ToString());
                DateTime timestamp = StringToTimestamp(parsedValue[TIMESTAMP].ToString());

                // Ignore old readings
                if (timestamp <= _mostRecentReadingTimestamp)
                    return;

                string type = parsedValue[READING_TYPE].ToString();

                // value. reading type 
                try
                {
                    switch (type)
                    {
                        case Reading<object>.TypeOptions.TEMPERATURE:
                            PlantController.GetInstance().SetTemperature(
                                CreateReading(float.Parse(parsedValue[VALUE].ToString()),
                                    parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type)
                                );
                            break;
                        case Reading<object>.TypeOptions.HUMIDITY:
                            PlantController.GetInstance().SetHumidity(
                                CreateReading(float.Parse(parsedValue[VALUE].ToString()),
                                    parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type)
                                );
                            break;
                        case Reading<object>.TypeOptions.WATER_LEVEL:
                            PlantController.GetInstance().SetWaterLevel(
                                CreateReading(float.Parse(parsedValue[VALUE].ToString()),
                                    parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type)
                                );
                            break;
                        case Reading<object>.TypeOptions.SOIL_MOISTURE:
                            PlantController.GetInstance().SetSoilMoistures(
                                new List<Reading<float>>(){
                                    CreateReading(float.Parse(parsedValue[VALUE].ToString()),
                                    parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type) 
                                }
                                );
                            break;


                    }
                }
                // Catch errors caused by old bad readings.
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Sets the security values in the controller.
        /// </summary>
        /// <param name="values">A JArray of telemetry data.</param>
        private void SetSecurityValues(JArray values)
        {
            foreach (JToken value in values)
            {

                var parsedValue = JObject.Parse(value.ToString());
                DateTime timestamp = StringToTimestamp(parsedValue[TIMESTAMP].ToString());

                // Ignore old readings
                if (timestamp <= _mostRecentReadingTimestamp)
                    return;

                string type = parsedValue[READING_TYPE].ToString();

                try
                {
                    // value. reading type 
                    switch (type)
                    {
                        case Reading<object>.TypeOptions.NOISE:
                            SecurityController.GetInstance().SetNoiseLevel(
                                CreateReading(
                                    float.Parse(parsedValue[VALUE].ToString()),
                                    parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type)
                                );
                            break;
                        case Reading<object>.TypeOptions.MOTION:
                            SecurityController.GetInstance().SetMotionSensor(
                                CreateReading(bool.Parse(parsedValue[VALUE].ToString()),
                                parsedValue[READING_UNIT].ToString(),
                                    timestamp,
                                    type)
                                );
                            break;
                        case Reading<object>.TypeOptions.LUMINOSITY:
                            SecurityController.GetInstance().SetLuminosityLevels(
                                CreateReading(int.Parse(parsedValue[VALUE].ToString()),
                                parsedValue[READING_UNIT].ToString(),
                                timestamp,
                                type)
                            );
                        break;
                    case Reading<object>.TypeOptions.VIBRATION:
                        SecurityController.GetInstance().SetVibrationLevels(
                            CreateReading(float.Parse(parsedValue[VALUE].ToString()),
                            parsedValue[READING_UNIT].ToString(),
                                timestamp,
                                type)
                            );
                        break;

                    }
                }
                // Catch errors caused by old bad readings.
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        public async Task<bool> SetBuzzerState(bool state)
        {
            string METHOD = "buzzer-on";
            return await ConnectionManager.GetInstance().InvokeMethodAsync(METHOD, CreateJsonPayload(state));
        }

        public async Task<bool> SetFanState(bool state)
        {
            string METHOD = "fan-on";
            return await ConnectionManager.GetInstance().InvokeMethodAsync(METHOD, CreateJsonPayload(state));
        }

        public async Task<bool> SetLEDState(bool state)
        {
            string METHOD = "led-on";
            return await ConnectionManager.GetInstance().InvokeMethodAsync(METHOD, CreateJsonPayload(state));
        }

        public async Task<bool> SetLockState(bool state)
        {
            string METHOD = "door-lock";
            bool a = await ConnectionManager.GetInstance().InvokeMethodAsync(METHOD, CreateJsonPayload(state));
            return a;
        }


        private string CreateJsonPayload(bool state)
        {
            return $"{{\"value\":{state.ToString().ToLower()}}}";
        }
    }
}
