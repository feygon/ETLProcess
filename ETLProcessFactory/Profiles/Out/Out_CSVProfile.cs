using System;

using ETLProcessFactory.IO;
using ETLProcessFactory.Containers.AbstractClasses;

namespace ETLProcessFactory.Profiles
{
    internal class Out_CSVProfile : SingletonProfile<Out_CSVProfile>, IDisposable {
        public Out_CSVProfile() : base(typeof(Out_CSVProfile), null) {
            throw new NotImplementedException();
            Console.WriteLine("TO DO: Implement this");
        }

        public Out_CSVProfile(Exception err) : base(typeof(Out_CSVProfile), null) {
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