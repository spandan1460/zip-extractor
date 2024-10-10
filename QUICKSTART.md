# QuickStart

## MacOS


### Pre-requisites

1. Install brew: ```/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"```
2. Install xcode ```xcode-select –install```
3. Install .NET 7 [download and install](https://learn.microsoft.com/en-us/dotnet/core/install/macos)
4. Install docker [Docker](https://docker.com)
5. To allow docker to run SQL Server on your OS/X machine (assuming you have an Apple Silca system M1+), you will need to enable [Rosetta Emulation](https://collabnix.com/warning-the-requested-images-platform-linux-amd64-does-not-match-the-detected-host-platform-linux-arm64-v8/)


### Installation & Execution

1. Git clone this repository to your local computer
2. Execute ```./build.sh``` to just run the service
3. Alternatively, run ```docker compose up``` this will start Flagr (feature flags service), SQL Server and the example FbM service.  SQL Server is on port 10433, Flagr is on port 18000 and you can navigate to http://localhost:18000.
4. To view the service, open a browser to http://localhost:8000

## Windows

TBD - if anyone is willing to experiment on a windows machine, please reach out to matt.greenwood@maersk.com
