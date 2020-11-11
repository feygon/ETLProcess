using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

using ETLProcessFactory.IO;
using ETLProcessFactory.Containers.Members;


namespace ETLProcess.Tests
{
    class HashCodeTest
    {
        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">Object to be compared in the test.</param>
        /// <param name="comparedCols">Columns to be compared in the test.</param>
        /// <returns></returns>
        static public int GetHashCode(DataRow obj, DataColumn[] comparedCols)
        {
            DataColumn[] keyCols = comparedCols;
            string hashStr = IODirectory.PrepGuid.ToString();

            foreach (DataColumn col in keyCols)
            {
                hashStr = string.Concat(hashStr, "##");
                hashStr = string.Concat(hashStr, obj.ItemArray[col.Ordinal].ToString());
            }

            Log.Write(string.Format($"GetHashCode found hashStr of {hashStr}.\n" +
                $"Returning int.GetHashCode of that: {hashStr.GetHashCode()}"));
            return hashStr.GetHashCode();
        }
        public static void TestHashSystem()
        {
            DataTable table = new DataTable("The Table");
            table.Columns.Add(new DataColumn("num32", typeof(int)));
            table.Columns.Add(new DataColumn("string1", typeof(string)));
            table.Columns.Add(new DataColumn("dateTime", typeof(DateTime)));
            table.Columns.Add(new DataColumn("Date", typeof(Date)));
            table.PrimaryKey = new DataColumn[] { table.Columns[0] };
            DataColumn[] compareCols = new DataColumn[] { table.Columns[1], table.Columns[2] };
            DataRow newRow = table.NewRow();

            newRow.SetField("num32", 1);
            newRow.SetField("string1", "Yo Mama");
            newRow.SetField("dateTime", new DateTime(1982, 7, 7));
            newRow.SetField("Date", new Date(newRow.Field<DateTime>("dateTime")));
            table.Rows.Add(newRow);

            newRow = table.NewRow();
            newRow.SetField("num32", 2);
            newRow.SetField("string1", "Eat at Joe's");
            newRow.SetField("dateTime", new DateTime(1942, 12, 5));
            newRow.SetField("Date", new Date(newRow.Field<DateTime>("dateTime")));
            table.Rows.Add(newRow);

            newRow = table.NewRow();
            newRow.SetField("num32", 3);
            newRow.SetField("string1", "Eat at Joe's");
            newRow.SetField("dateTime", new DateTime(1942, 12, 5));
            newRow.SetField("Date", new Date(newRow.Field<DateTime>("dateTime")));
            table.Rows.Add(newRow);

            //int rowInTableHash0 = table.Rows[0].GetHashCode();
            //int num32Hash0 = table.Rows[0].ItemArray[table.Columns.IndexOf("num32")].GetHashCode();
            //int string1Hash0 = table.Rows[0].ItemArray[table.Columns.IndexOf("string1")].GetHashCode();
            //int dateTimeHash0 = table.Rows[0].ItemArray[table.Columns.IndexOf("dateTime")].GetHashCode();
            //int DateHash0 = table.Rows[0].ItemArray[table.Columns.IndexOf("Date")].GetHashCode();

            //int rowInTableHash1 = table.Rows[1].GetHashCode();
            //int num32Hash1 = table.Rows[1].ItemArray[table.Columns.IndexOf("num32")].GetHashCode();
            //int string1Hash1 = table.Rows[1].ItemArray[table.Columns.IndexOf("string1")].GetHashCode();
            //int dateTimeHash1 = table.Rows[1].ItemArray[table.Columns.IndexOf("dateTime")].GetHashCode();
            //int DateHash1 = table.Rows[1].ItemArray[table.Columns.IndexOf("Date")].GetHashCode();

            //int rowInTableHash2 = table.Rows[2].GetHashCode();
            //int num32Hash2 = table.Rows[2].ItemArray[table.Columns.IndexOf("num32")].GetHashCode();
            //int string1Hash2 = table.Rows[2].ItemArray[table.Columns.IndexOf("string1")].GetHashCode();
            //int dateTimeHash2 = table.Rows[2].ItemArray[table.Columns.IndexOf("dateTime")].GetHashCode();
            //int DateHash2 = table.Rows[2].ItemArray[table.Columns.IndexOf("Date")].GetHashCode();

            Log.Write(string.Format($"{HashCodeTest.GetHashCode(table.Rows[0], compareCols)}"));
            Log.Write(string.Format($"{HashCodeTest.GetHashCode(table.Rows[1], compareCols)}"));
            Log.Write(string.Format($"{HashCodeTest.GetHashCode(table.Rows[2], compareCols)}"));

            /*Log.Write(((int)52).ToString());

            Log.Write(string.Format($"rowInTableHash: {rowInTableHash0}, {rowInTableHash1}, {rowInTableHash2}\n" +
                $"num32Hash: {num32Hash0}, {num32Hash1}, {num32Hash2}\n" +
                $"string1Hash: {string1Hash0}, {string1Hash1}, {string1Hash2}\n" +
                $"dateTimeHash: {dateTimeHash0}, {dateTimeHash1}, {dateTimeHash2}\n" +
                $"DateHash: {DateHash0}, {DateHash1}, {DateHash2}."));*/
        }
    }
}
