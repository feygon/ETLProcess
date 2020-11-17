using System;
using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.IO;

namespace ETLProcessFactory.Profiles
{
    internal class Out_FlatfileProfile : SingletonProfile<Out_FlatfileProfile>, IDisposable {

        public Out_FlatfileProfile() : base(typeof(Out_FlatfileProfile), null) {
            throw new NotImplementedException();
            Console.WriteLine("TO DO: Implement this");
        }

        public Out_FlatfileProfile(Exception err) : base(typeof(Out_FlatfileProfile), null) {
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