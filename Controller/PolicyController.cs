using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/[controller]")]
public class PolicyController : ControllerBase
{
    [HttpPost("add")]
    [Authorize(Policy = "Policy.Add")]
    public IActionResult Add() => Ok("Policy added");

    [HttpPut("edit")]
    [Authorize(Policy = "Policy.Edit")]
    public IActionResult Edit() => Ok("Policy edited");

    [HttpDelete("delete")]
    [Authorize(Policy = "Policy.Delete")]
    public IActionResult Delete() => Ok("Policy deleted");

    [HttpGet("list")]
    [Authorize(Policy = "Policy.List")]
    public IActionResult List() => Ok("List of policies");
}