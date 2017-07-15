using System.Linq;
using System.Xml.Linq;

namespace Suitability
{
	class Regions
	{
		public Regions()
		{
			
		}

		public string GetRegionalEMails(string region, string location)
		{
			XDocument XMLDoc = XDocument.Load(location);

			//Not ideal but gets the job done (will be better once we move into the DB)
			var Contact = (from xml2 in XMLDoc.Descendants("Region")
							   where xml2.Element("ID").Value == region
							   select xml2.Element("EMail").Value).FirstOrDefault();

			if (Contact == null)
				return string.Empty;

			return Contact.ToString().Trim();
		}

        public string GetRegionalPhoneNumbers(string region, string location)
        {
            XDocument XMLDoc = XDocument.Load(location);

            //Not ideal but gets the job done (will be better once we move into the DB)
            var PhoneNumber = (from xml2 in XMLDoc.Descendants("Region")
                           where xml2.Element("ID").Value == region
                           select xml2.Element("PhoneNumber").Value).FirstOrDefault();

            return (PhoneNumber == null) ? string.Empty : PhoneNumber.ToString().Trim();
        }		
	}
}
