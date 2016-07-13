using System.Collections.Generic;
using ContractVerifier;
using FP.ContractVerifier;
using NUnit.Framework;

namespace SampleTests
{
    // Separeted test methods in this class are created only for purpose of test results readability.
    // Can be substituded with single method, that asserts all contracts.
    public class SampleAppTestsSamples
    {
        private readonly SampleAppTests _sampleAppTests2 = new SampleAppTests();

        [TestCaseSource("GetExpectedStatusCodeSamplesContracts")]
        public void ExpectedStatusCodeSamples(ServiceContract contract)
        {
            contract.AssertSelf("http://localhost:33991/api/");
        }

        [TestCaseSource("GetExpectedResponseObjectAndObjectKeysSamplesContracts")]
        public void ExpectedResponseObjectAndObjectKeysSamples(ServiceContract contract)
        {
            contract.AssertSelf("http://localhost:33991/api/");
        }

        [TestCaseSource("GetExpectedResponseArrayValuesAndArrayObjectsSamplesContracts")]
        public void ExpectedResponseArrayValuesAndArrayObjectsSamples(ServiceContract contract)
        {
            contract.AssertSelf("http://localhost:33991/api/");
        }

        [TestCaseSource("GetNotExpectedResponseArrayValuesAndArrayObjectsSamplesContracts")]
        public void NotExpectedResponseArrayValuesAndArrayObjectsSamples(ServiceContract contract)
        {
            contract.AssertSelf("http://localhost:33991/api/");
        }

        private static List<ServiceContract> GetExpectedStatusCodeSamplesContracts()
        {
            return ServiceContractsProvider.GetFromJsonFile(AssemblyHelper.AssemblyDirectory + @"\Contracts\SampleAppSamples\ExpectedStatusCodeSamples.json");
        }

        private static List<ServiceContract> GetExpectedResponseObjectAndObjectKeysSamplesContracts()
        {
            return ServiceContractsProvider.GetFromJsonFile(AssemblyHelper.AssemblyDirectory + @"\Contracts\SampleAppSamples\ExpectedResponseObjectAndObjectKeysSamples.json");
        }

        private static List<ServiceContract> GetExpectedResponseArrayValuesAndArrayObjectsSamplesContracts()
        {
            return ServiceContractsProvider.GetFromJsonFile(AssemblyHelper.AssemblyDirectory + @"\Contracts\SampleAppSamples\ExpectedResponseArrayValuesAndArrayObjectsSamples.json");
        }
        
        private static List<ServiceContract> GetNotExpectedResponseArrayValuesAndArrayObjectsSamplesContracts()
        {
            return ServiceContractsProvider.GetFromJsonFile(AssemblyHelper.AssemblyDirectory + @"\Contracts\SampleAppSamples\NotExpectedResponseArrayValuesAndArrayObjectsSamples.json");
        }
    }
}
