# Godot Tutorial

## Installation (C# Support) for Windows


### Steps: 

1. Install [.NET SDK](https://dotnet.microsoft.com/en-us/download).
2. Install [Godot 4 Engine - .NET](https://godotengine.org/download/windows/), which supports C#.
3. Open a PowerShell terminal. 
4. Install the RabbitMQ library for C#. 
   ```powershell
   C:\path\to\your\godot\project dotnet add package RabbitMQ.Client
   ```
5. Install Scoop for CLI.
   
   **Optional:** (Needed to run a remote script the first time)
    PowerShell execution policy is required to be one of: `Unrestricted`, `RemoteSigned` or `ByPass` to execute the installer. For example, run:

    ```powershell
    Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
    ```
   Run to install Scoop:
    ```powershell
    irm get.scoop.sh | iex
    ```
6. Install Godot with C# Support.
   ```powershell
   scoop install godot-mono
   ```
   If the command fails, try:
     ```powershell
     scoop bucket add extra
     ```
     This will install extra buckets, which is a collection of apps. 
     Then try installing `godot-mono` again.

## Build and Run the Godot Project

### Build and Run the Godot Project with UI

**Prerequisites:**
* C# Library RabbitMQ.Client
* A running Local RabbitMQ Server

**Steps:**
1. Open Godot
2. Click Import (or press ```Ctrl+I```)
3. Insert the project path and click "Import and Edit"
4. Build and run the project by clicking on the "play" button in the top-right corner (or press ```F5```). If the application crashes when opening the project, it is due to the GPU not supporting the Vulkan rendering API. Instead open a terminal and run the following command: ```C:\path\to\your\godot4.exe --rendering-driver opengl3```
5. Run ```python test_localhost.py``` found in the current folder.

Contact Claudio claudio.gomes@ece.au.dk for the password to get access to RabbitMQ on AWS. No argument or wrong password will fail.
