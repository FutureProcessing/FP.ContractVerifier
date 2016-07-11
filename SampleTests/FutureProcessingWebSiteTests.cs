using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ContractVerifier;
using FP.ContractVerifier;
using NUnit.Framework;

namespace SampleTests
{
    public class FutureProcessingWebSiteTests
    {
        [TestCaseSource("GetContracts")]
        public void ServiceTypes(ServiceContract contract)
        {
            contract.AssertSelf("http://future-processing.com");
        }

        private static List<ServiceContract> GetContracts()
        {
            return ServiceContractsProvider.GetFromJsonFiles(AssemblyHelper.AssemblyDirectory + @"\Contracts\FutureProcessingWebSite");
        }
    }
}
