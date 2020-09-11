using System;
using ETLProcess.General.Containers.AbstractClasses;

namespace ETLProcess.General.Profiles
{
    // See Linq to XML: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/linq-to-xml-overview
    internal class IO_XMLOut : SingletonProfile<IO_XMLOut>, IDisposable {

        public IO_XMLOut() : base(typeof(IO_XMLOut), null) {
            throw new NotImplementedException();
        }

        public IO_XMLOut(Exception err) : base(typeof(IO_XMLOut), null) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            if (firstRun) {
                disposable.Dispose();
                GC.SuppressFinalize(this);
            } else {
                Log.WriteException("Dispose called on singleton or non-initial instance.");
            }
        }
    }
}
