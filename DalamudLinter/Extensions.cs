using System;
using System.Collections.Generic;

namespace DalamudLinter {
    internal static class Extensions {
        internal static IEnumerable<string> Lines(this string s, StringSplitOptions opts = StringSplitOptions.None) {
            return s.Split(new[] { "\r\n", "\n" }, opts);
        }
    }
}
