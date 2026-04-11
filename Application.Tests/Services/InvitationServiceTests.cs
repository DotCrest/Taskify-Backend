using Application.MappingProfiles;
using Application.ServiceAbstractions;
using Application.Services;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Domain.Settings;
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
}
