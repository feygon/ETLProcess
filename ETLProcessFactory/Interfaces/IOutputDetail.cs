using ETLProcessFactory.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ETLProcessFactory.Interfaces
{
    public interface IOutputDetail : IOutputDoc
    {
        public Type GetChildType();
        // TO DO: A promise that a List of IOutputDetails can be generated from a Table 
        //  filtered by corresponding outputdoc
    }
}
