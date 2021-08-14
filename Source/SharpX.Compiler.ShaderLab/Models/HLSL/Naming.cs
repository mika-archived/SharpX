using System.Collections.Generic;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    internal static class Naming
    {
        private static readonly Dictionary<string, int> Namings = new();

        public static string GetSafeName(string candidateName)
        {
            if (Namings.ContainsKey(candidateName))
            {
                Namings[candidateName]++;
                return $"{candidateName}_{Namings[candidateName]}";
            }

            Namings.Add(candidateName, 0);
            return candidateName;
        }
    }
}