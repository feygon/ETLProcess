using ETLProcess.General.IO;
using ETLProcess.General.Profiles;
using ETLProcess.Specific.Boilerplate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

namespace ETLProcess.General.ExtendLinQ {
    /// <summary>
    /// An IEQualityComparer of DataRows, by composite-key dataRelations.
    /// </summary>
    public class DataRelationKeyColumnComparer : IEqualityComparer<DataRow>
    {
        /// <summary>
        /// Relation of this comparer.
        /// </summary>
        public readonly DataRelation relation;
        DataRow left;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="relation">DataRelation for the constructed IEqualityComparer</param>
        public DataRelationKeyColumnComparer(DataRelation relation) {
            if (relation.ChildColumns.Length != relation.ParentColumns.Length) { Log.WriteWarningException("Incomparable number of columns. All rows will be inequal."); }
            this.relation = relation;
        }

        /// <summary>
        /// Constructor with prepopulated DataRow objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="relation"></param>
        public DataRelationKeyColumnComparer(DataRow left, DataRelation relation) {
            if (relation.ChildColumns.Length != relation.ParentColumns.Length) { Log.WriteWarningException("Incomparable number of columns. All rows will be inequal."); }
            this.relation = relation;
            this.left = left;
        }

        /// <summary>
        /// Equality call for DataRelationEqualityComparer class -- no need to specify which is parent/child in the DataRelation.
        /// </summary>
        /// <param name="left">The left dataRow to compare</param>
        /// <param name="right">The right dataRow to compare</param>
        /// <returns>True for matching key values in the relation.</returns>
        public bool Equals(DataRow left, DataRow right) {
            string leftString = GetHashString(left);
            string rightString = GetHashString(right);
            return string.Compare(leftString, rightString, StringComparison.InvariantCulture) == 0;
        }

        private string GetHashString(DataRow row) {
            bool isChild = row.Table.TableName == relation.ChildTable.TableName;
            string keyStr = IOFiles.PrepGuid.ToString().Substring(0, 8);
            DataColumn[] keyCols;
            if (isChild) { keyCols = relation.ChildColumns; } else { keyCols = relation.ParentColumns; }
            foreach (DataColumn col in keyCols)
            {
                keyStr = string.Concat(keyStr, "#/#");
                keyStr = string.Concat(keyStr, row.ItemArray[col.Ordinal]);
            }
            return keyStr;
        }

        /// <summary>
        /// Returns a hash code for the specified object. 
        /// <para>Uses non-thread-safe operations.</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(DataRow obj) {
            bool parent;
            if (obj.Table == relation.ParentTable) { parent = true; } else { parent = false; }
            DataColumn[] keyCols;
            if (parent) { keyCols = relation.ParentColumns; } else { keyCols = relation.ChildColumns; }

            string hashStr = IOFiles.PrepGuid.ToString().Substring(0,8);

            foreach (DataColumn col in keyCols) {
                hashStr = string.Concat(hashStr, "##");
                hashStr = string.Concat(hashStr, obj.ItemArray[col.Ordinal].ToString());
            }

            Log.Write(string.Format($"GetHashCode found hashStr of {hashStr}.\n" +
                $"Returning int.GetHashCode of that: {hashStr.GetHashCode()}"));
            return hashStr.GetHashCode();
        }

        /// <summary>
        /// Get a lambda comparator for left and right functions.
        /// </summary>
        /// <param name="left">Left dataRow to compare. If null, function will attempt to use constructed member, or throw exception. </param>
        /// <param name="right">Right dataRow to compare. If null, will throw an exception.</param>
        /// <returns></returns>
        public Func<DataRow, bool> GetLambda(DataRow left, DataRow right) {
            DataRow leftish = left ?? this.left ?? throw new Exception("Null reference exception imminent. Left member must be populated by parameter or constructor.");
            DataRow rightish = right ?? throw new Exception("Null reference exception imminent. Right member must be populated by parameter or constructor.");

            DelRet<bool, (DataRow lf, DataRow rt)> comparator =
                ((DataRow lf, DataRow rt) rows) => {
                    return Equals(rows.lf, rows.rt);
                };

            return (rightish) => comparator((leftish, rightish));
        }

        //private List<int> GetHashList(IEnumerable<DataRow> rows) {
        //    List<int> hashList = new List<int>();
        //    foreach (var x in rows)
        //    {
        //        int xHash = GetHashCode(x);
        //        if (!hashList.Contains(xHash))
        //        {
        //            hashList.Add(xHash);
        //        }
        //    }
        //    return hashList;
        //}
        
        //// return a non-distinct map of rows by hashes.
        //private Dictionary<int, IEnumerable<DataRow>> GetHashMap(IEnumerable<DataRow> rows) {
        //    var hashMap = new Dictionary<int, IEnumerable<DataRow>>();
        //    foreach (var x in rows)
        //    {
        //        int xHash = GetHashCode(x);
        //        if (!hashMap.ContainsKey(xHash)) { hashMap.Add(xHash, new List<DataRow>() { x }.AsEnumerable()); }
        //        else { hashMap[xHash].Append(x); }
        //    }
        //    return hashMap;
        //}

