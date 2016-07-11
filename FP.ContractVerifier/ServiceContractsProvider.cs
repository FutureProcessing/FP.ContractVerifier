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
                    var serviceContract = new ServiceContract();
                    serviceContracts.Add(serviceContract);
                    serviceContract.ContractName = GetKeyOrDefault(contractProperties, "contractName", Path.GetFileNameWithoutExtension(file));
                    serviceContract.HttpMethod = GetKeyOrDefault(contractProperties, "httpMethod", "GET");
                    serviceContract.Url = GetKeyOrDefault(contractProperties, "url", string.Empty);
                    serviceContract.RequestBody = GetKeyOrDefault(contractProperties, "requestBody", (object)null);
                    serviceContract.ExpectedStatusCodeRegExp = GetKeyOrDefault(contractProperties, "expectedStatusCode", "200");
                    serviceContract.ExpectedResponseObject = GetKeyOrDefault(contractProperties, "expectedResponseObject", (JObject)null);
                    serviceContract.ExpectedResponseObjectsArray = GetKeyOrDefault(contractProperties, "expectedResponseObjectsArray", (JArray)null);
                    serviceContract.ExpectedResponseArray = GetKeyOrDefault(contractProperties, "expectedResponseArray", (JArray)null);
                    serviceContract.UnexpectedResponseArray = GetKeyOrDefault(contractProperties, "unexpectedResponseArray", (JArray)null);
                    serviceContract.ExpectedResponseObjectKeys = GetKeyOrDefault(contractProperties, "expectedResponseObjectKeys", (JArray)null);
                    serviceContract.NotExpectedResponseObject = GetKeyOrDefault(contractProperties, "notExpectedResponseObject", (JObject)null);
                    serviceContract.NotExpectedResponseObjectsArray = GetKeyOrDefault(contractProperties, "notExpectedResponseObjectsArray", (JArray)null);
                    serviceContract.DisableDbRestore = GetKeyOrDefault(contractProperties, "disableDbRestore", false);

                    var sqlQueryArray = GetKeyOrDefault(contractProperties, "sqlQueryAfter", (JArray)null);
                    if (sqlQueryArray != null)
                    {
                        string[] queries = sqlQueryArray.Select(q => q.ToString()).ToArray();
                        serviceContract.SqlQueryAfter = string.Join(string.Empty, queries);
                    }
                }
            }

            return serviceContracts;
        }

        private static T GetKeyOrDefault<T>(JObject contratProperties, string key, T defaultValue)
        {
            return contratProperties[key] != null ? JsonConvert.DeserializeObject<T>(contratProperties[key].ToString(Formatting.None)) : defaultValue;
        }
    }
}