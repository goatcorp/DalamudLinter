using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Wmhelp.XPath2;

namespace DalamudLinter {
    public static class XmlUtil {
                private static readonly Regex ReferenceRegex = new(@"\$\((\w+)\)", RegexOptions.Compiled);

        public static void Preprocess(XNode xml) {
            void EnsureElement(XElement reference, string name) {
                var attr = reference.Attribute(name);
                if (attr == null) {
                    return;
                }

                reference.SetElementValue(name, attr.Value);
                reference.SetAttributeValue(name, null);
            }

            void EnsureAttribute(XElement reference, string name) {
                var elem = reference.Element(name);
                if (elem == null) {
                    return;
                }

                reference.SetAttributeValue(name, elem.Value);
                reference.SetElementValue(name, null);
            }

            // standardise references
            foreach (var reference in xml.XPath2SelectElements("/Project/ItemGroup/Reference")) {
                // make sure include is an attribute
                EnsureAttribute(reference, "Include");

                // make sure hintpath is an element
                EnsureElement(reference, "HintPath");

                // make sure private is an element
                EnsureElement(reference, "Private");
            }

            // collect all properties and their values
            var allProps = xml
                .XPath2SelectElements("/Project/PropertyGroup/node()")
                .ToDictionary(elem => elem.Name, elem => elem.Value);

            // replace any reference to a local property with the property's value
            bool anyChanged;
            do {
                anyChanged = false;

                foreach (var withRef in xml.XPath2SelectElements("/Project//node()[contains(text(), '$(')]")) {
                    var compiledValue = withRef.Value;

                    foreach (Match reference in ReferenceRegex.Matches(withRef.Value)) {
                        var name = reference.Groups[1].Value;
                        if (allProps.TryGetValue(name, out var value)) {
                            compiledValue = compiledValue.Replace($"$({name})", value);
                        }
                    }

                    anyChanged |= compiledValue != withRef.Value;

                    withRef.Value = compiledValue;
                }
            } while (anyChanged);
        }
    }
}
