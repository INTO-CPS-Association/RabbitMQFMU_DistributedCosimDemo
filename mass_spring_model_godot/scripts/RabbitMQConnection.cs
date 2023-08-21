using Godot;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;

public partial class RabbitMQConnection : Node3D
{
    // // ssl connection
    // private ConnectionFactory factory = new ConnectionFactory() {   
    //     Ssl = new SslOption() { 
    //         Enabled = true,
    //         Version = SslProtocols.Tls12,
    //     },
    //     Uri = new Uri("amqps://distributed_cosim_demo:CONTACT-CLAUDIO@b-14c95d1b-b988-4039-a4fe-b5c6744b8a97.mq.eu-north-1.amazonaws.com:5671")
    // };

    // Uncomment to connect to local 
    private ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };

	private IConnection connection;
	private IModel channel;
    
    private string exchangeName = "mass";
    private string queueName = "test_topic";
    private string bindingKey = "mass.position";
    private List<string> messages = new();
    private RichTextLabel msgLabel;

    [Signal]
    public delegate void PositionMessageEventHandler(string[] coordinates);
    
    /// <summary>
    /// Creates a connection to server.
    /// Creates an exchange and queue and binds them.
    /// Calls ReceiveMessage()
    /// </summary>
    public override void _Ready() {
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        GD.Print("Connection created");

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);
        channel.QueueDeclare(queue: queueName, durable: false, autoDelete: true, exclusive: false, arguments: null);
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: bindingKey);
        msgLabel = GetRichTextLabel("msgLabel");

        ReceiveMessage();
    }

    public override void _Process(double delta) {
        for (int i = 0; i < messages.Count; i++) {
            msgLabel.Text = "Received message: " + messages[i];

            string[] coordinates = messages[i].Split(", ");
            coordinates = coordinates.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            
            if (coordinates.Length == 3) {
                coordinates[0] = coordinates[0].Replace("(", "");
                coordinates[2] = coordinates[2].Replace(")", "");

                // Check if the message is on this form: (x, y, z)
                if (float.TryParse(coordinates[0], out float temp) && float.TryParse(coordinates[1], out temp) && float.TryParse(coordinates[2], out temp)) {
                    EmitSignal(SignalName.PositionMessage, coordinates);
                }
            }
        }
        messages.Clear();
    }
    /// <summary>
    /// Method for receiving messages from a specific queue
    /// </summary>
    private void ReceiveMessage() {
        GD.Print("Waiting for messages");
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) => {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            messages.Add(message);
            GD.Print($"[x] Received message: {message}");
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    /// <summary>
    /// Retrieve the existing node of type RichTextLabel
    /// </summary>
    /// <param name="name"> the name of the node in the scene </param>
    /// <returns> node </returns>
    private RichTextLabel GetRichTextLabel(string name) { return GetNode<RichTextLabel>(name);}
}
