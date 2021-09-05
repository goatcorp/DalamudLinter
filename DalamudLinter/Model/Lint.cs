using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Wmhelp.XPath2;

namespace DalamudLinter.Model {
    /// <summary>
    /// A lint.
    /// </summary>
    [Serializable]
    public class Lint {
        #pragma warning disable 8618

        /// <summary>
        /// A short code assigned to this lint, like <c>D0001</c>.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// <para>
        /// The name of the lint.
        /// </para>
        ///
        /// <para>
        /// Should be a short summary of the problem the lint checks for.
        /// </para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <para>
        /// The severity of this lint.
        /// </para>
        ///
        /// <para>
        /// See <see cref="Severity"/> for more information about severity levels.
        /// </para>
        /// </summary>
        public Severity Severity { get; set; }

        /// <summary>
        /// <para>
        /// A longform description of the problem the lint checks for.
        /// </para>
        ///
        /// <para>
        /// This should start with a statement describing the condition that caused this lint to be activated.
        /// It should then start a new paragraph describing why this condition is detrimental. The description
        /// should be impersonal.
        /// </para>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// <para>
        /// The actions a user can take to fix the problem this lint checks for.
        /// </para>
        ///
        /// <para>
        /// Actions should be imperative and impersonal.
        /// </para>
        /// </summary>
        public string Fix { get; set; }

        /// <summary>
        /// <para>
        /// Whether to show the tags that matched the XPath in <see cref="Csproj"/>.
        /// </para>
        ///
        /// <para>
        /// Note that if this is true, the lint will be printed for every tag that matches <see cref="Csproj"/>.
        /// </para>
        /// </summary>
        public bool ShowContext { get; set; } = true;

        /// <summary>
        /// <para>
        /// The XPath query to match for this lint.
        /// </para>
        ///
        /// <para>
        /// If any tag matches this query, the lint is activated.
        /// </para>
        /// </summary>
        public string? Csproj { get; set; }

        #pragma warning restore 8618

        public IEnumerable<XElement> Matches(XDocument xml) {
            return xml.XPath2SelectElements(this.Csproj);
        }
    }
}
