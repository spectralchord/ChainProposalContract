using System;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.ChainProposals
{
    public partial class ChainProposalsContract
    {
        private void AssertSenderIsOwner()
        {
            var owner = new Address(Address.FromBase58("q9W7hYWciC5B9iF1cBKz8sqevh2nrXSVnsVazoofnDFYjmwAa"));
            Assert(Context.Sender == owner, "No permission, you are not the owner.");
        }
        
        private void AssertContractIsInitialized()
        {
            Assert(State.Owner.Value != null, "Contract is not initialized");
        }
        
        public Boolean CheckOrganisationId(Int64 organisationId)
        {
            return State.OrganisationsInfoMap[organisationId] != null;
        }
        
        public Boolean CheckProposalId(Int64 organisationId, Int64 proposalId)
        {
            return State.OrganisationProposalsInfoMap[organisationId][proposalId] != null;
        }
        
        
    }
}