using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace ETLProcess.General.LinqOnDataSet {
    /// <summary>
    /// An IEQualityComparer of DataRows, by composite-key dataRelations.
    /// </summary>
    public class DataRelationEqualityComparer : IEqualityComparer<DataRow>
    {
        DataRelation relation;
        DataRow left;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="relation">DataRelation for the constructed IEqualityComparer</param>
        public DataRelationEqualityComparer(DataRelation relation)
        {
            this.relation = relation;
        }

        /// <summary>
        /// Constructor with prepopulated DataRow objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="relation"></param>
        public DataRelationEqualityComparer(DataRow left, DataRelation relation)
        {
            this.relation = relation;
            this.left = left;
        }

        /// <summary>
        /// Equality call for DataRelationEqualityComparer class -- no need to specify which is parent/child in the DataRelation.
        /// </summary>
        /// <param name="left">The left dataRow to compare</param>
        /// <param name="right">The right dataRow to compare</param>
        /// <returns>True for matching key values in the relation.</returns>
        public bool Equals(DataRow left, DataRow right)
        {
            bool leftIsChild = relation.ChildTable.TableName == left.Table.TableName;
            if (leftIsChild) {
                if (relation.ParentTable.TableName != right.Table.TableName) { throw new Exception("Wrong tables."); }
            } else {
                if (relation.ChildTable.TableName != right.Table.TableName
                    || relation.ParentTable.TableName != left.Table.TableName) { throw new Exception("Wrong tables."); }
            }

            DataTable childTable, parentTable;
            DataRow childRow, parentRow;
            if (leftIsChild) {
                childTable = left.Table; childRow = left;
                parentTable = right.Table; parentRow = right;
            } else {
                childTable = right.Table; childRow = right;
                parentTable = left.Table; parentRow = left;
            }
            if (childTable.PrimaryKey.Length != parentTable.PrimaryKey.Length) {
                throw new Exception("Bad primary key match: different lengths of key column arrays.");
            }
            List<(DataColumn child, DataColumn parent)> relationColumns = new List<(DataColumn child, DataColumn parent)>();
            for (int i = 0; i < childTable.PrimaryKey.Length; i++) {
                relationColumns.Add((childTable.PrimaryKey[i], parentTable.PrimaryKey[i]));
                var childKeyValue = childRow.ItemArray[childTable.PrimaryKey[i].Ordinal];
                var parentKeyValue = parentRow.ItemArray[parentTable.PrimaryKey[i].Ordinal];
                if (!childKeyValue.Equals(parentKeyValue)) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Get a lambda comparator for left and right functions.
        /// </summary>
        /// <param name="left">Left dataRow to compare. If null, function will attempt to use constructed member, or throw exception. </param>
        /// <param name="right">Right dataRow to compare. If null, will throw an exception.</param>
        /// <returns></returns>
        public Func<bool> GetLambda(DataRow left, DataRow right) {
            DataRow leftish = left ?? this.left ?? throw new Exception("Null reference exception imminent. Left member must be populated by parameter or constructor.");
            DataRow rightish = right ?? throw new Exception("Null reference exception imminent. Right member must be populated by parameter or constructor.");

            DelRet<bool, (DataRow lf, DataRow rt)> comparator =
                ((DataRow lf, DataRow rt) rows) => {
                    return Equals(rows.lf, rows.rt);
                };

            return () => comparator((leftish, rightish));
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(DataRow obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// A static method, for when there is no need to construct and keep a class member around.
        /// </summary>
        /// <param name="left">Left side datarows</param>
        /// <param name="right">Right side datarows</param>
        /// <param name="relation">Relation between the sides</param>
        /// <returns>Returns a boolean for whether or not they match.</returns>
        public static bool Compare(DataRow left, DataRow right, DataRelation relation)
        {
            DataRelationEqualityComparer DREC = new DataRelationEqualityComparer(left, relation);
            return DREC.GetLambda(left, right)();
        }

        /// <summary>
        /// Returns a set of intersecting rows from a set of DataRows, 
        ///     where they intersect with another set by this comparator.
        /// </summary>
        /// <param name="left">Left side data</param>
        /// <param name="right">Right side data</param>
        /// <param name="returnLeft">Return left side if true, right side if false.</param>
        /// <returns></returns>
        public IEnumerable<DataRow> Intersect(
            IEnumerable<DataRow> left
            , IEnumerable<DataRow> right
            , bool returnLeft)
        {
            if (returnLeft) { 
                return left.Intersect(right, this);
            } else {
                return right.Intersect(left, this);
            }
        }

        /// <summary>
        /// Returns a set of intersecting rows as a valuetuple of DataRow,
        ///     cross-referenced with an IEnumerable of matching DataRows.
        /// </summary>
        /// <param name="left">Left side data</param>
        /// <param name="right">Right side data</param>
        /// <returns></returns>
        public IEnumerable<ValueTuple<DataRow, IEnumerable<DataRow>>> Intersect(
            IEnumerable<DataRow> left,
            IEnumerable<DataRow> right)
        {
            return left.GroupJoin(
                right
                , left => left
                , right => right
                , (lf, rt) => new ValueTuple<DataRow, IEnumerable<DataRow>>(lf, rt)
                , this);
        } // end method

        /// <summary>
        /// Returns a set of intersecting rows combined iwth a set of distinct left-side rows,
        ///     with null values on the right, like a left outer join.
        /// </summary>
        /// <param name="left">Left side data</param>
        /// <param name="right">Right side data</param>
        /// <returns></returns>
        public IEnumerable<ValueTuple<DataRow, IEnumerable<DataRow>>> LeftOuter(
            IEnumerable<DataRow> left
            , IEnumerable<DataRow> right)
        {
            return left.GroupJoin(
                right
                , left => left
                , right => right
                , (lf, rt) => new ValueTuple<DataRow, IEnumerable<DataRow>>(lf, rt.DefaultIfEmpty())
                , this);
        }

        /// <summary>
        /// Returns a set of non-intersecting rows by this comparator.
        /// </summary>
        /// <param name="left">Left side data</param>
        /// <param name="right">Right side data</param>
        /// <returns></returns>
        public IEnumerable<DataRow> Except(
            IEnumerable<DataRow> left
            , IEnumerable<DataRow> right)
        {
            var except = left.Except(right, this);
            return except;
        } // end method

        /// <summary>
        /// Removes duplicates from a set of data rows.
        /// </summary>
        /// <param name="data">Data to be filtered on this relation.</param>
        /// <returns></returns>
        public void Distinct(
            IEnumerable<DataRow> data)
        {
            data.Distinct(this);
        }

        /// <summary>
        /// Combines 2 sets of DataRows into 1 set of distinct DataRows.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public IEnumerable<DataRow> Union(
            IEnumerable<DataRow> left
            , IEnumerable<DataRow> right)
        {
            var union = left.Union(right, this);
            union.Distinct(this);
            return union;
        }

        /// <summary>
        /// Joins 2 sets of DataRows into matched valuetuples.
        /// </summary>
        /// <param name="left">Left side data</param>
        /// <param name="right">Right side data</param>
        /// <returns></returns>
        public IEnumerable<ValueTuple<DataRow, DataRow>> Join(
            IEnumerable<DataRow> left
            , IEnumerable<DataRow> right)
        {
            var join = left.Join(
                right
                , left => left
                , right => right
                , (lf, rt) => new ValueTuple<DataRow, DataRow>(lf, rt)
                , this);
            return join;
        }

        /// <summary>
        /// Joins 2 sets of DataRows into matched buckets of 
        /// </summary>
        /// <param name="left">Left side data</param>
        /// <param name="right">Right side data</param>
        /// <param name="returnLeft">Return results indexed on left side or right?</param>
        /// <param name="returnOuter">True for outer join, false for intersection</param>
        /// <returns></returns>
        public IEnumerable<ValueTuple<DataRow, IEnumerable<DataRow>>> GroupJoin(
            IEnumerable<DataRow> left
            , IEnumerable<DataRow> right
            , bool returnLeft
            , bool returnOuter)
        {
            IEnumerable<ValueTuple<DataRow, IEnumerable<DataRow>>> groupJoin;
            if (returnOuter)
            {
                if (returnLeft) {
                    groupJoin = left.GroupJoin(
                        right
                        , left => left
                        , right => right
                        , (lf, rt) => new ValueTuple<DataRow, IEnumerable<DataRow>>(lf, rt.DefaultIfEmpty())
                        , this);
                }
                else
                {
                    groupJoin = right.GroupJoin(
                        left
                        , right => right
                        , left => left
                        , (rt, lf) => new ValueTuple<DataRow, IEnumerable<DataRow>>(rt, lf.DefaultIfEmpty())
                        , this);
                }
            } else {
                if (returnLeft) {
                    groupJoin = left.GroupJoin(
                        right
                        , left => left
                        , right => right
                        , (lf, rt) => new ValueTuple<DataRow, IEnumerable<DataRow>>(lf, rt)
                        , this);
                } else {
                    groupJoin = right.GroupJoin(
                        left
                        , right => right
                        , left => left
                        , (rt, lf) => new ValueTuple<DataRow, IEnumerable<DataRow>>(rt, lf)
                        , this);
                }
            }
            return groupJoin;
        } // end method

        /// <summary>
        /// Get the relation which is already established between the left and right dataSets,
        ///     provided that they're tables in the master set.
        /// </summary>
        /// <param name="masterSet"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static DataRelation GetRelation(DataSet masterSet, DataTable left, DataTable right)
        {
            if (!masterSet.Tables.Contains(left.TableName)
                || !masterSet.Tables.Contains(right.TableName))
            {
                throw new Exception("Imminent null reference exception: at least one table is missing from the DataSet.");
            }
            if (left.TableName == right.TableName)
            {
                throw new WarningException("Possible null reference exception: left and right are the same table.");
            }
            var ret = ((IEnumerable<DataRelation>)masterSet.Relations).Where
                ((y) => (
                    y.ChildTable.TableName == left.TableName && y.ParentTable.TableName == right.TableName)
                    || (y.ChildTable.TableName == right.TableName && y.ParentTable.TableName == left.TableName
                    )
                ).First();
            return ret;
        }
    } // end class DataRelationEqualityComparer
} // end namespace
