using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading.Tasks;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.UseCase;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;

public class AvaliacaoUseCaseTests
{
    private readonly Mock<IRepository<Avaliacao>> _repositoryMock;
    private readonly AvaliacaoUseCase _useCase;
    private readonly AppDbContext _context;

    public AvaliacaoUseCaseTests()
    {
        _repositoryMock = new Mock<IRepository<Avaliacao>>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDB_Avaliacao")
            .Options;

        _context = new AppDbContext(options);
        _useCase = new AvaliacaoUseCase(_repositoryMock.Object, _context);
    }

    // CREATE
    [Fact]
    public async Task CreateAvaliacao_Sucesso()
    {
        var request = new CreateAvaliacaoRequest
        {
            Nota = 10,
            Comentario = "Muito bom",
            UsuarioCPF = "111",
            StartupCNPJ = "222"
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Avaliacao>()))
                       .Callback<Avaliacao>(a => a.IdAvaliacao = 1)
                       .Returns(Task.CompletedTask);

        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.CreateAvaliacaoAsync(request);

        Assert.NotNull(result);
        Assert.Equal(1, result.IdAvaliacao);
    }

    // UPDATE
    [Fact]
    public async Task UpdateAvaliacao_Sucesso()
    {
        var existente = new Avaliacao
        {
            IdAvaliacao = 1,
            Nota = 5,
            Comentario = "Teste"
        };

        var request = new CreateAvaliacaoRequest
        {
            Nota = 9,
            Comentario = "Excelente",
            UsuarioCPF = "111",
            StartupCNPJ = "222"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
                       .ReturnsAsync(existente);

        _repositoryMock.Setup(r => r.Update(existente));
        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.UpdateAvaliacaoAsync(1, request);

        Assert.True(result);
        Assert.Equal(9, existente.Nota);
        Assert.Equal("Excelente", existente.Comentario);
    }

    // DELETE
    [Fact]
    public async Task DeleteAvaliacao_Sucesso()
    {
        var existente = new Avaliacao { IdAvaliacao = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
                       .ReturnsAsync(existente);

        _repositoryMock.Setup(r => r.Delete(existente));
        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.DeleteAvaliacaoAsync(1);

        Assert.True(result);
    }
}
