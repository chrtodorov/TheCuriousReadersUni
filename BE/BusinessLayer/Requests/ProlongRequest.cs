using BusinessLayer.Attributes;

namespace BusinessLayer.Requests
{
    public class ProlongRequest
    {
        [ValidDate]
        public DateTime ExtendedTo { get; set; }
    }
}