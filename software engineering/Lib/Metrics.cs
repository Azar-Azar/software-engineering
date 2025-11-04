using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace software_engineering.Lib
{
    public class Metrics
    {
        protected List<List<short>> data = new List<List<short>>();

        public Metrics(string dataFilePath)
        {
            var parser = new TextFieldParser(dataFilePath);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                string[]? fields = parser.ReadFields();
                if (fields == null) continue;


                data.Add(fields.Select(short.Parse).ToList());
            }
        }

        public void Print()
        {
            Debug.WriteLine(data.Count());

            foreach (List<short> row in data)
            {
                
            }
        }
    }
}
