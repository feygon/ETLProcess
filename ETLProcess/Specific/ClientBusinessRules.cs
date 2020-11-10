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
    /// <summary>
    /// Sequestered client business rules.
    /// <para>This is a class for the containment and sequestration of client-specified business rules.
    /// For instance, this particular implementation has a business rule that an output document consists of:
    ///     A statement matched to one and only one member, with zero to many balance forward details.
    ///     See: <see cref="ClientBusinessRules.GetStatementRows(DataSet)"/>.
    /// </para>
    /// </summary>
    public class ClientBusinessRules
    {
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

        /// <summary>
        /// Get statement rows from a dataset, with their accompanying data, and return a collection of outputdocuments.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a collection of members where there does not exist a corresponding statement.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a collection of statements where there does not exist a corresponding member.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a collection of balances where there does not exists any corresponding statement.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get balances which have corresponding statements.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get statements which have corresponding members.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get members which have corresponding statements.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get balances which have corresponding members.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
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