        /// <summary>
        /// Returns a set of non-intersecting rows by this comparator, from the left side data. Distinct by default.
        /// </summary>
        /// <param name="left">Left side data</param>
        /// <param name="right">Right side data</param>
        /// <param name="distinct">Return only distinct elements in the left side data which are not in the right side data? Default true.</param>
        /// <returns></returns>
        public IEnumerable<DataRow> Except(
            IEnumerable<DataRow> left
            , IEnumerable<DataRow> right
            , bool distinct = true)
        {
            List<DataRow> excepted = new List<DataRow>();
            List<string> filter = new List<string>();
            foreach (DataRow rightRow in right) {
                filter.Add(GetHashString(rightRow));
            }
            foreach (DataRow leftRow in left) {
                string leftRowKeyString = GetHashString(leftRow);
                if (distinct) { filter.Add(leftRowKeyString); }
                if (!filter.Contains(leftRowKeyString)) {
                    excepted.Add(leftRow);
                }
            }
            return excepted;
        } // end method


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
            IEnumerable<DataRow> x, y;
            if (returnLeft) { x = left; y = right; }
            else { x = right; y = left; }

            List<DataRow> intersected = new List<DataRow>();
            List<string> filter = new List<string>();
            foreach (DataRow yRow in y) {
                filter.Add(GetHashString(yRow));
            }
            foreach (DataRow xRow in x) {
                string xRowKeyString = GetHashString(xRow);
                if (filter.Contains(xRowKeyString)) {
                    intersected.Add(xRow);
                }
            }
            return intersected;
        }

        /// <summary>
        /// Returns a set of intersecting rows as a valuetuple of DataRow,
        ///     cross-referenced with an IEnumerable of matching DataRows.
        /// <para>Used with distinct values for right, this will produce no redundant indices.</para>
        /// </summary>
        /// <param name="left">Left side data, test set -- will be indexed by right</param>
        /// <param name="right">Right side data, filter set -- will be an index for left</param>
        /// <returns></returns>
        public IEnumerable<ValueTuple<DataRow, IEnumerable<DataRow>>> Intersect(
            IEnumerable<DataRow> left,
            IEnumerable<DataRow> right)
        {
            List<KeyValuePair<string, DataRow>> leftRowsByKeyStr = new List<KeyValuePair<string, DataRow>>()
                                              , rightRowsByKeyStr = new List<KeyValuePair<string, DataRow>>();
            foreach (DataRow LRow in left) {
                leftRowsByKeyStr.Add(new KeyValuePair<string, DataRow>(GetHashString(LRow), LRow));
            }
            foreach (DataRow RRow in right) {
                rightRowsByKeyStr.Add(new KeyValuePair<string, DataRow>(GetHashString(RRow), RRow));
            }
            var Intersected = new List<(DataRow, IEnumerable<DataRow>)>();
            foreach (var RKVP in rightRowsByKeyStr) {
                var matchingLefts = leftRowsByKeyStr.Where((lft) => lft.Key == RKVP.Key);
                if (matchingLefts.Count() > 0) {
                    var matches = matchingLefts.Select((y) => y.Value);
                    Intersected.Add((RKVP.Value, matches));
                }
            }
            return Intersected.AsEnumerable();
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
            var leftOuter = new List<ValueTuple<DataRow, IEnumerable<DataRow>>>();
            leftOuter.AddRange(Intersect(left, right));
            var leftExcept = Except(left, right, false);
            foreach (var x in leftExcept)
            {
                leftOuter.Add((x, null));
            }
            return leftOuter;
        }

        /// <summary>
        /// Removes duplicates from a set of data rows.
        /// </summary>
        /// <param name="data">Data to be filtered on this relation.</param>
        /// <returns></returns>
        public IEnumerable<DataRow> Distinct(
            IEnumerable<DataRow> data)
        {
            Dictionary<string, DataRow> filter = new Dictionary<string, DataRow>();
            foreach (DataRow row in data) {
                string rowStr = GetHashString(row);
                if (!filter.ContainsKey(rowStr)) {
                    filter.Add(rowStr, row);
                }
            }
            return filter.Select((x) => x.Value).AsEnumerable();
        }

        /// <summary>
        /// Combines distinct DataRows of 2 Tables into a new table with 1 set of distinct DataRows and all column sets.
        /// The table has the name of the left table, and no relations.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public DataTable Union(
            DataTable left
            , DataTable right)
        {
            DataTable union = new DataTable(left.TableName);
            if (left.Rows.Count <= 0 || right.Rows.Count <= 0) { 
                Log.WriteWarningException("A set of rows is empty. Nothing to union.");
                if (left.Rows.Count <= 0) { return right; } else { return left; }
            }
            Dictionary<int, int> leftOrdinals = new Dictionary<int, int>();
            Dictionary<int, int> rightOrdinals = new Dictionary<int, int>();
            
            // Build a new schema by merging the left and right schemas.
              // Add columns from the left side to the table and get their ordinals.
            union.Columns.AddRange(left.Columns.Cast<DataColumn>().ToArray());
            leftOrdinals = left.Columns.Cast<DataColumn>().ToDictionary((x) => union.Columns[x.ColumnName].Ordinal, (y) => y.Ordinal);
              // Add distinct columns from the right side to the table and get their ordinals.
            var remainder = right.Columns.Cast<DataColumn>().Where((x) => !union.Columns.Contains(x.ColumnName)).ToArray();
            union.Columns.AddRange(remainder);
            rightOrdinals = right.Columns.Cast<DataColumn>().ToDictionary((x) => union.Columns[x.ColumnName].Ordinal, (y) => y.Ordinal);

            // Get Excepted rows from right and left, and intersecting rows from right and left.
            var leftRows = left.Select();
            var rightRows = right.Select();
            var leftException = Except(leftRows, rightRows);
            var rightException = Except(rightRows, leftRows);
            var leftIntersection = Intersect(leftRows, rightRows, true).ToList();
            var rightIntersection = Intersect(leftRows, rightRows, false).ToList();

            // Add the left side excepted rows.
            foreach (DataRow LERow in leftException) {
                DataRow newRow = union.NewRow();
                foreach (var ord in leftOrdinals) {
                    newRow.ItemArray[ord.Key] = LERow.ItemArray[ord.Value];
                }
                union.Rows.Add(newRow);
            }
            // Add the right side excepted rows.
            foreach (DataRow RERow in rightException) {
                DataRow newRow = union.NewRow();
                foreach (var ord in rightOrdinals) {
                    newRow.ItemArray[ord.Key] = RERow.ItemArray[ord.Value];
                }
                union.Rows.Add(newRow);

            }
            // Merge the left and right side intersected rows into the new Schema, and add them.
            for (int i = 0; i < leftIntersection.Count(); i++) {
                DataRow newMergedRow = union.NewRow();
                foreach (var LtOrd in leftOrdinals) {
                    newMergedRow.ItemArray[LtOrd.Key] = leftIntersection[i].ItemArray[LtOrd.Value];
                }
                foreach (var RtOrd in rightOrdinals) {
                    newMergedRow.ItemArray[RtOrd.Key] ??= rightIntersection[i].ItemArray[RtOrd.Value];
                }
                union.Rows.Add(newMergedRow);
            }
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
            var intersectLeft = Intersect(left, right, true);
            var intersectRight = Intersect(left, right, false);
            var innerJoin = from ltRow in intersectLeft
                            select new ValueTuple<DataRow, DataRow>(
                                ltRow
                                , intersectRight.Where((rtRow) => Equals(rtRow, ltRow)).FirstOrDefault());
            return innerJoin;
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
            var intersectLeft = Intersect(left, right, true);
            var intersectRight = Intersect(left, right, false);
            List<ValueTuple<DataRow, IEnumerable<DataRow>>> groupJoin = new List<(DataRow, IEnumerable<DataRow>)>();
            if (returnOuter)
            {
                if (returnLeft) {
                    var outerLeft = Except(left, right);
                    groupJoin.AddRange(from ltRow in intersectLeft select 
                                       new ValueTuple<DataRow, IEnumerable<DataRow>>(
                                           ltRow
                                           , intersectRight.Where((rtRow) => Equals(ltRow, rtRow))));
                    groupJoin.AddRange(from ltRow in outerLeft select
                                       new ValueTuple<DataRow, IEnumerable<DataRow>>(ltRow, null));
                } else {
                    var outerRight = Except(right, left);
                    groupJoin.AddRange(from rtRow in intersectRight select
                                       new ValueTuple<DataRow, IEnumerable<DataRow>>(
                                           rtRow
                                           , intersectLeft.Where((ltRow) => Equals(ltRow, rtRow))));
                    groupJoin.AddRange(from rtRow in outerRight select
                                       new ValueTuple<DataRow, IEnumerable<DataRow>>(rtRow, null));
                }
            } else {
                if (returnLeft) {
                    groupJoin.AddRange(from ltRow in intersectLeft select
                                       new ValueTuple<DataRow, IEnumerable<DataRow>>(
                                           ltRow
                                           , intersectRight.Where((rtRow) => Equals(ltRow, rtRow))));
                } else {
                    groupJoin.AddRange(from rtRow in intersectRight select
                                       new ValueTuple<DataRow, IEnumerable<DataRow>>(
                                           rtRow
                                           , intersectLeft.Where((ltRow) => Equals(ltRow, rtRow))));
                } // end returnLeft conditional
            } // end returnOuter conditional
            return groupJoin;
        } // end method

        /// <summary>
        /// Get a relation which is already established between the left and right DataTables in a DataSet.
        /// </summary>
        /// <param name="masterSet"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static DataRelation GetExistingRelation(DataSet masterSet, DataTable left, DataTable right) {
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
        } // end method GetExistingRelation
    } // end class DataRelationEqualityComparer
} // end namespace
