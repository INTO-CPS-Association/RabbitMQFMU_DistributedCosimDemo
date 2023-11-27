using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class RabbitMQServerManager
{
    private Process? _rabbitMQServerProcess;
    private string _containerName = "rabbitmq-container";

    public async Task StartServer()
    {
        // Check if the container is already running
        if (IsContainerRunning())
        {
            Console.WriteLine("RabbitMQ server is already running.");
            return;
        }

        _rabbitMQServerProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"run --rm --name {_containerName} -d -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _rabbitMQServerProcess.Start();
        Console.WriteLine("Starting RabbitMQ server...");

        // Wait for the server to start
        await Task.Delay(5000); // Adjust this delay as needed

        if (IsContainerRunning())
        {
            Console.WriteLine("RabbitMQ Server started successfully.");
        }
        else
        {
            Console.WriteLine("Failed to start RabbitMQ Server.");
        }
    }

    public void StopServer()
    {
        if (IsContainerRunning())
        {
            Console.WriteLine("Stopping RabbitMQ server...");
            ExecuteDockerCommand($"stop {_containerName}");
            Console.WriteLine("RabbitMQ Server stopped.");
        }
        else
        {
            Console.WriteLine("RabbitMQ server is not running.");
        }
    }

    private bool IsContainerRunning()
    {
        var checkProcess = ExecuteDockerCommand($"inspect -f '{{{{.State.Running}}}}' {_containerName}");
        return checkProcess.StandardOutput.ReadToEnd().Trim() == "true";
    }

    private Process ExecuteDockerCommand(string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
        return process;
    }
}
