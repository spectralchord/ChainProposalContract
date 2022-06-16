using System;
using System.Collections.Generic;
using System.Linq;
using AElf.CSharp.Core;
using AElf.Sdk.CSharp;
using AElf.Types;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;


namespace AElf.Contracts.ChainProposals
{
    public partial class ChainProposalsContract : ChainProposalsContractContainer.ChainProposalsContractBase
    {
        public override Empty Initialize(Empty input)
        {
            AssertSenderIsOwner();
            Assert(State.Owner.Value == null, "Contract is already initialized.");
            State.Owner.Value = Context.Sender;
            State.OrganisationCounter.Value = 0;
            return new Empty();
        }
        
        public override Int64Value CreateOrganisation(CreateOrganisationInput input)
        {
            AssertContractIsInitialized();
            var newOrganisationId = State.OrganisationCounter.Value.Add(1);
            
            State.OrganisationCounter.Value = newOrganisationId;
            State.OrganisationsInfoMap[newOrganisationId] = new OrganisationInfoOutput
            {
                Id = newOrganisationId,
                Name = input.Name,
                DescriptionIpfs = input.DescriptionIpfs,
                IsVerified = false,
                Creator = Context.Sender,
                
            };
            
            State.OrganisationsMembersMap[newOrganisationId] = new OrganisationMembersOutput
            {
                Members = { Context.Sender }
            };

            State.OrganisationProposalsCounter[newOrganisationId] = 0;
            
            Context.Fire(new CreateOrganisation
            {
                Creator = Context.Sender,
                OrganisationId = newOrganisationId
            });
            
            return new Int64Value{Value = Convert.ToInt64(newOrganisationId)};
        }

      
        
        public override Empty ChangeOrganisationStatus(ChangeOrganisationStatusInput input)
        {
            AssertContractIsInitialized();
            AssertSenderIsOwner();
            
            var organisationId = Convert.ToInt64(input.Id);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            
            var organisation = State.OrganisationsInfoMap[organisationId];
            organisation.IsVerified = input.Verified;
            State.OrganisationsInfoMap[organisationId] = organisation;
            return new Empty();
        }

        public override Empty EditOrganisationInfo(EditOrganisationInfoInput input)
        {
            AssertContractIsInitialized();

            var organisationId = Convert.ToInt64(input.Id);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            
            var organisation = State.OrganisationsInfoMap[organisationId];
            
            Assert(organisation.Creator == Context.Sender, "No permission, only creator of organization can edit it.");
            organisation.Name = input.Name;
            organisation.DescriptionIpfs = input.DescriptionIpfs;
            State.OrganisationsInfoMap[organisationId] = organisation;
            
            return new Empty();
        }

        public override Empty AddMemberToOrganisation(AddMemberToOrganisationInput input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.Id);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            var organisationInfo = State.OrganisationsInfoMap[organisationId];
            Assert(organisationInfo.Creator == Context.Sender, "No permission, only creator of organization can add new members.");
            
            var organisationMembers = State.OrganisationsMembersMap[organisationId];
            var memberExist = organisationMembers.Members.Contains(input.Member);
            Assert(!memberExist, "This member already exists in this organisation.");
            organisationMembers.Members.Add(input.Member);
            State.OrganisationsMembersMap[organisationId] = organisationMembers;

            return new Empty();
        }

        public override Empty RemoveMemberFromOrganisation(RemoveMemberFromOrganisationInput input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.Id);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            var organisationInfo = State.OrganisationsInfoMap[organisationId];
            Assert(organisationInfo.Creator == Context.Sender, "No permission, only creator of organization can remove members.");
            Assert(organisationInfo.Creator != input.Member, "Organization creator cannot be removed from member list.");
            
            var organisationMembers = State.OrganisationsMembersMap[organisationId];
            var memberExist = organisationMembers.Members.Contains(input.Member);
            Assert(memberExist, "This member does not exists in this organisation.");
            organisationMembers.Members.Remove(input.Member);
            State.OrganisationsMembersMap[organisationId] = organisationMembers;

