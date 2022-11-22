using Newtonsoft.Json;
using SimToCare.DigitalTwin.DataInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput
{
    public class DataReader
    {
        public static Recording Read(string filePath)
        {
            using var reader = new StreamReader(filePath);

            var json = reader.ReadToEnd();

            var recording = JsonConvert.DeserializeObject<Recording>(json);

            if (recording is null)
                throw new ArgumentNullException(nameof(recording));

            return recording;
        }

    }
}
