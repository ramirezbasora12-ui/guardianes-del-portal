namespace Guardianes
{
    // Allows the actual storage implementation to be exercised without substituting the Unity API.
    public interface ISaveCodec
    {
        string Encode<T>(T value);
        T Decode<T>(string json);
    }
}
