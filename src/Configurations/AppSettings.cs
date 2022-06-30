namespace WebApi.KeyCloak.Example.Configurations;

public class AppSettings
{
    public KeyCloakSettings? KeyCloakSettings { get; set; }
}

public class KeyCloakSettings
{
    private Dictionary<string, KeyCloakReference> KeyCloakReference => new();

    public KeyCloakReference? Keycloak =>
        KeyCloakReference.TryGetValue(nameof(Keycloak), out var value) ? value : default;
}

public class KeyCloakReference
{
    public string? OpenIdConnect { get; set; }
    public string? Authority { get; set; }
    public string? AuthorityRealms { get; set; }
    public string? Realms { get; set; }
    public string? ClientId { get; set; }
    public string? MetadataAddress { get; set; }
    public string? ClientSecret { get; set; }
}