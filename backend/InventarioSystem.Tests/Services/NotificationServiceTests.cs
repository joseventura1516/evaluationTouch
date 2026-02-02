using FluentAssertions;
using InventarioSystem.Core.Entities;
using InventarioSystem.Core.Interfaces;
using InventarioSystem.Core.Services;
using Moq;
using Xunit;

namespace InventarioSystem.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _notificationService = new NotificationService(
                _notificationRepositoryMock.Object,
                _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetNotificationsByUserIdAsync_ReturnsUserNotifications()
        {
            // Arrange
            var userId = 1;
            var notifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, Message = "Test 1", Type = "Info" },
                new Notification { Id = 2, UserId = userId, Message = "Test 2", Type = "Warning" }
            };

            _notificationRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(notifications);

            // Act
            var result = await _notificationService.GetNotificationsByUserIdAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetUnreadCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var userId = 1;
            _notificationRepositoryMock.Setup(r => r.GetUnreadCountByUserIdAsync(userId))
                .ReturnsAsync(5);

            // Act
            var result = await _notificationService.GetUnreadCountAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().Be(5);
        }

        [Fact]
        public async Task MarkAsReadAsync_WithValidNotification_ReturnsSuccess()
        {
            // Arrange
            var notification = new Notification
            {
                Id = 1,
                UserId = 1,
                Message = "Test",
                IsRead = false
            };

            _notificationRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(notification);

            _notificationRepositoryMock.Setup(r => r.MarkAsReadAsync(1))
                .ReturnsAsync(new Notification { Id = 1, UserId = 1, Message = "Test", IsRead = true });

            // Act
            var result = await _notificationService.MarkAsReadAsync(1, 1);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task MarkAsReadAsync_WithWrongUser_ReturnsFailure()
        {
            // Arrange
            var notification = new Notification
            {
                Id = 1,
                UserId = 2, // Different user
                Message = "Test",
                IsRead = false
            };

            _notificationRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(notification);

            // Act
            var result = await _notificationService.MarkAsReadAsync(1, 1); // User 1 trying to access User 2's notification

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("permiso");
        }

        [Fact]
        public async Task MarkAsReadAsync_WithNonExistingNotification_ReturnsFailure()
        {
            // Arrange
            _notificationRepositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Notification?)null);

            // Act
            var result = await _notificationService.MarkAsReadAsync(999, 1);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no encontrada");
        }

        [Fact]
        public async Task NotifyAdministratorsAsync_CreatesNotificationsForAllAdmins()
        {
            // Arrange
            var admins = new List<User>
            {
                new User { Id = 1, Username = "admin1", Role = "Administrador" },
                new User { Id = 2, Username = "admin2", Role = "Administrador" }
            };

            _userRepositoryMock.Setup(r => r.GetAdministratorsAsync())
                .ReturnsAsync(admins);

            _notificationRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Notification>()))
                .ReturnsAsync((Notification n) => n);

            // Act
            await _notificationService.NotifyAdministratorsAsync("Low stock alert", "Warning");

            // Assert
            _notificationRepositoryMock.Verify(
                r => r.CreateAsync(It.IsAny<Notification>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task CreateNotificationAsync_ReturnsCreatedNotification()
        {
            // Arrange
            _notificationRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Notification>()))
                .ReturnsAsync((Notification n) =>
                {
                    n.Id = 1;
                    return n;
                });

            // Act
            var result = await _notificationService.CreateNotificationAsync(1, "Test message", "Info");

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Message.Should().Be("Test message");
        }

        [Fact]
        public async Task MarkAllAsReadAsync_ReturnsSuccess()
        {
            // Arrange
            var userId = 1;
            _notificationRepositoryMock.Setup(r => r.MarkAllAsReadByUserIdAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _notificationService.MarkAllAsReadAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
        }
    }
}
