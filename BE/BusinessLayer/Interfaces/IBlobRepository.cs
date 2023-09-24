using BusinessLayer.Requests;
using BusinessLayer.Responses;

namespace BusinessLayer.Interfaces;

public interface IBlobRepository
{
    Task<BlobMetadataResponse> Create(BlobMetadataRequest blobRequest);
    Task Delete(string name);
}