using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Starbender.FileClerk.Permissions;
using Volo.Abp;

namespace Starbender.FileClerk.Configuration;

[Area(FileClerkRemoteServiceConsts.ModuleName)]
[RemoteService(Name = FileClerkRemoteServiceConsts.RemoteServiceName)]
[Authorize(FileClerkPermissions.ManageFileClerk)]
[Route("api/file-clerk/configuration")]
public sealed class FileClerkConfigurationController :
    FileClerkController,
    IFileClerkConfigurationAppService
{
    private readonly IFileClerkConfigurationAppService _service;

    public FileClerkConfigurationController(
        IFileClerkConfigurationAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<FileClerkConfigurationDto> GetAsync()
    {
        return _service.GetAsync();
    }

    [HttpPut]
    public Task<FileClerkConfigurationDto> UpdateAsync(
        UpdateFileClerkConfigurationDto input)
    {
        return _service.UpdateAsync(input);
    }
}
