using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.UseCase;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;

public class PossuiUseCaseTests
{
    private readonly Mock<IRepository<Possui>> _repositoryMock;
    private readonly PossuiUseCase _useCase;
    private readonly AppDbContext _context;

    public PossuiUseCaseTests()
    {
        _repositoryMock = new Mock<IRepository<Possui>>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDB_Possui")
            .Options;

        _context = new AppDbContext(options);
        _useCase = new PossuiUseCase(_repositoryMock.Object, _context);
    }

    // CREATE
    [Fact]
    public async Task CreatePossui_Sucesso()
    {
        var request = new CreatePossuiRequest
        {
            StartupCNPJ = "123",
            IdHabilidade = 50
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Possui>()))
                       .Callback<Possui>(e => e.IdPossui = 1)
                       .Returns(Task.CompletedTask);

        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.CreatePossuiAsync(request);

        Assert.NotNull(result);
        Assert.Equal(1, result.IdPossui);
    }

    // UPDATE
    [Fact]
    public async Task UpdatePossui_Sucesso()
    {
        var existente = new Possui
        {
            IdPossui = 1,
            StartupCNPJ = "123",
            IdHabilidade = 100
        };

        var request = new CreatePossuiRequest
        {
            StartupCNPJ = "123",
            IdHabilidade = 999
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
                       .ReturnsAsync(existente);

        _repositoryMock.Setup(r => r.Update(existente));
        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.UpdatePossuiAsync(1, request);

        Assert.True(result);
        Assert.Equal(999, existente.IdHabilidade);
    }

    // DELETE
    [Fact]
    public async Task DeletePossui_Sucesso()
    {
        var existente = new Possui { IdPossui = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
                       .ReturnsAsync(existente);

        _repositoryMock.Setup(r => r.Delete(existente));
        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.DeletePossuiAsync(1);

        Assert.True(result);
    }
}
