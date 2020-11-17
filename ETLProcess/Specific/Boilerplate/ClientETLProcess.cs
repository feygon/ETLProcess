using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Runtime.Serialization;

using ETLProcessFactory.Interfaces;
using ETLProcessFactory.IO;
using ETLProcessFactory.Containers;
using ETLProcessFactory.Algorithms;
using ETLProcessFactory.Profiles;
using ETLProcess.Specific.Documents;
using ETLProcessFactory.Interfaces.Profile_Interfaces;
using ETLProcessFactory.ExtendLinQ;
using ETLProcessFactory.GP;
using String = System.String;
using System.Data.SqlClient;


namespace ETLProcess.Specific.Boilerplate
{
    /// <summary>
    /// A boilerplate example of a class to fulfill Client statement and welcome letter document types.
    /// </summary>
    public class ClientETLProcess : 
        DataSet
        , ILoadable_CSVFile<IO_FilesIn>
        , IExportable_XML<Out_XMLProfile, OutputDoc>
        // interface to promise a way to export the Statement records to SQL DB.
        , IExportable_SQL_TSQL
            <Out_SQLBulkProfile<FileDataRecords<Record_Statement, ClientETLProcess>>
                , FileDataRecords<Record_Statement, ClientETLProcess>
                >
        , IExportable_SQL_TSQL
            <Out_SQLBulkProfile<
                FileDataRecords<Record_Members, ClientETLProcess>>
                , FileDataRecords<Record_Members, ClientETLProcess>
                >
    {
        #region Members
        /// <summary>
        /// Bucket for tables, for easy reference by type instead of unique identifying string.
        /// </summary>
        public Dictionary<Type, List<DataTable>> TablesByType { get; } = new Dictionary<Type, List<DataTable>>();
        //Class members.
        private readonly IO_FilesIn Process_FilesIn;
        private readonly Out_XMLProfile Process_XMLOut;
        private FileDataRecords<Record_Statement, ClientETLProcess> statementRecords; // is a DataTable.
        private FileDataRecords<Record_Members, ClientETLProcess> memberRecords;
        private FileDataRecords<Record_BalFwd, ClientETLProcess> balFwdRecords;


        // Key Columns of each class (not including indexers).
        /// <summary>
        /// Dictionary of column types.
        /// </summary>
        public Dictionary<Type, TableHeaders> AllTableHeadersByType { get; } = null;
        /// <summary>
        ///  Public parameterless constructor, for inheritance.
        /// </summary>
        public ClientETLProcess() : base(IODirectory.PrepGuid.ToString()) { }
        /// <summary>
        /// Record of the argument that was passed into the boilerplate implementation of ClientETLProcess.
        /// </summary>
        public static string argIn = "";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for boilerplate implementation class files, required by interface.
        /// </summary>
        /// <param name="inArg">Filename argument for IOFilesIn initialization.</param>
        /// <param name="outArg">Filename argument for IO_XMLOut initialization.</param>
        public ClientETLProcess(string inArg, string outArg)
            : base(IODirectory.PrepGuid.ToString())
        {
            Record_Statement.InitSample();
            Record_Members.InitSample();
            Record_BalFwd.InitSample();
            AllTableHeadersByType = new Dictionary<Type, TableHeaders> {
                 { typeof(Record_Statement), new TableHeaders((Record_Statement.Sample).ColumnTypes) }
                ,{ typeof(Record_Members), new TableHeaders((Record_Members.Sample).ColumnTypes) }
                ,{ typeof(Record_BalFwd), (Record_BalFwd.Sample).ColumnTypes }
            };
            argIn = inArg;

            IO_FilesIn.Init(new object[] { inArg });
            this.Process_FilesIn = IO_FilesIn.GetDerivedInstance();
            Out_XMLProfile.Init(new object[] { outArg });
            this.Process_XMLOut = Out_XMLProfile.GetDerivedInstance();

            Process_FilesIn.Check_Input(CheckFiles_Delegate);

            if (!Process_FilesIn.Check_Input(CheckFiles_Delegate)) {
                Log.WriteException("Bad file count in zipfile.");
            } // specified check for implementation Statement/WelcomeLetters
            Log.Write("Client ETL Profile Loaded.");
        }
        #endregion

        #region Promises from ILoadable_CSVFile
        #region Promised Member
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
            }; // end delegate member
        #endregion
        #region Promised Methods
        /// <summary>
        /// Return an enumeration of which document type it is.
        /// <br>Implementation Specific</br>
        /// </summary>
        /// <param name="filename">Filename of the file in question.</param>
        /// <returns></returns>
        public int IdentifyRecordFile(string filename)
        {
            if (Path.GetFileNameWithoutExtension(filename).StartsWith("Statement", StringComparison.InvariantCultureIgnoreCase)) { 
                return (int)RecordType.Statements; 
            }
            if (Path.GetFileNameWithoutExtension(filename).StartsWith("Member", StringComparison.InvariantCultureIgnoreCase)) { 
                return (int)RecordType.Members; 
            }
            if (Path.GetFileNameWithoutExtension(filename).Contains("Balance")) { 
                return (int)RecordType.BalancesForward; 
            }
            return (int)RecordType.Error; // error code;
        }//end method

        /// <summary>
        /// Public call to populate docs.
        /// </summary>
        public void PopulateRecords() 
        {
            PopulateRecords(Process_FilesIn.Files);
            Log.Write("Records Populated.");
        }

