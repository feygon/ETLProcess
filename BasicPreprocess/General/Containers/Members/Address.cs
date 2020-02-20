namespace BasicPreprocess
{
    /// <summary>
    /// Container for postal address-related information.
    /// </summary>
	public class Address
	{
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
