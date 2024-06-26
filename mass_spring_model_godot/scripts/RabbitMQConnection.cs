using Godot;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
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
    private ConnectionFactory factory = new ConnectionFactory() {
        Uri = new Uri("amqp://guest:guest@localhost:5672/")
    };
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
		connection = factory.CreateConnection();
		channel = connection.CreateModel();
		GD.Print("Connection created");
		
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
