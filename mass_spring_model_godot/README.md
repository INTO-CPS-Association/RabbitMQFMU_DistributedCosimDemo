# Godot Tutorial

## Installation (C# Support) for Windows

### Steps: 

1. Install [.NET SDK](https://dotnet.microsoft.com/en-us/download).
2. Install [Godot 4 Engine - .NET](https://godotengine.org/download/windows/), which supports C#.
   
## Build and Run the Godot Project

**Prerequisites:**
* C# Library RabbitMQ.Client
* A running Local RabbitMQ Server

### Steps:
1. Open Godot
2. Click Import (or press ```Ctrl+I```)
3. Insert the project path and click "Import and Edit"
4. Build and run the project by clicking on the "play" button in the top-right corner (or press ```F5```). 

   If the application crashes when opening the project, it is due to the GPU not supporting the Vulkan rendering API. Instead open a terminal and run the following command: ```C:\path\to\your\godot4.exe --rendering-driver opengl3```.
5. Run ```python test_localhost.py``` found in the current folder.