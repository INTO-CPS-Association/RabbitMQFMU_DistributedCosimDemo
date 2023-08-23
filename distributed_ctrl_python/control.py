#!/usr/bin/env python3
import pika
import json
from datetime import datetime as dt
import datetime

import ssl
import pika

# SSL Context for TLS configuration of Amazon MQ for RabbitMQ
ssl_context = ssl.SSLContext(ssl.PROTOCOL_TLSv1_2)
ssl_context.set_ciphers('ECDHE+AESGCM:!ECDSA')

url = f"amqps://distributed_cosim_demo:CONTACT_CLAUDIO@b-14c95d1b-b988-4039-a4fe-b5c6744b8a97.mq.eu-north-1.amazonaws.com:5671"
# Uncomment for connecting to local rabbitmq server
# url = f"172.20.0.2"
parameters = pika.URLParameters(url)
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
    print(msg_in["timestep"])

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
