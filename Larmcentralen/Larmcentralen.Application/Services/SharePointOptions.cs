namespace Larmcentralen.Application.Services;

public class SharePointOptions
{
    public string TenantId { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string SiteHostname { get; set; } = "";
    public string DocumentLibrary { get; set; } = "Shared Documents";
}