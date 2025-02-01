using System.Text;
using System.Text.Json;
using ProblemSolverAPI.Models.Responses;

namespace ProblemSolverAPI.Services
{
    public class OpenRouterService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenRouterService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenRouter:ApiKey"] 
                      ?? throw new ArgumentNullException(nameof(configuration), "OpenRouter:ApiKey is missing.");
        }
        
        public async Task<ApiResponse<string>> GetPromptFromImageFileAsync(IFormFile image)
        {
            if (image == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Image is null"
                };
            }

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            var imageBytes = ms.ToArray();
            var base64Image = Convert.ToBase64String(imageBytes);

            var payload = new
            {
                model = "openai/gpt-4o-mini",
                messages = new object[]
                {
                    new 
                    {
                        role = "system",
                        content = "You are a prompt generator tasked with converting question images into plain text, preserving the original language of each question. Transcribe only the question content without adding any additional information, avoiding the use of special characters, and excluding question numbers. If you encounter a visual question, extract the textual content from the image and include it in the transcription. If the image contains non-textual information, provide a clear and concise description of the visual elements as part of the question."
                    },
                    new 
                    {
                        role = "user",
                        content = new object[]
                        {
                            new {
                                type = "image_url",
                                image_url = new {
                                    url = $"data:image/jpeg;base64,{base64Image}"
                                }
                            }
                        }
                    }
                },
                temperature = 0.0
            };

            var jsonString = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions")
            {
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        ErrorMessage = $"API Error: {response.StatusCode} - {responseContent}"
                    };
                }
                
                using var doc = JsonDocument.Parse(responseContent);
                var answer = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return new ApiResponse<string>
                {
                    Success = true,
                    Data = answer
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        
        public async Task<ApiResponse<string>> GetTitleFromImageFileAsync(IFormFile image)
        {
            if (image == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Image is null"
                };
            }

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            var imageBytes = ms.ToArray();
            var base64Image = Convert.ToBase64String(imageBytes);

            var payload = new
            {
                model = "openai/gpt-4o-mini",
                messages = new object[]
                {
                    new 
                    {
                        role = "system",
                        content = "You are the title generator. Your task is to write a short and concise title that summarizes the question sent to you. This title should be in the same language as the question."
                    },
                    new 
                    {
                        role = "user",
                        content = new object[]
                        {
                            new {
                                type = "image_url",
                                image_url = new {
                                    url = $"data:image/jpeg;base64,{base64Image}"
                                }
                            }
                        }
                    }
                },
                temperature = 0.0
            };

            var jsonString = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions")
            {
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        ErrorMessage = $"API Error: {response.StatusCode} - {responseContent}"
                    };
                }
                
                using var doc = JsonDocument.Parse(responseContent);
                var answer = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return new ApiResponse<string>
                {
                    Success = true,
                    Data = answer
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        
        public async Task<ApiResponse<string>> GetQuestionAnswerAsync(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    ErrorMessage = "Question cannot be empty."
                };
            }

            var payload = new
            {
                model = "openai/o1-mini",
                messages = new object[]
                {
                    new 
                    {
                        role = "system",
                        content = "You are a teacher. Your goal is to explain and answer the question written to you in the language in which the question is written."
                    },
                    new 
                    {
                        role = "user",
                        content = question
                    }
                },
                temperature = 0.0
            };

            var jsonString = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions")
            {
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        ErrorMessage = $"API Error: {response.StatusCode} - {responseContent}"
                    };
                }

                using var doc = JsonDocument.Parse(responseContent);
                var answer = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return new ApiResponse<string>
                {
                    Success = true,
                    Data = answer
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}