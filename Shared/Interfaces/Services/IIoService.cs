using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services
{
    public interface IIoService
    {
        Task<Result<string[]>> TryLoadCsvLines(string filePath);

        Task<Result> TryWriteLines(string path, string[] lines);

        Task<Result<string>> SelectFile(string rootPath = null);
    }
}
