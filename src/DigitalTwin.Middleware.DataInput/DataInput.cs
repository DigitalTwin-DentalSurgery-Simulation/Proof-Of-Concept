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
        public string Time => DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:sszzzz", CultureInfo.InvariantCulture);
        [JsonProperty("output_op_pos_x")]
        public string PosX { get; set; }
        [JsonProperty("output_op_pos_y")]
        public string PosY { get; set; }
        [JsonProperty("output_op_pos_z")]
        public string PosZ { get; set; }
    }
}
