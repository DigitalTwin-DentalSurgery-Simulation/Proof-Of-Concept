import os
import sys
import matplotlib.pyplot as plt
import random
import numpy
from datetime import datetime, timedelta

def main():
    """values to update and put into graph array"""
    op_x = random.randint(-5, 5)
    op_y = random.randint(-5, 5)
    op_z = random.randint(-5, 5)

    user_x = random.randint(-5, 5)
    user_y = random.randint(-5, 5)
    user_z = random.randint(-5, 5)

    """graph arrays of values, to be on graph"""
    op_x_array = numpy.empty([0])
    op_y_array = numpy.empty([0])
    op_z_array = numpy.empty([0])

    user_x_array = numpy.empty([0])
    user_y_array = numpy.empty([0])
    user_z_array = numpy.empty([0])

    """interactive on"""
    plt.ion()
    fig = plt.figure()
    ax = fig.add_subplot(projection='3d')
    ax.legend()
    ax.set_xlabel('X')
    ax.set_ylabel('Y')
    ax.set_zlabel('Z')
    
    while(True):
        """make graph increase"""
        time_start = datetime.now()

        op_x = op_x + random.randint(1, 5)
        op_y = op_y + random.randint(-1, 3)
        op_z = op_z + random.randint(-1, 3)

        op_x_array = numpy.append(op_x_array, op_x)
        op_y_array = numpy.append(op_y_array, op_y)
        op_z_array = numpy.append(op_z_array, op_z)

        user_x = user_x + random.randint(1, 5)
        user_y = user_y + random.randint(-1, 3)
        user_z = user_z + random.randint(-1, 3)

        user_x_array = numpy.append(user_x_array, user_x)
        user_y_array = numpy.append(user_y_array, user_y)
        user_z_array = numpy.append(user_z_array, user_z)

        ax.plot(op_x_array, op_y_array, zs=op_z_array, zdir='z')
        ax.plot(user_x_array, user_y_array, zs=user_z_array, zdir='z')
        
        plt.draw()

        time_end = datetime.now()

        print("timedelta: " + str(time_end - time_start))

        plt.pause(0.5)

        ax.cla()









if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print('Interrupted')
        try:
            sys.exit(0)
        except SystemExit:
            os._exit(0)