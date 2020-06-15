using ETLProcess.General.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ETLProcess.General.Interfaces
{
    internal interface IOutputDetail : IOutputDoc
    {
        public Type GetChildType();
        // TO DO: A promise that a List of IOutputDetails can be generated from a Table 
        //  filtered by corresponding outputdoc
    }
}
