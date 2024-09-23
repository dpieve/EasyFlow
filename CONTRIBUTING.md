# Welcome

Contributions are welcome! Please feel free to open a Pull request.

## Guide

### Issues

Open an issue in case you find any bugs to the latest released version.

### Developers

1) Create a Fork.
2) Make changes in your Fork.
3) Create a Pull request
   Explain the changes and how they will contribute to the project.

## Publish

Currently there is `src/publish-script.bat` available to publish. It is Windows only.
For Linux, it is required to update the .csproj and create a new release.

### Steps

(1) Open the terminal in the `\src` directory

(2) Make sure Visual Studio is closed

(3) Write the command below and press enter

```bash
.\publish-script.bat
```

(4) A .zip is generated with the setup .exe
