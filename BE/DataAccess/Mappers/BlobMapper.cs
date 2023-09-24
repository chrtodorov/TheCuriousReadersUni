using BusinessLayer.Requests;
using BusinessLayer.Responses;
using DataAccess.Entities;

namespace DataAccess.Mappers;

public static class BlobMapper
{
    public static BlobMetadata ToBlobMetadata(this BlobMetadataRequest blobRequest)
    {
        return new BlobMetadata
        {
            BlobName = blobRequest.BlobName,
            Url = blobRequest.Url,
            ContentType = blobRequest.ContentType,
            CreatedOn = blobRequest.CreatedOn
        };
    }

    public static BlobMetadataResponse ToBlobResponse(this BlobMetadata blobEntity)
    {
        return new BlobMetadataResponse
        {
            Id = blobEntity.Id,
            BlobName = blobEntity.BlobName,
            Url = blobEntity.Url,
            ContentType = blobEntity.ContentType,
            CreatedOn = blobEntity.CreatedOn
        };
    }
}