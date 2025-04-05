using System.Text.Json;

namespace Project_SophieBakes.Extensions //Added namespace
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson<T>(this Microsoft.AspNetCore.Http.ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObjectFromJson<T>(this Microsoft.AspNetCore.Http.ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}