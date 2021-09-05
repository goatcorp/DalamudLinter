using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DalamudLinter.Model;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Wmhelp.XPath2;
using Wmhelp.XPath2.Value;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DalamudLinter {
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class DalamudLinter : Task {
        [Required]
        public string ProjectDir { get; set; } = null!;

        [Required]
        public string ProjectFilePath { get; set; } = null!;

        public override bool Execute() {
            var lints = GetLints();
            var csproj = this.GetCsproj();
            XmlUtil.Preprocess(csproj);

            var ignored = string.Join(
                    ";",
                    csproj.XPath2SelectValues("/Project/PropertyGroup/IgnoredLints/text()")
                        .Cast<UntypedAtomic>()
                        .Select(atomic => atomic.Value)
                )
                .Split(new[] { ";", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ig => ig.Trim())
                .ToArray();

            foreach (var lint in lints) {
                if (ignored.Contains(lint.Code) || lint.Csproj == null) {
                    continue;
                }

                var offenders = csproj.XPath2SelectElements(lint.Csproj);

                string FormatLint(XElement? offender) {
                    var sb = new StringBuilder();

                    sb.AppendLine($"{lint.Severity.ToString().ToUpperInvariant()}[{lint.Code}]: {lint.Name}");

                    foreach (var line in lint.Description.Lines()) {
                        sb.AppendLine($"  {line}");
                    }

                    sb.AppendLine(" ");

                    if (offender != null && lint.ShowContext) {
                        foreach (var line in offender.ToString().Lines()) {
                            sb.AppendLine($"  {line}");
                        }

                        sb.AppendLine(" ");
                    }

                    sb.AppendLine("  Fix:");

                    foreach (var line in lint.Fix.Lines()) {
                        sb.AppendLine($"  {line}");
                    }

                    return sb.ToString();
                }

                var formatted = new List<string>();
                if (lint.ShowContext) {
                    formatted.AddRange(offenders.Select(FormatLint));
                } else if (offenders.Any()) {
                    formatted.Add(FormatLint(null));
                }

                foreach (var formattedLint in formatted) {
                    Action<string> method = lint.Severity switch {
                        // Severity.Hint => s => this.Log.LogMessage(MessageImportance.Normal, s),
                        Severity.Warn => s => this.Log.LogWarning(s),
                        Severity.Error => s => this.Log.LogError(s),
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    method(formattedLint);
                }
            }

            return true;
        }

        private static List<Lint> GetLints() {
            return new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build()
                .Deserialize<List<Lint>>(Resourcer.Resource.AsString("Resources/lints.yaml"));
        }

        private XDocument GetCsproj() {
            using var csproj = File.Open(this.ProjectFilePath, FileMode.Open);
            return XDocument.Load(csproj);
        }
    }
}
