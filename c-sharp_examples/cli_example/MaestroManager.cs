using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class MaestroManager
{
    public async Task StartMaestro()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = "../../distributed_oneway/run_cosim.sh", // Update the path as needed
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        // Asynchronously read the output to console
        await Task.Run(() =>
        {
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
        });

        // Asynchronously read the standard error to console
        await Task.Run(() =>
        {
            while (!process.StandardError.EndOfStream)
            {
                string line = process.StandardError.ReadLine();
                Console.WriteLine("ERROR: " + line);
            }
        });

        process.WaitForExit();
        Console.WriteLine("Maestro execution completed.");
    }
}
