using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class FileUploadController : ControllerBase
{
    private readonly string _uploadPath = "uploads";

    public FileUploadController()
    {
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var filePath = Path.Combine(_uploadPath, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { fileName = file.FileName, filePath });
    }

    [HttpGet("download/{fileName}")]
    public IActionResult DownloadFile(string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found");

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }

    [HttpGet("list")]
    public IActionResult GetFileList()
    {
        var files = Directory.GetFiles(_uploadPath)
            .Select(Path.GetFileName)
            .ToList();

        return Ok(files);
    }
}
