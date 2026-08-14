using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Starbender.FileClerk.Configuration;

public interface IFileClerkConfigurationAppService : IApplicationService
{
    Task<FileClerkConfigurationDto> GetAsync();

    Task<FileClerkConfigurationDto> UpdateAsync(
        UpdateFileClerkConfigurationDto input);
}
