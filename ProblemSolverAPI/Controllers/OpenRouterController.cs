using Microsoft.AspNetCore.Mvc;
using ProblemSolverAPI.Services;
using System.Text.Json;

namespace ProblemSolverAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenRouterController(OpenRouterService openRouterService)
        : ControllerBase
    {
        [HttpPost("prompt")]
        public async Task<IActionResult> GetPrompt([FromForm] IFormFile image)
        {
            var result = await openRouterService.GetPromptFromImageFileAsync(image);
            if (result.Success)
                return Ok(result.Data);
            return StatusCode(500, result.ErrorMessage);
        }
        
        [HttpPost("title")]
        public async Task<IActionResult> GetTitle([FromForm] IFormFile image)
        {
            var result = await openRouterService.GetTitleFromImageFileAsync(image);
            if (result.Success)
                return Ok(result.Data);
            return StatusCode(500, result.ErrorMessage);
        }
        
        [HttpPost("question")]
        public async Task<IActionResult> GetQuestionAnswer([FromBody] JsonElement request)
        {
            if (!request.TryGetProperty("question", out JsonElement questionProperty))
            {
                return BadRequest("Missing 'question' property in the request body.");
            }
            
            var question = questionProperty.GetString();
            var result = await openRouterService.GetQuestionAnswerAsync(question);
            
            if (result.Success)
                return Ok(result.Data);
            return StatusCode(500, result.ErrorMessage);
        }
    }
}