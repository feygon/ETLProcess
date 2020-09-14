using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.Containers;
using ETLProcess.Specific.Documents;

namespace ETLProcess.General.Interfaces
{
    public interface IOutputDoc
    {
        public IOutputDoc Record(DataRow data, object[] obj = null);
    }
}