using System;
using System.Runtime.Serialization;
namespace ETLProcess
{
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
    }
}
