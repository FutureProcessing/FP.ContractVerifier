using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContractVerifier
{
    public static class ServiceContractsProvider
    {
        public static List<ServiceContract> GetFromJsonFiles(string contractsFolder)
        {
            var files = Directory.GetFiles(contractsFolder);
            var serviceContracts = new List<ServiceContract>(files.Length);
            foreach (var file in files)
            {
                var readAllText = File.ReadAllText(file);
                var deserilizedContract = JsonConvert.DeserializeObject(readAllText);
                var contractsArray = deserilizedContract as JArray ?? new JArray { deserilizedContract as JObject };

                foreach (JObject contractProperties in contractsArray)
                {
                    var serviceContract = CreateServiceContract(contractProperties, Path.GetFileNameWithoutExtension(file));
                    serviceContracts.Add(serviceContract);
                }
            }

            return serviceContracts;
        }

        private static ServiceContract CreateServiceContract(JObject contractProperties, string contractName)
        {
            var serviceContract = new ServiceContract();
            serviceContract.ContractName = GetKeyOrDefault(contractProperties, "contractName", contractName);
            serviceContract.HttpMethod = GetKeyOrDefault(contractProperties, "httpMethod", "GET");
            serviceContract.Url = GetKeyOrDefault(contractProperties, "url", string.Empty);
            serviceContract.RequestBody = GetKeyOrDefault(contractProperties, "requestBody", (object) null);
            serviceContract.ExpectedStatusCodeRegExp = GetKeyOrDefault(contractProperties, "expectedStatusCode", "200");
            serviceContract.ExpectedResponseObject = GetKeyOrDefault(contractProperties, "expectedResponseObject", (JObject) null);
            serviceContract.ExpectedResponseArrayObjects = GetKeyOrDefault(contractProperties, "expectedResponseArrayObjects", (JArray)null);
            serviceContract.ExpectedResponseArrayValues = GetKeyOrDefault(contractProperties, "expectedResponseArrayValues", (JArray)null);
            serviceContract.NotExpectedResponseArrayValues = GetKeyOrDefault(contractProperties, "notExpectedResponseArrayValues", (JArray)null);
            serviceContract.ExpectedResponseObjectKeys = GetKeyOrDefault(contractProperties, "expectedResponseObjectKeys", (JArray) null);
            serviceContract.NotExpectedResponseArrayObjects = GetKeyOrDefault(contractProperties, "notExpectedResponseArrayObjects", (JArray)null);
            serviceContract.DisableDbRestore = GetKeyOrDefault(contractProperties, "disableDbRestore", false);

            var sqlQueryArray = GetKeyOrDefault(contractProperties, "sqlQueryAfter", (JArray) null);
            if (sqlQueryArray != null)
            {
                string[] queries = sqlQueryArray.Select(q => q.ToString()).ToArray();
                serviceContract.SqlQueryAfter = string.Join(string.Empty, queries);
            }

            return serviceContract;
        }

        public static List<ServiceContract> GetFromJsonFile(string path)
        {
            var serviceContracts = new List<ServiceContract>();
            var readAllText = File.ReadAllText(path);
            var deserilizedContract = JsonConvert.DeserializeObject(readAllText);
            var contractsArray = deserilizedContract as JArray ?? new JArray { deserilizedContract as JObject };

            foreach (JObject contractProperties in contractsArray)
            {
                var serviceContract = CreateServiceContract(contractProperties, Path.GetFileNameWithoutExtension(path));
                serviceContracts.Add(serviceContract);
            }
            
            return serviceContracts;
        }

        private static T GetKeyOrDefault<T>(JObject contratProperties, string key, T defaultValue)
        {
            return contratProperties[key] != null ? JsonConvert.DeserializeObject<T>(contratProperties[key].ToString(Formatting.None)) : defaultValue;
        }
    }
}