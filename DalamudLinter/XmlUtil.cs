using System;
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
                .XPath2Select("/Project/PropertyGroup/node()")
                .Where(elem => elem is XElement)
                .Cast<XElement>()
                .GroupBy(elem => elem.Name.LocalName, elem => elem.Value, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First());

            // replace any reference to a local property with the property's value
            bool anyChanged;
            do {
                anyChanged = false;

                var withRefs = xml.XPath2Select("/Project//node()[contains(text(), '$(')]")
                    .Where(elem => elem is XElement)
                    .Cast<XElement>();
                foreach (var withRef in withRefs) {
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
