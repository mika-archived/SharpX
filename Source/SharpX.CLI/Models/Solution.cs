using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpX.CLI.Models
{
    internal record Solution(string Version, string[] Projects)
    {
        [JsonIgnore]
        public string? BaseDirectory { get; init; }

        public List<Project> GetProjects()
        {
            var projects = new List<Project>();

            foreach (var path in Projects.Select(w => Path.Combine(BaseDirectory ?? "", w)))
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException(null, path);

                var root = Path.GetDirectoryName(path);
                var rawProject = JsonSerializer.Deserialize<Project>(File.ReadAllText(path));
                if (rawProject == null)
                    throw new InvalidOperationException();

                var project = rawProject with
                {
                    BaseDir = Path.GetFullPath(Path.Combine(root ?? "", rawProject.BaseDir!)),
                    Plugins = rawProject.Plugins?.Select(w => Path.Combine(root ?? "", w)).ToArray(),
                    References = rawProject.References?.Select(w => Path.Combine(root ?? "", w)).ToArray(),
                    Out = Path.Combine(root ?? "", rawProject.Out ?? ""),
                };

                try
                {
                    project.Validate();
                    projects.Add(project);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"failed to validate project configuration of {path}: {e.Message}", e);
                }
            }

            return projects;
        }
    }
}