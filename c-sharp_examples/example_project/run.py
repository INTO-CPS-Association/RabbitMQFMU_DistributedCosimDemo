
#This file is for running the example project manually. It is not used in the C# project 
import argparse
import csv
import http.client
import json
import sys
import websocket
import threading

# Argument Parsing
parser = argparse.ArgumentParser(prog='Example of Maestro Master Web Interface', usage='%(prog)s [options]')
parser.add_argument('--live', help='live output from API', action='store_true')
parser.add_argument('--port', help='Maestro connection port')
parser.set_defaults(live=False)
parser.set_defaults(port=8082)

args = parser.parse_args()

port = args.port
liveOutput = args.live

# WebSocket Functions
def on_message(ws, message):
    print("Received message: ", message)

def on_error(ws, error):
    print("Error: ", error)

def on_close(ws, close_status_code, close_msg):
    print("### closed ###")

def on_open(ws):
    print("WebSocket opened")

def run_websocket(session_id):
    websocket.enableTrace(False)
    ws_url = f"ws://localhost:{port}/attachSession/{session_id}"
    ws = websocket.WebSocketApp(ws_url,
                                on_open=on_open,
                                on_message=on_message,
                                on_error=on_error,
                                on_close=on_close)
    ws.run_forever()

# HTTP Functions
def post(c, location, data_path):
    headers = {'Content-type': 'application/json'}
    foo = json.load(open(data_path))
    json_data = json.dumps(foo)
    c.request('POST', location, json_data, headers)
    res = c.getresponse()
    return res

# Main Script
config = json.load(open("scenario.json"))

conn = http.client.HTTPConnection('localhost:' + str(port))

print("Create session")
conn.request('GET', '/createSession')
response = conn.getresponse()
if not response.status == 200:
    print("Could not create session")
    sys.exit()

status = json.loads(response.read().decode())
print ("Session '%s', data=%s'" % (status["sessionId"], status))

if liveOutput:
    ws_thread = threading.Thread(target=run_websocket, args=(status["sessionId"],))
    ws_thread.start()

response = post(conn, '/initialize/' + status["sessionId"], "scenario.json")
if not response.status == 200:
    print("Could not initialize")
    sys.exit()

print ("Initialize response code '%d, data=%s'" % (response.status, response.read().decode()))

response = post(conn, '/simulate/' + status["sessionId"], "simulate.json")
if not response.status == 200:
    print("Could not simulate")
    sys.exit()

print ("Simulate response code '%d, data=%s'" % (response.status, response.read().decode()))

conn.request('GET', '/result/' + status["sessionId"] + "/plain")
response = conn.getresponse()
if not response.status == 200:
    print("Could not receive results")
    sys.exit()

result_csv_path = "result.csv"
csv_data = response.read().decode()
print ("Result response code '%d" % (response.status))
f = open(result_csv_path, "w")
f.write(csv_data)
f.close()

conn.request('GET', '/destroy/' + status['sessionId'])
response = conn.getresponse()
if not response.status == 200:
    print("Could not destroy session")
    sys.exit()

print ("Destroy response code '%d, data='%s'" % (response.status, response.read().decode()))

if liveOutput:
    ws_thread.join()



#Commands to remove process from port if it is still running
#netstat -ano | findstr :8082
#taskkill /PID 1234 /F