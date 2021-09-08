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

        public Address(
            string firstName,
            string lastName,
            string line1,
            string line2,
            string city,
            string state, 
            string zip)
        {
            bool twoNames = (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName));
            this.name = $"{firstName}{(twoNames ? "" : " ")}{lastName}";
            this.line1 = line1;
            this.line2 = line2;
            this.city = city;
            this.state = state;
            this.zip = zip;
        }
    }
}
