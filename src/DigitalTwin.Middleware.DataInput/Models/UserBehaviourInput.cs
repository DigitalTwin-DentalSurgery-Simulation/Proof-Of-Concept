using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput.Models
{
    public class UserBehaviourInput
    {
        public UserBehaviourInput(float userPositionX, float userPositionY, float userPositionZ)
        {
            UserPositionX = userPositionX;
            UserPositionY = userPositionY;
            UserPositionZ = userPositionZ;
        }

        [JsonProperty("time")]
        public string Time { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss\\.ffffzzzz", CultureInfo.InvariantCulture);
        [JsonProperty("output_user_pos_x_to_op")]
        public float UserPositionX { get; set; }
        [JsonProperty("output_user_pos_y_to_op")]
        public float UserPositionY { get; set; }
        [JsonProperty("output_user_pos_z_to_op")]
        public float UserPositionZ { get; set; }
    }
}