        /// <summary>
        /// Check that the input is a table and not null.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public DelRet<bool> GetCheck_SQL_Output(object[] options)
        {
            try
            {
                DataTable dataTable = ((DataTable)options[0]);
                if (dataTable == null) { throw new Exception("Null DataTable sql output check."); }
            }
            catch (Exception err)
            {
                Log.WriteException("Error casting DataTable or unknown Exception on checking SQL Output DataTable", err);
            }
            return () => { return true; };
        } // end method

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
                RecordType docType = (RecordType)IdentifyRecordFile(file);
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
            while (dict.Count > 0) {
                int minInt = dict.Values.Min();
                string min = dict.Keys.Where((x) => dict[x] == minInt).FirstOrDefault(); // allows multiple keys hashing to a bucket int value.
                ret.Enqueue(min);
                dict.Remove(min);
            }
            return ret;
        } // end method
        #endregion
        #endregion

        #region Private Core Algorithm
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
                recordType = (RecordType)IdentifyRecordFile(filename);

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
                            , AllTableHeadersByType
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
                            , AllTableHeadersByType
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
                        List<string> headers = Record_BalFwd.Sample.Headers;

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
                            , AllTableHeadersByType
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
        #endregion

        #region Public Core Algorithm
        /// <summary>
        /// <Pre>PopulateDocs has already gotten all records and made them into tables.</Pre>
        /// <Post>A schema has been built around the relations of these tables.<br>
        /// Any relevant filters have been run, and reports have been made on bad data.</br></Post>
        /// </summary>
        /// <returns></returns>
        internal List<OutputDoc> ProcessRecords(
            out IEnumerable<DataRow> membersWithoutStatements
            , out IEnumerable<DataRow> balancesWithoutStatements
            , out IEnumerable<DataRow> statementsWithoutMembers)
        {
            // PopulateDocs has already:
            // Gotten All Invoices, Bal Fwd Records, Member Records, and made them into tables in a dataset.

            // Get the report lists out.
            // Datarows from members where there DOES NOT EXIST a statement with a matching Member ID.
            membersWithoutStatements = ClientBusinessRules.GetMembersWOStmts(this);
            // DataRows from balances where there DOES NOT EXIST statement with a matching account ID.
            balancesWithoutStatements = ClientBusinessRules.GetBalancesWOStmts(this);
            // DataRows from members where there DOES NOT EXIST a member to match a statement.
            statementsWithoutMembers = ClientBusinessRules.GetStmtsWOMembers(this);

            //var balancesWStms = ClientBusinessRules.GetBalancesWStmts(this);
            //var membersWithStatements = ClientBusinessRules.GetMembersWStmts(this);
            //var balancesWithMembers = ClientBusinessRules.GetBalancesWMembers(this);

            // Make outputDocs from each statement, its corresponding member, and any and all balances forward.
            List<OutputDoc> outputDocs = ClientBusinessRules.GetStatementRows(this).ToList();



            return outputDocs;
        }
        /// <summary>
        /// Export SQL Reports. Call after ProcessRecords.
        /// </summary>
        public void ExportReports()
        {
            ExportSQLOutput(memberRecords);
            ExportSQLOutput(statementRecords);
        }
        #endregion

        #region Promises from IExportable_XML
        /// <summary>
        /// Fulfillment of IOut_A_OutputProfile output profile
        /// , that this class will run a lambda meant to check output.
        /// </summary>
        /// <param name="outputs"></param>
        /// <returns></returns>
        public bool Check_Output(DelRet<bool, string[]> outputs)
        {
            return outputs(new String[]{"out.txt"});
        }

        /// <summary>
        /// Fulfillment of IOut_C_XMLOut interface requirement of an output-document-type-specific XMLOutput function.
        /// </summary>
        /// <param name="outputDocs"></param>
        public void XMLExport(List<OutputDoc> outputDocs) 
        {
            Process_XMLOut.Export(outputDocs.AsEnumerable().ToList());
        }

        /// <summary>
        /// Fulfillment of IOut_B_Files interface requirement of a method returning a lambda,
        /// to pss into the Check_Output method promised by IOut_A_OutputProfile.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public DelRet<bool, string[]> GetCheck_File_Output(object[] options)
        {
            return (strs) => 
                strs.Length > 0 
                && ((List<OutputDoc>)(options[0])).Count > 0;

            throw new NotImplementedException();
        } //end method
        #endregion

        #region Promises from IExportable_SQL_TSQL
        /// <summary>
        /// Method to initialize IO_SQLOut with a datatable from Record_Statement File Data Records,
        /// instantiate it, and export it. This fires the SQLBulkCopy adapter.
        /// </summary>
        /// <param name="dataTable">DataTable to be exported.</param>
        public void ExportSQLOutput(FileDataRecords<Record_Statement, ClientETLProcess> dataTable)
        {
            Out_SQLBulkProfile<FileDataRecords<Record_Statement, ClientETLProcess>>.Init(new object[] {dataTable});
            var sqlOutput = new Out_SQLBulkProfile<FileDataRecords<Record_Statement, ClientETLProcess>>(
                "ETLProcess_Record_Statement", SqlBulkCopyOptions.Default);
            sqlOutput.Export(GetCheck_SQL_Output(new object[] { statementRecords }));
        } //end method

        /// <summary>
        /// Method to initialize IO_SQLOut with a datatable from Record_Members File Data Records,
        /// instantiate it, and export it. This fires the SQLBulkCopy adapter.
        /// </summary>
        public void ExportSQLOutput(FileDataRecords<Record_Members, ClientETLProcess> dataTable)
        {
            Out_SQLBulkProfile<FileDataRecords<Record_Members, ClientETLProcess>>.Init(new object[] {dataTable});
            var sqlOutput = new Out_SQLBulkProfile<FileDataRecords<Record_Members, ClientETLProcess>>(
                "ETLProcess_Record_Members", SqlBulkCopyOptions.Default);
            sqlOutput.Export(GetCheck_SQL_Output(new object[] { statementRecords }));
        } //end method
        #endregion
    } // end class
} // end namespace