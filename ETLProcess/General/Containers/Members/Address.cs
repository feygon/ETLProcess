using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
namespace ETLProcess.General.Containers.Members
{
    /// <summary>
    /// Wrapper for generic static function to be populated by the end-developer and used below.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Address<T> {
        /// <summary>
        /// Generic static function to be populated by the end-developer 
        ///     and used in generic overload of Address.ParseAddress, below.>"/>.
        /// </summary>
        public static DelRet<T, Address> Func =
            (Address address) => { throw new NotImplementedException("Lambda not populated for this generic type. Did you get T right?"); };
    }
    /// <summary>
    /// Container for postal address-related information.
    /// </summary>
    [Serializable]
	public class Address
	{
        private readonly Address m_ad;
        /// <summary>
        /// A string, as named.
        /// </summary>
		public string name,
            line1,
            line2,
            line3,
            line4,
            city,
            state,
            zip,
            country;

        private Address(SerializationInfo info, StreamingContext context)
        {
            // TO DO: Investigate whether this boilerplate serializeation constructor, 
            //  or indeed serialization at all, is needed.
            m_ad = new Address();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Address()
        {
            name = "";
            line1 = "";
            line2 = "";
            line3 = "";
            line4 = "";
            city = "";
            state = "";
            zip = "";
            country = "";
        }

        /// <summary>
        /// Optional Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="line3"></param>
        /// <param name="line4"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zip"></param>
        /// <param name="country"></param>
        public Address(
            string name = ""
            , string line1 = ""
            , string line2 = ""
            , string line3 = ""
            , string line4 = ""
            , string city = ""
            , string state = ""
            , string zip = ""
            , string country = "")
        {
            this.name = name;
            this.line1 = line1;
            this.line2 = line2;
            this.line3 = line3;
            this.line4 = line4;
            this.city = city;
            this.state = state;
            this.zip = zip;
            this.country = country;
        }

        /// <summary>
        /// A handler for user-designer to input a lambda dictating the
        ///     way an address generates a tuple as output.
        /// </summary>
        /// <typeparam name="T">The configuration of the tuple, including field names.</typeparam>
        /// <param name="func">The lambda to instantiate that tuple.</param>
        /// <param name="address">The address to parse.</param>
        /// <returns></returns>
        public static T ParseAddress<T>(Address address, DelRet<T, Address> func = null)
        {
            if (func == null)
            {
                return Address<T>.Func(address);
            } else
            {
                return func(address);
            }
        }

        /// <summary>
        /// A simple boilerplate way to generate address strings from an address,
        ///     using only lines 1-4.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static (string line1, string line2, string line3, string line4)ParseAddress(Address address)
        {
            return (address.line1, address.line2, address.line3, address.line4);
        }
    }
}
