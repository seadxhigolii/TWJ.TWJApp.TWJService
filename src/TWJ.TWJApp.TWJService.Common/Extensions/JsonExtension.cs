﻿using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Arcenox.Cloud.School.Common.Extensions
{
    public static class JsonExtension
    {
        private static Assembly _currentAssembly = Assembly.GetExecutingAssembly();

        public static void ForAssembly(Assembly assembly) => _currentAssembly = assembly;

        public static T Deserialize<T>(this string value) where T : class
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string Serialize<T>(this T value) where T : class
        {
            return JsonConvert.SerializeObject(value);
        }

        public async static Task<T> JsonFileDeserializeAsync<T>(string fileName)
        {
            var resources = _currentAssembly.GetManifestResourceNames();

            using Stream stream = _currentAssembly.GetManifestResourceStream(resources.First(x => x.Contains(fileName)));

            using StreamReader reader = new(stream);

            return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());
        }
    }
}
