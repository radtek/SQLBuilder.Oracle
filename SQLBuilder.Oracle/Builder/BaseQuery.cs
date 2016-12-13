using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SQLBuilder.Oracle.Builder {
    /// <summary>
    /// Base Query Builder Abstract Class
    /// </summary>
    public abstract class BaseQuery {
        #region Protected Properties
        /// <summary>
        /// Virtual Fields property.
        /// </summary>
        protected Dictionary<string, string> _VirtualFields { get; private set; }

        /// <summary>
        /// Database property.
        /// </summary>
        protected string _Database { get; set; }

        /// <summary>
        /// Table property.
        /// </summary>
        protected string _Table { get; set; }

        /// <summary>
        /// Table Alias property.
        /// </summary>
        protected string _TableAlias { get; set; }

        /// <summary>
        /// Parameters property.
        /// </summary>
        protected Dictionary<string, object> _Parameters { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseQuery() {
            this._InitProperties();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns>The parameters.</returns>
        public Dictionary<string, object> GetParameters() {
            return this._Parameters;
        }

        /// <summary>
        /// Prints the parameters to the output window.
        /// </summary>
        public void PrintParameters() {
            Debug.WriteLine(null);
            Debug.WriteLine("--------------------------------------------------");
            if (this._Parameters == null || this._Parameters.Count == 0) {
                Debug.WriteLine("No parameters available.");
                return;
            }
            Debug.WriteLine("Oracle Parameters:");
            int intCounter = 1;
            foreach (KeyValuePair<string, object> kvp in this._Parameters) {
                Debug.WriteLine(String.Format("Parameter {0}:", intCounter));
                Debug.WriteLine(String.Format("Type:\t{0}", kvp.Value != null ? kvp.Value.GetType() : null));
                Debug.WriteLine(String.Format("Name:\t{0}", kvp.Key));
                Debug.WriteLine(String.Format("Value:\t{0}", kvp.Value));
                Debug.WriteLine(null);
                intCounter++;
            }
            Debug.WriteLine(null);
        }

        /// <summary>
        /// Merges the parameters with other parameters.
        /// </summary>
        /// <param name="Parameters">The parameters to be merged.</param>
        public void MergeParameters(params Dictionary<string, object>[] Parameters) {
            if (Parameters == null || Parameters.Length == 0) {
                return;
            }
            foreach (Dictionary<string, object> Parameter in Parameters) {
                foreach (KeyValuePair<string, object> kvp in Parameter) {
                    if (this._Parameters.ContainsKey(kvp.Key)) {
                        this._Parameters[kvp.Key] = kvp.Value;
                    } else {
                        this._Parameters.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }
        #endregion

        #region Public Virtual Method
        /// <summary>
        /// Prints the query to the output window.
        /// </summary>
        public virtual void PrintQuery() {
            Debug.WriteLine(null);
            Debug.WriteLine("==================================================");
            Debug.WriteLine("Oracle Query:");
            Debug.WriteLine(this);
            Debug.WriteLine(null);
            this.PrintParameters();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Sets a virtual field to be used in the query.
        /// </summary>
        /// <param name="Name">The name of the virtual field.</param>
        /// <param name="Expression">The query expression.</param>
        protected void _SetVirtualField(string Name, string Expression) {
            if (Name.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Name argument should not be empty.");
            }
            if (Regex.IsMatch(Name, @"\W")) {
                throw new ArgumentException("Name argument should only contain any word character (letter, number, underscore).");
            }
            if (Expression.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Expression argument should not be empty.");
            }
            if (this._VirtualFields.ContainsKey(Name)) {
                this._VirtualFields[Name] = Expression;
            } else {
                this._VirtualFields.Add(Name, Expression);
            }
        }

        /// <summary>
        /// Sets a parameter to be used in the query.
        /// </summary>
        /// <param name="Name">The name of the parameter.</param>
        /// <param name="Value">The value of the parameter.</param>
        protected void _SetParameter(string Name, object Value) {
            if (Name.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Name argument should not be empty.");
            }
            if (!Regex.IsMatch(Name, @"^\:[\w]+$")) {
                throw new ArgumentException("Name argument should only contain ':' and any word character (letter, number, underscore) after.");
            }
            if (this._Parameters.ContainsKey(Name)) {
                this._Parameters[Name] = Value;
            } else {
                this._Parameters.Add(Name, Value);
            }
        }

        /// <summary>
        /// Checks if a value is numeric.
        /// </summary>
        /// <param name="Value">The value to be checked.</param>
        /// <returns>True if numeric. Otherwise, false.</returns>
        protected bool _IsNumeric(object Value) {
            if (Value is string) {
                return Regex.IsMatch(Value.ToString(), @"^(?:\+|\-)?(?:0|[1-9][\d]*)(?:\.[\d]+)?$");
            } else {
                return Value is sbyte || Value is byte || Value is short || Value is ushort || Value is int || Value is uint || Value is long || Value is ulong || Value is float || Value is double || Value is decimal;
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void _InitProperties() {
            this._VirtualFields = new Dictionary<string, string>();
            this._Parameters = new Dictionary<string, object>();
        }
        #endregion
    }
}
