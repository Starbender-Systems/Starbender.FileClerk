using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Starbender.FileClerk.Permissions;
using Volo.Abp;

namespace Starbender.FileClerk.BlobProviders;

[Area(FileClerkRemoteServiceConsts.ModuleName)]
[RemoteService(Name = FileClerkRemoteServiceConsts.RemoteServiceName)]
[Authorize(FileClerkPermissions.ManageFileClerk)]
[Route("api/file-clerk/blob-providers")]
public sealed class FileClerkBlobProviderController :
    FileClerkController,
    IFileClerkBlobProviderAppService
{
    private readonly IFileClerkBlobProviderAppService _service;

    public FileClerkBlobProviderController(
        IFileClerkBlobProviderAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<IReadOnlyList<FileClerkBlobProviderDto>> GetListAsync()
    {
        return _service.GetListAsync();
    }

    [HttpPut("{id:int}/enabled")]
    public Task<FileClerkBlobProviderDto> SetEnabledAsync(
        int id,
        SetFileClerkBlobProviderEnabledDto input)
    {
        return _service.SetEnabledAsync(id, input);
    }
}
