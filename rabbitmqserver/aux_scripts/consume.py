#!/usr/bin/env python3
import pika
import json
import datetime
import time
import ssl

connection = pika.BlockingConnection(pika.ConnectionParameters('172.17.0.1'))

channel = connection.channel()

print("Declaring exchange")
channel.exchange_declare(exchange='example_exchange', exchange_type='direct')

print("Creating queue")
result = channel.queue_declare(queue='', exclusive=True)
queue_name = result.method.queue

channel.queue_bind(exchange='example_exchange', queue=queue_name,
                   routing_key='example_queue.data.from_cosim')

print(' [*] Waiting for logs. To exit press CTRL+C')
print(' [*] I am consuming the commands sent from rbMQ')

def callback(ch, method, properties, body):
    print("Received [x] %r" % body)


channel.basic_consume(
    queue=queue_name, on_message_callback=callback, auto_ack=True)

channel.start_consuming()

connection.close()
