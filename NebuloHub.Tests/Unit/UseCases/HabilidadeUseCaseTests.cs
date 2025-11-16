using Moq;
using Xunit;
using System.Threading.Tasks;
using NebuloHub.Application.UseCase;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Repositores;

public class HabilidadeUseCaseTests
{
    private readonly Mock<IRepository<Habilidade>> _repositoryMock;
    private readonly HabilidadeUseCase _useCase;

    public HabilidadeUseCaseTests()
    {
        _repositoryMock = new Mock<IRepository<Habilidade>>();
        _useCase = new HabilidadeUseCase(_repositoryMock.Object);
    }

    // CREATE
    [Fact]
    public async Task CreateHabilidade_Sucesso()
    {
        var request = new CreateHabilidadeRequest
        {
            NomeHabilidade = "Programação",
            TipoHabilidade = "Backend"
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Habilidade>()))
                       .Callback<Habilidade>(e => e.IdHabilidade = 1)
                       .Returns(Task.CompletedTask);

        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.CreateHabilidadeAsync(request);

        Assert.NotNull(result);
        Assert.Equal(1, result.IdHabilidade);
    }

    // UPDATE
    [Fact]
    public async Task UpdateHabilidade_Sucesso()
    {
        var existente = new Habilidade
        {
            IdHabilidade = 1,
            NomeHabilidade = "Old",
            TipoHabilidade = "OldType"
        };

        var request = new CreateHabilidadeRequest
        {
            NomeHabilidade = "New",
            TipoHabilidade = "Backend"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
                       .ReturnsAsync(existente);

        _repositoryMock.Setup(r => r.Update(existente));
        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.UpdateHabilidadeAsync(1, request);

        Assert.True(result);
        Assert.Equal("New", existente.NomeHabilidade);
        Assert.Equal("Backend", existente.TipoHabilidade);
    }

    // DELETE
    [Fact]
    public async Task DeleteHabilidade_Sucesso()
    {
        var existente = new Habilidade { IdHabilidade = 1 };

        _repositoryMock.Setup(r => r.GetByIdAsync(1))
                       .ReturnsAsync(existente);

        _repositoryMock.Setup(r => r.Delete(existente));
        _repositoryMock.Setup(r => r.SaveChangesAsync())
                       .Returns(Task.CompletedTask);

        var result = await _useCase.DeleteHabilidadeAsync(1);

        Assert.True(result);
    }
}
