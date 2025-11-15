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

        // Banco em memória para simular o Oracle
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);

        _useCase = new UsuarioUseCase(_repositoryMock.Object, _context);
    }

    [Fact]
    public async Task CreateUsuario_DeveCriarUsuarioComSucesso()
    {
        // Arrange
        var request = new CreateUsuarioRequest
        {
            CPF = "48302968275",
            Nome = "Teste",
            Email = "testeDaSilva@gmail.com",
            Senha = "Teste@010203",
            Role = NebuloHub.Domain.Enum.Role.USER,
            Telefone = 11972846657
            
        };

        // Act
        var result = await _useCase.CreateUsuarioAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.CPF, result.CPF);
        Assert.Equal(request.Nome, result.Nome);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
