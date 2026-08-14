using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Starbender.FileClerk.BlobProviders;

public interface IFileClerkBlobProviderAppService : IApplicationService
{
    Task<IReadOnlyList<FileClerkBlobProviderDto>> GetListAsync();

    Task<FileClerkBlobProviderDto> SetEnabledAsync(
        int id,
        SetFileClerkBlobProviderEnabledDto input);
}
