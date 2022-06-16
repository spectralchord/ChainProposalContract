using System;
using System.ComponentModel;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.ChainProposals
{
    public partial class ChainProposalsContract
    {
        public override GetOrganisationCountOutput GetOrganisationCount(Empty input)
        {
            AssertContractIsInitialized();
            return new GetOrganisationCountOutput{Value = State.OrganisationCounter.Value.ToString()};
        }


        public override OrganisationInfoOutput GetOrganisationInfo(Int64Value input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.Value);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            return State.OrganisationsInfoMap[organisationId];
        }


        public override OrganisationMembersOutput GetOrganisationMembers(Int64Value input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.Value);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            
            return State.OrganisationsMembersMap[organisationId];
            
        }

        public override GetOrganisationProposalsCountOutput GetOrganisationProposalCount(Int64Value input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.Value);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");

            var proposalsCount = State.OrganisationProposalsCounter[organisationId];
            return new GetOrganisationProposalsCountOutput{Value = Convert.ToString(proposalsCount)};
        }


        public override ProposalInfoOutput GetProposalInfo(GetProposalInfoInput input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.OrganisationId);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            var proposalId = Convert.ToInt64(input.ProposalId);
            Assert(CheckProposalId(organisationId, proposalId), "Proposal with such ID does not exists.");
            
            return State.OrganisationProposalsInfoMap[organisationId][proposalId];
        }
        
        public override ProposalOptionVotes GetProposalOptionVotes(GetProposalOptionVotesInput input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.OrganisationId);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            var proposalId = Convert.ToInt64(input.ProposalId);
            Assert(CheckProposalId(organisationId, proposalId), "Proposal with such ID does not exists.");
            
            return State.OrganisationProposalOptionVotesMap[organisationId][proposalId];
        }


        public override ProposalVoters GetProposalVoters(GetProposalVotersInput input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.OrganisationId);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            var proposalId = Convert.ToInt64(input.ProposalId);
            Assert(CheckProposalId(organisationId, proposalId), "Proposal with such ID does not exists.");
            return State.OrganisationProposalVotersMap[organisationId][proposalId];
        }

    }
}