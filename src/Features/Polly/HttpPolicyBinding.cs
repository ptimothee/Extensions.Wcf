using Polly;
using System;
using System.ServiceModel.Channels;

namespace Codemancer.Extensions.Wcf.Polly
{
    public class HttpPolicyBinding: TransportBindingElement
    {
        private readonly TransportBindingElement _transportBindingElement;
        private readonly IAsyncPolicy<Message> _policy;

        public HttpPolicyBinding(TransportBindingElement transportBinding, IAsyncPolicy<Message> policy)
            : base(transportBinding)
        {
            _transportBindingElement = transportBinding;
            _policy = policy;
        }

        public override string Scheme => _transportBindingElement.Scheme;

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IRequestChannel);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            var channel = _transportBindingElement.BuildChannelFactory<TChannel>(context);
            if(!(channel is IChannelFactory<IRequestChannel> requestChannelFactory))
            {
                var innerException = new InvalidCastException($"Cannot cast {channel.GetType()} to {nameof(IChannelFactory<IRequestChannel>)}. ");
                throw new InvalidOperationException("Policies are only applicable to operation implemented against a request-reply communication channel. ", innerException);
            }
             
            return (IChannelFactory<TChannel>)new PollyChannelFactory(requestChannelFactory, _policy);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return _transportBindingElement.GetProperty<T>(context);
        }

        public override BindingElement Clone()
        {
            return new HttpPolicyBinding(_transportBindingElement, _policy);
        }
    }
}
