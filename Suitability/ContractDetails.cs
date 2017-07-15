using System.Data;

namespace Suitability
{
    class ContractDetails
    {
        public string ContractorCompany = string.Empty;
        public string ContractNumber = string.Empty;

        //Should change to generic
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
