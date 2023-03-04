using Cledev.OpenAI.V1.Contracts;
using Cledev.OpenAI.V1.Contracts.Files;
using Microsoft.AspNetCore.Components.Forms;

namespace Cledev.OpenAI.Playground.Pages;

public class FilesPage : PageComponentBase
{
    public UploadFileRequest UploadFileRequest { get; set; } = null!;

    public List<FileResponse> Files { get; set; } = new();
    public string? FileIdToDelete { get; set; }

    protected bool IsUploading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        UploadFileRequest = new UploadFileRequest
        {
            File = new byte[1],
            FileName = "Something",
            Purpose = "fine-tune"
        };

        await LoadFiles();
    }

    public async Task OnInputFileChange(InputFileChangeEventArgs e)
    {
        UploadFileRequest.File = await GetFileBytes(e);
        UploadFileRequest.FileName = e.File.Name;
    }

    private async Task<byte[]> GetFileBytes(InputFileChangeEventArgs e)
    {
        using var memoryStream = new MemoryStream();

        try
        {
            await e.File.OpenReadStream(maxAllowedSize: 4000000).CopyToAsync(memoryStream);
        }
        catch (Exception exception)
        {
            Error = new Error
            {
                Message = exception.Message
            };
        }

        return memoryStream.ToArray();
    }

    protected async Task OnSubmitAsync()
    {

    }

    protected async Task LoadFiles()
    {
        IsLoading = true;
        Files.Clear();

        StateHasChanged();

        var response = await OpenAIClient.ListFiles();
        Error = response?.Error;
        if (response is not null)
        {
            Files.AddRange(response.Data);
        }

        IsLoading = false;
    }

    protected void SetFileIdToDelete(string fileIdToDelete)
    {
        FileIdToDelete = fileIdToDelete;
    }

    protected async Task DeleteFile()
    {
        var deleteFileResponse = await OpenAIClient.DeleteFile(FileIdToDelete!);
        if (deleteFileResponse is not null && deleteFileResponse.Deleted)
        {
            await LoadFiles();
        }
    }
}
