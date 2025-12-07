namespace software_engineering.Lib
{
    public class PressurePoint
    {
        public int x;
        public int y;
        public int pressure;

        public PressurePoint(int x, int y, int pressure)
        {
            this.x = x;
            this.y = y;
            this.pressure = pressure;
        }

        public string Serialise()
        {
            return this.x.ToString() + "," + this.y.ToString();
        }
    }
}
