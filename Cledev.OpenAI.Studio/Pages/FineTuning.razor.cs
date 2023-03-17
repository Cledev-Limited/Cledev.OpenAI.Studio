using Cledev.OpenAI.V1.Contracts.FineTunes;

namespace Cledev.OpenAI.Studio.Pages;

public class FineTuningPage : PageComponentBase
{
    //public UploadFileRequest UploadFileRequest { get; set; } = null!;

    public List<FineTuneResponse> FineTunes { get; set; } = new();
    //public string? FileIdToDelete { get; set; }

    //public bool IsUploading { get; set; }
    //protected Error? UploadError { get; set; }

    //public bool IsDeleting { get; set; }
    //protected Error? DeleteError { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //UploadFileRequest = new UploadFileRequest
        //{
        //    File = new byte[1],
        //    FileName = "Something",
        //    Purpose = "fine-tune"
        //};

        await LoadFineTunes();
    }

    //public async Task OnInputFileChange(InputFileChangeEventArgs e)
    //{
    //    UploadFileRequest.File = await GetFileBytes(e);
    //    UploadFileRequest.FileName = e.File.Name;
    //}

    //protected async Task OnSubmitAsync()
    //{
    //    IsUploading = true;

    //    var response = await OpenAIClient.UploadFile(UploadFileRequest);
    //    UploadError = response?.Error;
    //    if (UploadError is null)
    //    {
    //        await JsRuntime.InvokeVoidAsync("toggleModal", "uploadModal");
    //        await LoadFineTunes();
    //    }

    //    IsUploading = false;
    //}

    protected async Task LoadFineTunes()
    {
        IsProcessing = true;
        FineTunes.Clear();

        StateHasChanged();

        var response = await OpenAIClient.ListFineTunes();
        Error = response?.Error;
        if (response is not null)
        {
            FineTunes.AddRange(response.Data);
        }

        IsProcessing = false;
    }

    //protected void SetFileIdToDelete(string fileIdToDelete)
    //{
    //    FileIdToDelete = fileIdToDelete;
    //}

    //protected async Task DeleteFile()
    //{
    //    IsDeleting = true;

    //    var deleteFileResponse = await OpenAIClient.DeleteFile(FileIdToDelete!);
    //    DeleteError = deleteFileResponse?.Error;
    //    if (DeleteError is null)
    //    {
    //        await JsRuntime.InvokeVoidAsync("toggleModal", "deleteModal");
    //        await LoadFineTunes();
    //    }

    //    IsDeleting = false;
    //}
}
