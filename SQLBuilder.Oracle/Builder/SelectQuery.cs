using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLBuilder.Oracle.Builder {
    /// <summary>
    /// Select Query Builder Class
    /// </summary>
    public class SelectQuery : BaseQuery {
        #region Private Properties
        private string _From { get; set; }
        private bool _IsDistinct { get; set; }
        private List<string> _Fields { get; set; }
        private List<string> _Joins { get; set; }
        private List<string> _Wheres { get; set; }
        private List<string> _Groups { get; set; }
        private bool _IsWithRollUp { get; set; }
        private List<string> _Havings { get; set; }
        private List<string> _Orders { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the schema, table, and table alias for the SELECT statement.
        /// </summary>
        /// <param name="Schema">The schema of the query.</param>
        /// <param name="Table">The table of the query.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        public SelectQuery(string Schema, string Table, string TableAlias) {
            if (Schema.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Schema argument should not be empty.");
            }
            this._Schema = Schema;
            if (Table.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Table argument should not be empty.");
            }
            this._Table = Table;
            if (!TableAlias.IsNullOrWhiteSpace()) {
                this._TableAlias = TableAlias;
            } else {
                this._TableAlias = Table;
            }
            this._From = String.Format("{0}.{1}{2}", Schema, Table, !TableAlias.IsNullOrWhiteSpace() ? String.Format(" {0}", TableAlias) : null);
            this._InitProperties();
        }

        /// <summary>
        /// Initializes the schema, and table for the SELECT statement.
        /// </summary>
        /// <param name="Schema">The schema of the query.</param>
        /// <param name="Table">The table of the query.</param>
        public SelectQuery(string Schema, string Table)
            : this(Schema, Table, null) {
        }

        /// <summary>
        /// Initializes the query as a table, and table alias for the SELECT statement.
        /// </summary>
        /// <param name="Select">The SelectQuery instance.</param>
        /// <param name="TableAlias">The alias of the query.</param>
        public SelectQuery(SelectQuery Select, string TableAlias) {
            if (Select == null) {
                throw new ArgumentException("Select argument should not be null.");
            }
            string strQuery = Select.ToString();
            if (strQuery.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Select argument should not be empty.");
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            this._From = String.Format("({0}) {1}", strQuery, TableAlias);
            this._TableAlias = TableAlias;
            this._InitProperties();
        }

        /// <summary>
        /// Initializes the query as a table, and table alias for the SELECT statement.
        /// </summary>
        /// <param name="Selects">The list of SelectQuery instances.</param>
        /// <param name="TableAlias">The alias of the query.</param>
        public SelectQuery(List<SelectQuery> Selects, string TableAlias) {
            if (Selects == null) {
                throw new ArgumentException("Select argument should not be null.");
            }
            if (Selects.Count == 0) {
                throw new ArgumentException("Select argument should not be null.");
            }
            List<string> lstQueries = new List<string>();
            foreach (var Select in Selects) {
                lstQueries.Add(Select.ToString());
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            this._From = String.Format("(({0})) {1}", String.Join(") UNION (", lstQueries.ToArray()), TableAlias);
            this._TableAlias = TableAlias;
            this._InitProperties();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets a virtual field to be used in the query.
        /// </summary>
        /// <param name="Name">The name of the virtual field.</param>
        /// <param name="Expression">The query expression.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetVirtualField(string Name, string Expression) {
            this._SetVirtualField(Name, Expression);
            return this;
        }

        /// <summary>
        /// Sets a parameter to be used in the query.
        /// </summary>
        /// <param name="Name">The name of the parameter.</param>
        /// <param name="Value">The value of the parameter.</param>
        public SelectQuery SetParameter(string Name, object Value) {
            this._SetParameter(Name, Value);
            return this;
        }

        /// <summary>
        /// Sets the query to be distinct or not. By default it is set to false.
        /// </summary>
        /// <param name="Distinct">Is the query distinct?</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetDistinct(bool Distinct) {
            this._IsDistinct = Distinct;
            return this;
        }

        /// <summary>
        /// Adds an expression/column to the SELECT clause.
        /// </summary>
        /// <param name="Condition">The condition to check before adding to the SELECT clause.</param>
        /// <param name="Expression">The expression/column to be added.</param>
        /// <param name="Alias">The alias of the expression.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetSelect(bool Condition, string Expression, string Alias) {
            if (Condition) {
                if (Expression.IsNullOrWhiteSpace()) {
                    throw new ArgumentException("Expression argument should not be empty.");
                }
                if (this._VirtualFields.ContainsKey(Expression)) {
                    if (Alias.IsNullOrWhiteSpace()) {
                        Alias = Expression;
                    }
                    Expression = this._VirtualFields[Expression];
                }
                this._Fields.Add(String.Format("{0}{1}", Expression, Alias.IsNullOrWhiteSpace() ? null : String.Format(" AS {0}", Alias)));
            }
            return this;
        }

        /// <summary>
        /// Adds an expression/column to the SELECT clause.
        /// </summary>
        /// <param name="Condition">The condition to check before adding to the SELECT clause.</param>
        /// <param name="Expression">The expression/column to be added.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetSelect(bool Condition, string Expression) {
            return this.SetSelect(Condition, Expression, null);
        }

        /// <summary>
        /// Adds an expression/column to the SELECT clause.
        /// </summary>
        /// <param name="Expression">The expression/column to be added.</param>
        /// <param name="Alias">The alias of the expression.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetSelect(string Expression, string Alias) {
            return this.SetSelect(true, Expression, Alias);
        }

        /// <summary>
        /// Adds an expression/column to the SELECT clause.
        /// </summary>
        /// <param name="Expression">The expression/column to be added.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetSelect(string Expression) {
            return this.SetSelect(true, Expression, null);
        }

        /// <summary>
        /// Adds an expression/column to the SELECT clause.
        /// </summary>
        /// <param name="Condition">The condition to check before adding to the SELECT clause.</param>
        /// <param name="Select">The SelectQuery instance.</param>
        /// <param name="Alias">The alias of the expression.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetSelect(bool Condition, SelectQuery Select, string Alias) {
            if (Condition) {
                if (Select == null) {
                    throw new ArgumentException("Select argument should not be null.");
                }
                string strQuery = Select.ToString();
                if (strQuery.IsNullOrWhiteSpace()) {
                    throw new ArgumentException("Select argument should not be empty.");
                }
                if (Alias.IsNullOrWhiteSpace()) {
                    throw new ArgumentException("Alias argument should not be empty.");
                }
                this._Fields.Add(String.Format("({0}) AS {1}", strQuery, Alias));
            }
            return this;
        }

        /// <summary>
        /// Adds an expression/column to the SELECT clause.
        /// </summary>
        /// <param name="Select">The SelectQuery instance.</param>
        /// <param name="Alias">The alias of the expression.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetSelect(SelectQuery Select, string Alias) {
            return this.SetSelect(true, Select, Alias);
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="Schema">The schema of the table to be joined.</param>
        /// <param name="Table">The table to be joined.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetLeftJoin(string Schema, string Table, string TableAlias, string ConditionStatement) {
            if (Schema.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Schema argument should not be empty.");
            }
            if (Table.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Table argument should not be empty.");
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("LEFT JOIN {0}.{1}{2} ON ({3})", Schema, Table, !Table.Equals(TableAlias) ? String.Format(" {0}", TableAlias) : null, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="Schema">The schema of the table to be joined.</param>
        /// <param name="Table">The table to be joined.</param>
        /// <param name="IncrementTableAlias">The alias of the table using the incremented table name.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetLeftJoin(string Schema, string Table, uint IncrementTableAlias, string ConditionStatement) {
            if (IncrementTableAlias == 0) {
                return this.SetLeftJoin(Schema, Table, Table, ConditionStatement);
            }
            return this.SetLeftJoin(Schema, Table, Table + "_" + IncrementTableAlias, ConditionStatement);
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="Table">The table to be joined.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetLeftJoin(string Table, string TableAlias, string ConditionStatement) {
            if (Table.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Table argument should not be empty.");
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("LEFT JOIN {0}.{1}{2} ON ({3})", this._Schema, Table, !Table.Equals(TableAlias) ? String.Format(" {0}", TableAlias) : null, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="Table">The table to be joined.</param>
        /// <param name="IncrementTableAlias">The alias of the table using the incremented table name.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetLeftJoin(string Table, uint IncrementTableAlias, string ConditionStatement) {
            if (IncrementTableAlias == 0) {
                return this.SetLeftJoin(Table, Table, ConditionStatement);
            }
            return this.SetLeftJoin(Table, Table + "_" + IncrementTableAlias, ConditionStatement);
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetLeftJoin(string TableAlias, string ConditionStatement) {
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("LEFT JOIN {0}.{1} {2} ON ({3})", this._Schema, this._Table, !this._Table.Equals(TableAlias) ? String.Format(" {0}", TableAlias) : null, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="IncrementTableAlias">The alias of the table using the incremented table name.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetLeftJoin(uint IncrementTableAlias, string ConditionStatement) {
            if (IncrementTableAlias == 0) {
                return this.SetLeftJoin(this._Table, ConditionStatement);
            }
            return this.SetLeftJoin(this._Table + "_" + IncrementTableAlias, ConditionStatement);
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="Select">The SelectQuery instance.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetLeftJoin(SelectQuery Select, string TableAlias, string ConditionStatement) {
            if (Select == null) {
                throw new ArgumentException("Select argument should not be null.");
            }
            string strQuery = Select.ToString();
            if (strQuery.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Select argument should not be empty.");
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("LEFT JOIN ({0}) {1} ON ({2})", strQuery, TableAlias, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="Selects">The list of SelectQuery instances.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetLeftJoin(List<SelectQuery> Selects, string TableAlias, string ConditionStatement) {
            if (Selects == null) {
                throw new ArgumentException("Selects argument should not be null.");
            }
            List<string> lstQueries = new List<string>();
            foreach (var Select in Selects) {
                if (Select == null) {
                    throw new ArgumentException("A member of Selects argument should not be null.");
                }
                if (Select.ToString().IsNullOrWhiteSpace()) {
                    throw new ArgumentException("A member of Selects argument should not be empty.");
                }
                lstQueries.Add(Select.ToString());
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("LEFT JOIN ({0}) {1} ON ({2})", String.Format("({0})", String.Join(") UNION (", lstQueries.ToArray())), TableAlias, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a RIGHT JOIN clause.
        /// </summary>
        /// <param name="Schema">The schema of the table to be joined.</param>
        /// <param name="Table">The table to be joined.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetRightJoin(string Schema, string Table, string TableAlias, string ConditionStatement) {
            if (Schema.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Schema argument should not be empty.");
            }
            if (Table.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Table argument should not be empty.");
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("RIGHT JOIN {0}.{1}{2} ON ({3})", Schema, Table, !Table.Equals(TableAlias) ? String.Format(" {0}", TableAlias) : null, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a RIGHT JOIN clause.
        /// </summary>
        /// <param name="Schema">The schema of the table to be joined.</param>
        /// <param name="Table">The table to be joined.</param>
        /// <param name="IncrementTableAlias">The alias of the table using the incremented table name.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetRightJoin(string Schema, string Table, uint IncrementTableAlias, string ConditionStatement) {
            if (IncrementTableAlias == 0) {
                return this.SetRightJoin(Schema, Table, Table, ConditionStatement);
            }
            return this.SetRightJoin(Schema, Table, Table + "_" + IncrementTableAlias, ConditionStatement);
        }

        /// <summary>
        /// Adds a RIGHT JOIN clause.
        /// </summary>
        /// <param name="Table">The table to be joined.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetRightJoin(string Table, string TableAlias, string ConditionStatement) {
            if (Table.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Table argument should not be empty.");
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("RIGHT JOIN {0}.{1}{2} ON ({3})", this._Schema, Table, !Table.Equals(TableAlias) ? String.Format(" {0}", TableAlias) : null, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a RIGHT JOIN clause.
        /// </summary>
        /// <param name="Table">The table to be joined.</param>
        /// <param name="IncrementTableAlias">The alias of the table using the incremented table name.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetRightJoin(string Table, uint IncrementTableAlias, string ConditionStatement) {
            if (IncrementTableAlias == 0) {
                return this.SetRightJoin(Table, Table, ConditionStatement);
            }
            return this.SetRightJoin(Table, Table + "_" + IncrementTableAlias, ConditionStatement);
        }

        /// <summary>
        /// Adds a RIGHT JOIN clause.
        /// </summary>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetRightJoin(string TableAlias, string ConditionStatement) {
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("RIGHT JOIN {0}.{1} {2} ON ({3})", this._Schema, this._Table, !this._Table.Equals(TableAlias) ? String.Format(" {0}", TableAlias) : null, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a RIGHT JOIN clause.
        /// </summary>
        /// <param name="IncrementTableAlias">The alias of the table using the incremented table name.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetRightJoin(uint IncrementTableAlias, string ConditionStatement) {
            if (IncrementTableAlias == 0) {
                return this.SetRightJoin(this._Table, ConditionStatement);
            }
            return this.SetRightJoin(this._Table + "_" + IncrementTableAlias, ConditionStatement);
        }

        /// <summary>
        /// Adds a RIGHT JOIN clause.
        /// </summary>
        /// <param name="Select">The SelectQuery instance.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetRightJoin(SelectQuery Select, string TableAlias, string ConditionStatement) {
            if (Select == null) {
                throw new ArgumentException("Select argument should not be null.");
            }
            string strQuery = Select.ToString();
            if (strQuery.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Select argument should not be empty.");
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("RIGHT JOIN ({0}) {1} ON ({2})", strQuery, TableAlias, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a LEFT JOIN clause.
        /// </summary>
        /// <param name="Selects">The list of SelectQuery instances.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetRightJoin(List<SelectQuery> Selects, string TableAlias, string ConditionStatement) {
            if (Selects == null) {
                throw new ArgumentException("Selects argument should not be null.");
            }
            List<string> lstQueries = new List<string>();
            foreach (var Select in Selects) {
                if (Select == null) {
                    throw new ArgumentException("A member of Selects argument should not be null.");
                }
                if (Select.ToString().IsNullOrWhiteSpace()) {
                    throw new ArgumentException("A member of Selects argument should not be empty.");
                }
                lstQueries.Add(Select.ToString());
            }
            if (TableAlias.IsNullOrWhiteSpace()) {
                throw new ArgumentException("TableAlias argument should not be empty.");
            }
            if (ConditionStatement.IsNullOrWhiteSpace()) {
                throw new ArgumentException("Condition argument should not be empty.");
            }
            this._Joins.Add(String.Format("RIGHT JOIN ({0}) {1} ON ({2})", String.Format("({0})", String.Join(") UNION (", lstQueries.ToArray())), TableAlias, ConditionStatement));
            return this;
        }

        /// <summary>
        /// Adds a condition to the WHERE clause.
        /// </summary>
        /// <param name="Condition">The condition to check before adding to the WHERE clause.</param>
        /// <param name="ConditionStatement">The condition statement/s to be added.</param>
        /// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetWhere(bool Condition, string ConditionStatement, params object[] ParameterValues) {
            if (Condition) {
                if (ConditionStatement.IsNullOrWhiteSpace()) {
                    throw new ArgumentException("ConditionStatement argument should not be empty.");
                }
                List<string> lstParameters = new List<string>();
                if (ParameterValues != null && ParameterValues.Length > 0) {
                    foreach (object objParameterValue in ParameterValues) {
                        string strParameter = String.Format(":where_condition_{0}", this._Parameters.Count(kv => kv.Key.Contains(":where_condition")) + 1);
                        this._SetParameter(strParameter, objParameterValue);
                        lstParameters.Add(strParameter);
                    }
                }
                this._Wheres.Add(String.Format(ConditionStatement, lstParameters.ToArray()));
            }
            return this;
        }

        /// <summary>
        /// Adds a condition to the WHERE clause.
        /// </summary>
        /// <param name="ConditionStatement">The condition statement/s to be added.</param>
        /// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetWhere(string ConditionStatement, params object[] ParameterValues) {
            return this.SetWhere(true, ConditionStatement, ParameterValues);
        }

        /// <summary>
        /// Adds an expression/s to the GROUP BY clause.
        /// </summary>
        /// <param name="Expressions">Additional expressions to be added.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetGroupBy(params string[] Expressions) {
            if (Expressions != null & Expressions.Length > 0) {
                foreach (string strExpression in Expressions) {
                    if (strExpression.IsNullOrWhiteSpace()) {
                        continue;
                    }
                    this._Groups.Add(strExpression);
                }
            }
            return this;
        }

        /// <summary>
        /// Adds a WITH ROLLUP clause to the GROUP BY clause.
        /// </summary>
        /// <param name="WithRollUp"></param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetWithRollUp(bool WithRollUp) {
            this._IsWithRollUp = WithRollUp;
            return this;
        }

        /// <summary>
        /// Adds a condition to the HAVING clause.
        /// </summary>
        /// <param name="Condition">The condition to check before adding to the HAVING clause.</param>
        /// <param name="ConditionStatement">The condition statement/s to be added.</param>
        /// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetHaving(bool Condition, string ConditionStatement, params object[] ParameterValues) {
            if (Condition) {
                if (ConditionStatement.IsNullOrWhiteSpace()) {
                    throw new ArgumentException("Condition argument should not be empty.");
                }
                List<string> lstParameters = new List<string>();
                if (ParameterValues != null && ParameterValues.Length > 0) {
                    foreach (object objParameterValue in ParameterValues) {
                        string strParameter = String.Format(":having_condition_{0}", this._Parameters.Count(kv => kv.Key.Contains(":having_condition")) + 1);
                        this._SetParameter(strParameter, objParameterValue);
                        lstParameters.Add(strParameter);
                    }
                }
                this._Havings.Add(String.Format(ConditionStatement, lstParameters.ToArray()));
            }
            return this;
        }

        /// <summary>
        /// Adds a condition to the HAVING clause.
        /// </summary>
        /// <param name="ConditionStatement">The condition statement/s to be added.</param>
        /// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetHaving(string ConditionStatement, params object[] ParameterValues) {
            return this.SetHaving(true, ConditionStatement, ParameterValues);
        }

        /// <summary>
        /// Adds an expression/s to the ORDER BY clause.
        /// </summary>
        /// <param name="Direction">The order direction of the expression/s.</param>
        /// <param name="Expressions">The expression to be ordered.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetOrderBy(OrderDirections Direction, params string[] Expressions) {
            if (Expressions != null & Expressions.Length > 0) {
                List<string> lstExpression = new List<string>();
                foreach (string strExpression in Expressions) {
                    if (strExpression.IsNullOrWhiteSpace()) {
                        continue;
                    }
                    if (this._VirtualFields.ContainsKey(strExpression)) {
                        lstExpression.Add(this._VirtualFields[strExpression]);
                    } else {
                        lstExpression.Add(strExpression);
                    }
                }
                if (lstExpression.Count > 0) {
                    this._Orders.Add(String.Format("{0} {1}", String.Join(", ", lstExpression.ToArray()), this._GetOrderDirection(Direction)));
                }
            }
            return this;
        }

        /// <summary>
        /// Adds an expression/s to the ORDER BY clause.
        /// </summary>
        /// <param name="Expressions">The expression to be ordered.</param>
        /// <returns>The current instance of this class.</returns>
        public SelectQuery SetOrderBy(params string[] Expressions) {
            this.SetOrderBy(OrderDirections.Asc, Expressions);
            return this;
        }
        #endregion

        #region Public Override Method
        /// <summary>
        /// Builds the query string.
        /// </summary>
        /// <returns>The query string.</returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT");
            if (this._Fields.Count > 0) {
                sb.Append(String.Format(" {0}", String.Join(", ", this._Fields.ToArray())));
            } else {
                sb.Append(" *");
            }
            sb.Append(String.Format("\nFROM {0}", this._From));
            if (this._Joins.Count > 0) {
                sb.Append(String.Format("\n{0}", String.Join("\n", this._Joins.ToArray())));
            }
            if (this._Wheres.Count > 0) {
                sb.Append(String.Format("\nWHERE ({0})", String.Join(" AND ", this._Wheres.ToArray())));
            }
            if (this._Groups.Count > 0) {
                string strGroupBy = String.Format("\nGROUP BY {0}", String.Join(", ", this._Groups.ToArray()));
                if (this._IsWithRollUp) {
                    sb.Append(strGroupBy);
                } else {
                    sb.Append(strGroupBy);
                }
            }
            if (this._IsWithRollUp) {
                sb.Append(" WITH ROLLUP");
            }
            if (this._Havings.Count > 0) {
                sb.Append(String.Format("\nHAVING ({0})", String.Join(" AND ", this._Havings.ToArray())));
            }
            if (this._Orders.Count > 0) {
                sb.Append(String.Format("\nORDER BY {0}", String.Join(", ", this._Orders.ToArray())));
            }
            return sb.ToString().Trim();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void _InitProperties() {
            this._Fields = new List<string>();
            this._Joins = new List<string>();
            this._Wheres = new List<string>();
            this._Groups = new List<string>();
            this._Havings = new List<string>();
            this._Orders = new List<string>();
        }

        /// <summary>
        /// Gets the order direction in string format.
        /// </summary>
        /// <param name="Dir">The order direction.</param>
        /// <returns>The order direction in string format</returns>
        private string _GetOrderDirection(OrderDirections Dir) {
            switch (Dir) {
                case OrderDirections.Asc:
                    return "ASC";
                case OrderDirections.Desc:
                    return "DESC";
                default:
                    return "ASC";
            }
        }
        #endregion
    }
}
