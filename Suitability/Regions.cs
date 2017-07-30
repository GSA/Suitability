using System.Linq;
using System.Xml.Linq;

namespace Suitability
{
	class Regions
	{
		public Regions(){}

        /// <summary>
        /// Get regional email of region passed in from xml doc located in location passed in
        /// </summary>
        /// <param name="region"></param>
        /// <param name="location"></param>
        /// <returns></returns>
		public string GetRegionalEMails(string region, string location)
		{
            //Load xml doc
			XDocument XMLDoc = XDocument.Load(location);

			//Not ideal but gets the job done (will be better once we move into the DB)
			var Contact = (from xml2 in XMLDoc.Descendants("Region")
							   where xml2.Element("ID").Value == region
							   select xml2.Element("EMail").Value).FirstOrDefault();

			if (Contact == null)
				return string.Empty;

			return Contact.ToString().Trim();
		}

        /// <summary>
        /// Get regional phone number of region from xml located at location
        /// </summary>
        /// <param name="region"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public string GetRegionalPhoneNumbers(string region, string location)
        {
            //Load xml doc
            XDocument XMLDoc = XDocument.Load(location);

            //Not ideal but gets the job done (will be better once we move into the DB)
            var PhoneNumber = (from xml2 in XMLDoc.Descendants("Region")
                           where xml2.Element("ID").Value == region
                           select xml2.Element("PhoneNumber").Value).FirstOrDefault();

            return (PhoneNumber == null) ? string.Empty : PhoneNumber.ToString().Trim();
        }
	}
}
