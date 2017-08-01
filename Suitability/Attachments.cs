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
        public string ApplicaitonInstruction(string region)
        {
            switch (region)
            {
                case "01":
                case "02":
                case "03":
                    return "ZoneA-ApplicationInstructions.pdf";
                case "04":
                case "07":
                    return "ZoneB-ApplicationInstructions.pdf";
                case "05":
                case "06":
                case "08":
                    return "ZoneC-ApplicationInstructions.pdf";
                case "09":
                case "10":
                    return "ZoneD-ApplicationInstructions.pdf";
                case "NCR":
                case "CO":
                    return "ZoneE-ApplicationInstructions.pdf";
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
