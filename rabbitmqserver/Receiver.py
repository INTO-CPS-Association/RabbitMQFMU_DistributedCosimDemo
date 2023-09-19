import os
import ssl
import pika

# SSL Context for TLS configuration of Amazon MQ for RabbitMQ
ssl_context = ssl.SSLContext(ssl.PROTOCOL_TLSv1_2)


#UNCOMMENT FOR CONNECTING TO AMAZON MQ
#url = f"amqps://distributed_cosim_demo:CONTACT_CLAUDIO@CONTACT_CLAUDIO:5671"
#parameters = pika.URLParameters(url)
#parameters.ssl_options = pika.SSLOptions(context=ssl_context) #Uncomment for connecting to server


#UNCOMMENT FOR CONNECTING TO LOCAL RABBITMQ SERVER 
url = "amqp://guest:guest@localhost:5672/"
#url "amqp://guest:guest@host.docker.internal:5672/" #uncomment if running the receiver as a docker container (NEED FIXING) 

parameters = pika.URLParameters(url)


connection = pika.BlockingConnection(parameters)
channel = connection.channel()


#channel.exchange_declare(exchange='logs')

channel.queue_declare(queue='Test')

def callback(ch, method, properties, body):
    print(f" [x] Received {body}")

channel.basic_consume(queue='Test',
                      on_message_callback=callback,
                      auto_ack=True)

print(' [*] Waiting for messages. To exit press CTRL+C')
channel.start_consuming()

