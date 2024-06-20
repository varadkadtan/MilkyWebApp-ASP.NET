using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Milky.Utility
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            var serializedValue = JsonSerializer.SerializeToUtf8Bytes(value, jsonOptions);
            session.Set(key, serializedValue);
        }

        public static T Get<T>(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
                return default;

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(data, jsonOptions);
        }
    }
}
