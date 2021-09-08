using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codemap.General.Containers
{
    public class DocTable<TBasicDoc> : DataTable where TBasicDoc: IBasicDoc
    {
        DataTable Table { get; set; }
        public DataColumnCollection Columns => Table.Columns;
        public DataRowCollection Rows => Table.Rows;
        public string TableName => Table.TableName;
        public DataSet DataSet => Table.DataSet;

    }
}
