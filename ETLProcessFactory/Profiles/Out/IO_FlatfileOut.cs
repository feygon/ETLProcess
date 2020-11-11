using System;
using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.IO;

namespace ETLProcessFactory.Profiles
{
    internal class IO_FlatfileOut : SingletonProfile<IO_FlatfileOut>, IDisposable {

        public IO_FlatfileOut() : base(typeof(IO_FlatfileOut), null) {
            throw new NotImplementedException();
            Console.WriteLine("TO DO: Implement this");
        }

        public IO_FlatfileOut(Exception err) : base(typeof(IO_FlatfileOut), null) {
            throw new NotImplementedException();
            Console.WriteLine("TO DO: Implement this");
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