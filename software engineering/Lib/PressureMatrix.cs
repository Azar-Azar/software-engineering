using software_engineering.Models;
using System.Diagnostics;
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
                origin + new Vector2(1, 1),
                origin + new Vector2(0, 1),
                origin + new Vector2(-1, 1),
                origin + new Vector2(-1, 0),
                origin + new Vector2(-1, -1),
                origin + new Vector2(0, -1),
                origin + new Vector2(1, -1)
            ];

            List<PressurePoint> points = [];

            foreach (Vector2 vec in vectors)
            {
                int x = Convert.ToInt32(vec.X);
                int y = Convert.ToInt32(vec.Y);

                if (y < 0 || y >= Data.Count) continue;
                if (x < 0 || x >= Data[y].Count) continue;

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

        private List<PressurePoint> GetRegionFrom(
            PressurePoint origin,
            int tolerance,
            HashSet<string>? discovered
        ) {
            discovered ??= [];

            List<PressurePoint> zone = [];
            Stack<PressurePoint> frontier = [];

            frontier.Push(origin);
            discovered.Add(origin.Serialise());

            while (frontier.Count > 0)
            {
                PressurePoint node = frontier.Pop();
                if (node.pressure < (origin.pressure - tolerance)) continue;
                if (node.pressure == 0) continue;

                zone.Add(node);

                List<PressurePoint> adjacentPoints = GetAdjacentPoints(node);

                foreach (PressurePoint adjPoint in adjacentPoints)
                {
                    if (discovered.Contains(adjPoint.Serialise())) continue;

                    frontier.Push(adjPoint);
                    discovered.Add(adjPoint.Serialise());
                }
            }

            return zone;
        }

        public List<PressurePoint> Flatten()
        {
            return Data.SelectMany((row, rowIndex) => row.Select(
                (point, column) => new PressurePoint(column, rowIndex, point))
            ).ToList();
        }

        public List<List<PressurePoint>> GetHighPressureRegions(
            int threshold = 200,
            int tolerance = 40
        ) {
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

                    zones.Add(GetRegionFrom(point, tolerance, discovered));
                }
            }

            return zones;
        }

        public int? GetPeakPressureIndex(int minimumPixels = 10)
        {
            List<PressurePoint> flatData = Flatten();

            while (true)
            {
                PressurePoint? highestPoint = flatData.MaxBy(point => point.pressure);
                if (highestPoint == null) return null;

                List<PressurePoint> zone = GetRegionFrom(highestPoint, 40, null);
                flatData.Remove(highestPoint);

                if (zone.Count >= minimumPixels) return highestPoint.pressure;
            }
        }

        public float GetContactAreaPercentage(int lowerThreshold = 64)
        {
            List<PressurePoint> flatData = Flatten();

            int eligibleCount = flatData.Where(
                point => point.pressure >= lowerThreshold
            ).Count();

            return ((float) eligibleCount / (float) flatData.Count) * 100f;
        }

        public void Print()
        {
            foreach (List<short> row in Data)
            {
                string rowString = "";

                foreach (short point in row)
                {
                    rowString += point.ToString() + ", ";
                }

                Debug.WriteLine(rowString);
            }
        }
    }
}
