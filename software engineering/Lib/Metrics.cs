using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Globalization;

namespace software_engineering.Lib
{
    public class Metrics
    {
        public List<PressureMatrix> matrices = new List<PressureMatrix>();

        // Assumes file name convention as provided by Sample data:
        // <User ID>_<Timestamp yyyyMMdd>.csv
        public Metrics(string dataFilePath, int matrixHeight = 32)
        {
            List<List<short>> rows = new List<List<short>>();

            TextFieldParser parser = new TextFieldParser(dataFilePath);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                string[]? fields = parser.ReadFields();
                if (fields == null) continue;

                rows.Add(fields.Select(short.Parse).ToList());

                if (rows.Count == matrixHeight)
                {
                    matrices.Add(new PressureMatrix(rows, DateTime.Now));
                    rows.Clear();
                }
            }

            // Interpolate matrix timestamps throughout day
            string? dateString = Path.GetFileNameWithoutExtension(dataFilePath)
                .Split("_").ElementAtOrDefault(1);
            if (dateString == null) return;

            int secondsInDay = 24 * 60 * 60;

            try
            {
                DateTime date = DateTime.ParseExact(
                    dateString, "yyyyMMdd", CultureInfo.InvariantCulture
                );

                for (int i = 0; i < matrices.Count; i++)
                {
                    matrices[i].Timestamp = date.AddSeconds((secondsInDay / matrices.Count) * i);
                }
            }
            catch
            {
                return;
            }
        }

        public void Print()
        {
            foreach (PressureMatrix matrix in matrices)
            {
                Debug.WriteLine(matrix.Timestamp.ToString());
            }
        }
    }
}
