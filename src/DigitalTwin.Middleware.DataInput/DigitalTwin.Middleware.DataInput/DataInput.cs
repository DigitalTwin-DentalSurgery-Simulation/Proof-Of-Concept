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
        [JsonProperty("posX")]
        public string PosX { get; set; }
        [JsonProperty("posY")]
        public string PosY { get; set; }
        [JsonProperty("posZ")]
        public string PosZ { get; set; }
    }
}
