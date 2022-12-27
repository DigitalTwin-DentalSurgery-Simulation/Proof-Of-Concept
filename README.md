# Proof-Of-Concept

This proof of concept aims to demonstrate the feasiability of Active Haptic Feedback (AHF) applied to dental simulators.


## Components

The proof of concept consists of 3 components, that should be run collectively

### [INTO-CPS co-simulation](src/DigitalTwin.Simulation)

#### Dependencies

Install the INTO-CPS application, including Maestro 2. INTO-CPS can be found [here](https://github.com/INTO-CPS-Association/into-cps-application/releases).

Follow the installation guide and download Maestro 2 from the download manager in INTO-CPS

### [Middleware](src/DigitalTwin.Middleware.DataInput)


#### Dependencies

The middleware has .NET 6 Runtime as dependency. Download Visual Studio 2022 [here](https://visualstudio.microsoft.com/vs/)

You will also need SimtoCare files as depency, these will are not public and should be requested. These will be submitted with the hand-in of this thesis.

### [Visualization](src/DigitalTwin.Visualization)

#### Dependencies

Python 3.10.x - Can be found [here](https://www.python.org/downloads/release/python-3109/)




## License

This project has [MIT License](LICENSE)

Please make sure your need is within the license of the depencies.
