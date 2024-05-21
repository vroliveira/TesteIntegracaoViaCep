using ExemploTestes.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExemploTestes.Services
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public ViaCepService(HttpClient httpClient)
        {
            _httpClient = ConfiguraRequisicaoViaCep(httpClient);
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = true
            };

        }

        public async Task<Endereco> BuscaCep(string cep)
        {
            using var response = await _httpClient.GetAsync($"{cep}/json", HttpCompletionOption.ResponseHeadersRead);

            string stringResponse = await response.Content.ReadAsStringAsync();

            if (stringResponse.Contains("erro"))
                return null;

            return JsonSerializer.Deserialize<Endereco>(stringResponse, _options);
        }

        private HttpClient ConfiguraRequisicaoViaCep(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://viacep.com.br/ws/");
            return httpClient;
        }
    }
}
