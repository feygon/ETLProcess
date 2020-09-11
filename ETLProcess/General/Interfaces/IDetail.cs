﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProcess.General.Containers;

namespace ETLProcess.General.Interfaces
{
    /// <summary>
    /// An interface to promise that each detail in a document will implement (a) certain method(s).
    /// </summary>
    /// <typeparam name="TDetail"></typeparam>
    interface IDetail<TDetail> where TDetail : IOutputDoc
    {
        /// <summary>
        /// Instantiate Detail element.
        /// </summary>
        /// <returns></returns>
        public TDetail GetDetail(Dictionary<string, string> stringMap, List<string> headers);

    }
}