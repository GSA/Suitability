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
			return GetXmlElement("EMail", region, location);
        }

        /// <summary>
        /// Get regional phone number of region from xml located at location
        /// </summary>
        /// <param name="region"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public string GetRegionalPhoneNumbers(string region, string location)
        {
            return GetXmlElement("PhoneNumber", region, location);
        }

        /// <summary>
        ///     Gets a value from the specified element of a region and location
        /// </summary>
        /// <param name="element">The element to return</param>
        /// <param name="region">The region</param>
        /// <param name="location">The location</param>
        /// <returns>The value of the specified element if found; otherwise <see cref="string.Empty"/></returns>
	    private static string GetXmlElement(string element, string region, string location)
	    {
	        //Load xml doc
	        var xmlDoc = XDocument.Load(location);

	        //Not ideal but gets the job done (will be better once we move into the DB)
	        var elementValue = (from xml2 in xmlDoc.Descendants("Region")
	                           where xml2.Element("ID")?.Value == region
	                           select xml2.Element(element)?.Value).FirstOrDefault();

	        return (elementValue == null) ? string.Empty : elementValue?.Trim();
        }
	}
}
