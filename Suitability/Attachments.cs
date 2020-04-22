using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suitability
{
    class Attachments
    {
        /// <summary>
        /// Returns instructions based on region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public string ApplicaitonInstruction(string region, string invest)
        {
            switch (region)
            {
                case "01":
                case "02":
                case "03":
                    return getPdfForZoneAndInvestType("A", invest);
                case "04":
                case "07":
                    return getPdfForZoneAndInvestType("B", invest);
                case "05":
                case "06":
                case "08":
                    return getPdfForZoneAndInvestType("C", invest);
                case "09":
                case "10":
                    return getPdfForZoneAndInvestType("D", invest);
                case "NCR":
                case "CO":
                    return getPdfForZoneAndInvestType("E", invest);
                default:
                    return "";
            }
        }

        string getPdfForZoneAndInvestType(string zone, string investType)
        {
            switch(investType.ToLower())
            {
                case "tier 1":
                case "naci":
                    return string.Format("T1Zone{0}-ApplicationInstructions.pdf", zone);
                case "tier 2s":
                case "mbi":
                case "tier 2":
                    return string.Format("T2Zone{0}-ApplicationInstructions.pdf", zone);
                case "tier 4":
                case "bi":
                    return string.Format("T4Zone{0}-ApplicationInstructions.pdf", zone);
                default:
                    return "";
            }
        }

        /// <summary>
        /// Returns State Form based on region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public string StateForm(string region)
        {
            switch (region)
            {
                case "01":
                case "02":
                case "03":
                    return "ZoneA-StateForm.pdf";
                case "04":
                case "07":
                    return "ZoneB-StateForm.pdf";
                case "05":
                case "06":
                case "08":
                    return "ZoneC-StateForm.pdf";
                case "09":
                case "10":
                    return "ZoneD-StateForm.pdf";
                case "NCR":
                case "CO":
                    return "ZoneE-StateForm.pdf";
                default:
                    return "";
            }
        }

    }
}
