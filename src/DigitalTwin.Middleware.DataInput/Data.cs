namespace DigitalTwin.Middleware.DataInput
{
    public class Data
    {
        public int Ts { get; set; }
        public float[] Rot { get; set; }
        public float[] Pos { get; set; }

        public Data()
        {
            Rot = new float[4];
            Pos = new float[3];
        }
    }
}