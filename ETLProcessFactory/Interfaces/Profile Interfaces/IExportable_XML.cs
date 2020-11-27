using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using ETLProcessFactory.Containers.AbstractClasses;
using ETLProcessFactory.Profiles;

namespace ETLProcessFactory.Interfaces.Profile_Interfaces
{
    /// <summary>
    /// Promise that a class will implement methods specific to outputting XML data files.
    /// <list type="table">
    /// <listheader>
    ///  <term>Name</term><term>Operator</term><term>Description</term>
    /// </listheader>
    /// <item>
    ///  <term><typeparamref name="Singleton"/></term><term> Type Argument</term><term> A reference of the <see cref="SingletonProfile{T}"/> class where T is <see cref="Out_XMLProfile"/></term>
    /// </item> <item>
    ///  <term><typeparamref name="TOutput"/></term><term> Type Argument</term><term> An <see cref="ISerializable"/> class to be used as a Type Argument in the XMLExport(List{TOutput})"/> method.</term>
    /// </item>
    /// </list>
    /// </summary>
    public interface IExportable_XML<Singleton, TOutput> : IIExportable_ToFile 
        where Singleton : SingletonProfile<Out_XMLProfile>
        where TOutput : ISerializable
    {
        /// <summary>
        /// Promise that a specific implementation will have methods to export to XML 
        ///     from a list of serializable outputdocs of generic type T.
        /// <para>Note: It is recommended to use <see cref="Out_XMLProfile.Export{T}()"/> in this method.</para>
        /// </summary>
        /// <param name="outputDocs">List of serializable docs.</param>
        public void XMLExport();
    }
}