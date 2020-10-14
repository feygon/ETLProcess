using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.IO;
using ETLProcess.Specific.Documents;
using ETLProcess.General;
using ETLProcess.General.Containers;
using ETLProcess.General.Containers.Members;
using ETLProcess.General.ExtendLinQ;

namespace ETLProcess.Specific
{
    class ClientBusinessRules
    {

        //  // seriously running the query for every single record where this is the case?
        // goofy.
        //            if (MCSB691.Count == 0 && implementation == "W")
        //            {

        //                // Set all documents to the nomail exclusion
        //                //foreach (Document doc in documents)
        //                //{
        //                //    doc.premiumWithhold = "S";
        //                //}

        //                // Perform query of all current member ID in our database
        //                using DataTable uluroWebAccounts = GetClientAccounts.Execute();
        //                // Filter the document list based on this query
        //                for (int iDoc = (documents.Count - 1); (iDoc >= 0); --iDoc)
        //                {
        //                    DocM691_Invoice doc = documents[iDoc];
        //                    // If the docs account/member ID pair exists in the system already then remove it
        //                    if (uluroWebAccounts.Select($@"MEMBERID = '{doc.memberID}' AND ACCOUNTID = '{doc.accountNumber}'").Length > 0)
        //                    {
        //                        documents.RemoveAt(iDoc);
        //                    }
        //                }
        //            }

        /*
            var mem = dataSet.Tables[typeof(Record_Members).Name];
            var stm = dataSet.Tables[typeof(Record_Statement).Name];
            var bal = dataSet.Tables[typeof(Record_BalFwd).Name];
            List<DataRelation> relations = new List<DataRelation>();
            foreach (var x in dataSet.Relations) { relations.Add((DataRelation)x); }

            DataRelation stmFKmem = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_Members).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_stm_FK_mem = new DataRelationKeyColumnComparer(stmFKmem);

            DataRelation stmFKbal = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_BalFwd).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_stm_FK_bal = new DataRelationKeyColumnComparer(stmFKbal);

            DataRelation stmFKmem = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_Members).Name}")).FirstOrDefault();
            DataRelation stmFKbal = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_BalFwd).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_mem_key_bal = new DataRelationKeyColumnComparer(new DataRelation("mem_key_bal", stmFKmem.ChildColumns, stmFKbal.ChildColumns));
        */


        public static IEnumerable<OutputDoc> GetStatementRows(
            DataSet dataSet)
        {
            List<OutputDoc> outputDocs = new List<OutputDoc>();
            var mem = dataSet.Tables[typeof(Record_Members).Name];
            var stm = dataSet.Tables[typeof(Record_Statement).Name];
            var bal = dataSet.Tables[typeof(Record_BalFwd).Name];

            List<DataRelation> relations = dataSet.Relations.Cast<DataRelation>().ToList();
            string memName = mem.TableName;
            string stmName = stm.TableName;
            string balName = bal.TableName;
            List<string> blaChildnames = new List<string>();
            List<string> blaParentnames = new List<string>();
            List<DataRelation> bla = new List<DataRelation>();
            foreach (DataRelation relation in relations) { 
                blaChildnames.Add(relation.ChildTable.TableName);
                blaParentnames.Add(relation.ParentTable.TableName);
            }
            
            DataRelation stmFKbal = relations.Where(
                (x) => ((x.ChildTable.TableName == stm.TableName && x.ParentTable.TableName == bal.TableName)
                || (x.ChildTable.TableName == bal.TableName && x.ParentTable.TableName == stm.TableName))).FirstOrDefault()
                ?? throw new System.ComponentModel.WarningException("Null return.");
            DataRelation stmFKmem = relations.Where(
                (x) => ((x.ChildTable.TableName == stm.TableName && x.ParentTable.TableName == mem.TableName)
                || (x.ChildTable.TableName == mem.TableName && x.ParentTable.TableName == stm.TableName))).FirstOrDefault()
                ?? throw new System.ComponentModel.WarningException("Null return.");
            DataRelationKeyColumnComparer DREC_stm_FK_mem = new DataRelationKeyColumnComparer(stmFKmem);
            DataRelationKeyColumnComparer DREC_stm_FK_bal = new DataRelationKeyColumnComparer(stmFKbal);

            var statementsWithMembers = ClientBusinessRules.GetStmtsWMembers(dataSet);

            foreach (var row in statementsWithMembers)
            {
                IEnumerable<DataRow> rowMembers = DREC_stm_FK_mem.Intersect(new List<DataRow>() { row }.AsEnumerable(), mem.AsEnumerable(), false);
                if (rowMembers.Count() > 1)
                {
                    Log.WriteWarningException(String.Format($"Warning! Multiple members found -- MemberID: " +
                        $"{rowMembers.First().ItemArray[rowMembers.First().Table.PrimaryKey[0].Ordinal]}"));
                }
                DataRow rowMember = rowMembers.FirstOrDefault();
                List<DataRow> rowBalances = DREC_stm_FK_bal.Intersect(new List<DataRow>() { row }.AsEnumerable(), bal.AsEnumerable(), false).ToList();
                outputDocs.Add(new OutputDoc(row, rowMember, rowBalances));
            }

            return outputDocs;
    
        }

