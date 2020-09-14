using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

using ETLProcess.General;
using ETLProcess.General.Interfaces;
using ETLProcess.General.IO;
using ETLProcess.General.Containers;
using ETLProcess.General.Algorithms;
using ETLProcess.General.Profiles;

using String = System.String;
using AcctID = System.String;
using MemberID = System.String;
using ETLProcess.General.Containers.AbstractClasses;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using ETLProcess.Specific.Documents;

namespace ETLProcess.Specific.Boilerplate
{
    /// <summary>
    /// A boilerplate example of a class to fulfill Client statement and welcome letter document types.
    /// </summary>
    public class ClientETLProcess : DataSet, IC_CSVFileIn<IO_FilesIn>
    {
        /// <summary>
        /// Bucket for tables, for easy reference by type instead of unique identifying string.
        /// </summary>
        public Dictionary<Type, List<DataTable>> TablesByType { get; } = new Dictionary<Type, List<DataTable>>();
        //Class members.
        private readonly IO_FilesIn Process_FilesIn;
        private FileDataRecords<Record_Statement, ClientETLProcess> statementRecords; // is a DataTable.
        private FileDataRecords<Record_Members, ClientETLProcess> memberRecords;
        private FileDataRecords<Record_BalFwd, ClientETLProcess> balFwdRecords;

        // Key Columns of each class (not including indexers).
        /// <summary>
        /// Dictionary of column types.
        /// </summary>
        public Dictionary<Type, SampleColumnTypes> SampleColumns { get; } = null;
        /// <summary>
        ///  Public parameterless constructor, for inheritance.
        /// </summary>
        public ClientETLProcess() : base(IOFiles.PrepGuid.ToString()) { }
        /// <summary>
        /// Record of the argument that was passed into the boilerplate implementation of ClientETLProcess.
        /// </summary>
        public static string argIn = "";
        /// <summary>
        /// Constructor for boilerplate implementation class files, required by interface.
        /// </summary>
        /// <param name="arg">Argument for IOFilesIn initialization.</param>
        public ClientETLProcess(string arg)
            : base(IOFiles.PrepGuid.ToString())
        {
            Record_Statement.InitSample();
            Record_Members.InitSample();
            Record_BalFwd.InitSample();
            SampleColumns = new Dictionary<Type, SampleColumnTypes> {
                 { typeof(Record_Statement), new SampleColumnTypes((Record_Statement.Sample).columnTypes) }
                ,{ typeof(Record_Members), new SampleColumnTypes((Record_Members.Sample).columnTypes) }
                ,{ typeof(Record_BalFwd), (Record_BalFwd.Sample).columnTypes }
            };
            argIn = arg;

            // TO DO: MetroEmail.InitClient(); Does mandrill email the client when this goes off?
            IO_FilesIn.Init(new object[] { arg });
            this.Process_FilesIn = IO_FilesIn.GetDerivedInstance();

            Process_FilesIn.Check_Input(CheckFiles_Delegate);

            if (!Process_FilesIn.Check_Input(CheckFiles_Delegate)) {
                Log.WriteException("Bad file count in zipfile.");
            } // specified check for implementation Statement/WelcomeLetters
            Log.Write("Client ETL Profile Loaded.");
        }

        internal void ExportRecords(List<OutputDoc> outputDocs)
        {
            XML.Export("out", outputDocs);
            // this may require linq left joins, which must be GP'd.
        }

        // Member interface for delegate to check files for requirements.
        /// <summary>
        /// Delegate to satisfy interface requirement of a delegate that will check files for validity.
        /// In this case, it checks for number of files.
        /// </summary>
        public DelRet<bool, string[]> CheckFiles_Delegate { get; } =
            (string[] files) => { 
                if (files.Length != 3) {
                    throw new Exception("Wrong number of files, expected 3");
                }
                return true;
            };

        //public bool Check_Input(DelRet<bool, string[]> inputs)
        //{
        //    return Process_FilesIn.Check_Input(inputs);
        //}


        /// <summary>
        /// Return an enumeration of which document type it is.
        /// <br>Implementation Specific</br>
        /// </summary>
        /// <param name="filename">Filename of the file in question.</param>
        /// <returns></returns>
        public RecordType IdentifyRecordFile(string filename)
        {
            if (Path.GetFileNameWithoutExtension(filename).StartsWith("Statement", StringComparison.InvariantCultureIgnoreCase)) { 
                return RecordType.Statements; 
            }
            if (Path.GetFileNameWithoutExtension(filename).StartsWith("Member", StringComparison.InvariantCultureIgnoreCase)) { 
                return RecordType.Members; 
            }
            if (Path.GetFileNameWithoutExtension(filename).Contains("Balance")) { 
                return RecordType.BalancesForward; 
            }
            return RecordType.Error; // error code;
        }

