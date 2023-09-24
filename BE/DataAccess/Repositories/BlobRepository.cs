using BusinessLayer.Interfaces;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories;

public class BlobRepository : IBlobRepository
{
    private readonly DataContext _dataContext;
    private readonly ILogger<BlobRepository> _logger;

    public BlobRepository(DataContext dataContext, ILogger<BlobRepository> logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<BlobMetadataResponse> Create(BlobMetadataRequest blobRequest)
    {
        var blobMetadata = blobRequest.ToBlobMetadata();

        try
        {
            await _dataContext.BlobsMetadata.AddAsync(blobMetadata);
            await _dataContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }

        return blobMetadata.ToBlobResponse();
    }

    public async Task Delete(string name)
    {
        try
        {
            var blob = await _dataContext.BlobsMetadata
                .FirstOrDefaultAsync(b => b.BlobName == name);
            if (blob is null)
                throw new KeyNotFoundException("Blob not found!");

            _dataContext.BlobsMetadata.Remove(blob);
            await _dataContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }
    }
}