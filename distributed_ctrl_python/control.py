#!/usr/bin/env python3
import pika
import json
from datetime import datetime as dt
import datetime

import ssl
import pika

local_rabbitmq_server = True # set False to connect to AWS rabbitmq server

ssl_context = None
if not local_rabbitmq_server:
  ssl_context = ssl.SSLContext(ssl.PROTOCOL_TLSv1_2)
  ssl_context.set_ciphers('ECDHE+AESGCM:!ECDSA')

url = f"amqp://guest:guest@rabbitmq-container"
if not local_rabbitmq_server:
  url = f"amqps://distributed_cosim_demo:CONTACT_CLAUDIO@CONTACT_CLAUDIO:5671"

parameters = pika.URLParameters(url)
if ssl_context is not None:
  parameters.ssl_options = pika.SSLOptions(context=ssl_context)

connection = pika.BlockingConnection(parameters)
channel = connection.channel()

print("Declaring exchange")
channel.exchange_declare(exchange='example_exchange', exchange_type='direct')

queue_incoming = channel.queue_declare(queue='', exclusive=True)
queue_name = queue_incoming.method.queue
channel.queue_bind(exchange='example_exchange', queue=queue_name,
                   routing_key='msd')

print(' [*] Waiting for logs. To exit press CTRL+C: ')

utc_delta = datetime.timedelta(hours=2) 
dk_tz = datetime.timezone(utc_delta, name="UTC")
time_0 = dt.now(tz = dk_tz)

def control_loop(ch, method, properties, body):
  global time_0
  print(" [x] %r" % body)

  msg_in = json.loads(body)

  msg = {}

  if "internal_status" in msg_in:
    time_0 = dt.now(tz = dk_tz)
    msg['time'] = time_0.isoformat(timespec='milliseconds')
    msg['fk'] = 0.0
  else:
    assert "simstep" in msg_in
    msg['time'] = (time_0 + datetime.timedelta(milliseconds=float(msg_in["simstep"]))).isoformat(timespec='milliseconds')
    print(msg_in["simstep"])
  
  msg['fk'] = 1.0

  if "simstep" in msg_in:
    simstep = float(msg_in["simstep"])
    
    if simstep < 10000:
      msg['fk'] = 1.0
    elif simstep < 20000:
      msg['fk'] = 2.0
    elif simstep < 30000:
      msg['fk'] = 0.5
    elif simstep < 40000:
      msg['fk'] = 1.0

  channel.basic_publish(exchange='example_exchange', routing_key='controller', body=json.dumps(msg))
  print("Sent:")
  print(msg)


channel.basic_consume(
    queue=queue_name, on_message_callback=control_loop, auto_ack=True)

channel.start_consuming()
connection.close()
