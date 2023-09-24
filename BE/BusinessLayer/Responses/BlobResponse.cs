namespace BusinessLayer.Responses;

public class BlobMetadataResponse
{
    public Guid? Id { get; set; }

    public string BlobName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }
}