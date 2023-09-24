namespace BusinessLayer.Requests;

public class BlobMetadataRequest
{
    public string BlobName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }
}