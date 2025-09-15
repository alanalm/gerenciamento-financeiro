using FinanceiroPessoal.Dominio.Comum;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FinanceiroPessoal.Servicos.Api
{
    public abstract class ApiServiceBase
    {
        protected readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILocalStorageService _localStorage;

        protected ApiServiceBase(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;

            // Configuração padrão de serialização/deserialização
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        #region Métodos Públicos Auxiliares

        protected async Task<RespostaApi<T>> GetAsync<T>(string url)
        {
            await AdicionarTokenAsync();
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await TratarResposta<T>(response);
            }
            catch (Exception ex)
            {
                return RespostaApi<T>.ErroResposta($"Erro de conexão: {ex.Message}");
            }
        }

        protected async Task<RespostaApi<T>> PostAsync<T>(string url, object dados)
        {
            await AdicionarTokenAsync();
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(dados, _jsonOptions),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(url, content);
                return await TratarResposta<T>(response);
            }
            catch (Exception ex)
            {
                return RespostaApi<T>.ErroResposta($"Erro de conexão: {ex.Message}");
            }
        }

        protected async Task<RespostaApi<T>> PutAsync<T>(string url, object dados)
        {
            await AdicionarTokenAsync();
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(dados, _jsonOptions),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PutAsync(url, content);
                return await TratarResposta<T>(response);
            }
            catch (Exception ex)
            {
                return RespostaApi<T>.ErroResposta($"Erro de conexão: {ex.Message}");
            }
        }

        protected async Task<RespostaApi<bool>> DeleteAsync(string url)
        {
            await AdicionarTokenAsync();
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                return await TratarResposta<bool>(response);
            }
            catch (Exception ex)
            {
                return RespostaApi<bool>.ErroResposta($"Erro de conexão: {ex.Message}");
            }
        }

        #endregion

        #region Tratamento de Respostas

        protected async Task<RespostaApi<T>> TratarResposta<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var resultado = await response.Content.ReadFromJsonAsync<RespostaApi<T>>(_jsonOptions);

                    if (resultado != null)
                        return resultado;

                    return RespostaApi<T>.ErroResposta("Não foi possível processar a resposta da API.");
                }
                catch (Exception ex)
                {
                    return RespostaApi<T>.ErroResposta($"Erro ao processar resposta: {ex.Message}");
                }
            }

            var mensagemErro = await response.Content.ReadAsStringAsync();
            return RespostaApi<T>.ErroResposta($"Erro da API: {mensagemErro}");
        }

        #endregion

        #region Token

        private async Task AdicionarTokenAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");

            if (!string.IsNullOrEmpty(token))
            {
                // evita duplicar o header
                if (_httpClient.DefaultRequestHeaders.Authorization == null ||
                    _httpClient.DefaultRequestHeaders.Authorization.Parameter != token)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }

        #endregion
    }
}
