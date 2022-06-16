using AElf.Sdk.CSharp.State;
using AElf.Types;

namespace AElf.Contracts.ChainProposals
{
    public class ChainProposalsContractState : ContractState
    {
        public SingletonState<Address> Owner { get; set; }
        
        public SingletonState<long> OrganisationCounter { get; set; }
        
        public MappedState<long, OrganisationInfoOutput> OrganisationsInfoMap { get; set; }
        
        public MappedState<long, OrganisationMembersOutput> OrganisationsMembersMap { get; set; }
        
        public MappedState<long, long> OrganisationProposalsCounter { get; set; }
        
        public MappedState<long, long, ProposalInfoOutput> OrganisationProposalsInfoMap { get; set; }
        
        public MappedState<long, long, ProposalOptionVotes> OrganisationProposalOptionVotesMap { get; set; }
        
        public MappedState<long, long, ProposalVoters> OrganisationProposalVotersMap { get; set; }
        
    }
}