            return new Empty();
        }

        public override Int64Value CreateProposal(CreateProposalInput input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.OrganisationId);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            var organisationInfo = State.OrganisationsInfoMap[organisationId];
            var organisationMembers = State.OrganisationsMembersMap[organisationId];
            var isCreatorOrMember = organisationMembers.Members.Contains(Context.Sender) || (organisationInfo.Creator == Context.Sender);
            Assert( isCreatorOrMember, "No permission, only the creator and members of organization can create proposals.");
    
            Assert(input.EndTime > Context.CurrentBlockTime, "The proposal end date must be greater than the current date.");
            Assert(input.OptionVotes >= 2, "The proposal must have at least 2 voting options");
            
            var newProposalId = State.OrganisationProposalsCounter[organisationId].Add(1);
            State.OrganisationProposalsCounter[organisationId] = newProposalId;
            State.OrganisationProposalsInfoMap[organisationId][newProposalId] = new ProposalInfoOutput
            {
                ProposalId = newProposalId,
                OrganisationId = organisationId,
                InfoIpfs = input.InfoIpfs,
                Author = Context.Sender,
                StartTime = Context.CurrentBlockTime,
                EndTime = input.EndTime
            };
            
            State.OrganisationProposalVotersMap[organisationId][newProposalId] = new ProposalVoters
            {
                Voters = {  }
            };

            State.OrganisationProposalOptionVotesMap[organisationId][newProposalId] = new ProposalOptionVotes
            {
                OptionVotes = {Enumerable.Repeat<long>(0, Convert.ToInt32(input.OptionVotes)).ToArray()}
            };
            
            Context.Fire(new CreateProposal
            {
                Author = Context.Sender,
                OrganisationId = input.OrganisationId,
                ProposalId = newProposalId
            });
            
            return new Int64Value{Value = Convert.ToInt64(newProposalId)};
        }


        public override Empty VoteForProposal(VoteForProposalInput input)
        {
            AssertContractIsInitialized();
            var organisationId = Convert.ToInt64(input.OrganisationId);
            Assert(CheckOrganisationId(organisationId), "Organisation with such ID does not exists.");
            var proposalId = Convert.ToInt64(input.ProposalId);
            Assert(CheckProposalId(organisationId, proposalId), "Proposal with such ID does not exists.");
            
            var proposalInfo = State.OrganisationProposalsInfoMap[organisationId][proposalId];
            Assert(Context.CurrentBlockTime < proposalInfo.EndTime, "Proposal was ended.");
            
            var proposalVoters = State.OrganisationProposalVotersMap[organisationId][proposalId];
            var votedAlready = proposalVoters.Voters.Contains(Context.Sender);
            Assert(!votedAlready, "You already voted for this proposal.");
            
            var proposalOptionVotes = State.OrganisationProposalOptionVotesMap[organisationId][proposalId];
            var validChoice = input.OptionId >= 1 && input.OptionId <= proposalOptionVotes.OptionVotes.Count;
            Assert(validChoice, "Incorrect choice id");

            var currentChoiceVotes = Convert.ToInt32(proposalOptionVotes.OptionVotes[Convert.ToInt32(input.OptionId) - 1]);
            proposalOptionVotes.OptionVotes[Convert.ToInt32(input.OptionId) - 1] = currentChoiceVotes.Add(1);
            State.OrganisationProposalOptionVotesMap[organisationId][proposalId] = proposalOptionVotes;
            
            proposalVoters.Voters.Add(Context.Sender);
            State.OrganisationProposalVotersMap[organisationId][proposalId] = proposalVoters;
                
            Context.Fire(new Vote
            {
                OrganisationId = organisationId,
                ProposalId = proposalId,
                Voter = Context.Sender,
                OptionId = input.OptionId
            });

            return new Empty();
        }

    }
}