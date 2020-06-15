using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using ETLProcess.General.Containers;

namespace ETLProcess.General.Containers
{
    /// <summary>
    /// Wrapper class for critical components of a foreign key constraint.
    /// </summary>
    public class ForeignKeyConstraintElements
    {
        /// <summary>
        /// Master Data set of the intended Foreign Key Constraint
        /// </summary>
        public DataSet masterSet;
        /// <summary>
        /// Columns in a table in the Master Set to add a Foreign Key Constraint against.
        /// </summary>
        public DataColumn[] primaryFKColumns;
        /// <summary>
        /// Names of columns used to add a Foreign Key Constraint to a child table.
        /// </summary>
        public string[] childFKColumnNames;

        /// <summary>
        /// Constructor for critical components a foreign key (FK) constraint,
        ///     with length checks.
        /// </summary>
        /// <param name="masterSet"></param>
        /// <param name="primaryFKColumns">The Primary FK columns to match on</param>
        /// <param name="childFKColumnNames">The child columns to form a foreign key on.</param>
        public ForeignKeyConstraintElements(
            DataSet masterSet
            , DataColumn[] primaryFKColumns
            , string[] childFKColumnNames)
        {
            if (masterSet == null || primaryFKColumns == null || childFKColumnNames == null)
            {
                throw new Exception("Errant Null values passed to Foreign Key Constraint struct.");
            }
            if (primaryFKColumns.Length != childFKColumnNames.Length) {
                throw new Exception("Number of columns in foreign key constraint must match."); 
            }
            this.masterSet = masterSet;
            this.primaryFKColumns = primaryFKColumns;
            this.childFKColumnNames = childFKColumnNames;
        }

        /// <summary>
        /// Constructor for critical components of a foreign key constraint,
        ///     by column names, with length checks.
        /// </summary>
        /// <param name="masterSet"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryFKColumnNames"></param>
        /// <param name="childFKColumnNames"></param>
        public ForeignKeyConstraintElements(
            DataSet masterSet
            , string tableName
            , string[] primaryFKColumnNames
            , string[] childFKColumnNames)
        {
            if (masterSet == null 
             || tableName == null || tableName == ""
             || primaryFKColumnNames == null
             || childFKColumnNames == null)
            {
                throw new Exception("Errant Null values passed to Foreign Key Constraint struct.");
            }
            if (primaryFKColumnNames.Length != childFKColumnNames.Length)
            {
                throw new Exception("Number of columns in foreign key constraint must match.");
            }
            this.masterSet = masterSet;
            this.childFKColumnNames = childFKColumnNames;
            DataColumn[] primaryFKColumns = new DataColumn[childFKColumnNames.Length];
            for (int col = 0; col <= childFKColumnNames.Length; col++)
            {
                string colName = primaryFKColumnNames[col];
                primaryFKColumns[col] = masterSet.Tables[tableName].Columns[primaryFKColumnNames[col]];
            }
            this.primaryFKColumns = primaryFKColumns;
        }

        /// <summary>
        /// Constructor for object with only a master DataSet.
        /// </summary>
        /// <param name="masterSet">The master data set, to add this to.</param>
        public ForeignKeyConstraintElements(
            DataSet masterSet)
        {
            if (masterSet == null)
            {
                throw new Exception("Errant Null value passed to Foreign Key Constraint struct.");
            }
            this.masterSet = masterSet;
            this.primaryFKColumns = null;
            this.childFKColumnNames = null;
        }
    }
}
