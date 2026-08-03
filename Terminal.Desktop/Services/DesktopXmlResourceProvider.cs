using System.Threading;
using System.Threading.Tasks;
using Terminal.Core.Interfaces;

namespace Terminal.Desktop.Services;

public class DesktopXmlResourceProvider : IXmlResourceProvider
{
    public async Task<string> LoadXmlContentAsync(string fileName, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<bool> XmlFileExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}