using System;

namespace DalamudLinter.Model {
    [Serializable]
    public enum Severity {
        // /// <summary>
        // /// The lint should print a message but not be considered a warning.
        // /// </summary>
        // Hint,

        /// <summary>
        /// The lint should be considered a warning and print a message.
        /// </summary>
        Warn,

        /// <summary>
        /// The lint should prevent compilation from completing successfully and print a message.
        /// </summary>
        Error,
    }
}