        /// <summary>
        /// Public call to populate docs.
        /// </summary>
        public void PopulateRecords() 
        {
            PopulateRecords(Process_FilesIn.Files);
            Log.Write("Records Populated.");
        }

        /// <summary>
        /// Populate documents with information, in the proper order.
        /// </summary>
        /// <param name="files"></param>
        private void PopulateRecords(string[] files)
        {
            RecordType recordType;
            string filename
                , fileExtension;
            Queue<string> fileList = OrderFileList(files);

            foreach (string filePath in fileList)
            {
                filename = Path.GetFileName(filePath);
                fileExtension = Path.GetExtension(filePath);
                recordType = IdentifyRecordFile(filename);

                DataColumn[] _parentColumns;
                String[] _childColumns;
                // put each document type into its headersource (struct of Stringmap and headers list)
                switch (recordType)
                {
                    case (RecordType.Statements):

                        List<string> StatementRecordData = CSV.ImportRows(filePath);

                        HeaderSource<List<StringMap>, List<string>> statementSrcData =
                            Record_Statement.Sample.ParseRows(StatementRecordData.ToArray());
                        statementRecords = new FileDataRecords<Record_Statement, ClientETLProcess>(
                            statementSrcData
                            , new ForeignKeyConstraintElements(this, typeof(Record_Statement).Name));
                        Log.Write("Statement Records files populated.");
                        
                        if (!TablesByType.ContainsKey(typeof(Record_Statement))) { TablesByType.Add(typeof(Record_Statement), new List<DataTable>() { statementRecords }); }
                        else { TablesByType[typeof(Record_Statement)].Add(statementRecords); }

                        break;

                    case (RecordType.Members):
                        // TO DO: once-over post-DataSet/DataTable type changes.
                        // statementTable already populated, per OrderFileList(files) method.

                        var membersByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , delimiter: "|"
                            , useQuotes: false);

                        _parentColumns = new DataColumn[] { Tables[statementRecords.TableName].Columns["Group Billing Acct ID"] }; // parent key columns.
                        _childColumns = new String[] { "Billing Account Number" };

                        // "must belong to a column" error.
                        memberRecords = new FileDataRecords<Record_Members, ClientETLProcess>(
                            membersByAcctID
                            , new ForeignKeyConstraintElements(
                                this
                                , _parentColumns
                                , _childColumns
                            )
                        );
                        if (!TablesByType.ContainsKey(typeof(Record_Members))) { TablesByType.Add(typeof(Record_Members), new List<DataTable>() { memberRecords }); }
                        else { TablesByType[typeof(Record_Members)].Add(memberRecords); }
                        Log.Write("Member Records populated.");
                        break;

                    case (RecordType.BalancesForward):
                        // TO DO: once-over post-DataSet/DataTable type changes.
                        // balfwdfile has no internal headers.
                        List<string> headers = Record_BalFwd.Sample.headers;

                        // TO DO: 
                        var balFwdByAcctID = CSV.ImportCSVWithHeader(
                            filePath
                            , ","
                            , useQuotes: true
                            , headers);

                        _parentColumns = new DataColumn[] { Tables[statementRecords.TableName].Columns["Group Billing Acct ID"] }; // parent key columns.
                        _childColumns = new String[] { "Account ID" };

                        balFwdRecords = new FileDataRecords<Record_BalFwd, ClientETLProcess>(
                            balFwdByAcctID
                            , new ForeignKeyConstraintElements(
                                this
                                , _parentColumns
                                , _childColumns )
                            );

                        Log.Write("Balance Forward Records populated.");
                        if (!TablesByType.ContainsKey(typeof(Record_BalFwd))) { TablesByType.Add(typeof(Record_BalFwd), new List<DataTable>() { balFwdRecords }); }
                        else { TablesByType[typeof(Record_BalFwd)].Add(balFwdRecords); }
                        break;

                    case (RecordType.Error):
                        throw new Exception($"Unexpected file found: {filePath}");
                }
            }
        }

