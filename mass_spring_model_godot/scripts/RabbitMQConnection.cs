using Godot;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;


/// <summary>
/// Class for deserializing messages from RabbitMQ
public class RabbitMQMessage {
	public double x { get; set; }
	public string timestep { get; set; }
	public string simstep { get; set; }
}

/// <summary>
/// Class for connecting to RabbitMQ server and receiving messages
public partial class RabbitMQConnection : Node3D
{
	// SSL Context for TLS configuration of Amazon MQ for RabbitMQ
	private ConnectionFactory factory;
	private IConnection connection;
	private IModel channel;

	private string localQueue;
	private string exchangeName = "example_exchange";
	private string bindingKey = "msd";
	private List<RabbitMQMessage> messages = new();
	private RichTextLabel msgLabel;

	/// Signal for sending position messages to the Mass script
	[Signal]
	public delegate void PositionMessageEventHandler(string[] coordinates);
	
	/// <summary>
	/// Called when the node and children enter the scene tree for the first time.
	/// Creates a connection to server. Creates an exchange and queue, binds them, and calls ReceiveMessage()
	/// </summary>
	public override void _Ready() {
		string[] args = OS.GetCmdlineArgs();
		
		var useLocalRabbitMq = true;
		var password = "guest";
		
		// Get password from command line argument
		//if (args.Length == 1) {
		//	password = args[0];
		//	useLocalRabbitMq = false;
		//}
		
		if (useLocalRabbitMq) {
			factory = new ConnectionFactory() {
				UserName = "guest",
				Password = password,
				Port = 5672,
				Uri = new Uri("amqp://localhost")
			};
		} else {
			factory = new ConnectionFactory() {   
				Ssl = new SslOption() { 
					Enabled = true,
					Version = SslProtocols.Tls12,
				},
				UserName = "distributed_cosim_demo",
				Password = password,
				Port = 5671,
				Uri = new Uri("amqps://b-14c95d1b-b988-4039-a4fe-b5c6744b8a97.mq.eu-north-1.amazonaws.com")
			};
		}
		
		try {
			connection = factory.CreateConnection();
			channel = connection.CreateModel();
			GD.Print("Connection created");
		}
		catch (Exception e) {
			GetTree().Quit();
			throw e;
		}

		localQueue = channel.QueueDeclare(autoDelete: true, exclusive: true);
		channel.QueueBind(queue: localQueue, exchange: exchangeName, routingKey: bindingKey);
		msgLabel = GetNode<RichTextLabel>("msgLabel");

		ReceiveMessage();
	}

	/// <summary>
	/// Called every frame. Emits a signal with the x-value of the received message.
	/// </summary>
	public override void _Process(double delta) {
		for (int i = 0; i < messages.Count; i++) {
			msgLabel.Text = "Received message: " + messages[i].x;
			EmitSignal(SignalName.PositionMessage, -messages[i].x);
		}
		messages.Clear();
	}
	/// <summary>
	/// Receives messages from the queue and adds them to the messages list.
	/// </summary>
	private void ReceiveMessage() {
		GD.Print("Waiting for messages");
		var consumer = new EventingBasicConsumer(channel);

		consumer.Received += (model, ea) => {
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			var msgJson = JsonSerializer.Deserialize<RabbitMQMessage>(message); 
			messages.Add(msgJson);
			GD.Print($"[x] Received: {msgJson.x}");
		};

		channel.BasicConsume(queue:localQueue, autoAck: true, consumer: consumer);   
	}
}
