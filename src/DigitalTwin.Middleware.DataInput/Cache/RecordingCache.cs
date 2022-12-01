using SimToCare.DigitalTwin.DataInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput.Cache
{
    public class RecordingCache
    {
        private readonly Recording cachedRecording;

        public RecordingCache(Recording cachedRecording)
        {
            this.cachedRecording = cachedRecording;
        }

        public Recording GetRecording() => cachedRecording;
        public List<Data> GetData() => cachedRecording.Data;
    }
}
