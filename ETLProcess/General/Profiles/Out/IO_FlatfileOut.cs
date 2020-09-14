using System;
using ETLProcess.General.Containers.AbstractClasses;
using ETLProcess.General.IO;

namespace ETLProcess.General.Profiles
{
    internal class IO_FlatfileOut : SingletonProfile<IO_FlatfileOut>, IDisposable {

        public IO_FlatfileOut() : base(typeof(IO_FlatfileOut), null) {
            throw new NotImplementedException();
        }

        public IO_FlatfileOut(Exception err) : base(typeof(IO_FlatfileOut), null) {
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
