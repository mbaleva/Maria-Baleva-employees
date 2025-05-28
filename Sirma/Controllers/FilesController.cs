using Sirma.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using Sirma.Services;

namespace Sirma.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IEmployeeOverlapService _service;

    public FilesController(IEmployeeOverlapService service)
    {
        _service = service;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var result = await _service.ProcessCsvAsync(file);
        return Ok(result);
    }
}