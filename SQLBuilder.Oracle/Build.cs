using System.Collections.Generic;
using SQLBuilder.Oracle.Builder;

namespace SQLBuilder.Oracle {
    /// <summary>
    /// Build Class
    /// </summary>
    public static class Build {
        /// <summary>
        /// OracleSql Select builder.
        /// </summary>
        /// <param name="Schema">The schema.</param>
        /// <param name="Table">The table.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <returns>The instance of SelectQuery.</returns>
        public static SelectQuery Select(string Schema, string Table, string TableAlias) {
            return new SelectQuery(Schema, Table, TableAlias);
        }

        /// <summary>
        /// OracleSql Select builder.
        /// </summary>
        /// <param name="Schema">The schema.</param>
        /// <param name="Table">The table.</param>
        /// <returns>The instance of SelectQuery.</returns>
        public static SelectQuery Select(string Schema, string Table) {
            return new SelectQuery(Schema, Table);
        }

        /// <summary>
        /// OracleSql Select builder.
        /// </summary>
        /// <param name="Select">The instance of SelectQuery</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <returns>The instance of SelectQuery.</returns>
        public static SelectQuery Select(SelectQuery Select, string TableAlias) {
            return new SelectQuery(Select, TableAlias);
        }

        /// <summary>
        /// OracleSql Select builder.
        /// </summary>
        /// <param name="Selects">The list of instance of SelectQuery isntances.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <returns>The instance of SelectQuery.</returns>
        public static SelectQuery Select(List<SelectQuery> Selects, string TableAlias) {
            return new SelectQuery(Selects, TableAlias);
        }

        /// <summary>
        /// OracleSql Select Count builder.
        /// </summary>
        /// <param name="Schema">The schema.</param>
        /// <param name="Table">The table.</param>
        /// <param name="TableAlias">The alias of the table.</param>
        /// <returns>The instance of SelectCountQuery.</returns>
        public static SelectCountQuery SelectCount(string Schema, string Table, string TableAlias) {
            return new SelectCountQuery(Schema, Table, TableAlias);
        }

        /// <summary>
        /// OracleSql Select builder.
        /// </summary>
        /// <param name="Schema">The schema.</param>
        /// <param name="Table">The table.</param>
        /// <returns>The instance of SelectCountQuery.</returns>
        public static SelectCountQuery SelectCount(string Schema, string Table) {
            return new SelectCountQuery(Schema, Table);
        }
    }
}
