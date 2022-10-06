using System.ServiceModel;

namespace Codemancer.Extensions.Wcf.Tests.Fixtures
{
    [ServiceContract]
    public interface ISampleTestService
    {
        [OperationContract]
        string Example1(int param1, string param2, bool param3);

        [OperationContract]
        Task<string> Example1Async(int param1, string param2, bool param3);
    }
}
