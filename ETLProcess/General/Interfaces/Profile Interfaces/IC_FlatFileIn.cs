
﻿using System.Collections.Generic;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// Promise that a class will implement methods specific to taking in position-delimited data files.
    /// </summary>
    public interface IC_FlatFileIn
    {
        /// <summary>
        /// Promise that a flatfile profile using this interface will provide a schema
        ///     for scraping substrings from a flat file, for use with Parse.GetSubString(3)
        /// </summary>
        public Dictionary<string, (int colStart, int colEnd, int row)> Substrings { get; }
        /// <summary>
        /// Number of lines in a record.
        /// </summary>
        public int LinesPerRecord { get; }
        /// <summary>
        /// Strings which mark the beginning and/or end of a record.
        /// </summary>
        public (string recordBegin, string recordEnd) RecordMarkers { get; }
    }

}