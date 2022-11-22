using DigitalTwin.Middleware.DataInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimToCare.DigitalTwin.DataInput;

public class Recording
{
    public DateTime Started { get; set; }
    public List<Data> Data { get; set; }

    public Recording()
    {
        Data = new List<Data>();
    }
}
