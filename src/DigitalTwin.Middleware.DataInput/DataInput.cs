using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput
{
    public class DataInput
    {
        [JsonProperty("time")]
        public string Time => DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss\\.ffffzzzz", CultureInfo.InvariantCulture);
        [JsonProperty("output_op_pos_x_to_op")]
        public string PosX { get; set; }
        [JsonProperty("output_op_pos_y_to_op")]
        public string PosY { get; set; }
        [JsonProperty("output_op_pos_z_to_op")]
        public string PosZ { get; set; }
        [JsonProperty("output_user_pos_x_to_op")]
        public string UserPosX { get; set; }
        [JsonProperty("output_user_pos_y_to_op")]
        public string UserPosY { get; set; }
        [JsonProperty("output_user_pos_z_to_op")]
        public string UserPosZ { get; set; }
    }
}
