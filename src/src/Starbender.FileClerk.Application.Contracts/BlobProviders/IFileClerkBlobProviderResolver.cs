using System.Threading.Tasks;
using Volo.Abp.BlobStoring;

namespace Starbender.FileClerk.BlobProviders;

public interface IFileClerkBlobProviderResolver
{
    Task<IBlobProvider> ResolveAsync(int providerId, string containerName);
}
