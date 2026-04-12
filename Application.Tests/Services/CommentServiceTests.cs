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
using System.Linq.Expressions;

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
    [Fact]
    public async Task GetCommentsByQuestIdAsync_WhenThereIsNoComments_ReturnsEmptyResponse()
    {
        // arrange
        var queryFilter = CommentFaker.GetFakeQueryFilter().Generate();
        var emptyCommentList = new List<Comment>();

        _commentRepositoryMock.Setup(c => c.CountAsync(It.IsAny<ISpecification<Comment>>())).ReturnsAsync(0);
        _commentRepositoryMock.Setup(c => c.FindAll(It.IsAny<ISpecification<Comment>>())).ReturnsAsync(emptyCommentList);
        // act
        var result = await _sut.GetCommentsByQuestIdAsync(questId: 1, queryFilter);
        // assert
        _commentRepositoryMock.Verify(c => c.CountAsync(It.IsAny<ISpecification<Comment>>()), Times.Once);
        _commentRepositoryMock.Verify(c => c.FindAll(It.IsAny<ISpecification<Comment>>()), Times.Once);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TotalRecords.Should().Be(emptyCommentList.Count);
    }
    #endregion

    #region UpdateCommentAsync
    [Fact]
    public async Task UpdateCommentAsync_WithValidData_ReturnsSuccess()
    {
        // arrange
        // The seed will make sure that the generated UpdateCommentDto and Comment have the same data.
        var updateCommentDto = CommentFaker.GetUpdateCommentDto(seed: 2).Generate();
        var comment = CommentFaker.GetComment(seed: 2).Generate();

        _commentRepositoryMock.Setup(c => c.Find(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(comment);
        // act
        var result = await _sut.UpdateCommentAsync(updateCommentDto);
        // assert
        _commentRepositoryMock.Verify(c => c.Update(It.Is<Comment>(comment => comment.Content == updateCommentDto.UserComment)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task UpdateCommentAsync_WhenCommentNotFound_ReturnNotFound()
    {
        // arrange
        var updateCommentDto = CommentFaker.GetUpdateCommentDto().Generate();

        _commentRepositoryMock.Setup(c => c.Find(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync((Comment?)null);
        // act
        var result = await _sut.UpdateCommentAsync(updateCommentDto);
        // assert
        _commentRepositoryMock.Verify(c => c.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(c => c.SaveAsync(), Times.Never);

        result.IsSuccess.Should().BeFalse();
        result.ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(CommentErrors.NotFound);
    }
    [Fact]
    public async Task UpdateCommentAsync_WhenUserWhoEditingTheCommentIsNotTheOwner_ReturnAccessDenied()
    {
        // arrange
        // different seed's will generate different data, so the UserId in the UpdateCommentDto will be different from the UserId in the Comment.
        var updateCommentDto = CommentFaker.GetUpdateCommentDto(seed: 4).Generate();
        var comment = CommentFaker.GetComment(seed: 2).Generate();

        _commentRepositoryMock.Setup(c => c.Find(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(comment);
        // act 
        var result = await _sut.UpdateCommentAsync(updateCommentDto);
        // assert
        _commentRepositoryMock.Verify(c => c.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(c => c.SaveAsync(), Times.Never);

        result.ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(CommentErrors.AccessDenied);
    }
    [Fact]
    public async Task UpdatecommentAsync_WhenEditTimeIsExceeded_ReturnEditTimeout()
    {
        // arrange
        // different seeds here will create a comment with created time that exceed the allowed edit time
        // and the other values like (userId, commentId) will be the same.
        var updateCommentDto = CommentFaker.GetUpdateCommentDto(seed: 3).Generate();
        var comment = CommentFaker.GetComment(seed: 3).Generate();

        _commentRepositoryMock.Setup(c => c.Find(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(comment);
        // act
        var result = await _sut.UpdateCommentAsync(updateCommentDto);
        // assert
        _commentRepositoryMock.Verify(c => c.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(c => c.SaveAsync(), Times.Never);

        result.ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(CommentErrors.EditTimeout);
    }
    #endregion

    #region DeleteCommentAsync
    [Fact]
    public async Task DeleteCommentAsync_WithValidData_ReturnsSuccess()
    {
        // arrange
        var comment = CommentFaker.GetComment().Generate();
        _commentRepositoryMock.Setup(c => c.Find(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(comment);
        // act
        var result = await _sut.DeleteCommentAsync(comment.Id, comment.UserId);
        // assert
        _commentRepositoryMock.Verify(c => c.Delete(It.Is<Comment>(c => c.Id == comment.Id)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);

        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task DeleteCommentAsync_WhenCommentIsNotFound_ReturnNotFound()
    {
        // arrange
        _commentRepositoryMock.Setup(c => c.Find(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync((Comment?)null);
        // act
        var result = await _sut.DeleteCommentAsync(commentId: 1, userId: "userId");
        //  assert
        _commentRepositoryMock.Verify(c => c.Delete(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(c => c.SaveAsync(), Times.Never);

        result.IsSuccess.Should().BeFalse();
        result.ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(CommentErrors.NotFound);
    }
    [Fact]
    public async Task DeleteCommentAsync_WhenUserWhoDeletingTheCommentIsNotTheOwner_ReturnAccessDenied()
    {
        // arrange
        var comment = CommentFaker.GetComment().Generate();
        _commentRepositoryMock.Setup(c => c.Find(It.IsAny<Expression<Func<Comment, bool>>>())).ReturnsAsync(comment);
        // act
        var result = await _sut.DeleteCommentAsync(comment.Id, userId: "differentUserId");
        // assert
        _commentRepositoryMock.Verify(c => c.Delete(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(c => c.SaveAsync(), Times.Never);
        result.IsSuccess.Should().BeFalse();
        result.ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(CommentErrors.AccessDenied);
    }
    #endregion
}
