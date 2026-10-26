using UnityEngine;
namespace Guardianes
{
    public sealed class UnitySaveCodec : ISaveCodec
    {
        public string Encode<T>(T value) { return JsonUtility.ToJson(value); }
        public T Decode<T>(string json) { return JsonUtility.FromJson<T>(json); }
    }
    public static class SaveLocation
    {
        // Isolated test runner directory; null in normal game sessions.
        internal static string TestDirectory;
        public static string Directory { get { return TestDirectory ?? Application.persistentDataPath; } }
    }
}
