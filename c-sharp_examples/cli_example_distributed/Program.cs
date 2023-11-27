using System;
using System.Diagnostics;

namespace cli_example_distributed
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting docker compose...");
            RunDockerComposeCommand("up -d");

            // Give some time for the containers to initialize before executing commands
            System.Threading.Thread.Sleep(5000); // 5 seconds, adjust as necessary

            Console.WriteLine("Running control.py in controller-container...");
            RunCommand("docker", "exec controller-container python3 control.py");

            Console.WriteLine("Executing run_cosim.sh in rabbitmqfmu-container...");
            RunCommand("docker", "exec rabbitmqfmu-container ./run_cosim.sh");
        }

        static void RunDockerComposeCommand(string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "docker-compose",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                if (process.ExitCode != 0) // This means docker-compose failed
                {
                    // Check if the error message indicates a name conflict
                    var standardError = process.StandardError.ReadToEnd(); // read the error messages
                    if (standardError.Contains("controller-container"))
                    {
                        Console.WriteLine("Detected container naming conflict. Attempting to resolve...");

                        // Stop and remove the conflicting container
                        RunCommand("docker", "stop controller-container");
                        RunCommand("docker", "rm controller-container");

                        // Retry the docker-compose command
                        RunDockerComposeCommand(arguments);
                    }
                }
            }
        }

        static void RunCommand(string command, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
                process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }
        }
    }
}
