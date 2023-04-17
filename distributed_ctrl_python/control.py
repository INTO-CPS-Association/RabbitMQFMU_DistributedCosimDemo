#!/usr/bin/env python3
import pika
import json
from datetime import datetime as dt
import datetime

connection = pika.BlockingConnection(pika.ConnectionParameters('172.20.0.2'))
channel = connection.channel()

print("Declaring exchange")
channel.exchange_declare(exchange='example_exchange', exchange_type='direct')

queue_incoming = channel.queue_declare(queue='', exclusive=True)
queue_name = queue_incoming.method.queue
channel.queue_bind(exchange='example_exchange', queue=queue_name,
                   routing_key='msd')

print(' [*] Waiting for logs. To exit press CTRL+C: ')

def control_loop(ch, method, properties, body):
  print(" [x] %r" % body)

  msg_in = json.loads(body)

  utc_delta = datetime.timedelta(hours=0)
  utc_tz = datetime.timezone(utc_delta,
                                  name="UTC")

  msg = {}
  msg['time'] = dt.now(tz = utc_tz).isoformat(timespec='milliseconds')
  msg['fk'] = 1.0

  if "timestep" in msg_in:
    msg['time'] = msg_in["timestep"]
  
  channel.basic_publish(exchange='example_exchange', routing_key='controller', body=json.dumps(msg))
  print("Sent:")
  print(msg)


channel.basic_consume(
    queue=queue_name, on_message_callback=control_loop, auto_ack=True)

channel.start_consuming()
connection.close()
