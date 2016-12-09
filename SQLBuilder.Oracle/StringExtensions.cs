using System;

namespace SQLBuilder.Oracle {
    /// <summary>
    /// String Extensions Static Class.
    /// </summary>
    public static class StringExtensions {
        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or System.String.Empty, or if value consists exclusively of white-space characters.</returns>
        public static bool IsNullOrWhiteSpace(this string value) {
            if (value == null) {
                return true;
            }
            return String.IsNullOrEmpty(value.Trim());
        }
    }
}
