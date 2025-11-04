using Microsoft.VisualBasic.FileIO;

namespace software_engineering.Lib
{
    public class Metrics
    {
        protected byte[][] data = [];

        public Metrics(string dataFilePath)
        {
            var parser = new TextFieldParser(dataFilePath);

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                string[]? fields = parser.ReadFields();
                if (fields == null) continue;

                data.Append([.. fields.Select(byte.Parse)]);
            }
        }

        public void Print()
        {
            foreach (byte[] row in data)
            {
                Console.WriteLine(row);
            }
        }
    }
}
