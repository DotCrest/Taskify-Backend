using Application.MappingProfiles;
using Application.ServiceAbstractions;
using Application.Services;
using Application.Shared.Errors;
using Application.Tests.Fakers;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Domain.Settings;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Application.Tests.Services;

public class InvitationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Invitation>> _invitationRepositoryMock;
    private readonly Mock<IGenericRepository<Workspace>> _workspaceRepositoryMock;
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IWorkSpaceMemberService> _workSpaceMemberServiceMock;
    private readonly IOptions<UrlOptions> _urlOptions;

    private readonly IMapper _mapper;
    private readonly InvitationService _sut;

    public InvitationServiceTests()
    {
        // Repositories
        _invitationRepositoryMock = new Mock<IGenericRepository<Invitation>>();
        _workspaceRepositoryMock = new Mock<IGenericRepository<Workspace>>();
        // unitOfWork
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Repository<Invitation>()).Returns(_invitationRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Workspace>()).Returns(_workspaceRepositoryMock.Object);
        // services
        _accountServiceMock = new Mock<IAccountService>();
        _emailServiceMock = new Mock<IEmailService>();
        _workSpaceMemberServiceMock = new Mock<IWorkSpaceMemberService>();
        // options
        var mySettings = new UrlOptions
        {
            FrontendUrl = "https://example.com"
        };
        _urlOptions = Options.Create(mySettings);

        // mapper
        var nullLoggerFactory = new NullLoggerFactory();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InvitationProfile>();
        },
        nullLoggerFactory);
        _mapper = config.CreateMapper();

        _sut = new InvitationService(
            _unitOfWorkMock.Object,
            _accountServiceMock.Object,
            _emailServiceMock.Object,
            _workSpaceMemberServiceMock.Object,
            _urlOptions,
            _mapper,
            NullLogger<InvitationService>.Instance
        );
    }
    #region GetAllInvitationsAsync
    // Read the summary Tag for `GetFakeGetInvitationDto` & `GetFakeInvitation` in the InvitationFaker class to understand why we are using different seeds for the test cases
    [Theory]
    [InlineData(0, 2)]
    [InlineData(2, 6)]
    [InlineData(3, 7)]
    public async Task GetAllInvitationsAsync_WithValidInput_Returns_PagedResponseWithSpecificStatus(int index, int seed)
    {
        // arrange
        var fakeQueryFilter = InvitationFaker.GetFakeQueryFilter().Generate();
        var fakeGetInvitationDtos = InvitationFaker.GetFakeGetInvitationDto(index, seed).Generate();
        var fakeInvitations = InvitationFaker.GetFakeInvitation(index, seed)
            .RuleFor(i => i.WorkspaceId, f => fakeGetInvitationDtos.WorkspaceId)
            .RuleFor(i => i.SenderId, f => fakeGetInvitationDtos.UserId)
            .Generate(5);

        var workspace = new Workspace { Id = fakeInvitations[0].WorkspaceId, OwnerId = fakeGetInvitationDtos.UserId! };

        _workspaceRepositoryMock.Setup(w => w.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(workspace);
        _invitationRepositoryMock.Setup(i => i.FindAll(It.IsAny<ISpecification<Invitation>>())).ReturnsAsync(fakeInvitations);
        _invitationRepositoryMock.Setup(i => i.CountAsync(It.IsAny<ISpecification<Invitation>>())).ReturnsAsync(5);
        // act
        var result = await _sut.GetAllInvitationsAsync(fakeQueryFilter, fakeGetInvitationDtos);
        // assert

        _workspaceRepositoryMock.Verify(w => w.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _invitationRepositoryMock.Verify(i => i.FindAll(It.IsAny<ISpecification<Invitation>>()), Times.Once);

        result.Should().NotBeNull();
        result!.Value!.Data.Should().NotBeNull();
        result.Value.TotalRecords.Should().Be(5);
        if (seed % 2 == 0) // if the seed is even, the status will be the same as the one in the fakeGetInvitationDtos
        {
            result.Value.Data.Should().AllSatisfy(invitation =>
                invitation.Status
                .Should()
                .Be(
                    Enum.Parse<InvitationStatusEnum>(fakeGetInvitationDtos.Status!, true)
                ));
        }
    }

    [Fact]
    public async Task GetAllInvitationsAsync_WithInValidWorkspaceId_ReturnsNotFound()
    {
        // arrange
        var fakeQueryFilter = InvitationFaker.GetFakeQueryFilter().Generate();
        var fakeGetInvitationDtos = InvitationFaker.GetFakeGetInvitationDto(2).Generate();

        _workspaceRepositoryMock.Setup(w => w.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Workspace)null);
        // act
        var result = await _sut.GetAllInvitationsAsync(fakeQueryFilter, fakeGetInvitationDtos);
        // assert
        result.Should().NotBeNull();
        result!.Value.Should().BeNull();
        result.ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(WorkspaceErrors.NotFound);
    }

    [Fact]
    public async Task GetAllInvitationsAsync_WhenUserIsNotTheOwner_ReturnsAccessDenied()
    {
        // arrange
        var fakeQueryFilter = InvitationFaker.GetFakeQueryFilter().Generate();
        var fakeGetInvitationDtos = InvitationFaker.GetFakeGetInvitationDto(2).Generate();
        var workspace = new Workspace { Id = fakeGetInvitationDtos.WorkspaceId, OwnerId = fakeGetInvitationDtos.UserId! + 1 };

        _workspaceRepositoryMock.Setup(w => w.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(workspace);
        // act
        var result = await _sut.GetAllInvitationsAsync(fakeQueryFilter, fakeGetInvitationDtos);
        // assert
        result.Should().NotBeNull();
        result!.Value.Should().BeNull();
        result.ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(WorkspaceErrors.AccessDenied);
    }
    #endregion
    #region SendInvitationAsync
    [Fact]
    public async Task SendInvitationAsync_WithValidInput_SendEmailAndReturnBaseToReturn()
    {
        // arrange
        var sendInvitationDto = InvitationFaker.GetFakeSendInvitationDto().Generate();
        var user = new User { Id = Guid.NewGuid().ToString(), Name = "John Doe" };
        var workspace = new Workspace { Id = sendInvitationDto.WorkspaceId, Name = "Test Workspace", OwnerId = user.Id };

        _accountServiceMock.Setup(a => a.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _workspaceRepositoryMock.Setup(w => w.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(workspace);
        _invitationRepositoryMock.Setup(i => i.Find(It.IsAny<ISpecification<Invitation>>())).ReturnsAsync((Invitation?)null);
        // act
        var result = await _sut.SendInvitationAsync(sendInvitationDto, user.Id);
        // assert
        _accountServiceMock.Verify(a => a.GetUserByIdAsync(It.IsAny<string>()), Times.Once);
        _workspaceRepositoryMock.Verify(w => w.GetByIdAsync(It.IsAny<int>()), Times.Once);
        _invitationRepositoryMock.Verify(i => i.Find(It.IsAny<ISpecification<Invitation>>()), Times.Once);
        _emailServiceMock.Verify(e => e.SendEmailAsync(
            It.Is<string>(email => email == sendInvitationDto.ReceiverEmail),
            It.Is<string>(subject => subject.Contains("You have been invited to join a workspace!")),
            It.Is<string>(body => body.Contains(workspace.Name))
        ),
        Times.Once);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task SendInvitationAsync_WhenUserIsNotFound_ReturnNotFound()
    {
        // arrange
        var sendInvitationDto = InvitationFaker.GetFakeSendInvitationDto().Generate();
        var userId = Guid.NewGuid().ToString();

        _accountServiceMock.Setup(a => a.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        // act
        var result = await _sut.SendInvitationAsync(sendInvitationDto, userId);

        // assert
        _accountServiceMock.Verify(a => a.GetUserByIdAsync(It.IsAny<string>()), Times.Once);
        _workspaceRepositoryMock.Verify(w => w.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _invitationRepositoryMock.Verify(i => i.Find(It.IsAny<ISpecification<Invitation>>()), Times.Never);
        _emailServiceMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        result.Should().NotBeNull();
        result.Value.Should().BeNull();
        result.ErrorsList
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .Be(AuthErrors.UserNotFound);
    }

    #endregion

}
