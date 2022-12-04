import numpy
import scipy.io as sio
import matplotlib.pyplot as plt
import time
import paho.mqtt.client as client
#from paho import mqtt  
import pika
import os
import sys
"""Write the whole code in here. For testing, just make a for while loop that loads some values on a graph every second or so"""


"""Constants"""
"""
hostname = "localhost"
port = 5672
topic = "visualization"
username = "guest"
password = "guest"
"""
"""MQTT functions"""
"""
def on_connect(client, userdata, flags, rc):
    print("MQTT Connection established, rc result: " + str(rc))
    client.subscribe(topic)

def on_message(client, userdata, msg):
    # Add message to graphs 
    print(msg.payload.decode)


client = client.Client(client_id="", userdata=None, protocol=client.MQTTv311)
client.on_connect = on_connect
client.on_message = on_message
client.connect(hostname)"""
"""Old above"""
####################################
def main():
    visualization_queue = "visualization"

    connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
    channel = connection.channel()
    channel.queue_declare(queue=visualization_queue)

    def callback(ch, method, properties, body):
        print(" [x] Received %r" % body)

    channel.basic_consume(queue=visualization_queue,
                        auto_ack=True,
                        on_message_callback=callback)

    print(' [*] Waiting for messages. To exit press CTRL+C')
    channel.start_consuming()

if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print('Interrupted')
        try:
            sys.exit(0)
        except SystemExit:
            os._exit(0)


"""Do not do this solution. Instead of polling, just make a solution, that updates the graphs, everytime new data is gotten from the MQ"""
"""while(True):
    print('hello geek!')
    time.sleep(50)"""