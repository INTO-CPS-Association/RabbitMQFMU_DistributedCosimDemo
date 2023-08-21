import pika
import time
import ssl

# # SSL Context for TLS configuration of Amazon MQ for RabbitMQ
# ssl_context = ssl.SSLContext(ssl.PROTOCOL_TLSv1_2)
# ssl_context.set_ciphers('ECDHE+AESGCM:!ECDSA')

# url = f"amqps://distributed_cosim_demo:CONTACT-CLAUDIO@b-14c95d1b-b988-4039-a4fe-b5c6744b8a97.mq.eu-north-1.amazonaws.com:5671"

# parameters = pika.URLParameters(url)
# parameters.ssl_options = pika.SSLOptions(context=ssl_context)

# connection = pika.BlockingConnection(parameters)
# channel = connection.channel()

# exchange_name = ''
# binding_key = 'test1'

# Connect to a broker on local host
connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()

exchange_name = 'mass'
binding_key = 'mass.position'

channel.exchange_declare(exchange=exchange_name, exchange_type='topic')

def send_message(message):
    channel.basic_publish(exchange=exchange_name,
                        routing_key=binding_key,
                        body=message)  
    print(f"[x] Sent message: {message}")
    

def short_pushes():
    for i in range(6):
        send_message('(0, 0, -0.5)')
        time.sleep(0.1)

    for i in range(7):
        send_message('(0, 0, 0.5)')
        time.sleep(0.1)

def long_pushes1():
    for i in range(20):
        send_message('(0, 0, 0.5)')
        time.sleep(0.1)

    for i in range(20):
        send_message('(0, 0, -0.5)')
        time.sleep(0.1)

def long_pushes2():
    for i in range(20):
        send_message('(0, 0, -0.5)')
        time.sleep(0.1)

    for i in range(20):
        send_message('(0, 0, 0.5)')
        time.sleep(0.1)
    

if __name__ == '__main__':
    while True:
        long_pushes1()
        short_pushes()
        long_pushes1()
        short_pushes()
        long_pushes2()
        long_pushes1()
        short_pushes()