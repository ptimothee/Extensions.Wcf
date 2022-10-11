# Extensions.Wcf

Codemancer.Extensions.Wcf is a library that modernizes consuming SOAP based Web services by offering a similiar development experience to working with HttpClient.

If you are upgrading your client application but are still required to connect to some legacy SOAP service than this package is for you as I hope to show that it is still possible to teach an old dog new tricks.

## Features

- Fluent builder API to configure WCF DI services
- Support for [Polly](https://github.com/App-vNext/Polly) policies

## Example Usage

### Given a WCF service

You can import service references via visual studio tooling for code generation or create service contract manually.
```csharp
    [ServiceContract]
    public interface ISampleService
    {
        [OperationContract]
        string Example1(int param1, string param2, bool param3);

        [OperationContract]
        Task<string> Example1Async(int param1, string param2, bool param3);
    }
```

### Configure WCF client

In your application Startup.cs file configure each WCF client in Startup.cs as follows:

```csharp

    services.AddWcf<ISampleService>()
            .ConfigureServiceEndpoint(endpoint =>
            {
                endpoint.Address = new EndpointAddress("http://some.domain.com/endpoint.svc");
            });
```

### Configure resilient strategies with Polly policies

```csharp
    var retryPolicy = Policy<Message>.Handle<FaultException>()
                                     .RetryAsync(3);

    services.AddWcf<ISampleService>()
            .ConfigureServiceEndpoint(endpoint =>
            {
                endpoint.Address = new EndpointAddress("http://some.domain.com/endpoint.svc");
            })
            .AddPolicyHandler(retryPolicy);
```
References:
- For more information about Polly policies see [here](https://github.com/App-vNext/Polly) policies
- For HttpClient example see [here](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly)
