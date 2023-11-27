using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var rabbitMQServer = new RabbitMQServerManager();
        var controllerManager = new ControllerManager();
        var maestroManager = new MaestroManager();

        try
        {
            // Start RabbitMQ Server
            await rabbitMQServer.StartServer();

            // Wait for RabbitMQ server to be fully up
            Console.WriteLine("Waiting for RabbitMQ server to be fully up and running...");
            await Task.Delay(10000); // Delay for RabbitMQ server

            // Start Controller and Maestro in parallel
            Console.WriteLine("Starting Controller...");
            var controllerTask = controllerManager.StartController();
            await Task.Delay(10000); // Delay for RabbitMQ server 
            //send a message to rabbitmq server to fix synchronization issues 

            Console.WriteLine("Starting Maestro...");
            var maestroTask = maestroManager.StartMaestro();

            // Wait for both tasks to complete
            await Task.WhenAll(controllerTask, maestroTask);

            Console.WriteLine("Press any key to stop the server and exit...");
            Console.ReadKey();
        }
        finally
        {
            // Ensure RabbitMQ Server is stopped
            rabbitMQServer.StopServer();
        }
    }
}




/*
class Program
{
    static async Task Main(string[] args)
    {
        var rabbitMQServer = new RabbitMQServerManager();
        var controllerManager = new ControllerManager();
        await controllerManager.StartController();

        
        try
        {
            // Start RabbitMQ Server
            await rabbitMQServer.StartServer();

            // Start Controller
            await controllerManager.StartController();

            Console.WriteLine("Press any key to stop the server and exit...");
            Console.ReadKey();
        }
        finally
        {
            // Ensure RabbitMQ Server is stopped
            rabbitMQServer.StopServer(); // If StopServer is not async, remove await
        }

    }
}

/*
class Program
{
    static async Task Main(string[] args)
    {
        // Start RabbitMQ Server
        string rabbitMQContainerName = "rabbitmq-container";
        await RunProcess("docker", $"run --rm --name {rabbitMQContainerName} -d -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management");

        
        // Define the paths
        //string maestroJarPath = "../distributed_oneway/maestro.jar";
        //string scenarioJsonPath = "../distributed_oneway/scenario.json";
        //string controllerScriptPath = "../../distributed_ctrl_python/control.py";
 
        // Controller command
        //string controllerCommand = "python3";
        //string controllerArgs = controllerScriptPath;
        //var controllerTask = RunProcess(controllerCommand, controllerArgs);
        //await controllerTask;  // Make sure this line is present and not commented out


        // Wait for the controller to complete its initialization
        // This delay is an arbitrary wait time to allow the controller to start up.
        // Adjust this based on the actual startup time of your controller.
        //await Task.Delay(5000); // Delay for 5 seconds (5000 milliseconds)
        /*
        // Maestro command
        string maestroCommand = "java";
        string maestroArgs = $"-jar {maestroJarPath} import Sg1 -output=results -v --interpret {scenarioJsonPath}";
        var maestroTask = RunProcess(maestroCommand, maestroArgs);

        // Wait for Maestro to complete
        await maestroTask;

        // Stop RabbitMQ Server
        await RunProcess("docker", $"stop {rabbitMQContainerName}");
        
    }

    
    static async Task RunProcess(string fileName, string arguments)
    {
        using (var process = new Process())
        {
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            // Asynchronously read the standard output and standard error.
            await Task.WhenAll(
                ReadStream(process.StandardOutput),
                ReadStream(process.StandardError)
            );

            process.WaitForExit();
        }
    }

    static async Task ReadStream(StreamReader reader)
    {
        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            Console.WriteLine(line);
        }
    }
}
*/

