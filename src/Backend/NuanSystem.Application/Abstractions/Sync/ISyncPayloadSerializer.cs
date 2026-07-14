namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncPayloadSerializer
{
    string Serialize<T>(T payload);
    T? Deserialize<T>(string payloadJson);
}
