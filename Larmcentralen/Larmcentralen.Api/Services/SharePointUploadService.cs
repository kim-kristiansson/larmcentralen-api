using Azure.Identity;
using Larmcentralen.Application.Services;
using Microsoft.Graph;
using Microsoft.Extensions.Options;

namespace Larmcentralen.Api.Services;

public class SharePointUploadService(IOptions<SharePointOptions> options)
{
    private readonly SharePointOptions _options = options.Value;

    public async Task<string?> UploadAsync(byte[] fileBytes, string fileName, string? folderPath = null)
    {
        var credential = new ClientSecretCredential(
            _options.TenantId,
            _options.ClientId,
            _options.ClientSecret);

        var client = new GraphServiceClient(credential);

        // Get the site
        var site = await client.Sites[$"{_options.SiteHostname}:"]
            .GetAsync();

        if (site?.Id is null) return null;

        // Get the default drive for the site
        var drive = await client.Sites[site.Id]
            .Drive
            .GetAsync();

        if (drive?.Id is null) return null;

        // Build the path
        var path = string.IsNullOrWhiteSpace(folderPath)
            ? fileName
            : $"{folderPath}/{fileName}";

        // Upload using the drive ID
        using var stream = new MemoryStream(fileBytes);
        var driveItem = await client.Drives[drive.Id]
            .Root
            .ItemWithPath(path)
            .Content
            .PutAsync(stream);

        return driveItem?.WebUrl;
    }
}