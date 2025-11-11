using System.Numerics;

namespace software_engineering.Lib
{
    public class PressureMatrix
    {
        private List<List<short>> Data;

        public DateTime Timestamp;

        public PressureMatrix(List<List<short>> data, DateTime timestamp)
        {
            this.Data = data;
            this.Timestamp = timestamp;
        }

        public List<List<Vector2>> GetHighPressureRegions(int tolerance = 20)
        {
            throw new NotImplementedException();
        }

        public int GetPeakPressureIndex()
        {
            throw new NotImplementedException();
        }

        public int GetContactAreaPercentage()
        {
            throw new NotImplementedException();
        }
    }
}
