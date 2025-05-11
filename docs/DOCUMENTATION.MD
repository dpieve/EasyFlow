# Documentation

Welcome to the documentation. 

If you find any issues or have suggestions, contributions are welcome.

## Table of Contents

- [Documentation](#documentation)
  - [Table of Contents](#table-of-contents)
  - [Installation](#installation)
    - [Linux](#linux)
  - [Data Handling](#data-handling)
    - [Delete data](#delete-data)
    - [Export data](#export-data)
  - [Notifications](#notifications)
  - [Feedback](#feedback)

## Installation

Download the [release](https://github.com/dpieve/EasyFocus/releases/) file for the desired system.

Support for Windows, Linux (x64 and arm64) and Android.

The browser version is available [here](https://dpieve.github.io/EasyFocus/).

### Linux

Linux sounds implementation depends on ALSA architecture (read [NetCoreAudio](https://www.nuget.org/packages/NetCoreAudio/2.0.0)). 
Therefore, the machine needs to have the utilities installed.

* Run the following command or the equivalent for your system:

```bash
sudo apt-get install alsa-utils
```

* Open the terminal in the download directory and make the `EasyFocus.Linux` file executable:

```bash
chmod +x EasyFocus.Linux
```

* Run the file:

```bash
./EasyFocus.Linux
```

### Data Handling

We respect your privacy. There's no data collected. 
All data is kept local to your device, ensuring that only you have access to your data.

#### Delete data

* Desktop: Close the app and delete the `EasyFocus.json` file.

* Browser: Clean the local storage.

#### Export data

Desktop: Copy the `EasyFocus.json` file to the project's directory.

## Notifications

In case the notification are not showing, make sure you have the correct permissions.

## Feedback

Open a new [Issue](https://github.com/dpieve/EasyFocus/issues) on Github for any feedback or suggestions.


