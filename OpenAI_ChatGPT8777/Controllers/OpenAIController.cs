using OpenAI_ChatGPT8777.Interfaces;
using OpenAI_ChatGPT8777.Service.OpenAI;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace OpenAI_ChatGPT8777.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController : ControllerBase
    {
        private readonly IOpenAITextService _openAIService;

        public OpenAIController(IOpenAITextService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpPost]
        public async Task<IActionResult> CompletePrompt([FromBody] CompletionRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request cannot be null");
                }

                if (request.Messages == null || request.Messages.Count == 0)
                {
                    return BadRequest("Messages cannot be empty");
                }

                var response = await _openAIService.CompletePrompt(request);

                if (response == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get response from OpenAI");
                }

                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, $"OpenAI API error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
