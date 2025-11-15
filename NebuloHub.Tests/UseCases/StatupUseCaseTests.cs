using Moq;
using NebuloHub.Application.UseCase;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;
using Microsoft.EntityFrameworkCore;

public class StartupUseCaseTests
{
    private readonly Mock<IRepository<Startup>> _repositoryMock;
    private readonly AppDbContext _context;
    private readonly StartupUseCase _useCase;

    public StartupUseCaseTests()
    {
        _repositoryMock = new Mock<IRepository<Startup>>();

        // Banco em memória para simular o Oracle
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);

        _useCase = new StartupUseCase(_repositoryMock.Object, _context);
    }

    [Fact]
    public async Task CreateStartup_DeveCriarStartupComSucesso()
    {
        // Arrange
        var request = new CreateStartupRequest
        {
            CNPJ = "38206824750298",
            Video = "https://videotop.com.br",
            NomeStartup = "SuperNova",
            Site = "https://SuperNova",
            Descricao = "Supernova startup para inovacoes",
            NomeResponsavel = "Joao carlos de almeida",
            EmailStartup = "SuperNova@gmail.com",
            UsuarioCPF = "48302968275"

        };

        // Act
        var result = await _useCase.CreateStartupAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.CNPJ, result.CNPJ);
        Assert.Equal(request.NomeStartup, result.NomeStartup);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Startup>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