        /// <summary>
        /// <Pre>PopulateDocs has already gotten all records and made them into tables.</Pre>
        /// <Post>A schema has been built around the relations of these tables.<br>
        /// Any relevant filters have been run, and reports have been made on bad data.</br></Post>
        /// </summary>
        /// <returns></returns>
        internal List<OutputDoc> ProcessRecords()
        {
            // PopulateDocs has already:
            // Gotten All Invoices, Bal Fwd Records, Member Records, and made them into tables in a dataset.
            try
            {
                // Filter out missing members from statements, return a count of the missing members,
                //  and put out a DataTable of invoices missing members.
                int missingMembersQty = ClientBusinessRules.Filter_MissingMembers(
                    this,
                    Tables[typeof(Record_Statement).ToString()]
                    , out DataTable invoices_MissingMembers);
                
                // Filter out missing members from balances forward, return a count of the missing members,
                //  and put out a DataTable of balances forward missing members.
                int missingBalFwdQty = ClientBusinessRules.Filter_MissingMembers(
                    this
                    , Tables[typeof(Record_BalFwd).ToString()]
                    , out DataTable balances_MissingMembers);
                
                // Query a selection of new members by sql call, return a count of new members,
                //  and put out a DataTable of new members.
                int newClientQty = ClientBusinessRules.Query_NewMembers(
                    this
                    , out DataTable newMembers);
            } catch (Exception err) { Log.Write(err.Message + "\n" + err.StackTrace); }
            string memStr = $"FilesIn_Table_{typeof(Record_Members).Name}_{IOFiles.PrepGuid}_0";
            string stmStr = $"FilesIn_Table_{typeof(Record_Statement).Name}_{IOFiles.PrepGuid}_0";
            string balStr = $"FilesIn_Table_{typeof(Record_BalFwd).Name}_{IOFiles.PrepGuid}_0";

            var mem = Tables[memStr];
            var stm = Tables[stmStr];
            var bal = Tables[balStr];

            //example: query a selection of statements which don't have corresponding members
            var newMemberQuery = from left in stm.AsEnumerable()
                                where !(from right in mem.AsEnumerable()
                                        select right[mem.PrimaryKey[0]]
                                        ).Contains(left[stm.PrimaryKey[0]])
                                select left;

            List<OutputDoc> outputDocs = new List<OutputDoc>();
            var eStatementTable = TablesByType[typeof(Record_Statement)][0].AsEnumerable();
            var eMemberTable = TablesByType[typeof(Record_Members)][0].AsEnumerable();
            var eBalsFwdTable = TablesByType[typeof(Record_BalFwd)][0].AsEnumerable();


            var statementRows = from a_stmRow in eStatementTable
                                join a_memRow in eMemberTable
                                on a_stmRow.Field<string>("Group Billing Acct ID") 
                                equals a_memRow.Field<string>("Billing Account Number")
                                select a_stmRow;
            var memberRows = from b_stmRow in eStatementTable
                             join b_memRow in eMemberTable
                             on b_stmRow.Field<string>("Group Billing Acct ID") equals b_memRow.Field<string>("Billing Account Number")
                             select b_memRow;
            var balFwdRows = from c_balRow in eBalsFwdTable
                             join c_memRow in memberRows
                             on c_balRow.Field<string>("Member ID")
                             equals c_memRow.Field<string>("MemberID")
                             select c_balRow;
            foreach (var row in statementRows)
            {
                DataRow memberRow = memberRows.Where(x => x.Field<string>("Billing Account Number") == row.Field<string>("Group Billing Acct ID")).First();
                var eBalFwdRows = (from BalFwdRow in TablesByType[typeof(Record_BalFwd)][0].AsEnumerable()
                                                     where BalFwdRow.Field<string>("Member ID") == memberRow.Field<string>("MemberID")
                                                     select BalFwdRow).ToList();
                outputDocs.Add(new OutputDoc(row, memberRow, eBalFwdRows));
            }

            return outputDocs;
        }

        /// <summary>
        /// Specific ETLProcess implementations need to order the entry of each file into the table, so the foreign key constraints
        ///     will be added in order.
        /// </summary>
        /// <param name="files">The array of filenames to be processed.</param>
        /// <returns>A <see cref="Queue{T}"/> which orders the files to be processed into tables.</returns>
        public Queue<string> OrderFileList(string[] files)
        {
            var dict = new Dictionary<string, int> ();
            foreach (string file in files)
            {
                RecordType docType = IdentifyRecordFile(file);
                switch (docType)
                {
                    case RecordType.Statements:
                        dict.Add(file, 0);
                        break;
                    case RecordType.Members:
                        dict.Add(file, 1);
                        break;
                    case RecordType.BalancesForward:
                        dict.Add(file, 2);
                        break;
                }
            }

            var ret = new Queue<String>();
            while (dict.Count > 0)
            {
                int minInt = dict.Values.Min();
                string min = dict.Keys.Where((x) => dict[x] == minInt).FirstOrDefault(); // allows multiple keys hashing to a bucket int value.
                ret.Enqueue(min);
                dict.Remove(min);
            }
            return ret;
        }
    }
}
