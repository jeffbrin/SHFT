# <div align='center'> Welcome to SHFT</div>

# <div align='center'>(420-6A6-AB) APP DEV III <br>(420-6P3-AB) Connected Objects <br>Winter 2023</div>

## <div align='center'>Final Project</div>

# <div align='center'> </div>

# Table of contents

- [ Welcome to SHFT](#-welcome-to-shft)
- [(420-6A6-AB) APP DEV III (420-6P3-AB) Connected Objects Winter 2023](#420-6a6-ab-app-dev-iii-420-6p3-ab-connected-objects-winter-2023)
  - [Final Project](#final-project)
- [ ](#-)
- [Table of contents](#table-of-contents)
- [Team Information](#team-information)
- [Project Description](#project-description)
- [Mobile App](#mobile-app)
  - [App Setup](#app-setup)
  - [App functionality](#app-functionality)
  - [App Screenshots](#app-screenshots)
- [Controlling Actuators](#controlling-actuators)
    - [Actuator - Buzzer](#actuator---buzzer)
    - [Actuator - RGB LED Stick](#actuator---rgb-led-stick)
    - [Actuator - Fan](#actuator---fan)
    - [Actuator - Servo (Door Lock)](#actuator---servo-door-lock)
- [Intended Peripheral Base Hat GPIO Connections](#intended-peripheral-base-hat-gpio-connections)
- [Custom configurations](#custom-configurations)
- [Future Work](#future-work)
  - [Contributions](#contributions)


# Team Information

| Name | Student ID | Team Letter |
| :---:   | :---: | :---: |
| Jeffrey Bringolf | 2075606 | H |
| Abdessalam Ait Haqi | 1975004 | H |
| Pleasure Singh Ghotra | 6150381 | H |



# Project Description

SHFT is a comprehensive mobile application designed for seamless farm management. The app provides two key user roles: Fleet Owner and Farm Technician. Each role is equipped with unique capabilities to foster efficient, personalized, and real-time control over farm operations.



[(Back to top)](#table-of-contents)

# Mobile App

- As a farm technician, users can access real-time sensor readings, control various aspects of the farm like lighting and ventilation (known as actuators), and review historical data to track changes and patterns over time.

- Fleet owners, on the other hand, have access to a broader range of capabilities. They can monitor the data generated by the farm technicians, control actuators, review historical data, and track the precise location of each container in their fleet. This level of control and visibility ensures seamless operation and maximum productivity.

## App Setup
To get started with SHFT, you will need a Raspberry Pi with all the requisite sensors and actuators properly connected. If you are unfamiliar with how to set this up, please refer to our guide on [Intended Peripheral Base Hat GPIO Connections](#gpio_connection). It provides step-by-step instructions on how to correctly connect your devices to the Raspberry Pi.

- Once you have your Raspberry Pi set up, you need to modify the device connection string for Azure IoT devices in the .env file. This step is crucial as it allows your devices to establish a connection with the Azure IoT Hub, enabling the seamless transfer of data from your devices to the application.


Azure Configuration: You need to update the Azure configuration with your own values. This can be done in the appsetting.json file:
    
    EventHubConnectionString: Replace this with your own Event Hub Connection String.
    EventHubName: Replace this with your own Event Hub Name.
    ConsumerGroup: Replace this with your own Consumer Group.
    StorageConnectionString: Replace this with your own Storage Connection String.
    BlobContainerName: Replace this with your own Blob Container Name.
    HubConnectionString: Replace this with your own Hub Connection String.
    DeviceId: Replace this with your own Device Id.

- To ensure the smooth operation of the SHFT application, it's vital to configure the Firebase database correctly. This involves setting up Firebase Authentication and changing some values in the Firebase configuration.

Firebase Configuration: You need to update the Firebase configuration with your own values. This can be done in the config folder:

    Api Key: Replace this with your own Firebase API key.
    Auth Domain: Replace this with your own Firebase Auth Domain.
    Database URL: Replace this with your own Firebase Database URL.
    By following these steps, you ensure that the app is properly connected to your Firebase database and that all user data is securely managed.


## App functionality 

1. Farm Techs: Farm techs have the ability to monitor various sensor readings, such as temperature, humidity, luminosity, and noise level, within the farm environment. They can also control specific actuators based on the sensor readings and maintain optimal conditions for farm operations. Moreover, they have access to historical data, which can be utilized for analytical purposes and for making informed decisions regarding farm management.

Farm techs will have access to the following views:

- **Historical Plant Data**: This view provides a historical perspective on the various sensor readings. It's a valuable tool for analyzing trends over time and making informed decisions about future operations.
  
- **Plant Dashboard (Current Sensor Readings)**: This view displays the current readings from all the sensors. It provides real-time data that can be used to monitor the immediate conditions of the farm.

- **Soil Moisture**: This view specifically focuses on the soil moisture levels. This is crucial data as soil moisture significantly affects plant health and growth.

2. Fleet owners have a more supervisory role in the system. They can observe the same data as farm techs, allowing them to oversee the farm conditions from a high-level perspective. In addition, fleet owners have the power to control the door-lock of the farm, ensuring the security of the farm operations. They can also access historical data, providing them with valuable insights into the farm’s operational history. The application also offers a feature to track the location of the farm, making logistics and management easier for the fleet owner.

Fleet owners will have access to the following views:

- **Container Select**: This view enables fleet owners to select any container from their fleet for detailed observation and data analysis. This provides a centralized control over all containers in the fleet.

- **Container Location**: This view shows the geographical location of the selected container. This can be useful for logistics and tracking purposes.

- **Dashboard Security**: This view displays the current security status of the selected container. It provides real-time data that can be used to monitor the security conditions of the container.

- **Historical Data**: This view provides a historical perspective on the various sensor readings and security status. It's a valuable tool for analyzing trends over time and making informed decisions about future operations.

- **Security Toggle (Controlling the Actuators)**: This view allows fleet owners to remotely control the actuators (like door locks, fans, buzzers etc.) in the container. This feature provides an additional level of control and security.

## App Screenshots

<table>
  <tr>
    <td>
      <a href="https://i.imgur.com/j7clrei"><img src="https://i.imgur.com/j7clrei.png" title="source: imgur.com" /></a>
    </td>
    <td>
      <a href="https://i.imgur.com/jC7Bp2B" ><img src="https://i.imgur.com/jC7Bp2B.png" title="source: imgur.com" /></a>
    </td>
     <td>
      <a href="https://i.imgur.com/nVJQix3" ><img src="https://i.imgur.com/nVJQix3.png" title="source: imgur.com" /></a>
    </td>
      <td>
      <a href="https://i.imgur.com/VhJMvU2.png" ><img src="https://i.imgur.com/VhJMvU2.png" title="source: imgur.com" /></a>
    </td>
      <td>
      <a href="https://i.imgur.com/UeEXFuG.png" ><img src="https://i.imgur.com/UeEXFuG.png" title="source: imgur.com" /></a>
    </td>
      <td>
      <a href="https://i.imgur.com/UCeSaGk.png" ><img src="https://i.imgur.com/UCeSaGk.png" title="source: imgur.com" /></a>
    </td>
  </tr>
  <tr>
    <td>
      <a href="https://i.imgur.com/UwKY9oa"><img src="https://i.imgur.com/UwKY9oa.png" title="source: imgur.com" /></a>
    </td>
    <td>
      <a href="https://i.imgur.com/C4nneMG.png"><img src="https://i.imgur.com/C4nneMG.png" title="source: imgur.com" /></a>
    </td>
    <td>
      <a href="https://i.imgur.com/uFWtSam.png"><img src="https://i.imgur.com/uFWtSam.png" title="source: imgur.com"/></a>
    </td>
    <td>
      <a href="https://i.imgur.com/ZhSG3O2.png"><img src="https://i.imgur.com/ZhSG3O2.png" title="source: imgur.com"/></a>
    </td>
    <td>
      <a href="https://i.imgur.com/AlppN7u.png"><img src="https://i.imgur.com/AlppN7u.png" title="source: imgur.com"/></a>
    </td>
     <td>
      <a href="https://i.imgur.com/LZdlKg7.png"><img src="https://i.imgur.com/LZdlKg7.png" title="source: imgur.com"/></a>
    </td>
  </tr>
</table>

[(Back to top)](#table-of-contents)

# Controlling Actuators

**Strategy: Direct Method**

Reason: Direct Method was chosen over Device Twins and C2D messages for all the actuators because it provides most direct way of sending a message and immediately receiving a response. For this application, C2D are not the optimial choice since the caller does not receive an immediate response. Device Twins are also non-optimal since our application does not need to set a value while the farming container is offline. If the user sets a value and the container does not respond, that is valuable feeback for the service.

### Actuator - Buzzer

**Turn buzzer on**

`az iot hub invoke-device-method --hub-name {iothub_name} --device-id {device_id} --method-name buzzer-on --method-payload '{"value":true}'`

**Turn buzzer off**

`az iot hub invoke-device-method --hub-name {iothub_name} --device-id {device_id} --method-name buzzer-on --method-payload '{"value":false}'`

### Actuator - RGB LED Stick

**Turn RGB LED Stick on**

`az iot hub invoke-device-method --hub-name {iothub_name} --device-id {device_id} --method-name led-on --method-payload '{"value":true}'`

**Turn RGB LED Stick off**

`az iot hub invoke-device-method --hub-name {iothub_name} --device-id {device_id} --method-name led-on --method-payload '{"value":false}'`

### Actuator - Fan

**Turn fan on**

`az iot hub invoke-device-method --hub-name {iothub_name} --device-id {device_id} --method-name fan-on --method-payload '{"value":true}'`

**Turn fan off**

`az iot hub invoke-device-method --hub-name {iothub_name} --device-id {device_id} --method-name fan-on --method-payload '{"value":false}'`

### Actuator - Servo (Door Lock)

**Lock the door**

`az iot hub invoke-device-method --hub-name {iothub_name} --device-id {device_id} --method-name door-lock --method-payload '{"value":true}'`

**Unlock the door**

`az iot hub invoke-device-method --hub-name {iothub_name} --device-id {device_id} --method-name door-lock --method-payload '{"value":false}'`

# Intended Peripheral Base Hat GPIO Connections

| Peripheral | Connection    | Bus|
| :---:   | :---: | :---: |
| Water Level Sensor | A2 | 0x04 |
| Soil Moisture Sensor | A0 | 0x04 |
| RGB Led Stick | D18 | None |
| Cooling Fan | D5 | None |
| AHT20 Temp & Humidity Sensor | D26 | 0x38 |
| GPS (Air530) | UART | None |
| reTerminal’s built-in accelerometer | Built-In | None |
| reTerminal’s built-in buzzer | Built-In | None |
| PIR Motion Sensor | D16 | None |
| Magnetic door sensor reed switch | D24 | None |
| MG90S 180° Micro Servo | PWM | None |
| Sound Sensor/ Noise Detector | A4 | 0x04 |


[(Back to top)](#table-of-contents)

# Custom configurations

[(Back to top)](#table-of-contents)

Make sure to run as python3 and not python2.7. 
The following are the commands to run the application:

```sh
python3 farm.py
```

Set your IOTHUB_DEVICE_CONNECTION_STRING in your .env file to 

```"HostName=ConnectedIoT.azure-devices.net;DeviceId=Device1;SharedAccessKey=gJFLoYSjY2jGWfe49cyfS5umU9RQECjOlyArTeYwU1c="```

## Test accounts
You may signup and create a test account or you can use the following.

- teacher@owner.co
- teacher@farmer.co

The password for each is password


# Future Work

1. Have a dark mode and light mode for the application.
2. Have multiple moisture sensors for different plants.
3. Optimize the application for mobile device with the database.
4. Have actual notification tabs for the application and not pop ups.
5. Able to set a "sleep" time when the lights automatically turn off
6. Able to set a "wake up" time when the lights automatically turn on

[(Back to top)](#table-of-contents)


## Contributions

| | | |
|-|-|-|
| <img src="https://i.imgur.com/pisior7.png" width="100px;"/><br>[Jeffrey Bringolf](https://www.linkedin.com/in/jeffrey-bringolf-474b54205/) | <img src="https://i.imgur.com/b7KQt6r.png" width="100px;"/><br>[Abdessalam Ait Haqi](https://www.linkedin.com/in/abdessalam-ait-haqi-45752723a/) | <img src="https://i.imgur.com/FaJsIdU.png" width="100px;"/><br>[Pleasure Ghotra](https://www.linkedin.com/in/pleasure-ghotra-0b8a01203/) |
| Jeffrey worked on the geo-location subsystem, user log in and creating users, reviewed and helped when needed to complete tasks, created the architecture of the application, and tweaked the historical page to work with the Azure IoT data. He also created the loading page. | Abdessalam worked on the plant subsystem, UI, and Firebase database. He was responsible for all the pages related to the plant subsystem he was assigned to. He also worked on reviews for others in the team and made sure the milestones were completed | Pleasure worked on the security subsystem and the connection to Azure IoT. He was also responsible for creating the dummy historical page with line series with random values and all of the fleet owner pages he was assigned to. |




[(Back to top)](#table-of-contents)

