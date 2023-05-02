using CSharpFunctionalExtensions;
using Shared.Interfaces.Services;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shared.Services
{
    public class IoService : IIoService
    {
        public async Task<Result<string>> SelectFile(string rootPath = null)
        {
            var openFileDialog = new OpenFileDialog();
            DialogResult result;
            try
            {
                result = openFileDialog.ShowDialog();
            }
            catch (IOException e)
            {
                return Result.Failure<string>(e.Message);
            }

            if(result == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }

            return Result.Failure<string>(null);
        }

        public async Task<Result<string[]>> TryLoadCsvLines(string filePath)
        {
            try
            {
                return await File.ReadAllLinesAsync(filePath);
            }
            catch (FileNotFoundException e)
            {
                return Result.Failure<string[]>(e.Message);
            }
        }

        public async Task<Result> TryWriteLines(string path, string[] lines)
        {
            var directory = Path.GetDirectoryName(path);

            try
            {
                Directory.CreateDirectory(directory);
                await File.WriteAllLinesAsync(path, lines);
            }
            catch (IOException)
            {
                return Result.Failure("An error occurred while trying to write the contents to file.");
            }

            return Result.Success();
        }
    }
}
