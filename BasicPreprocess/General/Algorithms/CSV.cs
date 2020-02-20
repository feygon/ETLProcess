﻿using System;
using System.Collections.Generic;

using System.IO;
using Microsoft.VisualBasic.FileIO;
using BasicPreprocess.Specific;
using static BasicPreprocess.Parse;
using BasicPreprocess.General.Containers;
using BasicPreprocess.General;
using BasicPreprocess.General.IO;
using BasicPreprocess.Specific.Boilerplate;

using String = System.String;
using AcctID = System.String;
using MemberID = System.String;

namespace BasicPreprocess.General.Algorithms
{
    using StringMap = Dictionary<string, string>;
    /// <summary>
    /// A class of static algorithms, for parsing CSV input files.
    /// </summary>
    public class CSV
    {
        /// <summary>
        /// Imports a single file by row. Each string object in the list is 1 row in the file.
        /// </summary>
        /// <param name="fileName">The full path of the file to import</param>
        /// <returns>A List of 1 string per row</returns>
        public static List<string> ImportRows(string fileName)
        {
            var result = new List<string>();
            using (Stream readFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (TextReader textReader = new StreamReader(readFile))
                {
                    // Row parsing
                    string input = "";
                    while ((input = textReader.ReadLine()) != null)
                    {
                        result.Add(input);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Deprecated.
        /// <br>Imports a single file as a delimited file. The list is a collection of the delimited rows.</br>
        /// </summary>
        /// <param name="fileName">The ful path of the file to import</param>
        /// <param name="delimiter"></param>
        /// <param name="useQuotes"></param>
        /// <returns>A List of string[] per row</returns>
        private static List<string[]> ImportCSV(string fileName, string delimiter = ",", bool useQuotes = false)
        {
            var result = new List<string[]>();
            using (Stream readFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // CSV Parsing
                var csvRead = new TextFieldParser(readFile)
                {
                    CommentTokens = new[] { "#" },
                    Delimiters = new[] { delimiter },
                    HasFieldsEnclosedInQuotes = useQuotes,
                    TextFieldType = FieldType.Delimited,
                    TrimWhiteSpace = true
                };
                // Load result
                while (!csvRead.EndOfData)
                {
                    result.Add(csvRead.ReadFields());
                }
            }
            return result;
        }

        // this implementation requires reverse engineering every time.
        /// <summary>
        /// Imports a single file as a delimited file with a header. Row 1 is always parsed as a header, and is used to construct resulting dictionaries
        /// by row. Each dict is row 1 as the keys and each following row of the file as the values.
        /// </summary>
        /// <param name="fileName">The full path of the file to import</param>
        /// <param name="delimiter">What is the delimiting character? i.e. comma, pipe, tab, etc.</param>
        /// <param name="useQuotes">Are there quotes around values?</param>
        /// <param name="primaryKey">Primary key -- major index of composite if composite.</param>
        /// <param name="headers">A preloaded set of headers -- optional.</param>
        /// <returns>A List of Dictionary per row where KEY=Row1</returns>
        public  static HeaderSource<List<StringMap>, List<string>> ImportCSVWithHeader(
            string fileName
            , string primaryKey
            , string delimiter
            , bool useQuotes
            , IList<string> headers = null)
        {
            using (Stream readFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // CSV Parsing
                var csvRead = new TextFieldParser(readFile)
                {
                    CommentTokens = new[] { "#" },
                    Delimiters = new[] { delimiter },
                    HasFieldsEnclosedInQuotes = useQuotes,
                    TextFieldType = FieldType.Delimited,
                    TrimWhiteSpace = true
                };

                // if header is null, replace header with csvRead.ReadFields(), or with empty string if that's null.
                headers ??= csvRead.ReadFields() ?? new string[] { }; // no action if headers provided in arguments.

                List<StringMap> records = new List<StringMap>();
                while (!csvRead.EndOfData)
                {
                    string[] rowData = csvRead.ReadFields() ?? new string[] { };
                    var newRow = new StringMap();
                    for (int n = 0; n < rowData.Length; ++n) // len = number of fields.
                    {
                        newRow.Add(headers[n], rowData[n]);
                    }
                    records.Add(newRow);
                }

                List<string> headerList = new List<string>(headers);
                HeaderSource<List<StringMap>, List<string>> ret =
                    new HeaderSource<List<StringMap>, List<string>>(records, headerList.ToArray());


                return ret;

            }
        }
    }
}
