using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.API
{
    
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
    }






    public class SharedDocumentDto
    {
        public int ShareId { get; set; }
        public string Permission { get; set; }
        public DocumentDto Document { get; set; } 
                                                  
        public string Title { get; set; }
    }

    public class DocumentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Version { get; set; }
    }


    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://gsgpi.barred-sunfish.ts.net";

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<ApiResult> LoginAsync(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/auth/login", new { email, password });

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (data != null)
                    {
                        await SecureStorage.Default.SetAsync("jwt_token", data.Token);
                    }
                    return new ApiResult { IsSuccess = true, Message = "Login successful!" };
                }

                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new ApiResult { IsSuccess = false, Message = "Credentiale gresite (Email sau Parola incorecta)." },
                    HttpStatusCode.BadRequest => new ApiResult { IsSuccess = false, Message = "Datele trimise sunt invalide." },
                    _ => new ApiResult { IsSuccess = false, Message = $"Eroare server: {response.StatusCode}" }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult { IsSuccess = false, Message = "Nu s-a putut contacta serverul: " + ex.Message };
            }
        }

        public async Task<ApiResult> RegisterAsync(string username, string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/auth/register", new { username, email, password });

                if (response.IsSuccessStatusCode)
                    return new ApiResult { IsSuccess = true, Message = "Cont creat cu succes!" };

                return response.StatusCode switch
                {
                    HttpStatusCode.Conflict => new ApiResult { IsSuccess = false, Message = "Email-ul sau Username-ul este deja folosit." },
                    HttpStatusCode.BadRequest => new ApiResult { IsSuccess = false, Message = "Date invalide (ex: parola prea scurta)." },
                    _ => new ApiResult { IsSuccess = false, Message = $"Eroare la inregistrare: {response.StatusCode}" }
                };
            }
            catch (Exception ex)
            {
                return new ApiResult { IsSuccess = false, Message = "Eroare de retea: " + ex.Message };
            }
        }


        private async Task SetAuthHeader()
        {
            var token = await SecureStorage.Default.GetAsync("jwt_token");

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<SharedDocumentDto>> GetSharedDocsAsync()
        {
            try
            {
                await SetAuthHeader();
                return await _httpClient.GetFromJsonAsync<List<SharedDocumentDto>>("/shared-docs");
            }
            catch { return new List<SharedDocumentDto>(); }
        }

        public async Task<SharedDocumentDto> CreateDocumentAsync(string title, string content)
        {
            try
            {
                await SetAuthHeader();
                var response = await _httpClient.PostAsJsonAsync("/shared-docs", new { title, content });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SharedDocumentDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> UpdateDocumentAsync(int shareId, string content, int version)
        {
            await SetAuthHeader();
            var response = await _httpClient.PutAsJsonAsync($"/shared-docs/{shareId}", new { content, version });
            return response.IsSuccessStatusCode;
        }

        public async Task<ApiResult> DeleteDocumentAsync(int shareId)
        {
            await SetAuthHeader();
            var response = await _httpClient.DeleteAsync($"/shared-docs/{shareId}");

            if (response.IsSuccessStatusCode)
                return new ApiResult { IsSuccess = true, Message = "Sters cu succes." };

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return new ApiResult { IsSuccess = false, Message = "Doar proprietarul poate sterge acest document!" };

            return new ApiResult { IsSuccess = false, Message = "Eroare la stergere." };
        }

        public async Task<SharedDocumentDto> GetSharedDocByIdAsync(int shareId)
        {
            await SetAuthHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<SharedDocumentDto>($"/shared-docs/{shareId}");
            }
            catch { return null; }
        }

       
        public async Task<ApiResult> ShareDocumentAsync(int shareId, int targetUserId, string permission)
        {
            try
            {
                await SetAuthHeader();
                // Endpoint: POST /shared-docs/{shareId}/shares
                var response = await _httpClient.PostAsJsonAsync($"/shared-docs/{shareId}/shares", new
                {
                    userId = targetUserId,
                    permission = permission 
                });

                if (response.IsSuccessStatusCode)
                    return new ApiResult { IsSuccess = true, Message = "Document partajat cu succes!" };

                if (response.StatusCode == HttpStatusCode.Conflict)
                    return new ApiResult { IsSuccess = false, Message = "Documentul este deja partajat cu acest utilizator." };

                return new ApiResult { IsSuccess = false, Message = "Eroare la partajare: " + response.StatusCode };
            }
            catch (Exception ex)
            {
                return new ApiResult { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<string> GetSharesAsync(int shareId)
        {
            await SetAuthHeader();
            var response = await _httpClient.GetAsync($"/shared-docs/{shareId}/shares");
            return await response.Content.ReadAsStringAsync();
        }


        public async Task<List<ShareDetailDto>> GetDocumentSharesAsync(int shareId)
        {
            try
            {
                await SetAuthHeader();
                return await _httpClient.GetFromJsonAsync<List<ShareDetailDto>>($"/shared-docs/{shareId}/shares");
            }
            catch { return null; }
        }

        
        public async Task<bool> RevokeShareAsync(int shareId, int userId)
        {
            try
            {
                await SetAuthHeader();
      
                var response = await _httpClient.DeleteAsync($"/shared-docs/{shareId}/shares/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

      
        public class ShareDetailDto
        {
            public int UserId { get; set; }
            public string Permission { get; set; }
        }
    }
}