using System.Collections.Generic;

namespace RepairService.Shared.Contracts
{
    public static class DictionaryExtensions
    {
        public static object? GetValueOrNull(this Dictionary<string, object?> dict, string key)
        {
            dict.TryGetValue(key, out var value);
            return value;
        }

        public static object? GetValueOrNull(this IDictionary<string, object?> dict, string key)
        {
            dict.TryGetValue(key, out var value);
            return value;
        }
    }
}