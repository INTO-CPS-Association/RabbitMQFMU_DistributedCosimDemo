#!/usr/bin/env python3
import pika
import json

connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = connection.channel()

print("Declaring exchange")
channel.exchange_declare(exchange='example_exchange', exchange_type='direct')

msg = {}
msg['hello'] = "Hello world"
msg['level'] = 0

channel.basic_publish(exchange='example_exchange',
                        routing_key='example_queue.data.from_cosim',
                        body=json.dumps(msg))

print("Message sent")

connection.close()
