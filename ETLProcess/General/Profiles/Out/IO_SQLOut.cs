using System;
using ETLProcess.General.Containers.AbstractClasses;

namespace ETLProcess.General.Profiles
{
    // see Linq to SQL: https://docs.microsoft.com/en-us/dotnet/api/system.data.linq.mapping?view=netframework-4.8
    internal class IO_SQLOut : SingletonProfile<IO_SQLOut>, IDisposable
    {
        public IO_SQLOut() : base(typeof(IO_SQLOut), null) {
            throw new NotImplementedException();
        }

        public IO_SQLOut(Exception err) : base(typeof(IO_SQLOut), null) {
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