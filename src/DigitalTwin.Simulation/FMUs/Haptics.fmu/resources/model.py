import pickle


class Model:
    def __init__(self) -> None:
        self.input_user_pos_x_to_haptics = 0.0
        self.input_user_pos_y_to_haptics = 0.0
        self.input_user_pos_z_to_haptics = 0.0
        self.input_op_pos_x_to_haptics = 0.0
        self.input_op_pos_y_to_haptics = 0.0
        self.input_op_pos_z_to_haptics = 0.0
        self.input_errorscore_to_haptics = 0.0
        self.input_step_to_haptics = 0

        self.temp_output_hapticfeedback_x_to_middleware = 0.0
        self.temp_output_hapticfeedback_y_to_middleware = 0.0
        self.temp_output_hapticfeedback_z_to_middleware = 0.0
        self.temp_output_user_pos_x_to_middleware = 0.0
        self.temp_output_user_pos_y_to_middleware = 0.0
        self.temp_output_user_pos_z_to_middleware = 0.0
        self.temp_output_op_pos_x_to_middleware = 0.0
        self.temp_output_op_pos_y_to_middleware = 0.0
        self.temp_output_op_pos_z_to_middleware = 0.0

        self.output_step_to_middleware = 0


        self.reference_to_attribute = {
            0: "input_user_pos_x_to_haptics",
            1: "input_user_pos_y_to_haptics",
            2: "input_user_pos_z_to_haptics",
            3: "input_op_pos_x_to_haptics",
            4: "input_op_pos_y_to_haptics",
            5: "input_op_pos_z_to_haptics",
            6: "input_errorscore_to_haptics",
            7: "output_hapticfeedback_x_to_middleware",
            8: "output_hapticfeedback_y_to_middleware",
            9: "output_hapticfeedback_z_to_middleware",
            10: "output_user_pos_x_to_middleware",
            11: "output_user_pos_y_to_middleware",
            12: "output_user_pos_z_to_middleware",
            13: "output_op_pos_x_to_middleware",
            14: "output_op_pos_y_to_middleware",
            15: "output_op_pos_z_to_middleware",
            16: "input_step_to_haptics",
            17: "output_step_to_middleware"
        }

        self._update_outputs()

    def fmi2DoStep(self, current_time, step_size, no_step_prior):
        """Here do something with the input parameters"""

        """"Creating direction vectors"""
        direction_vector_x = self.input_op_pos_x_to_haptics - self.input_user_pos_x_to_haptics
        direction_vector_y = self.input_op_pos_y_to_haptics - self.input_user_pos_y_to_haptics
        direction_vector_z = self.input_op_pos_z_to_haptics - self.input_user_pos_z_to_haptics

        """Setting threshold for directed length based on errorscore"""
        yellow_zone_start_threshold = 0.02
        red_zone_start_threshold = 2.25
        yellow_zone_gradient_factor = 0.45

        """Calculating the force to be applied in the vector direction, based on errorscore"""
        if self.input_errorscore_to_haptics < yellow_zone_start_threshold:
            applied_force = 0
        elif self.input_errorscore_to_haptics >= red_zone_start_threshold:
            applied_force = 1.0
        else:
            applied_force = self.input_errorscore_to_haptics * yellow_zone_gradient_factor

        print(f'Error score: {self.input_errorscore_to_haptics}')

        """Applying force to vector direction"""
        self.temp_output_hapticfeedback_x_to_middleware = direction_vector_x * applied_force
        self.temp_output_hapticfeedback_y_to_middleware = direction_vector_y * applied_force
        self.temp_output_hapticfeedback_z_to_middleware = direction_vector_z * applied_force

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
                self.input_user_pos_x_to_haptics,
                self.input_user_pos_y_to_haptics,
                self.input_user_pos_z_to_haptics,
                self.input_op_pos_x_to_haptics,
                self.input_op_pos_y_to_haptics,
                self.input_op_pos_z_to_haptics,
                self.input_errorscore_to_haptics,
                self.input_step_to_haptics         
            )
        )
        return Fmi2Status.ok, bytes

    def fmi2ExtDeserialize(self, bytes) -> int:
        (
            input_user_pos_x_to_haptics,
            input_user_pos_y_to_haptics,
            input_user_pos_z_to_haptics,
            input_op_pos_x_to_haptics,
            input_op_pos_y_to_haptics,
            input_op_pos_z_to_haptics,
            input_errorscore_to_haptics,
            input_step_to_haptics     
        ) = pickle.loads(bytes)
        self.input_user_pos_x_to_haptics = input_user_pos_x_to_haptics
        self.input_user_pos_y_to_haptics = input_user_pos_y_to_haptics
        self.input_user_pos_z_to_haptics = input_user_pos_z_to_haptics
        self.input_op_pos_x_to_haptics = input_op_pos_x_to_haptics
        self.input_op_pos_y_to_haptics = input_op_pos_y_to_haptics
        self.input_op_pos_z_to_haptics = input_op_pos_z_to_haptics
        self.input_errorscore_to_haptics = input_errorscore_to_haptics
        self.input_step_to_haptics = input_step_to_haptics
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

    """updating output values that are just propagated.
    """
    def _update_outputs(self):
        self.output_user_pos_x_to_middleware = self.input_user_pos_x_to_haptics
        self.output_user_pos_y_to_middleware = self.input_user_pos_y_to_haptics
        self.output_user_pos_z_to_middleware = self.input_user_pos_z_to_haptics
        self.output_op_pos_x_to_middleware = self.input_op_pos_x_to_haptics
        self.output_op_pos_y_to_middleware = self.input_op_pos_y_to_haptics
        self.output_op_pos_z_to_middleware = self.input_op_pos_z_to_haptics
        self.output_hapticfeedback_x_to_middleware = self.temp_output_hapticfeedback_x_to_middleware
        self.output_hapticfeedback_y_to_middleware = self.temp_output_hapticfeedback_y_to_middleware
        self.output_hapticfeedback_z_to_middleware = self.temp_output_hapticfeedback_z_to_middleware
        self.output_step_to_middleware = self.input_step_to_haptics


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
    m = Model()

    """ for debugging
    assert m.real_a == 0.0
    assert m.real_b == 0.0
    assert m.real_c == 0.0
    assert m.integer_a == 0
    assert m.integer_b == 0
    assert m.integer_c == 0
    assert m.boolean_a == False
    assert m.boolean_b == False
    assert m.boolean_c == False
    assert m.string_a == ""
    assert m.string_b == ""
    assert m.string_c == ""

    m.real_a = 1.0
    m.real_b = 2.0
    m.integer_a = 1
    m.integer_b = 2
    m.boolean_a = True
    m.boolean_b = False
    m.string_a = "Hello "
    m.string_b = "World!"

    assert m.fmi2DoStep(0.0, 1.0, False) == Fmi2Status.ok

    assert m.real_c == 3.0
    assert m.integer_c == 3
    assert m.boolean_c == True
    assert m.string_c == "Hello World!"""
