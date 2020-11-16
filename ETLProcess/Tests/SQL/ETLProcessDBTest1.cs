using ETLProcess.Specific;
using System;
using System.Data.Entity;

namespace ETLProcess.Tests.SQL
{
    
    public class ETLProcessDBTest1 : DbContext
    {
        public DbSet<Record_Statement> StatementSet { get; set; }
        public ETLProcessDBTest1()
        {
            
        }
    }
}
