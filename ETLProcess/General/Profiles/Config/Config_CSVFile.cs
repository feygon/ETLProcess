﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using ETLProcess.General.Containers;
using ETLProcess.General.Interfaces;
using ETLProcess.General.IO;
using ETLProcess.General.Algorithms;
using ETLProcess.General.Containers.AbstractClasses;

namespace ETLProcess.Specific.Information
{
    internal class Config_CSVFile<T> : SingletonProfile<Config_CSVFile<T>>, IDisposable where T : BasicRecord<T>, IRecord<T>, new()
    {
        // TO DO: Should this be a singleton?
        string target;
        List<string> headers;
        Dictionary<string, IEnumerable<string>> headerAliases = new Dictionary<string, IEnumerable<string>>();

        public Config_CSVFile(string targetFileName, string delimiter = ",", bool useQuotes = false)
            : base(typeof(Config_CSVFile<T>)
                  , new object[] { targetFileName, delimiter, useQuotes })
        {
            target = targetFileName;
            headers = CSV.ImportCSV(targetFileName, delimiter, useQuotes).First().ToList();
            List<string[]> lines = CSV.ImportCSV(targetFileName, delimiter, useQuotes).ToList();
            // Add a dictionary item with a list of aliases for each header, including the key.
            for (int i = 0; i < headers.Count; i++) {
                headerAliases.Add(headers[i], from line in lines select line[i]);
            }
        }

        public Config_CSVFile(string targetFileName, List<string> headers, string delimiter = ",", bool useQuotes = false)
            : base(typeof(Config_CSVFile<T>)
                  , new object[] { targetFileName, delimiter, useQuotes })
        {
            target = targetFileName;
            bool existed = File.Exists(target);
            var csvReadWrite = File.OpenWrite(target);
            if (existed) {
                checkHeaders(headers, delimiter, useQuotes);
            } else {
                createHeaders(headers, delimiter, useQuotes);
            }
        }

        private void createHeaders(List<string> headers, string delimiter, bool useQuotes)
        {
            throw new NotImplementedException();
        }

        private void checkHeaders(List<string> headers, string delimiter, bool useQuotes)
        {
            bool fail = true;
            // check stuff.
            if (fail) {
                // output detailed comparison of headerAliases and provided headers.
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add an alias to the singleton dictionary.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="original"></param>
        public void AddAlias(string alias, string original) {
            if (!headerAliases.ContainsKey(original)) {
                headerAliases.Add(original, new List<string>() { original });
                if (string.Compare(alias, original,StringComparison.InvariantCulture) != 0)
                {
                    headerAliases[original].Append(alias);
                }
            } else { 
                headerAliases[original].Append(alias); 
            }
        }

        /// <summary>
        /// Find the header an alias is listed under, if it exists.
        /// </summary>
        /// <param name="alias">The alias string to find.</param>
        /// <returns>Return null for not found.</returns>
        public string GetHeaderByAlias(string alias)
        {
            IEnumerable<string> ret = headerAliases.Keys.Where((key) => headerAliases[key].Contains(
                alias, StringComparer.InvariantCulture));
            if (ret.Count() > 1) { Log.WriteWarningException("Warning: Requested alias returns multiple matching headers."); }
            return ret.FirstOrDefault();
        }

        public void Contains(string alias)
        {
            foreach (var header in headerAliases)
            {

            }
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
