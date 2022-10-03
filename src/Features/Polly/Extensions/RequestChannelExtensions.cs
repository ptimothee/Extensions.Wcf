using Polly;
using System;
using System.Threading.Tasks;
using System.ServiceModel.Channels;

namespace Codemancer.Extensions.Wcf.Polly
{
    internal static class RequestChannelExtensions
    {
        public static Message Send(this IRequestChannel channel, Message message, IAsyncPolicy<Message> policy, TimeSpan? timeout = null)
        {
            var context = new Context(nameof(IRequestChannel.Request));

            context.SetRequest(message);
            if (timeout.HasValue)
            {
                context.SetChannelTimeout(timeout.Value);
            }

            try
            {
                return policy.ExecuteAsync(ctx =>
                {
                    var response = channel.Send(ctx.GetRequestCopy(), context.GetChannelTimeout());

                    return Task.FromResult(response);

                }, context).Result;
            }
            finally
            {
                // Policy execution completed: clean up any context references
                context.Purge();
            }
        }

        public static async Task<Message> SendAsync(this IRequestChannel channel, Message message, IAsyncPolicy<Message> policy, AsyncCallback callback, object state, TimeSpan? timeout = null)
        {
            var context = new Context(nameof(IRequestChannel.BeginRequest));

            context.SetRequest(message);
            context.SetCallback(callback);
            context.SetState(state);

            if (timeout.HasValue)
            {
                context.SetChannelTimeout(timeout.Value);
            }

            try
            {
                return await policy.ExecuteAsync(ctx => channel.SendAsync(ctx.GetRequestCopy(), ctx.GetChannelCallback(), ctx.GetState(), ctx.GetChannelTimeout()), context);
            }
            finally
            {
                // Policy execution completed: clean up any context references
                context.Purge();
            }
        }

        private static Message Send(this IRequestChannel channel, Message request, TimeSpan? timeout = null)
        {
            if (timeout.HasValue)
            {
                return channel.Request(request, timeout.Value);
            }

            return channel.Request(request);
        }

        private static async Task<Message> SendAsync(this IRequestChannel channel, Message message, AsyncCallback callback, object state, TimeSpan? timeout)
        {
            if (timeout.HasValue)
            {
                var asyncResult = channel.BeginRequest(message, timeout.Value, callback, state);
                return await Task.Factory.FromAsync<Message>(asyncResult, channel.EndRequest)
                                                 .ConfigureAwait(false);
            }

            var asyncBeginResult = channel.BeginRequest(message, callback, state);
            return await Task.Factory.FromAsync<Message>(asyncBeginResult, channel.EndRequest)
                                             .ConfigureAwait(false);
        }
    }
}
