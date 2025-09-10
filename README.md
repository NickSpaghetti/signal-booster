# signal-booster

This command-line application is designed to process physician notes and send an order to a vendor. It can be used to process either a file containing the note or a string of text provided directly via the command line.

## Usage

To run the application, use the following syntax, providing either a file path or a direct input string.

### Parameters

The following parameters are supported for the command.

| Parameter | Alias | Description | Example |
| :--- | :--- | :--- | :--- |
| `--file` | `-f` | The path to the physician's note file. Supported File types are .text and .json | `--file "C:\notes\note1.txt"` |
| `--input` | `-i` | The physician's note as a string. | `--input "AHI > 20. Ordered by Dr. C."` |
| `--help` | `-h` | Displays the help message and usage information. | `--help` |

### Exit Codes

* **0**: The operation was successful.

* **1**: An error occurred during

### Setting Api Endpoints

By default the endpoint base address is set to `http://localhost:5000` which is the default adress of the vendorapi project.  You can override this by setting the env varible `vendor_api_baseurl`.

## Running the Project

### Running Locally

To invoke the CLI run the following in the `SignalBooster/SignalBoosterCLI` directory.

```bash
dotnet run  --file "path/to/note.txt"
```

```bash
dotnet run  --input "Patient needs a CPAP..."
```

To invoke the rest api that responds to an order run the following command in the `SignalBooster/VendorApi`

```bash
dotnet run"
```

## Running with Docker

There is a Makefile built for your convenience for running docker files.  If you are on windows you must use WSL and install `make`. Linux/macOS comes with make built in.

To start the api run the following command.

```bash
make start
```

If you want to run the CLI in docker you need to modify the Commented out `CMD` and change the file path to a valid file path containing either a text file using `-f`, or a raw string input using `-i`.  As well as uncomment out the console-app section.  Note this will only fire one command to the api then exit.

If you need to trouble shoot any of the docker files you can use the commands `make shell-cli` or `make shell-api`.  This will bring you into `/bin/bash` of the container.  

### Technologies used

* IDES: Rider, VSCode, and neovim.
* OS used: macOS, Ubuntu, and Windows
* Ai Development tools used: [OpenCode](https://opencode.ai/) with models runing locally and GitHub copilot.

### Assumptions

I have kept the logic to parse the orginal conditon if a file was not found `("Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.")`.  However the file fails the current validation in `PhysicianNoteValidator.Validate`.  We should look at implementing a factory/stragegy pattern if we have diffrent note types from other health care systems.  Likewise we would have to do the same thing when sending Orders to the respective vendor.  The assumption I took is we are a startup and we have one Note provider and one Order vendor.  Setting these patterns up now would likely be premature optimization and cause more refactor work.  Another assuption I took is our current Note Provider uses CLRF for delimiters for all their notes.  I have also created a cleaned up original logic that you can find at `SignalBoosterCLI/DecodedOriginal.cs`.  You can also find my first pass at parsing in `NoteProcessingService.cs`