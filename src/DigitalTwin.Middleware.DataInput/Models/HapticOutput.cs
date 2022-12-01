using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput.Models
{
    public class HapticOutput
    {
        [JsonProperty("time")]
        public string? Time { get; set; }

        [JsonProperty("output_hapticfeedback_x_to_middleware")]
        public float OutputHapticFeedbackX { get; set; }
        [JsonProperty("output_hapticfeedback_y_to_middleware")]
        public float OutputHapticFeedbackY { get; set; }
        [JsonProperty("output_hapticfeedback_z_to_middleware")]
        public float OutputHapticFeedbackZ { get; set; }

        [JsonProperty("output_user_pos_x_to_middleware")]
        public float OutputUserPosXToMiddleware { get;set; }
        [JsonProperty("output_user_pos_y_to_middleware")]
        public float OutputUserPosYToMiddleware { get;set; }
        [JsonProperty("output_user_pos_z_to_middleware")]
        public float OutputUserPosZToMiddleware { get; set; }

        [JsonProperty("output_op_pos_x_to_middleware")]
        public float OutputOpPosXToMiddleware { get; set; }
        [JsonProperty("output_op_pos_y_to_middleware")]
        public float OutputOpPosYToMiddleware { get; set; }
        [JsonProperty("output_op_pos_z_to_middleware")]
        public float OutputOpPosZToMiddleware { get; set; }
    }
}
