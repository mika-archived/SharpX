using System.Text.Json;

namespace SharpX.Compiler.Extensions
{
    public static class JsonElementExtensions
    {
        public static T? ToObject<T>(this JsonElement obj)
        {
            try
            {
                var json = obj.GetRawText();
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }
    }
}