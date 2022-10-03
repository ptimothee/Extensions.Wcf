using Polly;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Codemancer.Extensions.Wcf.Polly
{
    public class PollyChannelFactory : ChannelFactoryBase<IRequestChannel>
    {
        private readonly IChannelFactory<IRequestChannel> _channelFactory;
        private readonly IAsyncPolicy<Message> _policy;

        public PollyChannelFactory(IChannelFactory<IRequestChannel> channel, IAsyncPolicy<Message> policy)
            : base()
        {
            _channelFactory = channel;
            _policy = policy;
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channelFactory.BeginOpen(timeout, callback, state);
        }

        protected override IRequestChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            var channel = _channelFactory.CreateChannel(address, via);
            return new PolicyRequestChannel(channel, _policy);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _channelFactory.Open(timeout);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _channelFactory.EndOpen(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channelFactory.BeginClose(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _channelFactory.Close(timeout);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            _channelFactory.EndClose(result);
        }

        protected override void OnAbort()
        {
            _channelFactory.Abort();
        }
    }
}
