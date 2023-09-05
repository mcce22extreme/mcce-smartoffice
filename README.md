<p id="readme-top" align="center">
  <img src="images/logo.jpg" height="128">
  <h1  align="center">Smart Office</h1>
</p>

![Build Status](https://github.com/mcce22extreme/ss2023-ineni-smart-office/workflows/CI%20Pipeline/badge.svg)
![Deployment Status](https://github.com/mcce22extreme/ss2023-ineni-smart-office/workflows/CD%20Pipeline/badge.svg)

## Table of Contents

- [Motivation](#motivation)
- [Architectur](#architectur)
- [Built with](#builtwith)
- [Getting started](#gettingstarted)
- [Set up the IoT-Device](#setupiot)

## Motivation <a name="motivation"></a>

At the latest since the Corona crisis, there has been increased talk of co-working, desk sharing and office space reduction. Especially in offices with a high degree of digitalization, it was very easy to send the entire workforce to the home office and still keep the business running. Because of the associated savings in heating, electricity, water, cleaning, etc., many companies are sticking with offering home offices and achieving permanent reductions in operating costs.

For all the efficiency and cost savings, shared workspaces also come with some drawbacks. The personal touch as well as the ability to customize one's own workspace is lost. For example, desks that can be adjusted in height are always adapted to the needs and preferences of the predecessor. Personal pictures or objects usually have to be dispensed with. Another disadvantage is the choice of desk itself. Even if employees are literally free to choose where they want to work, the same workstations are usually chosen. Getting used to a new office structure every day and looking for a suitable workplace where efficient and undisturbed work is possible can be a major challenge. Aspects such as room temperature, light levels, noise levels, or simply proximity to the nearest coffee machine, can all play a role in choosing the optimal workstation. Common booking systems for managing shared desk workstations, while enabling their reservation, do not provide data on users' booking behavior.

With our solution, employees get the opportunity to customize their temporary workspace. Each workstation is equipped with an IoT device through which a stored user profile is applied. For example, the height adjustment of the desk is automatically adjusted or the display of a digital picture frame is controlled.
User-dependent settings are activated by means of facial recognition or control by an external system. In addition to this individualization of the shared workspace, the built-in sensors of the IoT device collect data on room temperature, light irradiation or noise level and transmit it to our cloud service. From there, this information can be retrieved and further processed by external systems. By correlating this data with the booking behavior of employees, unused workstations and the possible reasons for their avoidance can be uncovered, analyzed and remedied. The chosen solution approach represents the beginning of an entire ecosystem that can be expanded at will. In this way, control of heating, ventilation, air-conditioning or lighting systems can be implemented at a later stage.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Architectur <a name="architectur"></a>

The "Smart Office" application is implemented as a native cloud solution using a microservice architecture in \ac{aws} and mainly consists of the following services and components:

* **User Image Management:** Microservice for managing user information
* **Workspace Management:** Microservice for managing workspace information
* **Workspace Configuration Management:** Microservice for managing workspace configurations
* **Booking Management:** Microservice for managing workspace reservations
* **IoT Sensor Data Processing:** Microservice that processes the recorded sensor data from the IoT devices
* **IoT Device:** IoT Device with sensors for controlling a workstation and collecting data

The graphic below shows the different application parts of the Smart Office environment:

<p align="center">
  <img src="images/smart_office_microservices.png" height="400">
</p>

By using the microservice design pattern, the services of the "Smart Office" can be divided into small independent units, which contributes to better scalability as well as flexibility in the implementation of each individual service. In combination with Infrastructure as Code , this approach also helps to ensure that the individual services can be provided independently of one another. Each of the Smart Office Api Services runs in its own docker container and the coresponding data is stored in a relational database. Communication with the "Smart Office" microservices is done through a REST interface and bundled through a central API gateway.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Set up the IoT-Device <a name="setupiot"></a>
1. Download the IoT Licence Files from AWS IoT Core
2. Start the ./setup_smartoffice
3. Check the Bluetooth Connection | Set Bluetooth Mac Address manually
4. Start Python Script deskkontrol.py
5. Wait for Messages 

## Built with <a name="builtwith"></a>

- [Visual Studio](https://visualstudio.microsoft.com/de/vs/community/)
- [Net Core](https://dotnet.microsoft.com/)
- [Windows Presentation Foundation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/)
- [MahApps.Metro](https://mahapps.com/)
- [Mosquitto](https://https://mosquitto.org/)
- [MQTTnet](https://github.com/dotnet/MQTTnet) 
- [Raspberry PI](https://www.raspberrypi.com/)
- [Terraform](https://www.terraform.io/)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Getting started <a name="gettingstarted"></a>

### Smart Office Microservices
Since the Smart Office microservices have been defined as independent Docker containers, they can be operated in any Docker engine. The solution contains a docker compose file that can help to orchestrate the microservices. In addition, the docker compose file shows which environment variables must be provided for each microservice.

The microservices can be deployed to a local Docker Engine with the following command:

```
docker compose up -d
```