using Camply.Application.Contracts.Repositories;
using Camply.Application.Implementations;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Vote;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Shouldly;

namespace Camply.Application.Tests.Services
{
    public class VoteServiceTests
    {
        private readonly Mock<IVoteRepository> _voteRepo;
        private readonly Mock<ISpecifiedRepository<Vote>> _specRepo;
        private readonly Mock<IVoteOptionRepository> _voteOptionRepo;
        private readonly Mock<IUserVoteRepository> _userVoteRepo;

        private readonly Mock<IValidator<VoteCreateRequest>> _createValidator;
        private readonly Mock<IValidator<VoteUpdateRequest>> _updateValidator;
        private readonly Mock<IValidator<VoteOptionUpdateRequest>> _optionValidator;

        private readonly VoteService _service;

        public VoteServiceTests()
        {
            _voteRepo = new Mock<IVoteRepository>();
            _specRepo = new Mock<ISpecifiedRepository<Vote>>();
            _voteOptionRepo = new Mock<IVoteOptionRepository>();
            _userVoteRepo = new Mock<IUserVoteRepository>();

            _createValidator = new Mock<IValidator<VoteCreateRequest>>();
            _updateValidator = new Mock<IValidator<VoteUpdateRequest>>();
            _optionValidator = new Mock<IValidator<VoteOptionUpdateRequest>>();

            _createValidator.Setup(v => v.Validate(It.IsAny<VoteCreateRequest>()))
                .Returns(new ValidationResult());
            _updateValidator.Setup(v => v.Validate(It.IsAny<VoteUpdateRequest>()))
                .Returns(new ValidationResult());
            _optionValidator.Setup(v => v.Validate(It.IsAny<VoteOptionUpdateRequest>()))
                .Returns(new ValidationResult());

            _service = new VoteService(
                _voteRepo.Object, _specRepo.Object, _voteOptionRepo.Object,
                _createValidator.Object, _updateValidator.Object, _optionValidator.Object,
                _userVoteRepo.Object);
        }

        [Fact]
        public async Task GetForumVotesAsync_Should_MapVotes()
        {
            var currentUserId = Guid.NewGuid();
            var forum = new Forum { Id = Guid.NewGuid(), Title = "TestForum" };
            var user = new User { Id = Guid.NewGuid(), Username = "Author" };

            var votes = new List<Vote>
            {
                new()
                { 
                    Id = Guid.NewGuid(), 
                    Title = "Vote1",
                    Forum = forum,
                    ForumId = forum.Id,
                    User = user,
                    UserId = user.Id,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    VoteOptions = new List<VoteOption>
                    {
                        new VoteOption { Id = Guid.NewGuid(), Name = "Option1", Vote = null! , VoteId = Guid.NewGuid() }
                    }
                },
                new()
                { 
                    Id = Guid.NewGuid(), 
                    Title = "Vote2",
                    Forum = forum,
                    ForumId = forum.Id,
                    User = user,
                    UserId = user.Id,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    VoteOptions = new List<VoteOption>()
                }
            };

            _specRepo.Setup(r => r.ListAsync(It.IsAny<VoteSearchSpecification>()))
                .ReturnsAsync(votes);

            var request = new ForumVotesSearchRequest(currentUserId);
            var result = await _service.GetForumVotesAsync(request);

            var voteDtos = result.ToList();
            voteDtos.Count.ShouldBe(2);
            voteDtos.Select(v => v.Title).ShouldContain("Vote1");
            voteDtos.Select(v => v.Title).ShouldContain("Vote2");
        }

        [Fact]
        public async Task CreateVote_Should_AddVote()
        {
            var request = new VoteCreateRequest(
                "TestVote",
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<VoteOptionCreateDto> { new VoteOptionCreateDto("Option1") });

            await _service.CreateVote(request);

            _voteRepo.Verify(r => r.AddAsync(It.Is<Vote>(v =>
                v.Title == "TestVote" &&
                v.VoteOptions.Count == 1 &&
                v.VoteOptions.Any(o => o.Name == "Option1")
            )), Times.Once);
        }

        [Fact]
        public async Task CreateVote_Should_Throw_When_Invalid()
        {
            _createValidator.Setup(v => v.Validate(It.IsAny<VoteCreateRequest>()))
                .Returns(new ValidationResult(new[] { new ValidationFailure("Title", "Required") }));

            var request = new VoteCreateRequest("", Guid.NewGuid(), Guid.NewGuid(), new List<VoteOptionCreateDto>());

            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateVote(request));
        }

        [Fact]
        public async Task UpdateVote_Should_Update_When_Owner()
        {
            var vote = new Vote { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Title = "OldTitle" };
            _voteRepo.Setup(r => r.GetByIdAsync(vote.Id)).ReturnsAsync(vote);

            var request = new VoteUpdateRequest(vote.Id, vote.UserId, "NewTitle");

            await _service.UpdateVote(request);

            vote.Title.ShouldBe("NewTitle");
            _voteRepo.Verify(r => r.UpdateAsync(vote), Times.Once);
        }

