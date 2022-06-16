using AElf.Boilerplate.TestBase;
using AElf.Cryptography.ECDSA;

namespace AElf.Contracts.ChainProposals
{
    public class ChainProposalsContractTestBase : DAppContractTestBase<ChainProposalsContractTestModule>
    {
        // You can get address of any contract via GetAddress method, for example:
        // internal Address DAppContractAddress => GetAddress(DAppSmartContractAddressNameProvider.StringName);

        internal ChainProposalsContractContainer.ChainProposalsContractStub GetChainProposalsContractStub(ECKeyPair senderKeyPair)
        {
            return GetTester<ChainProposalsContractContainer.ChainProposalsContractStub>(DAppContractAddress, senderKeyPair);
        }
    }
}