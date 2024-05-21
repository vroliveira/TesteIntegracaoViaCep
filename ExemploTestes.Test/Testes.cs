using ExemploTestes.Models;
using ExemploTestes.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExemploTestes.Test
{
    [Collection("TestCollection")]
    public class Testes : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpClientService;
        private HttpClient _httpClientServiceUnit;
        private readonly JsonSerializerOptions _options;
        private readonly ViaCepService _viaCepService;
        private ViaCepService _viaCepServiceUnit;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public Testes(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
            _httpClientService = new HttpClient();
            _viaCepService = new ViaCepService(_httpClientService);
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = true
            };
        }

        [Fact]
        public async Task TesteViaCEP_Integracao_Erro()
        {
            var response = await _httpClient.GetAsync("/api/endereco/09700000");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task TesteViaCEP_Integracao_Ok()
        {
            var response = await _httpClient.GetAsync("/api/endereco/09725160");

            var obj = JsonSerializer.Deserialize<Endereco>(await response.Content.ReadAsStringAsync(), _options);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(obj);
            Assert.True(obj is Endereco);
        }

        [Theory]
        [InlineData("09725160")]
        public async Task TesteViaCEP_Integracao_Ok_Varias_Opcoes_NaLinha(string cep)
        {
            var response = await _httpClient.GetAsync($"/api/endereco/{cep}");

            var obj = JsonSerializer.Deserialize<Endereco>(await response.Content.ReadAsStringAsync(), _options);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(obj);
            Assert.True(obj is Endereco);
        }

        [Fact]
        public async Task TesteViaCEP_Integracao_Service_Erro()
        {
            var endereco = await _viaCepService.BuscaCep("09700000");

            Assert.Null(endereco);
        }

        [Fact]
        public async Task TesteViaCEP_Integracao_Service_Ok()
        {
            var endereco = await _viaCepService.BuscaCep("09725160");

            Assert.NotNull(endereco);
            Assert.True(endereco is Endereco);
        }

        [Fact]
        public async Task TesteUnitario_Service_Ok()
        {
            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new Endereco() {
                Bairro = "Bairro Teste",
                Cep = "09700000",
                Complemento = "Complemento",
                Ddd = "61",
                Gia = "",
                Ibge = "",
                Localidade = "Localidade",
                Logradouro = "Logradouro",
                Siafi = "",
                Uf = "DF"
                }))
            });

            _httpClientServiceUnit = new HttpClient(_httpMessageHandlerMock.Object);
            _viaCepServiceUnit = new ViaCepService(_httpClientServiceUnit);

            var endereco = await _viaCepServiceUnit.BuscaCep("09725160");

            Assert.NotNull(endereco);
            Assert.True(endereco is Endereco);
        }
    }
}
