using System.Drawing;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Requests;
using BusinessLayer.Responses;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;

namespace BusinessLayer.Services;

public class BlobService : IBlobService
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly IBlobRepository _blobRepository;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient, IBlobRepository blobRepository)
    {
        _blobServiceClient = blobServiceClient;
        _blobContainerClient = blobServiceClient.GetBlobContainerClient("book-covers");
        _blobRepository = blobRepository;
    }

    public async Task<IEnumerable<string>> ListBlobsAsync()
    {
        var items = new List<string>();

        await foreach (var blobItem in _blobContainerClient.GetBlobsAsync()) items.Add(blobItem.Name);

        return items;
    }

    // Upload image
    public async Task<BlobMetadataResponse> UploadAsync(FileModel model)
    {
        if (model.ImageFile?.ContentType is not ("image/jpg" or "image/png" or "image/jpeg"))
            throw new AppException("Content type must be image!");

        await using var ms = new MemoryStream();
        await model.ImageFile!.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        if (imageBytes.Length > 2097152)
            throw new AppException("Max allowed size is 2MB");

        ISupportedImageFormat format = new JpegFormat {Quality = 70};
        var size = new Size(400, 620);
        var resizeLayer = new ResizeLayer(size, ResizeMode.Stretch);
        await using var inStream = new MemoryStream(imageBytes);
        await using var outStream = new MemoryStream();

        using (var imageFactory = new ImageFactory(true))
        {
            // Load, resize, set the format and quality and save an image.
            imageFactory.Load(inStream)
                .Resize(resizeLayer)
                .Format(format)
                .Save(outStream);
        }

        try
        {
            var name = DateTime.Now.ToString("yyyyMMddHHmmss") + model.ImageFile?.FileName;
            var blobClient = _blobContainerClient.GetBlobClient(name);
            await blobClient.UploadAsync(outStream, new BlobHttpHeaders {ContentType = model.ImageFile?.ContentType});

            IDictionary<string, string> metadata =
                new Dictionary<string, string>();
            metadata.Add("url", blobClient.Uri.AbsoluteUri);
            metadata.Add("name", blobClient.Name);

            await blobClient.SetMetadataAsync(metadata);
            var props = await blobClient.GetPropertiesAsync();

            // Store metadata in Db
            var blobRequest = new BlobMetadataRequest
            {
                BlobName = blobClient.Name,
                Url = blobClient.Uri.AbsoluteUri,
                ContentType = props.Value.ContentType,
                CreatedOn = props.Value.CreatedOn.DateTime
            };

            return await _blobRepository.Create(blobRequest);
        }

        catch (RequestFailedException e)
        {
            throw new AppException(e.Message);
        }
    }

    public async Task<byte[]> GetAsync(string imageName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(imageName);
        var downloadContent = await blobClient.DownloadAsync();
        await using var ms = new MemoryStream();
        await downloadContent.Value.Content.CopyToAsync(ms);
        return ms.ToArray();
    }

    public async Task DeleteAsync(string blobName)
    {
        var blobClient = _blobContainerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
        await _blobRepository.Delete(blobName);
    }
}