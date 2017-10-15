using System;

namespace Producer
{
    internal static class RoutingKeyGenerator
    {
        private static readonly string[] Importances = {"info", "warning", "error", "critical"};
        private static readonly string[] Kinds = {"rate_limit", "authentication", "unknown"};
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        public static string Generate() => GetRandomValue(Importances) + "." + GetRandomValue(Kinds);

        private static string GetRandomValue(string[] values) => values[_random.Next(values.Length)];
    }
}
