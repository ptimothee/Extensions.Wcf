﻿using Xunit;
using Polly;
using FluentAssertions;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Codemancer.Extensions.DependencyInjection;
using Codemancer.Extensions.Wcf.Tests.Fixtures;

namespace Codemancer.Extensions.Wcf.Tests
{
    public class PolicyHandlerTests
    {
        private readonly IHostBuilder _hostBuilder;

        public PolicyHandlerTests()
        {
            _hostBuilder = Host.CreateDefaultBuilder()
                                .UseDefaultServiceProvider(options =>
                                {
                                    options.ValidateOnBuild = true;
                                    options.ValidateScopes = true;
                                })
                                .UseEnvironment(Environments.Development);
        }

        [Theory]
        [Trait("Category", "Functional")]
        [Trait("Category", "Integration")]
        [InlineData(1)]
        public async Task WhenConfigureRetryPolicyOnSyncOperationShouldApplyRetryCount(int expectedRetry)
        {
            var retryCompletionSource = new TaskCompletionSource<int>();
            using var host = _hostBuilder.ConfigureServices(services =>
            {
                var policy = Policy<Message>.Handle<EndpointNotFoundException>()
                                            .RetryAsync(expectedRetry, (resp, count) =>
                                            {
                                                retryCompletionSource.SetResult(count);
                                            });
                
                services.AddWcf<ISampleTestService>()
                        .ConfigureServiceEndpoint(endpoint =>
                        {
                            endpoint.Address = new EndpointAddress($"http://{Guid.NewGuid().ToString()}:1234");
                        })
                        .AddPolicyHandler(policy);

            }).Build();

            var service = host.Services.GetRequiredService<ISampleTestService>();

            Action serviceOperation = () => service.Example1(0, "", true);

            serviceOperation.Should().Throw<EndpointNotFoundException>();
            var actualRetry = await retryCompletionSource.Task;
            actualRetry.Should().Be(expectedRetry);
        }

        [Theory]
        [Trait("Category", "Functional")]
        [Trait("Category", "Integration")]
        [InlineData(1)]
        public async Task WhenConfigureRetryPolicyOnAsyncOperationShouldApplyRetryCount(int expectedRetry)
        {
            var retryCompletionSource = new TaskCompletionSource<int>();

            using var host = _hostBuilder.ConfigureServices(services =>
            {
                var policy = Policy<Message>.Handle<EndpointNotFoundException>()
                                            .RetryAsync(expectedRetry, (resp, count) =>
                                            {
                                                retryCompletionSource.SetResult(count);
                                            });

                services.AddWcf<ISampleTestService>()
                        .ConfigureServiceEndpoint(endpoint =>
                        {
                            endpoint.Address = new EndpointAddress($"http://{Guid.NewGuid().ToString()}:1234");
                        })
                        .AddPolicyHandler(policy);

            }).Build();

            var service = host.Services.GetRequiredService<ISampleTestService>();

            Func<Task> serviceOperation = () => service.Example1Async(0, "", true);

            await serviceOperation.Should().ThrowAsync<EndpointNotFoundException>();
            var actualRetry = await retryCompletionSource.Task;

            actualRetry.Should().Be(expectedRetry);
        }
    }
}
