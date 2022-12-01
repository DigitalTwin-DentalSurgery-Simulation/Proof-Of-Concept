using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput.Models
{
    public class VisualizationInput
    {
        [JsonProperty("output_user_pos_x_to_visualization")]
        public float UserPosX { get; set; }
        [JsonProperty("output_user_pos_y_to_visualization")]
        public float UserPosY { get; set; }
        [JsonProperty("output_user_pos_z_to_visualization")]
        public float UserPosZ { get; set; }

        [JsonProperty("output_op_pos_x_to_visualization")]
        public float OpPosX { get; set; }
        [JsonProperty("output_op_pos_y_to_visualization")]
        public float OpPosZ { get; set; }
        [JsonProperty("output_op_pos_z_to_visualization")]
        public float OpPosY { get; set; }
    }
}
