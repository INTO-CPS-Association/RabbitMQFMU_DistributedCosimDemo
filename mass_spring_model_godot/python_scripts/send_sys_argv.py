import pika
import sys

# # SSL Context for TLS configuration of Amazon MQ for RabbitMQ
# ssl_context = ssl.SSLContext(ssl.PROTOCOL_TLSv1_2)
# ssl_context.set_ciphers('ECDHE+AESGCM:!ECDSA')

# url = f"amqps://distributed_cosim_demo:CONTACT-CLAUDIO@b-14c95d1b-b988-4039-a4fe-b5c6744b8a97.mq.eu-north-1.amazonaws.com:5671"

# parameters = pika.URLParameters(url)
# parameters.ssl_options = pika.SSLOptions(context=ssl_context)

# connection = pika.BlockingConnection(parameters)
# channel = connection.channel()

connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()

exchange_name = 'mass'
routing_key = 'mass.position'

channel.exchange_declare(exchange=exchange_name, exchange_type='topic')

message = '(' + ', '.join(sys.argv[1:]) + ')' 

channel.basic_publish(exchange=exchange_name, routing_key='mass.position', body=message)
print(f'[x] Sent to routing key, {routing_key}: {message}')
connection.close()