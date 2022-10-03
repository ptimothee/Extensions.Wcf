using Polly;
using System;
using System.ServiceModel.Channels;

namespace Codemancer.Extensions.Wcf.Polly
{
    internal static class ContextExtensions
    {
        public static void SetCallback(this Context context, AsyncCallback callback)
        {
            context["callback"] = callback;
        }

        public static AsyncCallback GetChannelCallback(this Context context)
        {
            if (context.TryGetValue("callback", out object obj))
            {
                return (AsyncCallback)obj;
            }

            return null!;
        }

        public static void SetState(this Context context, object state)
        {
            context["state"] = state;
        }

        public static object GetState(this Context context)
        {
            context.TryGetValue("state", out object obj);           
            return obj;          
        }

        public static void SetChannelTimeout(this Context context, TimeSpan timeout)
        {
            context["timeout"] = timeout;
        }

        public static TimeSpan? GetChannelTimeout(this Context context)
        {
            if (context.TryGetValue("timeout", out object obj))
            {
                return (TimeSpan)obj;
            }

            return null;
        }

        public static Message GetRequestCopy(this Context context)
        {
            if (!context.TryGetValue("request", out object obj))
            {
                throw new InvalidOperationException("No request was set to facilitate copy operation. ");
            }
            var request = (Message)obj;

            var buffer = request.CreateBufferedCopy(int.MaxValue);
            request.Close();

            context["request"] = buffer.CreateMessage();

            return buffer.CreateMessage();         
        }

        public static void SetRequest(this Context context, Message message)
        {
            if (context.TryGetValue("request", out object obj))
            {
                ((Message)obj).Close();
            }

            context["request"] = message;
        }

        public static void Purge(this Context context)
        {
            if (context.TryGetValue("request", out object obj))
            {
                ((Message)obj).Close();
            }

            context.Clear();
        }
    }
}
