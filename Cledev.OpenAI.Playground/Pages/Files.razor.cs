using Cledev.OpenAI.V1.Contracts.Files;
using Microsoft.AspNetCore.Components.Forms;

namespace Cledev.OpenAI.Playground.Pages;

public class FilesPage : PageComponentBase
{
    public UploadFileRequest UploadFileRequest { get; set; } = null!;

    public List<FileResponse> Files { get; set; } = new();
    public string? FileIdToDelete { get; set; }

    public bool IsUploading { get; set; }
    public bool IsDeleting { get; set; }

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

    protected async Task OnSubmitAsync()
    {
        IsUploading = true;

        await Task.Delay(1000);

        // TODO: Upload file, close modal, and load files

        IsUploading = false;
    }

    protected async Task LoadFiles()
    {
        IsProcessing = true;
        Files.Clear();

        StateHasChanged();

        var response = await OpenAIClient.ListFiles();
        Error = response?.Error;
        if (response is not null)
        {
            Files.AddRange(response.Data);
        }

        IsProcessing = false;
    }

    protected void SetFileIdToDelete(string fileIdToDelete)
    {
        FileIdToDelete = fileIdToDelete;
    }

    protected async Task DeleteFile()
    {
        IsDeleting = true;
        var deleteFileResponse = await OpenAIClient.DeleteFile(FileIdToDelete!);
        if (deleteFileResponse is not null && deleteFileResponse.Deleted)
        {
            IsDeleting = false;
            // TODO: Close modal
            await LoadFiles();
        }
    }
}
