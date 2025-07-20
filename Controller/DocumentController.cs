using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    [HttpPost("add")]
    [Authorize(Policy = "Document.Add")]
    public IActionResult Add() => Ok("Document added");

    [HttpGet("list")]
    [Authorize(Policy = "Document.List")]
    public IActionResult List() => Ok("List of documents");
}