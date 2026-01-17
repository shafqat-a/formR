using FormR.API.DTOs;
using FormR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormR.API.Controllers;

[ApiController]
[Route("api/v1/controls")]
// [Authorize]  // Temporarily disabled for testing - TODO: Re-enable for production
public class ControlsController : ControllerBase
{
    private readonly FormBuilderContext _context;
    private readonly ILogger<ControlsController> _logger;

    public ControlsController(
        FormBuilderContext context,
        ILogger<ControlsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get the control library (all available control types)
    /// </summary>
    [HttpGet("library")]
    public async Task<ActionResult<IEnumerable<ControlLibraryDto>>> GetLibrary(CancellationToken cancellationToken)
    {
        try
        {
            var library = await _context.ControlLibrary
                .OrderBy(c => c.Category)
                .ThenBy(c => c.Type)
                .ToListAsync(cancellationToken);

            var result = library.Select(c => new ControlLibraryDto
            {
                Type = c.Type,
                Category = c.Category.ToString(),
                Icon = c.Icon,
                ConfigSchema = c.ConfigSchema,
                DefaultProps = c.DefaultProps
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving control library");
            return StatusCode(500, "An error occurred while retrieving the control library");
        }
    }
}
