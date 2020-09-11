using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.Specific;
using ETLProcess.General.Containers;
using ETLProcess.General.Interfaces;
using ETLProcess.General.IO;

namespace ETLProcess.Specific.Information
{
    internal class Info_CSVFile<T> where T : BasicRecord<T>, IRecord<T>, new()
    {
        // TO DO: Should this be a singleton?


        string target;
        Dictionary<string, Dictionary<string, bool>> headerAliases = new Dictionary<string, Dictionary<string, bool>>();

        public Info_CSVFile(string targetFileName) {
            target = targetFileName;
            // TO DO: Should this be a singleton?
        }

        /// <summary>
        /// Add an alias to the singleton dictionary.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="original"></param>
        public void AddAlias(string alias, string original)
        {
            if (!headerAliases.ContainsKey(original)) {
                headerAliases.Add(original, new Dictionary<string, bool>() { { original, true } });
                if ((string.Compare(alias, original) != 0)
                    && (string.Compare(alias.ToLowerInvariant(), original.ToLowerInvariant()) != 0))
                {
                    headerAliases.Add(original, new Dictionary<string, bool>() { { alias, true } });
                }
            } else { 
                headerAliases[original].Add(alias, true); 
            }
        }

        /// <summary>
        /// Find the header an alias is listed under, if it exists.
        /// </summary>
        /// <param name="alias">The alias string to find.</param>
        /// <returns>Return null for not found.</returns>
        public string GetHeaderByAlias(string alias)
        {
            foreach (var header in headerAliases) {
                if (header.Value.ContainsKey(alias)) { return header.Key; }
                if (header.Value.ContainsKey(alias.ToLowerInvariant())) { return header.Key; }
                if (header.Value.ContainsKey(alias.ToUpperInvariant())) { return header.Key; }
            }
            return null;
        }

        public void Contains(string alias)
        {
            foreach (var header in headerAliases)
            {

            }
        }

    }
}
