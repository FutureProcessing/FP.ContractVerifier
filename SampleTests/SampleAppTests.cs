using System.Collections.Generic;
using ContractVerifier;
using FP.ContractVerifier;
using NUnit.Framework;

namespace SampleTests
{
    public class SampleAppTests
    {
        [TestCaseSource("GetSampleAppContracts")]
        public void SampleAppContracts(ServiceContract contract)
        {
            contract.AssertSelf("http://localhost:33991/api/");
        }

        private static List<ServiceContract> GetSampleAppContracts()
        {
            return ServiceContractsProvider.GetFromJsonFiles(AssemblyHelper.AssemblyDirectory + @"\Contracts\SampleAppContracts");
        }
    }
}