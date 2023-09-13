namespace Auth0.Settings;

public record Auth0Setting
{
    public required string DomainUrl { get; init; }
    
    public required string PublicKeyUri { get; init; }

    public required string ApiManagementUri { get; set; }
    
    public required string Issuer { get; init; }
    
    public required List<string> Audiences { get; init; }
    
    public required string IssuerSigningKeysStr { get; init; }
    
    public required string ClientId { get; init; }
    
    public required string ClientSecret { get; init; }
}