namespace software_engineering.Models
{
    public class Metrics
    {
        public int MetricID { get; set; }//primary key
        public int DataId { get; set; }//Forign keys
        public int PeakPreasure { get; set; }
        public int PreasureArea { get; set; }
        public int AveragePressure { get; set; }
    }
}
