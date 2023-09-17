import json
import pika
import time
import ssl

connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel()


exchange_name = 'example_exchange'
binding_key = 'msd'

channel.exchange_declare(exchange=exchange_name, exchange_type='direct')
channel.queue_declare(queue='test', auto_delete=True)

msg = {}
msg['x'] = 0.0
msg['timestep'] = "10"
msg['simstep'] = "1000"

def send_message(message: dict):
    channel.basic_publish(exchange=exchange_name,
                        routing_key=binding_key,
                        body=json.dumps(message))  
    print(f"[x] Sent message: {message}")

while True:
    for i in range(100):
        msg['x'] += 0.01
        send_message(msg)
        time.sleep(0.02)
    for i in range(100):
        msg['x'] -= 0.01
        send_message(msg)
        time.sleep(0.02)