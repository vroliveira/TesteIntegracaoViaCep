using ExemploTestes.Models;
using System.Threading.Tasks;

namespace ExemploTestes.Services
{
    public interface IViaCepService
    {
        Task<Endereco> BuscaCep(string cep);
    }
}
