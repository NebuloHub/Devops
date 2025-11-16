using Moq;
using NebuloHub.Application.UseCase;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;
using Microsoft.EntityFrameworkCore;

public class UsuarioUseCaseTests
{
    private readonly Mock<IRepository<Usuario>> _repositoryMock;
    private readonly AppDbContext _context;
    private readonly UsuarioUseCase _useCase;

    public UsuarioUseCaseTests()
    {
        _repositoryMock = new Mock<IRepository<Usuario>>();

        // Banco em memória simulando Oracle
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);

        _useCase = new UsuarioUseCase(_repositoryMock.Object, _context);
    }

    // -----------------------------
    // TESTE: Criar usuário
    // -----------------------------
    [Fact]
    public async Task CreateUsuario()
    {
        var request = new CreateUsuarioRequest
        {
            CPF = "48302968275",
            Nome = "Teste",
            Email = "testeDaSilva@gmail.com",
            Senha = "Teste@010203",
            Role = NebuloHub.Domain.Enum.Role.USER,
            Telefone = 11972846657
        };

        var result = await _useCase.CreateUsuarioAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.CPF, result.CPF);
        Assert.Equal(request.Nome, result.Nome);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // -----------------------------
    // TESTE: Atualizar usuário (sucesso)
    // -----------------------------
    [Fact]
    public async Task UpdateUsuario_Sucesso()
    {
        var cpf = "48302968275";

        // Criando usuário usando construtor público (sem Create)
        var usuarioExistente = new Usuario
        {
            CPF = cpf,
            Nome = "Nome Antigo",
            Email = "a@a.com",
            Senha = "123",
            Role = NebuloHub.Domain.Enum.Role.USER,
            Telefone = 119999999
        };

        var request = new CreateUsuarioRequest
        {
            CPF = cpf,
            Nome = "Nome Novo",
            Email = "novo@email.com",
            Senha = "456",
            Role = NebuloHub.Domain.Enum.Role.ADMIN,
            Telefone = 11911111111
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(cpf))
                       .ReturnsAsync(usuarioExistente);

        var result = await _useCase.UpdateUsuarioAsync(cpf, request);

        Assert.True(result);
        Assert.Equal("Nome Novo", usuarioExistente.Nome);

        _repositoryMock.Verify(r => r.GetByIdAsync(cpf), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Usuario>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }


    // -----------------------------
    // TESTE: Atualizar usuário inexistente
    // -----------------------------
    [Fact]
    public async Task UpdateUsuario_Inexistente()
    {
        var cpf = "48302968275";

        _repositoryMock.Setup(r => r.GetByIdAsync(cpf))
                       .ReturnsAsync((Usuario?)null);

        var request = new CreateUsuarioRequest
        {
            CPF = cpf,
            Nome = "Teste",
            Email = "email@email.com",
            Senha = "123",
            Role = NebuloHub.Domain.Enum.Role.USER,
            Telefone = 123456789
        };

        var result = await _useCase.UpdateUsuarioAsync(cpf, request);

        Assert.False(result);

        _repositoryMock.Verify(r => r.Update(It.IsAny<Usuario>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    // -----------------------------
    // TESTE: Deletar usuário (sucesso)
    // -----------------------------
    [Fact]
    public async Task DeleteUsuario_Sucesso()
    {
        var cpf = "48302968275";

        var usuario = new Usuario
        {
            CPF = cpf,
            Nome = "Teste",
            Email = "email@email.com",
            Senha = "123",
            Role = NebuloHub.Domain.Enum.Role.USER,
            Telefone = 119999999
        };


        _repositoryMock.Setup(r => r.GetByIdAsync(cpf))
                       .ReturnsAsync(usuario);

        var result = await _useCase.DeleteUsuarioAsync(cpf);

        Assert.True(result);

        _repositoryMock.Verify(r => r.GetByIdAsync(cpf), Times.Once);
        _repositoryMock.Verify(r => r.Delete(usuario), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // -----------------------------
    // TESTE: Deletar usuário inexistente
    // -----------------------------
    [Fact]
    public async Task DeleteUsuario_Inexistente()
    {
        var cpf = "48302968275";

        _repositoryMock.Setup(r => r.GetByIdAsync(cpf))
                       .ReturnsAsync((Usuario?)null);

        var result = await _useCase.DeleteUsuarioAsync(cpf);

        Assert.False(result);

        _repositoryMock.Verify(r => r.Delete(It.IsAny<Usuario>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
