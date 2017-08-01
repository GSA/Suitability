using System.Data;

namespace Suitability
{
    class ContractDetails
    {
        //Default to empty string
        public string ContractorCompany = string.Empty;
        public string ContractNumber = string.Empty;

        //Should change to generic
        /// <summary>
        /// Takes an IDataRecord and returns the contract details
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static ContractDetails Create(IDataRecord record)
        {
            return new ContractDetails
            {
                ContractNumber = record["Contract Number"].ToString(),
                ContractorCompany = record["Contractor Company"].ToString() 
            };
        }
    }
}
