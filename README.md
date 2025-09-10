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

## Running Locally

```bash
dotnet run  --file "path/to/note.txt"
```

```bash
dotnet run  --input "Patient needs a CPAP..."
```