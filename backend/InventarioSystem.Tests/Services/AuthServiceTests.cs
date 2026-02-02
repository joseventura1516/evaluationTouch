using FluentAssertions;
using InventarioSystem.Core.DTOs;
using InventarioSystem.Core.Entities;
using InventarioSystem.Core.Interfaces;
using InventarioSystem.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace InventarioSystem.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Setup configuration
            _configurationMock.Setup(c => c["JwtSettings:SecretKey"])
                .Returns("SuperSecretKey123456789012345678901234567890");
            _configurationMock.Setup(c => c["JwtSettings:Issuer"])
                .Returns("TestIssuer");
            _configurationMock.Setup(c => c["JwtSettings:Audience"])
                .Returns("TestAudience");
            _configurationMock.Setup(c => c["JwtSettings:ExpirationMinutes"])
                .Returns("60");

            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "test@test.com",
                Password = "password123",
                Role = "Empleado"
            };

            _userRepositoryMock.Setup(r => r.EmailExistsAsync(registerDto.Email))
                .ReturnsAsync(false);

            _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) =>
                {
                    user.Id = 1;
                    return user;
                });

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Email.Should().Be(registerDto.Email);
            result.Data.Username.Should().Be(registerDto.Username);
            result.Data.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ReturnsFailure()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "existing@test.com",
                Password = "password123",
                Role = "Empleado"
            };

            _userRepositoryMock.Setup(r => r.EmailExistsAsync(registerDto.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("email ya está registrado");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = hashedPassword,
                Role = "Empleado",
                IsActive = true
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsFailure()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "wrongpassword"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = hashedPassword,
                Role = "Empleado",
                IsActive = true
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Credenciales inválidas");
        }

        [Fact]
        public async Task LoginAsync_WithNonExistingUser_ReturnsFailure()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexisting@test.com",
                Password = "password123"
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Credenciales inválidas");
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ReturnsFailure()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Empleado",
                IsActive = false
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("desactivado");
        }

        [Fact]
        public void GenerateJwtToken_ReturnsValidToken()
        {
            // Arrange
            var userId = 1;
            var email = "test@test.com";
            var role = "Administrador";

            // Act
            var token = _authService.GenerateJwtToken(userId, email, role);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Should().HaveCount(3); // JWT format: header.payload.signature
        }
    }
}