        [Fact]
        public async Task UpdateVote_Should_Throw_When_NotOwner()
        {
            var vote = new Vote { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Title = "OldTitle" };
            _voteRepo.Setup(r => r.GetByIdAsync(vote.Id)).ReturnsAsync(vote);

            var request = new VoteUpdateRequest(vote.Id, Guid.NewGuid(), "NewTitle");

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateVote(request));
        }

        [Fact]
        public async Task UpdateVote_Should_Throw_When_NotFound()
        {
            _voteRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vote)null);

            var request = new VoteUpdateRequest(Guid.NewGuid(), Guid.NewGuid(), "NewTitle");

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateVote(request));
        }

        [Fact]
        public async Task UpdateVoteOption_Should_Update_When_Owner()
        {
            var vote = new Vote { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var option = new VoteOption { Id = Guid.NewGuid(), VoteId = vote.Id, Vote = vote, Name = "Old", Index = 0 };
            _voteOptionRepo.Setup(r => r.GetByIdAsync(option.Id)).ReturnsAsync(option);
            _voteOptionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<VoteOption> { option });

            var request = new VoteOptionUpdateRequest(option.Id, vote.UserId, "New", 0);

            await _service.UpdateVoteOption(request);

            option.Name.ShouldBe("New");
            option.Index.ShouldBe(0);
            _voteOptionRepo.Verify(r => r.UpdateAsync(option), Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateVoteOption_Should_Throw_When_NotOwner()
        {
            var vote = new Vote { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var option = new VoteOption { Id = Guid.NewGuid(), VoteId = vote.Id, Vote = vote };
            _voteOptionRepo.Setup(r => r.GetByIdAsync(option.Id)).ReturnsAsync(option);

            var request = new VoteOptionUpdateRequest(option.Id, Guid.NewGuid(), "New", 0);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateVoteOption(request));
        }

        [Fact]
        public async Task UpdateVoteOption_Should_Throw_When_NotFound()
        {
            _voteOptionRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((VoteOption)null);

            var request = new VoteOptionUpdateRequest(Guid.NewGuid(), Guid.NewGuid(), "Name", 0);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateVoteOption(request));
        }

        [Fact]
        public async Task DeleteVoteOption_Should_Delete_Option_And_UserVotes()
        {
            var vote = new Vote { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var option = new VoteOption
            {
                Id = Guid.NewGuid(),
                VoteId = vote.Id,
                Vote = vote,
                UserVotes = new List<UserVote> { new UserVote(), new UserVote() }
            };
            _voteOptionRepo.Setup(r => r.GetByIdAsync(option.Id)).ReturnsAsync(option);
            _voteOptionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<VoteOption> { option });

            await _service.DeleteVoteOption(option.Id, vote.UserId);

            _userVoteRepo.Verify(r => r.DeleteAsync(It.IsAny<UserVote>()), Times.Exactly(2));
            _voteOptionRepo.Verify(r => r.DeleteAsync(option), Times.Once);
        }

        [Fact]
        public async Task DeleteVoteOption_Should_Throw_When_NotOwner()
        {
            var vote = new Vote { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var option = new VoteOption { Id = Guid.NewGuid(), VoteId = vote.Id, Vote = vote };
            _voteOptionRepo.Setup(r => r.GetByIdAsync(option.Id)).ReturnsAsync(option);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteVoteOption(option.Id, Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteVoteOption_Should_Throw_When_NotFound()
        {
            _voteOptionRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((VoteOption)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteVoteOption(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task Vote_Should_Add_When_FirstTime()
        {
            var vote = new Vote { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var option = new VoteOption { Id = Guid.NewGuid(), VoteId = vote.Id, Vote = vote };
            _voteOptionRepo.Setup(r => r.GetByIdAsync(option.Id)).ReturnsAsync(option);
            _userVoteRepo.Setup(r => r.GetByVoteAndUserAsync(vote.Id, vote.UserId)).ReturnsAsync((UserVote)null);

            var result = await _service.Vote(option.Id, vote.UserId);

            result.ShouldBeTrue();
            _userVoteRepo.Verify(r => r.AddAsync(It.Is<UserVote>(uv => uv.OptionId == option.Id)), Times.Once);
        }

        [Fact]
        public async Task Vote_Should_Remove_When_SameOption()
        {
            var vote = new Vote { Id = Guid.NewGuid() };
            var option = new VoteOption { Id = Guid.NewGuid(), VoteId = vote.Id, Vote = vote };
            var existingVote = new UserVote { OptionId = option.Id, UserId = Guid.NewGuid() };

            _voteOptionRepo.Setup(r => r.GetByIdAsync(option.Id)).ReturnsAsync(option);
            _userVoteRepo.Setup(r => r.GetByVoteAndUserAsync(vote.Id, existingVote.UserId)).ReturnsAsync(existingVote);

            var result = await _service.Vote(option.Id, existingVote.UserId);

            result.ShouldBeFalse();
            _userVoteRepo.Verify(r => r.DeleteAsync(existingVote), Times.Once);
        }
    }
}