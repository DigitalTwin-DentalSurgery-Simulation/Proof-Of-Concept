# Proof-Of-Concept

This proof of concept aims to demonstrate the feasibility of Active Haptic Feedback (AHF) applied to dental simulators.

<p align="center">
<img src="https://github.com/DigitalTwin-DentalSurgery-Simulation/Proof-Of-Concept/blob/main/docs/assets/media/visualization-2-images-stacked.png" width="500" height="500">
<p/>

## Components

The proof of concept consists of 3 components, that should be run collectively

### [INTO-CPS co-simulation](src/DigitalTwin.Simulation)

The INTO-CPS co-simulation runs the FMU's in the proof-of-concept, this includes RabbitmqFMU, HapticsFMU and OptimalPathFMU. This should be run as the first of the 3 components and wait for input from the middleware.

#### Dependencies

Install the INTO-CPS application, including Maestro 2. INTO-CPS can be found [here](https://github.com/INTO-CPS-Association/into-cps-application/releases).

Follow the installation guide and download Maestro 2 from the download manager in INTO-CPS

### [Middleware](src/DigitalTwin.Middleware.DataInput)

The middeware supplies the INTO-CPS simulation with generated positional simulation data. Run this after you have started the INTO-CPS simulation.

#### Dependencies

The middleware has .NET 6 Runtime as dependency. Download Visual Studio 2022 [here](https://visualstudio.microsoft.com/vs/)

You will need SimtoCare positional data, these are not public and should be requested. The files will be submitted with the hand-in of this thesis.

### [Visualization](src/DigitalTwin.Visualization)

The Visualization component receives it data from the RabbitMQ Server. You can start this after the Middleware or optionally run this after the simulation has finished and visualize the optimal path and user path.

#### Dependencies

Python 3.10.x - Can be found [here](https://www.python.org/downloads/release/python-3109/)

Additionally, numpy, matplotlib and pika is required. Download using the requirements.txt file, located [here](src/DigitalTwin.Visualization)

Run:
```
pip install -r requirements.txt
```

### Common Depencies

All 3 components depend on RabbitMQ. Run it using docker-compose.

[If you don't have docker, download Docker Desktop](https://www.docker.com/products/docker-desktop/)

[Download the Dockerfile](https://raw.githubusercontent.com/INTO-CPS-Association/fmu-rabbitmq/c15e6bd1fee36d1e7d701de39a06e482d1d1887b/server/Dockerfile)

Run: 
```
docker-compose up -d
```


## Contributing

Do you have questions or wish to raise an issue? [Use the Issues tab on Github](https://github.com/DigitalTwin-DentalSurgery-Simulation/Proof-Of-Concept/issues)

## License

This project has [MIT License](LICENSE)

Please make sure your need is within the license of the dependencies.

