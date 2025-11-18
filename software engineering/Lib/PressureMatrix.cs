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

        private List<PressurePoint> GetAdjacentPoints(PressurePoint originPoint)
        {
            Vector2 origin = new(originPoint.x, originPoint.y);

            List<Vector2> vectors = [
                origin + new Vector2(1, 0),
                origin - new Vector2(1, 0),
                origin + new Vector2(0, 1),
                origin - new Vector2(0, 1)
            ];

            List<PressurePoint> points = [];

            foreach (Vector2 vec in vectors)
            {
                int x = Convert.ToInt32(vec.X);
                int y = Convert.ToInt32(vec.Y);

                try
                {
                    points.Add(new PressurePoint(x, y, Data[y][x]));
                }
                catch
                {
                    continue;
                }
            }

            return points;
        }

        public List<List<PressurePoint>> GetHighPressureRegions(int tolerance = 20)
        {
            float threshold = 256 * (1 - tolerance / 100);

            List<List<PressurePoint>> zones = [];

            HashSet<string> discovered = [];

            for (int rowI = 0; rowI < Data.Count; rowI++)
            {
                List<short> row = Data[rowI];

                for (int columnI = 0; columnI < row.Count; columnI++)
                {
                    PressurePoint point = new(columnI, rowI, row[columnI]);
                    if (point.pressure < threshold) continue;
                    if (discovered.Contains(point.Serialise())) continue;

                    List<PressurePoint> zone = [];
                    Stack<PressurePoint> frontier = [];

                    frontier.Push(point);
                    discovered.Add(point.Serialise());

                    while (frontier.Count > 0)
                    {
                        PressurePoint node = frontier.Pop();
                        if (node.pressure < threshold) continue;

                        zone.Add(node);

                        List<PressurePoint> adjacentPoints = GetAdjacentPoints(node);

                        foreach (PressurePoint adjPoint in adjacentPoints)
                        {
                            if (discovered.Contains(adjPoint.Serialise())) continue;
                            frontier.Push(adjPoint);
                        }
                    }

                    zones.Add(zone);
                }
            }

            return zones;
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
