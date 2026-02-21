#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Effects
{
    public static class ParticlesEnumGenerator
    {
        private const string EnumTemplate = @"namespace _Project.Scripts.Effects
{{
    public enum ParticleType
    {{
        None = 0,
{0}
    }}
}}";

        [MenuItem("Tools/Particles/Generate Particles Enum")]
        public static void GenerateAll()
        {
            var guids = AssetDatabase.FindAssets("t:ParticlesConfig");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<ParticlesConfig>(path);

                if (config != null)
                {
                    Generate(config);
                }
            }

            Debug.Log("All ParticlesConfig enums generated!");
        }

        public static void Generate(ParticlesConfig config)
        {
            if (config == null)
            {
                Debug.LogError("ParticlesConfig is null!");
                return;
            }

            var allParticleNames = new HashSet<string>();
            var stringBuilder = new StringBuilder();

            foreach (var particle in config.Particles)
            {
                if (!string.IsNullOrEmpty(particle.ParticleName))
                {
                    allParticleNames.Add(particle.ParticleName);
                }
            }

            bool isFirst = true;
            int index = 1;
            foreach (var particleName in allParticleNames)
            {
                var enumName = SanitizeIdentifier(particleName);

                if (!isFirst)
                {
                    stringBuilder.AppendLine();
                }

                stringBuilder.Append($"        {enumName} = {index},");
                isFirst = false;
                index++;
            }

            var content = string.Format(EnumTemplate, stringBuilder);
            var path = Path.Combine(Application.dataPath, "_Project/Scripts/Effects/ParticleType.cs");

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content);

            AssetDatabase.Refresh();
            Debug.Log($"Particles enum generated with {allParticleNames.Count} entries");
        }

        private static string SanitizeIdentifier(string id)
        {
            if (string.IsNullOrEmpty(id))
                return "InvalidSound";

            var result = new StringBuilder();
            bool lastWasUnderscore = false;

            foreach (char c in id)
            {
                if (char.IsLetterOrDigit(c))
                {
                    result.Append(c);
                    lastWasUnderscore = false;
                }
                else if (c == '_' || c == ' ')
                {
                    if (!lastWasUnderscore && result.Length > 0)
                    {
                        result.Append('_');
                        lastWasUnderscore = true;
                    }
                }
            }

            if (result.Length > 0 && result[result.Length - 1] == '_')
            {
                result.Length--;
            }

            var sanitized = result.ToString();

            if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))
            {
                sanitized = "_" + sanitized;
            }

            if (string.IsNullOrEmpty(sanitized))
            {
                sanitized = "InvalidSound";
            }

            return sanitized;
        }
    }
}
#endif