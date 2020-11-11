using System;

using ETLProcessFactory.IO;
using ETLProcessFactory.Containers.AbstractClasses;

namespace ETLProcessFactory.Profiles
{
    internal class IO_CSVOut : SingletonProfile<IO_CSVOut>, IDisposable {
        public IO_CSVOut() : base(typeof(IO_CSVOut), null) {
            throw new NotImplementedException();
            Console.WriteLine("TO DO: Implement this");
        }

        public IO_CSVOut(Exception err) : base(typeof(IO_CSVOut), null) {
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