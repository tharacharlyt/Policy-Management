using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyManagement.Models.DTOs;

namespace PolicyManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PolicyController : ControllerBase
{
    private static readonly List<CreatePolicyDto> _policies = new();

    [HttpPost]
    [Authorize(Policy = "Policy.Add")]
    public IActionResult AddPolicy([FromBody] CreatePolicyDto dto)
    {
        _policies.Add(dto);
        return Ok(new { message = "Policy added successfully", policy = dto });
    }

    [HttpGet]
    [Authorize(Policy = "Policy.List")]
    public IActionResult GetPolicies()
    {
        return Ok(_policies);
    }

    [HttpDelete("{policyId}")]
    [Authorize(Policy = "Policy.Delete")]
    public IActionResult DeletePolicy(string policyId)
    {
        var policy = _policies.FirstOrDefault(p => p.PolicyId == policyId);
        if (policy == null) return NotFound("Policy not found");

        _policies.Remove(policy);
        return Ok(new { message = "Policy deleted", policyId });
    }

    [HttpPut("{policyId}")]
    [Authorize(Policy = "Policy.Edit")]
    public IActionResult UpdatePolicy(string policyId, [FromBody] CreatePolicyDto updatedPolicy)
    {
        var existing = _policies.FirstOrDefault(p => p.PolicyId == policyId);
        if (existing == null) return NotFound("Policy not found");

        existing.PolicyName = updatedPolicy.PolicyName;
        existing.Premium = updatedPolicy.Premium;
        existing.Coverage = updatedPolicy.Coverage;
        existing.ValidityInYears = updatedPolicy.ValidityInYears;

        return Ok(new { message = "Policy updated", policy = existing });
    }
}
