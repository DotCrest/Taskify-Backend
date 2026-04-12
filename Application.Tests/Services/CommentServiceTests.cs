using Application.MappingProfiles;
using Application.ServiceAbstractions;
using Application.Services;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Application.Tests.Services;

internal class CommentServiceTests
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
}
