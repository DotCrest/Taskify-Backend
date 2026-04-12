using Application.MappingProfiles;
using Application.ServiceAbstractions;
using Application.Services;
using Application.Shared.Errors;
using Application.Tests.Fakers;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Application.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Comment>> _commentRepositoryMock;
    private readonly Mock<IQuestService> _questServiceMock;
    private readonly Mock<IUserQuestService> _userQuestServiceMock;
    private readonly IMapper _mapper;

    private readonly CommentService _sut;
    public CommentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _commentRepositoryMock = new Mock<IGenericRepository<Comment>>();
        _unitOfWorkMock.Setup(uow => uow.Repository<Comment>()).Returns(_commentRepositoryMock.Object);

        _questServiceMock = new Mock<IQuestService>();
        _userQuestServiceMock = new Mock<IUserQuestService>();

        var nullLoggerFactory = new NullLoggerFactory();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CommentProfile>(), nullLoggerFactory);
        _mapper = config.CreateMapper();

        _sut = new CommentService(_unitOfWorkMock.Object, _mapper, _questServiceMock.Object, _userQuestServiceMock.Object);
    }

    #region AddCommentAsync
    [Fact]
    public async Task AddCommentAsync_WithValidData_ReturnsSuccess()
    {
        // arrange
        var AddCommentDto = CommentFaker.GetAddCommentDto().Generate();

        // these mocks for the private method (ExternalValidationsSteps)
        _questServiceMock.Setup(q => q.IsQuestExisted(It.IsAny<int>())).ReturnsAsync(true);
        _userQuestServiceMock.Setup(uq => uq.IsUserAssignedToQuest(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);
        // act
        var result = await _sut.AddCommentAsync(AddCommentDto);
        // assert
        result.IsSuccess.Should().BeTrue();
        _commentRepositoryMock.Verify(c => c.AddAsync(It.IsAny<Comment>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }
    [Fact]
    public async Task AddCommentAsync_WhenQuestIsNotFound_ReturnQuestNotFound()
    {
        // arrange
        var addCommentDto = CommentFaker.GetAddCommentDto().Generate();

        // these mocks for the private method (ExternalValidationsSteps)
        _questServiceMock.Setup(q => q.IsQuestExisted(It.IsAny<int>())).ReturnsAsync(false);

        // act
        var result = await _sut.AddCommentAsync(addCommentDto);
        // assert

        _commentRepositoryMock.Verify(c => c.AddAsync(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);

        result.IsSuccess.Should().BeFalse();
        result
            .ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(QuestErrors.NotFound);
    }
    [Fact]
    public async Task AddCommentAsync_WhenUserIsNotAssignedToQuest_ReturnsAccessDenied()
    {
        // arrange
        var addCommentDto = CommentFaker.GetAddCommentDto().Generate();

        // these mocks for the private method (ExternalValidationsSteps)
        _questServiceMock.Setup(q => q.IsQuestExisted(It.IsAny<int>())).ReturnsAsync(true);
        _userQuestServiceMock.Setup(uq => uq.IsUserAssignedToQuest(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);
        // act
        var result = await _sut.AddCommentAsync(addCommentDto);
        // assert
        _commentRepositoryMock.Verify(c => c.AddAsync(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);

        result.IsSuccess.Should().BeFalse();
        result
            .ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(CommentErrors.AccessDenied);
    }
    #endregion

    #region GetCommentsByQuestIdAsync
    [Fact]
    public async Task GetCommentsByQuestIdAsync_WhenThereIsComments_ReturnPagedResponse()
    {
        // arrange
        var queryFilter = CommentFaker.GetFakeQueryFilter().Generate();
        var comments = CommentFaker.GetComment().Generate(5);

        _commentRepositoryMock.Setup(c => c.CountAsync(It.IsAny<ISpecification<Comment>>())).ReturnsAsync(comments.Count);
        _commentRepositoryMock.Setup(c => c.FindAll(It.IsAny<ISpecification<Comment>>())).ReturnsAsync(comments);
        // act
        var result = await _sut.GetCommentsByQuestIdAsync(questId: 1, queryFilter);
        // assert
        _commentRepositoryMock.Verify(c => c.CountAsync(It.IsAny<ISpecification<Comment>>()), Times.Once);
        _commentRepositoryMock.Verify(c => c.FindAll(It.IsAny<ISpecification<Comment>>()), Times.Once);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TotalRecords.Should().Be(comments.Count);
    }
    #endregion
}
