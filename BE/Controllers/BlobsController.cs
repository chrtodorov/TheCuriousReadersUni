using BusinessLayer.Enumerations;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace API.Controllers;

[Authorize(Policy = Policies.RequireAdministratorOrLibrarianRole)]
[Route("api/[controller]")]
[ApiController]
public class BlobsController : ControllerBase
{
    private readonly IBlobService _blobService;

    public BlobsController(IBlobService blobService)
    {
        _blobService = blobService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListBlobs()
    {
        return Ok(await _blobService.ListBlobsAsync());
    }

    [Route("upload")]
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] FileModel model)
    {
        return Ok(await _blobService.UploadAsync(model));
    }

    [Route("delete")]
    [HttpDelete]
    public async Task<IActionResult> Delete(string fileName)
    {
        await _blobService.DeleteAsync(fileName);
        return Ok();
    }
}