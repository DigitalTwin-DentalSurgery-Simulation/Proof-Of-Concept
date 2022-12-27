import numpy
import matplotlib.pyplot as plt
import pika
import os
import sys
import json
import time

class VisualizationOutput:
    def __init__(self, json):
        self.output_user_pos_x = float(json['output_user_pos_x_to_visualization'])
        self.output_user_pos_y = float(json['output_user_pos_y_to_visualization'])
        self.output_user_pos_z = float(json['output_user_pos_z_to_visualization'])
        self.output_op_pos_x = float(json['output_op_pos_x_to_visualization'])
        self.output_op_pos_y = float(json['output_op_pos_y_to_visualization'])
        self.output_op_pos_z = float(json['output_op_pos_z_to_visualization'])
        self.output_op_haptics_x = float(json['HapticsX'])
        self.output_op_haptics_y = float(json['HapticsY'])
        self.output_op_haptics_z = float(json['HapticsZ'])
        self.output_op_step = int(json['Step'])

class Visualizer:
    def __init__(self):
        self.count = 0
        """graph arrays of values, to be on graph"""
        self.op_x_array = numpy.empty([0])
        self.op_y_array = numpy.empty([0])
        self.op_z_array = numpy.empty([0])

        self.user_x_array = numpy.empty([0])
        self.user_y_array = numpy.empty([0])
        self.user_z_array = numpy.empty([0])

        self.haptics_x_array = numpy.empty([0])
        self.haptics_y_array = numpy.empty([0])
        self.haptics_z_array = numpy.empty([0])
        self.haptics_step_array = numpy.empty([0])

        self.lasttenxhaptics = 0.0
        self.lasttenyhaptics = 0.0
        self.lasttenzhaptics = 0.0

        """interactive on"""
        plt.ion()
        self.fig = plt.figure()
        self.ax = self.fig.add_subplot(projection='3d')
        self.ax.legend()
        self.ax.set_xlabel('X')
        self.ax.set_ylabel('Y')
        self.ax.set_zlabel('Z')



        self.hapticsax = plt.figure().add_subplot(1,1,1)
        self.hapticsax.legend()
        self.hapticsax.set_xlabel('X')
        self.hapticsax.set_ylabel('Y')
        

    def callback(self, ch, method, properties, body):
        

        print(" [x] Received %r" % body)

        message = json.loads(body)

        visualizationOutput = VisualizationOutput(message)

        visualizationOutput.output_user_pos_x += self.count

        #time.sleep(2)

        """make graph increase"""
        self.op_x_array = numpy.append(self.op_x_array, visualizationOutput.output_op_pos_x)
        self.op_y_array = numpy.append(self.op_y_array, visualizationOutput.output_op_pos_y)
        self.op_z_array = numpy.append(self.op_z_array, visualizationOutput.output_op_pos_z)

        self.user_x_array = numpy.append(self.user_x_array, visualizationOutput.output_user_pos_x)
        self.user_y_array = numpy.append(self.user_y_array, visualizationOutput.output_user_pos_y)
        self.user_z_array = numpy.append(self.user_z_array, visualizationOutput.output_user_pos_z)

        self.haptics_x_array = numpy.append(self.haptics_x_array, visualizationOutput.output_op_haptics_x)
        self.haptics_y_array = numpy.append(self.haptics_y_array, visualizationOutput.output_op_haptics_y)
        self.haptics_z_array = numpy.append(self.haptics_z_array, visualizationOutput.output_op_haptics_z)
        self.haptics_step_array = numpy.append(self.haptics_step_array, visualizationOutput.output_op_step)

        if(self.haptics_step_array[-1] % 10 == 0):
            self.lasttenxhaptics = self.calculateAbsOfArray(self.haptics_x_array[-10:])
            self.lasttenyhaptics = self.calculateAbsOfArray(self.haptics_y_array[-10:])
            self.lasttenzhaptics = self.calculateAbsOfArray(self.haptics_z_array[-10:])

        self.ax.annotate('Accumulated haptic \n feedback for last 10 readings:', xy=(-0.10, 0.01), xytext=(0, 10), xycoords=('axes fraction', 'figure fraction'), textcoords='offset points', ha='center', va='bottom')
        self.ax.annotate(f'x: {self.lasttenxhaptics}', xy=(0.5, 0.035), xytext=(0, 10), xycoords=('axes fraction', 'figure fraction'), textcoords='offset points', ha='center', va='bottom')
        self.ax.annotate(f'y: {self.lasttenyhaptics}', xy=(0.5, 0.01), xytext=(0, 10), xycoords=('axes fraction', 'figure fraction'), textcoords='offset points', ha='center', va='bottom')
        self.ax.annotate(f'z: {self.lasttenzhaptics}', xy=(0.5, -0.015), xytext=(0, 10), xycoords=('axes fraction', 'figure fraction'), textcoords='offset points', ha='center', va='bottom')

        self.ax.plot(self.op_x_array, self.op_y_array, zs=self.op_z_array, zdir='z', color="black", label='Optimal Path')
        self.ax.plot(self.user_x_array, self.user_y_array, zs=self.user_z_array, zdir='z', color="green", label='User Path')
        self.ax.legend()

        self.hapticsax.plot(self.haptics_step_array, self.haptics_x_array, label='x-axis')
        self.hapticsax.plot(self.haptics_step_array, self.haptics_y_array, label='y-axis')
        self.hapticsax.plot(self.haptics_step_array, self.haptics_z_array, label='z-axis')
        self.hapticsax.legend()

                
        plt.draw()
        plt.pause(0.5)

        
        self.ax.cla()

        self.hapticsax.cla()

        self.ax.set_xlabel('X')
        self.ax.set_ylabel('Y')
        self.ax.set_zlabel('Z')

        ch.basic_ack(delivery_tag=method.delivery_tag)

    def calculateAbsOfArray(self, arr):
        total = 0.0
        for value in arr:
            total += abs(value)

        return total

def main():
    visualization_queue = "visualization"

    connectionParameters = pika.ConnectionParameters(host = 'localhost', port = 5672)

    connection = pika.BlockingConnection(connectionParameters)

    visuazer = Visualizer()

    channel = connection.channel()

    channel.basic_qos(prefetch_count = 1)

    channel.basic_consume(queue=visualization_queue,
                        auto_ack=False,
                        on_message_callback=visuazer.callback)

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
