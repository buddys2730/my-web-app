using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class TrainingDataController : ControllerBase
{
    private readonly string _trainingDataPath = "wwwroot/training_data";

    public TrainingDataController()
    {
        if (!Directory.Exists(_trainingDataPath))
        {
            Directory.CreateDirectory(_trainingDataPath);
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadTrainingData(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var filePath = Path.Combine(_trainingDataPath, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { fileName = file.FileName, filePath });
    }

    [HttpGet("download/{fileName}")]
    public IActionResult DownloadTrainingData(string fileName)
    {
        var filePath = Path.Combine(_trainingDataPath, fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found");

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }

    [HttpGet("list")]
    public IActionResult GetTrainingDataList()
    {
        var files = Directory.GetFiles(_trainingDataPath)
            .Select(Path.GetFileName)
            .ToList();

        return Ok(files);
    }
}