        public static IEnumerable<DataRow> GetMembersWOStmts(
            DataSet dataSet)
        {
            var mem = dataSet.Tables[typeof(Record_Members).Name];
            var stm = dataSet.Tables[typeof(Record_Statement).Name];
            List<DataRelation> relations = new List<DataRelation>();
            foreach (var x in dataSet.Relations) { relations.Add((DataRelation)x); }

            DataRelation stmFKmem = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_Members).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_stm_FK_mem = new DataRelationKeyColumnComparer(stmFKmem);
            return DREC_stm_FK_mem.Except(mem.AsEnumerable(), stm.AsEnumerable());
        }

        public static IEnumerable<DataRow> GetStmtsWOMembers(
            DataSet dataSet)
        {
            var mem = dataSet.Tables[typeof(Record_Members).Name];
            var stm = dataSet.Tables[typeof(Record_Statement).Name];
            List<DataRelation> relations = new List<DataRelation>();
            foreach (var x in dataSet.Relations) { relations.Add((DataRelation)x); }

            DataRelation stmFKmem = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_Members).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_stm_FK_mem = new DataRelationKeyColumnComparer(stmFKmem);
            return DREC_stm_FK_mem.Except(stm.AsEnumerable(), mem.AsEnumerable());
        }

        public static IEnumerable<DataRow> GetBalancesWOStmts(
            DataSet dataSet)
        {
            var stm = dataSet.Tables[typeof(Record_Statement).Name];
            var bal = dataSet.Tables[typeof(Record_BalFwd).Name];
            List<DataRelation> relations = new List<DataRelation>();
            foreach (var x in dataSet.Relations) { relations.Add((DataRelation)x); }

            DataRelation stmFKbal = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_BalFwd).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_stm_FK_bal = new DataRelationKeyColumnComparer(stmFKbal);
            return DREC_stm_FK_bal.Except(bal.AsEnumerable(), stm.AsEnumerable());
        }

        public static IEnumerable<DataRow> GetBalancesWStmts(
            DataSet dataSet)
        {
            var stm = dataSet.Tables[typeof(Record_Statement).Name];
            var bal = dataSet.Tables[typeof(Record_BalFwd).Name];
            List<DataRelation> relations = new List<DataRelation>();
            foreach (var x in dataSet.Relations) { relations.Add((DataRelation)x); }

            DataRelation stmFKbal = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_BalFwd).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_stm_FK_bal = new DataRelationKeyColumnComparer(stmFKbal);

            return DREC_stm_FK_bal.Intersect(bal.AsEnumerable(), stm.AsEnumerable(), true);
        }

        public static IEnumerable<DataRow> GetStmtsWMembers(
            DataSet dataSet)
        {
            var mem = dataSet.Tables[typeof(Record_Members).Name];
            var stm = dataSet.Tables[typeof(Record_Statement).Name];
            List<DataRelation> relations = new List<DataRelation>();
            foreach (var x in dataSet.Relations) { relations.Add((DataRelation)x); }

            DataRelation stmFKmem = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_Members).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_stm_FK_mem = new DataRelationKeyColumnComparer(stmFKmem);

            return DREC_stm_FK_mem.Intersect(stm.AsEnumerable(), mem.AsEnumerable(), true);
        }

        public static IEnumerable<DataRow> GetMembersWStmts(
            DataSet dataSet)
        {
            var mem = dataSet.Tables[typeof(Record_Members).Name];
            var stm = dataSet.Tables[typeof(Record_Statement).Name];
            List<DataRelation> relations = new List<DataRelation>();
            foreach (var x in dataSet.Relations) { relations.Add((DataRelation)x); }

            DataRelation stmFKmem = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_Members).Name}")).FirstOrDefault();
            DataRelationKeyColumnComparer DREC_stm_FK_mem = new DataRelationKeyColumnComparer(stmFKmem);

            return DREC_stm_FK_mem.Intersect(mem.AsEnumerable(), stm.AsEnumerable(), true);
        }

        public static IEnumerable<DataRow> GetBalancesWMembers(
            DataSet dataSet)
        {
            var mem = dataSet.Tables[typeof(Record_Members).Name];
            var bal = dataSet.Tables[typeof(Record_BalFwd).Name];
            List<DataRelation> relations = new List<DataRelation>();
            foreach (var x in dataSet.Relations) { relations.Add((DataRelation)x); }
            DataRelation stmFKmem = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_Members).Name}")).FirstOrDefault();
            DataRelation stmFKbal = relations.Where((x) => x.RelationName == string.Format($"{typeof(Record_Statement).Name}_FK_{typeof(Record_BalFwd).Name}")).FirstOrDefault();

            DataRelationKeyColumnComparer DREC_mem_key_bal = new DataRelationKeyColumnComparer(new DataRelation("mem_key_bal", stmFKmem.ChildColumns, stmFKbal.ChildColumns));

            return DREC_mem_key_bal.Intersect(bal.AsEnumerable(), mem.AsEnumerable(), true);
        }

        /// <summary>
        /// Add a graceperiod to a statement end date, to get the due date of that statement.
        /// </summary>
        /// <param name="statementEndDate">The end date of the statement.</param>
        /// <param name="gracePeriod">The number of days of grace period after a statement before payment is due.</param>
        /// <returns>Returns a due Date.</returns>
        internal static DateTime GetDueDate(DateTime statementEndDate, int gracePeriod) {
            return statementEndDate.AddDays(gracePeriod);
        }

        internal static readonly int StatementGracePeriod = 15;
    }
}
