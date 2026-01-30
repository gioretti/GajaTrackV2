namespace GajaTrack.Application.Interfaces;

public class ImportValidationException(string entityType, string externalId, string message) 
    : Exception($"[{entityType}] PK: {externalId} - {message}")
{
    public string EntityType { get; } = entityType;
    public string ExternalId { get; } = externalId;
}
