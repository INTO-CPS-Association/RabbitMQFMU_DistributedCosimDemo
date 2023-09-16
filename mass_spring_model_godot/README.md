# Godot Tutorial

## Installation (C# Support) for Windows


### Steps: 

1. Install [.NET SDK](https://dotnet.microsoft.com/en-us/download).
2. Install [Godot 4 Engine - .NET](https://godotengine.org/download/windows/), which supports C#.
3. Open a PowerShell terminal.
4. Install the RabbitMQ library for C#.
   ```powershell
   dotnet add package RabbitMQ.Client
   ```
5. Install Scoop for CLI.
   
   **Optional:** (Needed to run a remote script the first time)
    PowerShell execution policy is required to be one of: `Unrestricted`, `RemoteSigned` or `ByPass` to execute the installer. For example, run:

    ```powershell
    Set-ExecutionPolicy RemoteSigned -Scope CurrentUser # Optional
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

### Build the Godot Project 
This is only required before running the Godot Project the first time. 

**Steps:**
1. Open a PowerShell terminal.
2. Change directory to the Godot Project `mass_spring_model_godot`:
   ```powershell
   cd [YOUR PATH]\mass_spring_model_godot
   ```
3. Build the Godot Project.
   ```powershell
   godot-mono --build-solutions --quit
   ```

   This opens Godot, builds the project, and then quits the program automatically. 

Note that the following errors will show up, but they should be ignored: 

```powershell
ERROR: Condition "f.is_null()" is true.
   at: _save_to_cache (servers/rendering/renderer_rd/shader_rd.cpp:455)
ERROR: Condition "f.is_null()" is true.
   at: _save_to_cache (servers/rendering/renderer_rd/shader_rd.cpp:455)
ERROR: Condition "f.is_null()" is true.
   at: _save_to_cache (servers/rendering/renderer_rd/shader_rd.cpp:455)

WARNING: Blend file import is enabled in the project settings, but no Blender path is configured in the editor settings. Blend files will not be imported.
   at: _editor_init (modules/gltf/register_types.cpp:73)
ERROR: Condition "!EditorSettings::get_singleton() || !EditorSettings::get_singleton()->has_setting(p_setting)" is true. Returning: Variant()
   at: _EDITOR_GET (editor/editor_settings.cpp:1127)
```

### Run the Godot Project

```powershell
godot-mono [password]
```

Contant Claudio claudio.gomes@ece.au.dk for the password to get access to RabbitMQ on AWS. No argument or wrong password will fail.