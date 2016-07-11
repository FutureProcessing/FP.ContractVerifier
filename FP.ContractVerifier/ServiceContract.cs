using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace ContractVerifier
{
    public class ServiceContract
    {
        public string ContractName { get; set; }

        public string HttpMethod { get; set; }

        public string Url { get; set; }

        public object RequestBody { get; set; }

        public string ExpectedStatusCodeRegExp { get; set; }

        public JObject ExpectedResponseObject { get; set; }

        public JObject NotExpectedResponseObject { get; set; }

        public JArray NotExpectedResponseObjectsArray { get; set; }

        public JArray ExpectedResponseArray { get; set; }

        public JArray UnexpectedResponseArray { get; set; }

        public JArray ExpectedResponseObjectsArray { get; set; }

        public JArray ExpectedResponseObjectKeys { get; set; }

        public string SqlQueryAfter { get; set; }

        public bool DisableCoreDbRestore { get; set; }

        public bool DisableBookingDbRestore { get; set; }

        public bool DisablePricingDbRestore { get; set; }

        public override string ToString()
        {
            return ContractName;
        }

        public void AssertSelf(string serviceBase, string authToken)
        {
            var response = CreateAndExecuteRequestFromContract(serviceBase, authToken);

            AssertStatusCode(response);
            AssertResponseObjectContainsExpectedKeysAndValues(response);
            AssertResponseObjectContainsExpectedKeys(response);
            AssertResponseObjectsArrayContainsExpectedKeysAndValues(response);
            AssertResponseArrayContainsValues(response);
            AssertResponseArrayDoesNotContainValues(response);
            AssertResponseObjectsArrayDoesNotContainExpectedKeysAndValues(response);
            AssertResponseObjectsArrayNotContainsExpectedKeysAndValues(response);

            ExecuteSqlQueryAfter();
        }

        public IRestResponse CreateAndExecuteRequestFromContract(string serviceBase, string authToken)
        {
            var client = new RestClient(serviceBase);
            var request = new RestRequest(Url);

            request.Method = (Method)Enum.Parse(typeof(Method), HttpMethod);
            request.AddHeader("Authorization", "Bearer " + authToken);

            if (RequestBody != null)
            {
                var body = JsonConvert.SerializeObject(RequestBody);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
            }

            var response = client.Execute(request);
            return response;
        }

        private void AssertArrayContainsExpectedElements(JArray actualArray, JArray expectedArrayValues)
        {
            foreach (var objectExpectedInArray in expectedArrayValues)
            {
                Assert.True(
                    actualArray.Any(obj => ContainsExpectedPropertiesWithValues(objectExpectedInArray, obj)),
                    "Response array does not contain contain expected object: " + objectExpectedInArray);
            }
        }

        private void AssertArrayNotContainsExpectedElements(JArray actualArray, JArray expectedArrayValues)
        {
            foreach (var objectExpectedInArray in expectedArrayValues)
            {
                Assert.False(
                    actualArray.Any(obj => ContainsExpectedPropertiesWithValues(objectExpectedInArray, obj)),
                    "Response contains wrong object: " + objectExpectedInArray);
            }
        }

        private void AssertResponseObjectsArrayContainsExpectedKeysAndValues(IRestResponse response)
        {
            if (ExpectedResponseObjectsArray != null)
            {
                var responseContent = JsonConvert.DeserializeObject<JArray>(response.Content);
                AssertArrayContainsExpectedElements(responseContent, ExpectedResponseObjectsArray);
            }
        }

        private void AssertResponseArrayContainsValues(IRestResponse response)
        {
            if (ExpectedResponseArray != null)
            {
                var responseContent = JsonConvert.DeserializeObject<JArray>(response.Content);

                foreach (var expectedArrayValue in ExpectedResponseArray)
                {
                    Assert.True(
                        responseContent.Any(val => expectedArrayValue.Equals(val)),
                        "Response array does not contain contain expected value: " + expectedArrayValue);
                }
            }
        }

        private void AssertResponseArrayDoesNotContainValues(IRestResponse response)
        {
            if (UnexpectedResponseArray != null)
            {
                var responseContent = JsonConvert.DeserializeObject<JArray>(response.Content);

                foreach (var unexpectedArrayValue in UnexpectedResponseArray)
                {
                    {
                        Assert.False(
                            responseContent.Any(val => unexpectedArrayValue.Equals(val)),
                            "Response contains wrong object: " + unexpectedArrayValue);
                    }
                }
            }
        }

        private void AssertResponseObjectsArrayNotContainsExpectedKeysAndValues(IRestResponse response)
        {
            if (NotExpectedResponseObjectsArray != null)
            {
                var responseContent = JsonConvert.DeserializeObject<JArray>(response.Content);
                AssertArrayNotContainsExpectedElements(responseContent, NotExpectedResponseObjectsArray);
            }
        }

        private void AssertResponseObjectContainsExpectedKeys(IRestResponse response)
        {
            if (ExpectedResponseObjectKeys != null)
            {
                var responseContent = JsonConvert.DeserializeObject<JObject>(response.Content);
                foreach (JValue expectedKey in ExpectedResponseObjectKeys)
                {
                    var value = responseContent[expectedKey.Value];
                    Assert.NotNull(value, "Response does not contain expected property: " + expectedKey);
                }
            }
        }

        private void AssertResponseObjectContainsExpectedKeysAndValues(IRestResponse response)
        {
            if (ExpectedResponseObject != null)
            {
                var responseContent = JsonConvert.DeserializeObject<JObject>(response.Content);
                foreach (var expectedProperty in ExpectedResponseObject)
                {
                    var actualValue = responseContent[expectedProperty.Key];
                    Assert.NotNull(actualValue, "Response does not contain expected property: " + expectedProperty.Key);

                    if (expectedProperty.Value as JArray != null)
                    {
                        var expectedArray = expectedProperty.Value as JArray;
                        var actualArray = actualValue as JArray;
                        Assert.IsNotNull(
                            actualArray,
                            string.Format(CultureInfo.CurrentCulture, "Response property {0} is expected to be array but it is not.", expectedProperty.Key));

                        AssertArrayContainsExpectedElements(actualArray, expectedArray);
                    }
                    else if (expectedProperty.Value as JObject != null)
                    {
                        var expectedObject = expectedProperty.Value as JObject;
                        var actualObject = actualValue as JObject;
                        Assert.IsNotNull(
                            actualObject,
                            string.Format(CultureInfo.CurrentCulture, "Response property {0} is expected to be object but it is not.", expectedProperty.Key));

                        foreach (var expectedNestedProperty in expectedObject)
                        {
                            var actualNestedProperty = actualObject[expectedNestedProperty.Key];
                            Assert.NotNull(actualNestedProperty, "Response does not contain expected property: " + expectedNestedProperty.Key);

                            var expectedNestedArray = expectedNestedProperty.Value as JArray;
                            if (expectedNestedArray != null)
                            {
                                var actualNestedArray = actualNestedProperty as JArray;
                                Assert.IsNotNull(
                                    actualNestedArray,
                                    string.Format(CultureInfo.CurrentCulture, "Response property {0} is expected to be array but it is not.", expectedNestedProperty.Key));

                                AssertArrayContainsExpectedElements(actualNestedArray, expectedNestedArray);
                            }
                            else
                            {
                                Assert.AreEqual(
                                    expectedNestedProperty.Value.Value<string>(), 
                                    actualNestedProperty.Value<string>(), 
                                    "Invalid value of expected response property: " + expectedNestedProperty.Key);
                            }
                        }
                    }
                    else
                    {
                        Assert.AreEqual(
                            expectedProperty.Value.Value<string>(),
                            actualValue.Value<string>(), 
                            "Invalid value of expected response property: " + expectedProperty.Key);
                    }
                }
            }
        }

        private void AssertResponseObjectsArrayDoesNotContainExpectedKeysAndValues(IRestResponse response)
        {
            if (NotExpectedResponseObject != null)
            {
                var responseContent = JsonConvert.DeserializeObject<JObject>(response.Content);
                foreach (var expectedPropery in NotExpectedResponseObject)
                {
                    var value = responseContent[expectedPropery.Key];
                    Assert.NotNull(value, "Response does not contain expected property: " + expectedPropery.Key);

                    var expectedArrayValues = expectedPropery.Value as JArray;
                    if (expectedArrayValues != null)
                    {
                        var valueArray = value as JArray;
                        Assert.IsNotNull(
                            valueArray,
                            string.Format(CultureInfo.CurrentCulture, "Response property {0} is expected to be array but it is not.", expectedPropery.Key));

                        AssertArrayNotContainsExpectedElements(valueArray, expectedArrayValues);
                    }
                }
            }
        }

        private void AssertStatusCode(IRestResponse response)
        {
            StringAssert.IsMatch(
                ExpectedStatusCodeRegExp,
                response.StatusCode.ToString("d"),
                "Invalid response status code, Response content: " + response.Content);
        }

        private bool ContainsExpectedPropertiesWithValues(JToken expectedObject, JToken actualObject)
        {
            foreach (var expectedToken in expectedObject)
            {
                var expectedProperty = (JProperty)expectedToken;
                var actualPropertyValue = actualObject[expectedProperty.Name] as JValue;
                if (!Equals(actualPropertyValue, expectedProperty.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private void ExecuteSqlQueryAfter()
        {
            if (string.IsNullOrWhiteSpace(SqlQueryAfter))
            {
                return;
            }

            ServiceDatabaseAccess.ExecuteSqlQuery(SqlQueryAfter);
        }
    }
}
