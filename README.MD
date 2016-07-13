## Defining contract

### Request elements

        contractName - string - just name to display
        httpMethod - "GET"/"POST"/"PUT"/"DELETE"/etc
        url - string - URL patch (host is provided form outside to be configurable)
        requestBody - string / json object - just body 

### Assertions

- expectedStatusCodeRegExp - asserts if response status code match provided reqexp (e.g: "2..", "400")
- expectedResponseObject - asserts if properties and values of provided object exists response object.
- expectedResponseArray - asserts if elements of provided array exists response array.
- expectedResponseObjectsArray 
- expectedResponseObjectKeys 
- notExpectedResponseObjectsArray 
- unexpectedResponseArray 