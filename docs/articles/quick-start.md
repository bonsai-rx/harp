Quick Start
===========

Harp is a standardized solution for:
  - Automatic [sub-millisecond synchronization](https://harp-tech.org/protocol/SynchronizationClock.html) across devices
  - A [binary protocol](https://harp-tech.org/protocol/BinaryProtocol-8bit.html) for communication between devices and PC
  - [Hardware templates](https://harp-tech.org/protocol/Device.html) for developing new devices

All [Harp Devices](https://harp-tech.org/whoami/whoami.html) implement the [Harp Protocol](https://harp-tech.org/protocol/BinaryProtocol-8bit.html) to communicate with an host PC. The `Bonsai.Harp` library provides an implementation of the Harp protocol that can be used to interface with any Harp device.

## How to install

1. [Install Bonsai](https://bonsai-rx.org)
2. [Install FTDI D2XX Drivers](https://ftdichip.com/wp-content/uploads/2021/08/CDM212364_Setup.zip)
3. Install `Bonsai.Harp.Design` using the [Bonsai package manager](https://bonsai-rx.org/docs/articles/packages.html).

## Device specific packages

A high-level interface will usually be available for the specific Harp device you are using. To install them, first change the package manager **Package source** to `nuget.org`. Then, in the search bar, look for your device by typing: `harp.<device>`. For instance, for the [Harp Behavior](https://harp-tech.org/api/Harp.Behavior.html) board, you should find the following package:

<p><img alt="Installing a Harp device package" src="~/images/behavior-package.png" style="max-height:450px;object-fit:contain" /></p>

The device nodes should now be available in the Bonsai Toolbox and you can start using them in your workflows. See [Operators](operators.md) for examples of how to manipulate and control Harp devices.

## Next Steps

- [Logging](logging.md)
- [Firmware](firmware.md)
- [Data Interface](https://harp-tech.org/articles/python.html)
