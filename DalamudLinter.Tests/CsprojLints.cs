using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DalamudLinter.Model;
using NUnit.Framework;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DalamudLinter.Tests {
    public class CsprojLints {
        private List<Lint> Lints { get; } = new();

        [SetUp]
        public void Setup() {
            var lints = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build()
                .Deserialize<List<Lint>>(File.ReadAllText(@"..\..\..\..\DalamudLinter\Resources\lints.yaml"));

            this.Lints.Clear();
            this.Lints.AddRange(lints);
        }

        private string[] GetMatchingIds(string code) {
            var csproj = XDocument.Parse(File.ReadAllText($@"..\..\..\Resources\csproj\{code}.csproj"));
            XmlUtil.Preprocess(csproj);

            var lint = this.Lints.First(lint => lint.Code == code.Split('_')[0]);
            return lint.Matches(csproj)
                .Select(elem => elem.Attributes().FirstOrDefault(attr => attr.Name.LocalName == "id"))
                .Where(attr => attr != null)
                .Select(attr => attr!.Value)
                .ToArray();
        }

        private void CheckMatchingIds(string code, IEnumerable expected) {
            var matched = this.GetMatchingIds(code);
            Assert.AreEqual(expected, matched);
        }

        private void CheckMatchingIds(IEnumerable<string> codes, IEnumerable expected) {
            var matched = new List<string>();
            foreach (var code in codes) {
                matched.AddRange(this.GetMatchingIds(code));
            }

            Assert.AreEqual(expected, matched);
        }

        [Test]
        public void D0001() {
            this.CheckMatchingIds(nameof(this.D0001), new[] {
                "D0",
                "D1",
                "D2",
            });
        }

        [Test]
        public void D0002() {
            this.CheckMatchingIds(nameof(this.D0002), new[] {
                "D0",
            });
        }

        [Test]
        public void D0003() {
            this.CheckMatchingIds(nameof(this.D0003), new[] {
                "D2",
                "D3",
            });
        }

        [Test]
        public void D0004() {
            this.CheckMatchingIds(
                new[] {
                    $"{nameof(this.D0004)}_1",
                    $"{nameof(this.D0004)}_2",
                },
                new[] {
                    "D0",
                }
            );
        }

        [Test]
        public void D0005() {
            this.CheckMatchingIds(
                new[] {
                    $"{nameof(this.D0005)}_1",
                    $"{nameof(this.D0005)}_2",
                },
                new[] {
                    "D0",
                }
            );
        }

        [Test]
        public void D0006() {
            this.CheckMatchingIds(
                new[] {
                    $"{nameof(this.D0006)}_1",
                    $"{nameof(this.D0006)}_2",
                    $"{nameof(this.D0006)}_3",
                },
                new[] {
                    "D0",
                    "D1",
                }
            );
        }

        [Test]
        public void D0007() {
            this.CheckMatchingIds(
                new[] {
                    $"{nameof(this.D0007)}_1",
                    $"{nameof(this.D0007)}_2",
                    $"{nameof(this.D0007)}_3",
                },
                new[] {
                    "D0",
                    "D1",
                }
            );
        }

        [Test]
        public void D0008() {
            this.CheckMatchingIds(nameof(this.D0008), new[] {
                "D1",
                "D2",
            });
        }

        [Test]
        public void D0009() {
            this.CheckMatchingIds(nameof(this.D0009), new[] {
                "D2",
            });
        }
    }
}
