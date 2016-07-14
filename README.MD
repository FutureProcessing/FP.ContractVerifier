# FP.ContractVerifier

FP.ContractVerifier is microframework build to verify Web Services contracts.
It is build to work well with json content types.
Please note it is not fully featured testing framework. Usually will require some adaptation to use in your project.

## Using it

- Add sources to your project
- Create contract file (remember to set "Copy to output directory" fot file)
- Setup NUnit test to execute contract (copy from samples SampleTests\SampleAppTests.cs)
- Start your app (start on IIS express without debugging (ctrl+F5) or configure for running on local IIS and just build app or use with ASP.NET Core and start app selfhost from tests)
- Run tests with NUnit runner of your choose (recommended: Resharper)

## Defining contract

### Example

```javascript
{
    "contractName": "Get all items",
    "httpMethod": "GET",
    "url": "/items",
    "expectedStatusCode": 200,
    "expectedResponseArrayObjects": [
        {
            "name": "Yacht",
            "price": 50000.0
        },
        {
            "name": "Barbie Doll",
            "price": 20.0
        }
    ],
    "notExpectedResponseArrayObjects": [
        {
            "name": "Yacht",
            "price": 100
        },{
            "name": "Beyonce Bratz Doll",
            "price": 20.0
        },{
            "name": "Barbie Doll",
            "price": 20.0,
            "notExistingProp": "with some value"
        }
    ],
    "disableDbRestore": true
}
```


### Request elements

- contractName - string - just name to display
- httpMethod - "GET"/"POST"/"PUT"/"DELETE"/etc
- url - string - URL patch (host is provided form outside to be configurable)
- requestBody - string / json object - just body 

### Assertions

- expectedStatusCodeRegExp - asserts if response status code match provided reqexp (e.g: "2..", "400")
- expectedResponseObject - asserts if properties and values of provided object exists response object.
- expectedResponseObjectKeys - asserts if properties object exists in response object.
- expectedResponseArrayValues - asserts if elements of provided array exists in response array.
- expectedResponseArrayObjects -  asserts if objects in provided array exists in response array.
- notExpectedResponseArrayObjects - asserts if objects of provided array does not exists in response array.
- notExpectedResponseArrayValues - asserts if elements of provided array does not exists in response array.

### Other features

#### DbRestore

Build to create snapshot and restore SQL Database after every test (missing in samples).
Let you base your tests on preexisting database and restore it after all tests that modify data. 

To use:

- call ServiceDatabaseAccess.CreateDatabaseBackup in method marked with [TestFixtureSetUp] attribute.
- set "disableCoreDbRestore": false in contract file.
- call ServiceDatabaseAccess.DropDatabaseBackup in method with [TestFixtureTearDown] attribute.

Feature require redesign.

## TO DOs

- Refine Framework API to make it less static but still convenient to use with NUnit.
- Make DbRestore disabled by default.
- Describe DbRestore option here and add samples.
- Create more generic solution for service authentication.
