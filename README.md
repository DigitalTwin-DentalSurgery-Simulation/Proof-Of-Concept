# Proof-Of-Concept

This proof of concept aims to demonstrate the feasiability of Active Haptic Feedback (AHF) applied to dental simulators.


## Components

The proof of concept consists of 3 components, that should be run collectively

### [INTO-CPS co-simulation](src/DigitalTwin.Simulation)

The INTO-CPS co-simulation runs the FMU's in the proof-of-concept, this includes RabbitFMU, HapticsFMU and OptimalPathFMU. This should be run as the first of the 3 components and wait for input from the middleware.

#### Dependencies

Install the INTO-CPS application, including Maestro 2. INTO-CPS can be found [here](https://github.com/INTO-CPS-Association/into-cps-application/releases).

Follow the installation guide and download Maestro 2 from the download manager in INTO-CPS

### [Middleware](src/DigitalTwin.Middleware.DataInput)

The middeware supplies the INTO-CPS simulation with generated user data using the SimtoCare file. Run this after you have started the INTO-CPS simulation.

#### Dependencies

The middleware has .NET 6 Runtime as dependency. Download Visual Studio 2022 [here](https://visualstudio.microsoft.com/vs/)

You will also need SimtoCare files as depency, these will are not public and should be requested. These will be submitted with the hand-in of this thesis.

### [Visualization](src/DigitalTwin.Visualization)

The Visualization component gets it data from the RabbitMQ Server. You can start this after the Middleware or optionally run this after the simulation has finished and visualize the optimal path and user path.

#### Dependencies

Python 3.10.x - Can be found [here](https://www.python.org/downloads/release/python-3109/)



## Contribute

Questions or do you want to raise an issue? [Use the Issues tab on Github](https://github.com/DigitalTwin-DentalSurgery-Simulation/Proof-Of-Concept/issues)


## License

This project has [MIT License](LICENSE)

Please make sure your need is within the license of the depencies.

