import pickle
import os
import json

class Model:
    def __init__(self) -> None:
        self.input_user_pos_x_to_op = 0.0
        self.input_user_pos_y_to_op = 0.0
        self.input_user_pos_z_to_op = 0.0
        self.input_step_to_op = 0

        self.simtoCareJson = None

        self.temp_output_op_pos_x_to_haptics = 0.0
        self.temp_output_op_pos_y_to_haptics = 0.0
        self.temp_output_op_pos_z_to_haptics = 0.0

        self.temp_output_errorscore_to_haptics = 0.0

        self.temp_output_op_pos_x_to_haptics = 0.0
        self.temp_output_op_pos_y_to_haptics = 0.0
        self.temp_output_op_pos_z_to_haptics = 0.0

        self.reference_to_attribute = {
            0: "input_user_pos_x_to_op",
            1: "input_user_pos_y_to_op",
            2: "input_user_pos_z_to_op",
            3: "output_user_pos_x_to_haptics",
            4: "output_user_pos_y_to_haptics",
            5: "output_user_pos_z_to_haptics",
            6: "output_op_pos_x_to_haptics",
            7: "output_op_pos_y_to_haptics",
            8: "output_op_pos_z_to_haptics",
            9: "output_errorscore_to_haptics",
            10: "input_step_to_op"
        }

        self._update_outputs()

    def fmi2DoStep(self, current_time, step_size, no_step_prior):
        print("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH")
        
        if(self.simtoCareJson is None):
            simtoCareJsonFilePath = "simtocare-recording.json"
            simtoCareJsonFile = open(simtoCareJsonFilePath, 'r')
            self.simtoCareJson = json.load(simtoCareJsonFile)
        
        data = self.simtoCareJson['data'][self.input_step_to_op]

        self.input_step_to_op += 1

        optimal_path_position = data['pos']

        optimalPathPosition = Position(optimal_path_position)
        
        mean_absolute_error = abs(
            self.input_user_pos_x_to_op-optimalPathPosition.optimal_path_x 
            + self.input_user_pos_y_to_op-optimalPathPosition.optimal_path_y 
            + self.input_user_pos_z_to_op-optimalPathPosition.optimal_path_z
            ) / 3

        self.temp_output_op_pos_x_to_haptics = optimalPathPosition.optimal_path_x
        self.temp_output_op_pos_y_to_haptics = optimalPathPosition.optimal_path_y
        self.temp_output_op_pos_z_to_haptics = optimalPathPosition.optimal_path_z
        self.temp_output_errorscore_to_haptics = mean_absolute_error
        
        self._update_outputs()
        return Fmi2Status.ok

    def fmi2EnterInitializationMode(self):

        return Fmi2Status.ok

    def fmi2ExitInitializationMode(self):
        self._update_outputs()
        return Fmi2Status.ok

    def fmi2SetupExperiment(self, start_time, stop_time, tolerance):
        return Fmi2Status.ok

    def fmi2SetReal(self, references, values):
        return self._set_value(references, values)

    def fmi2SetInteger(self, references, values):
        return self._set_value(references, values)

    def fmi2SetBoolean(self, references, values):
        return self._set_value(references, values)

    def fmi2SetString(self, references, values):
        return self._set_value(references, values)

    def fmi2GetReal(self, references):
        return self._get_value(references)

    def fmi2GetInteger(self, references):
        return self._get_value(references)

    def fmi2GetBoolean(self, references):
        return self._get_value(references)

    def fmi2GetString(self, references):
        return self._get_value(references)

    def fmi2Reset(self):
        return Fmi2Status.ok

    def fmi2Terminate(self):
        return Fmi2Status.ok

    def fmi2ExtSerialize(self):

        bytes = pickle.dumps(
            (
                self.input_user_pos_x_to_op,
                self.input_user_pos_y_to_op,
                self.input_user_pos_z_to_op,
                self.input_step_to_op
            )
        )
        return Fmi2Status.ok, bytes

    def fmi2ExtDeserialize(self, bytes) -> int:
        (
            input_user_pos_x_to_op,
            input_user_pos_y_to_op,
            input_user_pos_z_to_op,
            input_step_to_op
        ) = pickle.loads(bytes)
        self.input_user_pos_x_to_op = input_user_pos_x_to_op
        self.input_user_pos_y_to_op = input_user_pos_y_to_op
        self.input_user_pos_z_to_op = input_user_pos_z_to_op
        self.input_step_to_op = input_step_to_op
        self._update_outputs()

        return Fmi2Status.ok

    def _set_value(self, references, values):

        for r, v in zip(references, values):
            setattr(self, self.reference_to_attribute[r], v)

        return Fmi2Status.ok

    def _get_value(self, references):

        values = []

        for r in references:
            values.append(getattr(self, self.reference_to_attribute[r]))

        return Fmi2Status.ok, values

    def _update_outputs(self):
        self.output_user_pos_x_to_haptics = self.input_user_pos_x_to_op
        self.output_user_pos_y_to_haptics = self.input_user_pos_y_to_op
        self.output_user_pos_z_to_haptics = self.input_user_pos_z_to_op
        self.output_op_pos_x_to_haptics = self.temp_output_op_pos_x_to_haptics
        self.output_op_pos_y_to_haptics = self.temp_output_op_pos_y_to_haptics
        self.output_op_pos_z_to_haptics = self.temp_output_op_pos_z_to_haptics
        self.output_errorscore_to_haptics = self.temp_output_errorscore_to_haptics

class Position:
    def __init__(self, json):
        self.optimal_path_x = json[0]
        self.optimal_path_y = json[1]
        self.optimal_path_z = json[2]

class Fmi2Status:
    """Represents the status of the FMU or the results of function calls.

    Values:
        * ok: all well
        * warning: an issue has arisen, but the computation can continue.
        * discard: an operation has resulted in invalid output, which must be discarded
        * error: an error has ocurred for this specific FMU instance.
        * fatal: an fatal error has ocurred which has corrupted ALL FMU instances.
        * pending: indicates that the FMu is doing work asynchronously, which can be retrived later.

    Notes:
        FMI section 2.1.3

    """

    ok = 0
    warning = 1
    discard = 2
    error = 3
    fatal = 4
    pending = 5


if __name__ == "__main__":

    simtoCareJsonFilePath = "resources/simtocare-recording.json"
    simtoCareJsonFile = open(simtoCareJsonFilePath, 'r')

    if(simtoCareJsonFile.closed):
        print('WTF')

    recording_step = 0

    recording = json.load(simtoCareJsonFile)

    data = recording['data'][recording_step]

    optimal_path = data['pos']

    optimalPathPosition = Position(optimal_path)

    print(optimalPathPosition.optimal_path_x)
    print(optimalPathPosition.optimal_path_y)
    print(optimalPathPosition.optimal_path_z)

    mae = abs( - optimalPathPosition.optimal_path_x + - optimalPathPosition.optimal_path_y + - optimalPathPosition.optimal_path_z) / 3


