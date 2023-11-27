
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class ControllerManager
{
   public async Task StartController()
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = "../../distributed_ctrl_python/control_csharp.py",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    process.Start();

    // Asynchronously read the output and error to console
    var outputTask = Task.Run(() =>
    {
        while (!process.StandardOutput.EndOfStream)
        {
            string? line = process.StandardOutput.ReadLine();
            if (line != null)
            {
                Console.WriteLine(line);
            }
        }
    });

    var errorTask = Task.Run(() =>
    {
        while (!process.StandardError.EndOfStream)
        {
            string? line = process.StandardError.ReadLine();
            if (line != null)
            {
                Console.WriteLine("ERROR: " + line);
            }
        }
    });

    await Task.WhenAll(outputTask, errorTask);

    process.WaitForExit();
    Console.WriteLine("Controller execution completed.");
}

}
