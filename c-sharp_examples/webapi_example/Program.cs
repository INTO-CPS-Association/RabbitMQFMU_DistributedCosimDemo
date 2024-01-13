//Based on example: https://into-cps-maestro.readthedocs.io/en/docs/user/getting-started.html

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Diagnostics;

class Program
{
    static readonly HttpClient client = new HttpClient();
    static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


    static async Task Main(string[] args)
    {
        try
        {
            //variables
            string baseUrl = "http://localhost";
            int port = 8082;
            client.BaseAddress = new Uri($"{baseUrl}:{port}");
            int delayMilliseconds = 0; // 1 second delay



            //STEP 1: Start COE
            var coeProcess = StartCOE(@"..\example_project"); // Path to the directory containing the


            //STEP 2: Create session
            string sessionId = await CreateSession();
            
            if (string.IsNullOrEmpty(sessionId))
            {
                Console.WriteLine("Could not create session");
                Environment.Exit(1);
            }

            Console.WriteLine($"Session '{sessionId}' created successfully.");


            //STEP 3: Initialize session
            string scenarioFilePath = "../example_project/scenario.json"; // Adjust the path as needed
            bool isInitialized = await InitializeSession(sessionId, scenarioFilePath);
           
            if (!isInitialized)
            {
                Console.WriteLine("Could not initialize");
                Environment.Exit(1);
            }

            Console.WriteLine("Session initialized successfully.");

            //STEP 3.1: Connect to WebSocket and livestream data
            Task webSocketTask = ConnectToWebSocketAndStreamData(sessionId, port, delayMilliseconds);


            //STEP 4: Run simulation
            string simulationFilePath = "../example_project/simulate.json"; // Adjust the path as needed
            bool isSimulationSuccessful = await RunSimulation(sessionId, simulationFilePath);
            
            if (!isSimulationSuccessful)
            {
                Console.WriteLine("Could not simulate");
                Environment.Exit(1);
            }

            Console.WriteLine("Simulation completed successfully.");


            //STEP 5: Get simulation results
            bool isResultRetrieved = await RetrieveResults(sessionId); //get results while the simulation is running (e.g. while-loop?/ read stream) -> Look at attach session command to see if streaming data is possible 

            if (!isResultRetrieved)
            {
                Console.WriteLine("Could not receive results");
                Environment.Exit(1);
            }

            Console.WriteLine("Results retrieved and saved successfully.");


            //STEP 6: Destroy session
            bool isSessionDestroyed = await DestroySession(sessionId);
            
            if (!isSessionDestroyed)
            {
                Console.WriteLine("Could not destroy session");
                Environment.Exit(1);
            }

            Console.WriteLine("Session destroyed successfully.");
            coeProcess.WaitForExit();
        }
    
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }


    //------------------------- Helper functions -------------------------
    //Start COE
    static Process StartCOE(string coeDirectory)
    {
        string coeCommand = "java";
        string coeArguments = "-jar coe-1.0.10.jar -p 8082";

        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = coeCommand,
            Arguments = coeArguments,
            WorkingDirectory = coeDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = Process.Start(startInfo);
        return process;
    }
    

    //Create session 
    static async Task<string> CreateSession()
    {
        HttpResponseMessage response = await client.GetAsync("/createSession");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();
        var status = JsonSerializer.Deserialize<JsonElement>(content);
        return status.GetProperty("sessionId").GetString();
    }


    //Initialize session
    static async Task<bool> InitializeSession(string sessionId, string scenarioFilePath)
    {
        string scenarioJson = await File.ReadAllTextAsync(scenarioFilePath);
        var content = new StringContent(scenarioJson, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync($"/initialize/{sessionId}", content);
        return response.IsSuccessStatusCode;
    }


    //Run simulation
        static async Task<bool> RunSimulation(string sessionId, string simulationFilePath)
    {
        string simulationJson = await File.ReadAllTextAsync(simulationFilePath);
        var content = new StringContent(simulationJson, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync($"/simulate/{sessionId}", content);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Simulate response code '{response.StatusCode}, data={responseContent}'");
            return true;
        }
        else
        {
            return false;
        }
    }


    //Retrieve results
        static async Task<bool> RetrieveResults(string sessionId)
    {
        HttpResponseMessage response = await client.GetAsync($"/result/{sessionId}/plain");

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        string resultCsv = await response.Content.ReadAsStringAsync();
        string resultCsvPath = "result.csv";
        await File.WriteAllTextAsync(resultCsvPath, resultCsv);
        
        Console.WriteLine($"Result response code '{response.StatusCode}'");
        return true;
    }


    //Delete session
            static async Task<bool> DestroySession(string sessionId)
        {
            HttpResponseMessage response = await client.GetAsync($"/destroy/{sessionId}");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            Console.WriteLine($"Destroy response code '{response.StatusCode}'");
            return true;
        }

    // Connect to WebSocket and livestream data
    static async Task ConnectToWebSocketAndStreamData(string sessionId, int port, int delayMilliseconds)
    {
        using (ClientWebSocket webSocket = new ClientWebSocket())
        {
            Uri serverUri = new Uri($"ws://localhost:{port}/attachSession/{sessionId}");
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connected to WebSocket.");

            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            string message = await reader.ReadToEndAsync();
                            Console.WriteLine("Received message: " + message);
                        }
                    }
                }

                 // Introduce a delay
                await Task.Delay(delayMilliseconds);
            }
        }
    }
}




/*NOTES:
- Multiple livestreamed data variables (Done)
- Livestreamed data is not equal to results.csv file  
- Speed in which data is streamed?
- Visualize livestreamed data differently? 
*/