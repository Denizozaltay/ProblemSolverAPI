using Microsoft.AspNetCore.Mvc;
using ProblemSolverAPI.Services;
using System.Text.Json;

namespace ProblemSolverAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenRouterController : ControllerBase
    {
        private readonly OpenRouterService _openRouterService;

        public OpenRouterController(OpenRouterService openRouterService)
        {
            _openRouterService = openRouterService;
        }

        [HttpPost("prompt")]
        public async Task<IActionResult> GetPrompt([FromForm] IFormFile image)
        {
            var result = await _openRouterService.GetPromptFromImageFile(image);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return StatusCode(500, result.ErrorMessage);
        }
        
        [HttpPost("title")]
        public async Task<IActionResult> GetTitle([FromForm] IFormFile image)
        {
            var result = await _openRouterService.GetTitleFromImageFile(image);
            if (result.Success)
            {
                return Ok(result.Data);
            }
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
            
            if (string.IsNullOrWhiteSpace(question))
            {
                return BadRequest("The 'question' property cannot be null or empty.");
            }
    
            var result = await _openRouterService.GetQuestionAnswer(question);
    
            if (result.Success)
            {
                return Ok(result.Data);
            }
    
            return StatusCode(500, result.ErrorMessage);
        }
    }
}