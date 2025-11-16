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

        // Banco em memória
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);

        _useCase = new StartupUseCase(_repositoryMock.Object, _context);
    }

    // ------------------------------------
    // TESTE: Criar Startup
    // ------------------------------------
    [Fact]
    public async Task CreateStartup_Sucesso()
    {
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

        var result = await _useCase.CreateStartupAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.CNPJ, result.CNPJ);
        Assert.Equal(request.NomeStartup, result.NomeStartup);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Startup>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // ------------------------------------
    // TESTE: Atualizar Startup (sucesso)
    // ------------------------------------
    [Fact]
    public async Task UpdateStartup_Sucesso()
    {
        var cnpj = "38206824750298";

        // Criando startup usando construtor público
        var startupExistente = new Startup
        {
            CNPJ = cnpj,
            Video = "https://antigo.com",
            NomeStartup = "Nome Antigo",
            Site = "https://siteAntigo",
            Descricao = "Descricao antiga",
            NomeResponsavel = "Responsavel Antigo",
            EmailStartup = "email@antigo.com",
            UsuarioCPF = "48302968275"
        };

        var request = new CreateStartupRequest
        {
            CNPJ = cnpj,
            Video = "https://novo.com",
            NomeStartup = "Nome Novo",
            Site = "https://siteNovo",
            Descricao = "Nova descricao",
            NomeResponsavel = "Novo Responsavel",
            EmailStartup = "novo@email.com",
            UsuarioCPF = "48302968275"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(cnpj))
                       .ReturnsAsync(startupExistente);

        var result = await _useCase.UpdateStartupAsync(cnpj, request);

        Assert.True(result);
        Assert.Equal("Nome Novo", startupExistente.NomeStartup);

        _repositoryMock.Verify(r => r.GetByIdAsync(cnpj), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Startup>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // ------------------------------------
    // TESTE: Atualizar Startup (não existe)
    // ------------------------------------
    [Fact]
    public async Task UpdateStartup_Inexistente()
    {
        var cnpj = "38206824750298";

        _repositoryMock.Setup(r => r.GetByIdAsync(cnpj))
                       .ReturnsAsync((Startup?)null);

        var request = new CreateStartupRequest
        {
            CNPJ = cnpj,
            NomeStartup = "Teste"
        };

        var result = await _useCase.UpdateStartupAsync(cnpj, request);

        Assert.False(result);

        _repositoryMock.Verify(r => r.Update(It.IsAny<Startup>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    // ------------------------------------
    // TESTE: Deletar Startup (sucesso)
    // ------------------------------------
    [Fact]
    public async Task DeleteStartup_Sucesso()
    {
        var cnpj = "38206824750298";

        var startup = new Startup
        {
            CNPJ = cnpj,
            NomeStartup = "Teste"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(cnpj))
                       .ReturnsAsync(startup);

        var result = await _useCase.DeleteStartupAsync(cnpj);

        Assert.True(result);

        _repositoryMock.Verify(r => r.GetByIdAsync(cnpj), Times.Once);
        _repositoryMock.Verify(r => r.Delete(startup), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // ------------------------------------
    // TESTE: Deletar Startup (não existe)
    // ------------------------------------
    [Fact]
    public async Task DeleteStartup_Inexistente()
    {
        var cnpj = "38206824750298";

        _repositoryMock.Setup(r => r.GetByIdAsync(cnpj))
                       .ReturnsAsync((Startup?)null);

        var result = await _useCase.DeleteStartupAsync(cnpj);

        Assert.False(result);

        _repositoryMock.Verify(r => r.Delete(It.IsAny<Startup>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
