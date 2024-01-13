using Hardware.Info;
using LinuxApp.Dtos;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using LinuxApp.Services.Remote;
using ByteSizeLib;
using HardwareInformation;
using System.Diagnostics;
using LinuxApp.Objects;
using LinuxApp.Services.Local;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR.Client;




ProgramRun runner = new ProgramRun();

static void Main()
    {

         var configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json");
    var config = configuration.Build();

    bool skipClockspeedTest;
    MachineInformation info = MachineInformationGatherer.GatherInformation(skipClockspeedTest = true);

    HubConnection hubConnection = new HubConnectionBuilder()
                // .WithUrl("https://api.panaro.uk/signalr") // Replace with the actual URL of your SignalR hub
                .WithUrl(new Uri(config.GetSection("BaseUrl").Value + "signalr"))
                .Build();


    // Start the connection
                try
                    {
                        await hubConnection.StartAsync();
                        
                }
                    catch (Exception ex) 
                    {
                    LogToBox("Connection To SignalR Failed");
                    }
                hubConnection.Closed += HubConnection_Closed;

                    var connId = hubConnection.ConnectionId;
                    
                LogToBox("Connection Id: " + connId);
                    // Invoke methods on the hub
                // await hubConnection.InvokeAsync("MethodName", arg1, arg2);

                    // Subscribe to hub events
                    hubConnection.On<PitsHubScriptDto>("ReceiveMessage", (message) =>
                    {

                        LogToBox("Running Script: " + message.Script);

                        runner.RunScript(message.Script);

                    });

        // Create a Timer with a 120-second interval
        Timer timer = new Timer(120000); // 120,000 milliseconds = 120 seconds

        // Hook up the Elapsed event for the timer
        timer.Elapsed += OnTimerElapsed;

        // Set the timer to repeat (AutoReset property)
        timer.AutoReset = true;

        // Start the timer
        timer.Start();

      //  Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();

        // Stop the timer before exiting the program
        timer.Stop();
        timer.Dispose();
    }

    static void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        runner.ProgramRun();
    }



