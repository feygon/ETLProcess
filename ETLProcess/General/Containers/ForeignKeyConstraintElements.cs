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
        /// Name of the primary table.
        /// </summary>
        public string primaryTableName;
        /// <summary>
        /// Names of columns used to add a Foreign Key Constraint to a child table.
        /// </summary>
        public string[] childFKColumnNames;

        /// <summary>
        /// Accumulator string for the name of the relation to be established.
        /// </summary>
        public string relationName = "trunk";

        /// <summary>
        /// Constructor for critical components a foreign key (FK) constraint,
        ///     with length checks.
        /// </summary>
        /// <param name="masterSet"></param>
        /// <param name="primaryFKColumns">The Primary FK columns to match on</param>
        /// <param name="childFKColumnNames">The child columns to form a foreign key on.</param>
        /// <param name="primaryTableName">The name of the primary table, for naming the relation.</param>
        public ForeignKeyConstraintElements(
            DataSet masterSet
            , DataColumn[] primaryFKColumns
            , string[] childFKColumnNames
            , string primaryTableName = "trunk")
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
            this.primaryTableName = primaryTableName;
            Log.Write("Branch table Foreign Key Constraint created.\n" + 
                this.primaryTableName + "_FK_ + <Child Table Name>");
        }

        /// <summary>
        /// Constructor for critical components of a foreign key constraint,
        ///     by column names, with length checks.
        /// </summary>
        /// <param name="masterSet">The DataSet this table will be included in.</param>
        /// <param name="tableName">The tablename of the other table, 
        ///     to which this table will be linked by a foreign key.</param>
        /// <param name="primaryFKColumnNames">The names of the columns in the other table 
        ///     to which this table will be linked by a foreign key</param>
        /// <param name="childFKColumnNames">The names of the columns in this table
        ///     which will be linked to the other table.</param>
        /// <param name="primaryTableName">The name of the primary table, which will be the first
        ///     part of the relation name. Default "trunk".</param>
        public ForeignKeyConstraintElements(
            DataSet masterSet
            , string tableName
            , string[] primaryFKColumnNames
            , string[] childFKColumnNames
            , string primaryTableName = "trunk")
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
            DataColumn[] primaryFKColumns = new DataColumn[childFKColumnNames.Length];
            for (int col = 0; col <= childFKColumnNames.Length; col++)
            {
                primaryFKColumns[col] = masterSet.Tables[tableName].Columns[primaryFKColumnNames[col]];
            }
            this.masterSet = masterSet;
            this.childFKColumnNames = childFKColumnNames;
            this.primaryFKColumns = primaryFKColumns;
            this.primaryTableName = primaryTableName;
        }

        /// <summary>
        /// Constructor for object with only a master DataSet.
        /// </summary>
        /// <param name="masterSet">The master data set, to add this to.</param>
        /// <param name="tableName">Intended name of the table.</param>
        public ForeignKeyConstraintElements(
            DataSet masterSet, string tableName)
        {
            this.masterSet = masterSet ?? throw new Exception("Cannot be null: Errant Null value passed to Foreign Key Constraint struct.");
            this.primaryFKColumns = null;
            this.childFKColumnNames = null;
            this.primaryTableName = tableName;
            this.primaryTableName = "trunk";
            Log.Write("Null Foreign Key Constraint created. Was this the trunk table of the master set?");
        }

        /// <summary>
        /// Use an object of this class to create a foreign key constraint between two tables.
        /// </summary>
        /// <param name="table">The child table to link.</param>
        public void SetFKConstraint(DataTable table)
        {
            relationName += "_FK_";
            relationName += table.TableName;
            if (masterSet != null
                && primaryFKColumns != null
                && childFKColumnNames != null)
            {
                masterSet.Relations.Add(
                    relationName
                    , primaryFKColumns
                    , (from hdr in childFKColumnNames select table.Columns[hdr]).ToArray());
            }
        }
    }
}