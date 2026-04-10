using Application.MappingProfiles;
using Application.ServiceAbstractions;
using Application.Services;
using Application.Shared.Errors;
using Application.Shared.Pagination;
using AutoMapper;
using Domain.Constants;
using Domain.Contracts;
using Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Application.Tests.Services;

public class WorkspaceServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Workspace>> _workspaceRepositoryMock;
    private readonly Mock<IWorkSpaceMemberService> _workspaceMemberServiceMock;
    private readonly Mock<ITagService> _tagServiceMock;
    private readonly Mock<IQuestService> _questServiceMock;
    private readonly Mock<IInvitationService> _invitationServiceMock;

    private readonly WorkSpaceService _sut;
    private readonly IMapper _mapper;

    public WorkspaceServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _workspaceRepositoryMock = new Mock<IGenericRepository<Workspace>>();
        _unitOfWorkMock.Setup(u => u.Repository<Workspace>()).Returns(_workspaceRepositoryMock.Object);

        var nullLoggerFactory = NullLoggerFactory.Instance;
        var config = new MapperConfiguration(
            cnfg =>
            {
                cnfg.AddProfile<WorkspaceProfile>();
                cnfg.AddProfile<WorkspaceMemberProfile>();
            },
            nullLoggerFactory
        );
        _mapper = config.CreateMapper();

        _workspaceMemberServiceMock = new Mock<IWorkSpaceMemberService>();
        _tagServiceMock = new Mock<ITagService>();
        _questServiceMock = new Mock<IQuestService>();
        _invitationServiceMock = new Mock<IInvitationService>();

        _sut = new WorkSpaceService(_unitOfWorkMock.Object, _mapper, _workspaceMemberServiceMock.Object,
            _tagServiceMock.Object, _questServiceMock.Object, _invitationServiceMock.Object);
    }

    #region GetAllWorkspacesAsync Tests

    [Fact]
    public async Task GetAllWorkspacesAsync_WithValidInput_ReturnsPagedResponse()
    {
        // Arrange
        var userId = "test-user-id";
        var queryFilter = new QueryFilter { PageNumber = 1, PageSize = 10 };
        var workspaces = new List<Workspace>
        {
            CreateTestWorkspace(1, "Workspace 1", userId),
            CreateTestWorkspace(2, "Workspace 2", userId)
        };

        _workspaceRepositoryMock
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(2);

        _workspaceRepositoryMock
            .Setup(r => r.FindAll(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(workspaces);

        // Act
        var result = await _sut.GetAllWorkspacesAsync(queryFilter, userId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().HaveCount(2);
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task GetAllWorkspacesAsync_WithEmptyWorkspaces_ReturnsEmptyPagedResponse()
    {
        // Arrange
        var userId = "test-user-id";
        var queryFilter = new QueryFilter { PageNumber = 1, PageSize = 10 };
        var emptyWorkspaces = new List<Workspace>();

        _workspaceRepositoryMock
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(0);

        _workspaceRepositoryMock
            .Setup(r => r.FindAll(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(emptyWorkspaces);

        // Act
        var result = await _sut.GetAllWorkspacesAsync(queryFilter, userId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Data.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
    }

    [Fact]
    public async Task GetAllWorkspacesAsync_WithDifferentPages_ReturnsPaginatedResults()
    {
        // Arrange
        var userId = "test-user-id";
        var queryFilter = new QueryFilter { PageNumber = 2, PageSize = 5 };
        var workspaces = new List<Workspace>
        {
            CreateTestWorkspace(6, "Workspace 6", userId),
            CreateTestWorkspace(7, "Workspace 7", userId),
        };

        _workspaceRepositoryMock
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(15);

        _workspaceRepositoryMock
            .Setup(r => r.FindAll(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(workspaces);

        // Act
        var result = await _sut.GetAllWorkspacesAsync(queryFilter, userId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.PageNumber.Should().Be(2);
        result.Value.PageSize.Should().Be(5);
        result.Value.TotalRecords.Should().Be(15);
    }

    #endregion

    #region GetWorkSpaceById Tests

    [Fact]
    public async Task GetWorkSpaceById_WithValidId_ReturnsWorkspace()
    {
        // Arrange
        var workspaceId = 1;
        var workspace = CreateTestWorkspace(workspaceId, "Test Workspace", "owner-id");

        _workspaceRepositoryMock
            .Setup(r => r.GetByIdAsync(workspaceId))
            .ReturnsAsync(workspace);

        // Act
        var result = await _sut.GetWorkSpaceById(workspaceId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(workspaceId);
        result.Name.Should().Be("Test Workspace");
    }

    [Fact]
    public async Task GetWorkSpaceById_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var workspaceId = 999;

        _workspaceRepositoryMock
            .Setup(r => r.GetByIdAsync(workspaceId))
            .ReturnsAsync((Workspace?)null);

        // Act
        var result = await _sut.GetWorkSpaceById(workspaceId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetWorkSpaceByIdAsync Tests

    [Fact]
    public async Task GetWorkSpaceByIdAsync_WhenUserIsOwner_ReturnsWorkspaceDetails()
    {
        // Arrange
        var workspaceId = 1;
        var userId = "owner-id";
        var workspace = CreateTestWorkspace(workspaceId, "Test Workspace", userId);

        _workspaceRepositoryMock
            .Setup(r => r.Find(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(workspace);

        // Act
        var result = await _sut.GetWorkSpaceByIdAsync(workspaceId, userId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkSpaceByIdAsync_WhenUserIsMember_ReturnsWorkspaceDetails()
    {
        // Arrange
        var workspaceId = 1;
        var userId = "member-id";
        var ownerId = "owner-id";
        var workspace = CreateTestWorkspace(workspaceId, "Test Workspace", ownerId);
        workspace.WorkspaceMembers.Add(new WorkspaceMember
        {
            UserId = userId,
            WorkspaceId = workspaceId,
            Role = Role.Member,
            User = new User { Id = userId, UserName = "member", Email = "member@test.com", Name = "Member Name" }
        });

        _workspaceRepositoryMock
            .Setup(r => r.Find(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(workspace);

        // Act
        var result = await _sut.GetWorkSpaceByIdAsync(workspaceId, userId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkSpaceByIdAsync_WhenWorkspaceNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var workspaceId = 999;
        var userId = "test-user";

        _workspaceRepositoryMock
            .Setup(r => r.Find(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync((Workspace?)null);

        // Act
        var result = await _sut.GetWorkSpaceByIdAsync(workspaceId, userId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorsList.Should().HaveCount(1);
        result.ErrorsList.First().Code.Should().Be(WorkspaceErrors.NotFound.Code);
    }

    [Fact]
    public async Task GetWorkSpaceByIdAsync_WhenUserHasNoAccess_ReturnsAccessDeniedError()
    {
        // Arrange
        var workspaceId = 1;
        var userId = "unauthorized-user";
        var ownerId = "owner-id";
        var workspace = CreateTestWorkspace(workspaceId, "Test Workspace", ownerId);
        workspace.WorkspaceMembers.Clear();

        _workspaceRepositoryMock
            .Setup(r => r.Find(It.IsAny<ISpecification<Workspace>>()))
            .ReturnsAsync(workspace);

        // Act
        var result = await _sut.GetWorkSpaceByIdAsync(workspaceId, userId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorsList.Should().HaveCount(1);
        result.ErrorsList.First().Code.Should().Be(WorkspaceErrors.AccessDenied.Code);
    }

    #endregion

    #region Helper Methods

    private static Workspace CreateTestWorkspace(int id, string name, string ownerId)
    {
        return new Workspace
        {
            Id = id,
            Name = name,
            Avatar = "avatar.jpg",
            CreatedAt = DateTime.UtcNow,
            OwnerId = ownerId,
            WorkspaceMembers = new List<WorkspaceMember>(),
            Spaces = new List<Space>(),
            Tags = new List<Tag>(),
            Categories = new List<Category>(),
            Invitations = new List<Invitation>(),
            User = new User { Id = ownerId, UserName = "owner", Name = "Owner Name" }
        };
    }

    #endregion
}
