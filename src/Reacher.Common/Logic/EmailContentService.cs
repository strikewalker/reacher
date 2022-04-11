using Azure.Storage.Blobs;
using System.Text;

namespace Reacher.Common.Logic;

public interface IEmailContentService
{
    Task<Stream> GetInboundEmail(Guid emailId);
    Task SaveInboundEmail(Guid emailId, Stream content);
    Task SaveOutboundEmail(Guid emailId, string body);
}

public class EmailContentService : IEmailContentService
{
    private readonly BlobContainerClient _inboundClient;
    private readonly BlobContainerClient _outboundClient;

    public EmailContentService(BlobServiceClient blobServiceClient)
    {
        _inboundClient = blobServiceClient.GetBlobContainerClient("inbound");
        _outboundClient = blobServiceClient.GetBlobContainerClient("outbound");
    }

    public Task SaveInboundEmail(Guid emailId, Stream content)
    {
        return _inboundClient.UploadBlobAsync(emailId.ToString(), content);
    }

    public async Task<Stream> GetInboundEmail(Guid emailId)
    {
        var blobClient = _inboundClient.GetBlobClient(emailId.ToString());
        var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);
        stream.Position = 0;
        return stream;
    }
    public Task SaveOutboundEmail(Guid emailId, string body)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(body ?? string.Empty));
        return _outboundClient.UploadBlobAsync(emailId.ToString(), stream);
    }
}
