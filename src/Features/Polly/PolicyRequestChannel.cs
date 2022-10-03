using Polly;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Codemancer.Extensions.Wcf.Polly
{
    public class PolicyRequestChannel : IRequestChannel
    {
        private readonly IAsyncPolicy<Message> _policy;

        public PolicyRequestChannel(IRequestChannel innerChannel, IAsyncPolicy<Message> policy)
        {
            InnerChannel = innerChannel;
            _policy = policy;
        }

        public IRequestChannel InnerChannel 
        {
            get;
        }

        public EndpointAddress RemoteAddress => InnerChannel.RemoteAddress;

        public Uri Via => InnerChannel.Via;

        public CommunicationState State => InnerChannel.State;

        public event EventHandler Closed
        {
            add { InnerChannel.Closed += value; }
            remove { InnerChannel.Closed -= value; }
        }

        public event EventHandler Closing
        {
            add { InnerChannel.Closing += value; }
            remove { InnerChannel.Closing -= value; }
        }

        public event EventHandler Faulted
        {
            add { InnerChannel.Faulted += value; }
            remove { InnerChannel.Faulted -= value; }
        }

        public event EventHandler Opened
        {
            add { InnerChannel.Opened += value; }
            remove { InnerChannel.Opened -= value; }
        }
        public event EventHandler Opening
        {
            add { InnerChannel.Opening += value; }
            remove { InnerChannel.Opening -= value; }
        }

        public void Abort()
        {
            InnerChannel.Abort();
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return InnerChannel.BeginClose(callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginClose(timeout, callback, state);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return InnerChannel.BeginOpen(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginOpen(timeout, callback, state);
        }

        public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
        {      
            return InnerChannel.SendAsync(message, _policy, callback, state);
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.SendAsync(message, _policy, callback, state, timeout);
        }

        public void Close()
        {
            InnerChannel.Close();
        }

        public void Close(TimeSpan timeout)
        {
            InnerChannel.Close(timeout);
        }

        public void EndClose(IAsyncResult result)
        {
            InnerChannel.EndClose(result);
        }

        public void EndOpen(IAsyncResult result)
        {
            InnerChannel.EndOpen(result);
        }

        public Message EndRequest(IAsyncResult result)
        {
            return InnerChannel.EndRequest(result);
        }

        public T GetProperty<T>() where T : class
        {
            return InnerChannel.GetProperty<T>();
        }

        public void Open()
        {
            InnerChannel.Open();
        }

        public void Open(TimeSpan timeout)
        {
            InnerChannel.Open(timeout);
        }

        public Message Request(Message message)
        {
            return InnerChannel.Send(message, _policy);
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            return InnerChannel.Send(message, _policy, timeout);
        }
    }
}
