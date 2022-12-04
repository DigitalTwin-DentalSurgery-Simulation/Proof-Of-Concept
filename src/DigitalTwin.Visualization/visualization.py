import numpy
import matplotlib.pyplot as plt
import pika
import os
import sys
import json

class VisualizationOutput:
    def __init__(self, json):
        self.output_user_pos_x = json['output_user_pos_x_to_visualization']
        self.output_user_pos_y = json['output_user_pos_y_to_visualization']
        self.output_user_pos_z = json['output_user_pos_z_to_visualization']
        self.output_op_pos_x = json['output_op_pos_x_to_visualization']
        self.output_op_pos_y = json['output_op_pos_y_to_visualization']
        self.output_op_pos_z = json['output_op_pos_z_to_visualization']

class Visualizer:
    def __init__(self) -> None:
        """graph arrays of values, to be on graph"""
        self.op_x_array = numpy.empty([0])
        self.op_y_array = numpy.empty([0])
        self.op_z_array = numpy.empty([0])

        self.user_x_array = numpy.empty([0])
        self.user_y_array = numpy.empty([0])
        self.user_z_array = numpy.empty([0])

        """interactive on"""
        plt.ion()
        self.fig = plt.figure()
        self.ax = self.fig.add_subplot(projection='3d')
        self.ax.legend()
        self.ax.set_xlabel('X')
        self.ax.set_ylabel('Y')
        self.ax.set_zlabel('Z')

def callback(self, ch, method, properties, body):
    print(" [x] Received %r" % body)

    message = json.loads(body)

    visualizationOutput = VisualizationOutput(message)

    print(visualizationOutput.output_op_pos_x)

    #time.sleep(2)

    ch.basic_ack(delivery_tag=method.delivery_tag)

    """make graph increase"""
    self.op_x_array = numpy.append(self.op_x_array, visualizationOutput.output_op_pos_x)
    self.op_y_array = numpy.append(self.op_y_array, visualizationOutput.output_op_pos_y)
    self.op_z_array = numpy.append(self.op_z_array, visualizationOutput.output_op_pos_z)

    user_x_array = numpy.append(user_x_array, visualizationOutput.output_user_pos_x)
    user_y_array = numpy.append(user_y_array, visualizationOutput.output_user_pos_y)
    user_z_array = numpy.append(user_z_array, visualizationOutput.output_user_pos_z)

    self.ax.plot(self.op_x_array, self.op_y_array, zs=self.op_z_array, zdir='z')
    self.ax.plot(self.user_x_array, self.user_y_array, zs=self.user_z_array, zdir='z')
            
    plt.draw()
    plt.pause(0.5)

    self.ax.cla()

def main():
    visualization_queue = "visualization"

    connectionParameters = pika.ConnectionParameters(host = 'localhost', port = 5672)

    connection = pika.BlockingConnection(connectionParameters)
    channel = connection.channel()

    channel.basic_qos(prefetch_count = 1)

    channel.basic_consume(queue=visualization_queue,
                        auto_ack=False,
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
