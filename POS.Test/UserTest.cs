using Application.Interfaces;
using Application.Mapper;
using Application.Models;
using Application.Services;
using AutoMapper;
using Moq;
using WebAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace POS.Test
{
    public class UserTest
    {
        private Mock<IUserRepositories> _userRepo;
        private readonly IMapper _mapper;

        public UserTest()
        {
            _userRepo = new Mock<IUserRepositories>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<User, UserDTO>();
            });
            _mapper = configuration.CreateMapper();
        }

        public UserService Subject()
        {
            return new UserService(_userRepo.Object, _mapper);
        }
        [Fact]
        public async Task GetUserListAsync()
        {
            var service = Subject();
            List<User> users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "johndoe",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "johndoe@example.com",
                    Password = "password123", // Always hash passwords in real applications
                    Gender = 1,
                    RoleId = 2,
                    CreateId = 101,
                    CreateDate = DateTime.Now.AddDays(-30),
                    UpdateId = 102,
                    UpdateDate = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Username = "janedoe",
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "janedoe@example.com",
                    Password = "securePass456", // Always hash passwords in real applications
                    Gender = 2,
                    RoleId = 3,
                    CreateId = 101,
                    CreateDate = DateTime.Now.AddDays(-20),
                    UpdateId = 102,
                    UpdateDate = DateTime.Now
                },
                new User
                {
                    Id = 3,
                    Username = "alexsmith",
                    FirstName = "Alex",
                    LastName = "Smith",
                    Email = "alexsmith@example.com",
                    Password = "password789", // Always hash passwords in real applications
                    Gender = 1,
                    RoleId = 1,
                    CreateId = 101,
                    CreateDate = DateTime.Now.AddDays(-10),
                    UpdateId = 103,
                    UpdateDate = DateTime.Now
                }
            };
            _userRepo.Setup(r => r.GetUserList()).ReturnsAsync(users);

            var data = await service.GetUserList();

            Assert.NotNull(data);
        }
        [Fact]
        public async Task DeleteUser_Success()
        {
            var service = Subject();
            ReturnStatus result = new()
            {
                Status = 0,
                Message = "Successfull! User was successfully deleted"
            };
            User user = new User
            {
                Id = 1,
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123", // Always hash passwords in real applications
                Gender = 1,
                RoleId = 2,
                CreateId = 101,
                CreateDate = DateTime.Now.AddDays(-30),
                UpdateId = 102,
                UpdateDate = DateTime.Now
            };

            _userRepo.Setup(r => r.GetUser(user.Id)).ReturnsAsync(user);
            _userRepo.Setup(repo => repo.DeleteUser(user.Id)).ReturnsAsync(result);

            ReturnStatus data = await service.DeleteUser(user.Id);

            Assert.Equal(result.Status, data.Status);
            Assert.Equal(result.Message, data.Message);
        }
        [Fact]
        public async Task DeleteUser_UserNotFound()
        {
            var service = Subject();
            int UserID = 1;
            ReturnStatus result = new()
            {
                Status = 1,
                Message = "No user found"
            };

            _userRepo.Setup(r => r.GetUser(UserID)).ReturnsAsync(new User());
            _userRepo.Setup(repo => repo.DeleteUser(UserID)).ReturnsAsync(result);

            ReturnStatus data = await service.DeleteUser(UserID);

            Assert.Equal(result.Status, data.Status);
            Assert.Equal(result.Message, data.Message);
        }
        [Fact]
        public async Task DeleteUser_Throws()
        {
            var service = Subject();

            // Arrange
            int userId = 1;
            var exceptionMessage = "Unexpected error";
            ReturnStatus result = new()
            {
                Status = 1,
                Message = "Unexpected error"
            };

            _userRepo.Setup(repo => repo.GetUser(userId)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var data = await service.DeleteUser(userId);

            // Assert
            Assert.Equal(result.Status, data.Status);
            Assert.Equal(result.Message, data.Message);
        }
        [Fact]
        public async Task SaveUser_AddUserSuccess()
        {
            // Arrange
            var service = Subject();
            ReturnStatus expectedReturnStatus = new()
            {
                Status = 0,
                Message = "Successful! User was successfully added"
            };

            // Mock repository methods
            _userRepo.Setup(r => r.GetUser(It.IsAny<int>())).ReturnsAsync(new User()); // Mock for new user (no existing user with the same ID)
            _userRepo.Setup(r => r.AddUser(It.IsAny<User>())).ReturnsAsync(expectedReturnStatus);  // Mock AddUser to return success

            // Test input
            UserDTO userDTO = new()
            {
                Id = 0,  // Indicating a new user
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123", // Always hash passwords in real applications
                Gender = 1,
                RoleId = 2
            };

            // Act
            ReturnStatus actualResult = await service.SaveUser(userDTO);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(expectedReturnStatus.Status, actualResult.Status);
            Assert.Equal(expectedReturnStatus.Message, actualResult.Message);

            // Verify AddUser was called once
            _userRepo.Verify(repo => repo.AddUser(It.Is<User>(u => u.Username == "johndoe" && u.Id == 0)), Times.Once);
        }
        [Fact]
        public async Task SaveUser_AddUserDuplicate()
        {
            var service = Subject();
            ReturnStatus result = new()
            {
                Status = 1,
                Message = "Username already existing"
            };
            User user = new User
            {
                Id = 0,
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123", // Always hash passwords in real applications
                Gender = 1,
                RoleId = 2,
                CreateId = 101,
                CreateDate = DateTime.Now.AddDays(-30),
                UpdateId = 102,
                UpdateDate = DateTime.Now
            };

            _userRepo.Setup(r => r.GetUser(user.Username)).ReturnsAsync(user);
            _userRepo.Setup(repo => repo.AddUser(user)).ReturnsAsync(new ReturnStatus() { Status = 0, Message = "Successfull! User was successfully added" });
            _userRepo.Setup(repo => repo.UpdateUser(user)).ReturnsAsync(new ReturnStatus() { Status = 0, Message = "Successfull! User was successfully updated" });

            ReturnStatus data = await service.SaveUser(new UserDTO
            {
                Id = 0,
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123", // Always hash passwords in real applications
                Gender = 1,
                RoleId = 2,
            });

            Assert.Equal(result.Status, data.Status);
            Assert.Equal(result.Message, data.Message);
        }
        [Fact]
        public async Task SaveUser_AddUser_Throw()
        {
            var service = Subject();
            ReturnStatus result = new()
            {
                Status = 1,
                Message = "Unexpected error"
            };

            _userRepo.Setup(r => r.GetUser("Username")).ReturnsAsync(new User());
            _userRepo.Setup(repo => repo.AddUser(It.IsAny<User>())).ThrowsAsync(new Exception("Unexpected error"));

            ReturnStatus data = await service.SaveUser(new UserDTO
            {
                Id = 0,
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123", // Always hash passwords in real applications
                Gender = 1,
                RoleId = 2
            });

            Assert.Equal(result.Status, data.Status);
            Assert.Equal(result.Message, data.Message);
        }
        [Fact]
        public async Task SaveUser_UpdateUserSuccess()
        {
            // Arrange
            var service = Subject();
            var expectedResult = new ReturnStatus() { Status = 0, Message = "Successful! User was successfully updated" };
            User user = new User
            {
                Id = 1,
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123", // Always hash passwords in real applications
                Gender = 1,
                RoleId = 2,
                CreateId = 101,
                CreateDate = DateTime.Now.AddDays(-30),
                UpdateId = 102,
                UpdateDate = DateTime.Now
            };

            // Mock repository methods
            _userRepo.Setup(r => r.GetUser(user.Username)).ReturnsAsync(user);  // Get user by Username
            _userRepo.Setup(r => r.GetUser(user.Id)).ReturnsAsync(user);  // Get user by Id
            _userRepo.Setup(repo => repo.AddUser(It.IsAny<User>())).ReturnsAsync(new ReturnStatus() { Status = 0, Message = "Successful! User was successfully added" });
            _userRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>())).ReturnsAsync(new ReturnStatus() { Status = 0, Message = "Successful! User was successfully updated" });  

            // Map User to UserDTO
            UserDTO userDTO = _mapper.Map<UserDTO>(user);

            // Act
            ReturnStatus actualResult = await service.SaveUser(userDTO);

            // Assert
            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult.Status, actualResult.Status);
            Assert.Equal(expectedResult.Message, actualResult.Message);

            // Verify that UpdateUser was called with the correct user
            _userRepo.Verify(repo => repo.UpdateUser(It.Is<User>(u => u.Id == user.Id && u.Username == user.Username)), Times.Once);
        }

        [Fact]
        public async Task SaveUser_UpdateUserDuplicate()
        {
            var service = Subject();
            ReturnStatus result = new()
            {
                Status = 1,
                Message = "Username already existing"
            };
            User user = new User
            {
                Id = 1,
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123", // Always hash passwords in real applications
                Gender = 1,
                RoleId = 2,
            };
            _userRepo.Setup(r => r.GetUser("johndoe")).ReturnsAsync(new User
            {
                Id = 2,
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "password123", // Always hash passwords in real applications
                Gender = 1,
                RoleId = 2,
                CreateId = 101,
                CreateDate = DateTime.Now.AddDays(-30),
                UpdateId = 102,
                UpdateDate = DateTime.Now
            });
            _userRepo.Setup(repo => repo.AddUser(user)).ReturnsAsync(new ReturnStatus() { Status = 0, Message = "Successfull! User was successfully added" });
            _userRepo.Setup(repo => repo.UpdateUser(user)).ReturnsAsync(new ReturnStatus() { Status = 0, Message = "Successfull! User was successfully updated" });

            UserDTO Param = _mapper.Map<UserDTO>(user);
            ReturnStatus data = await service.SaveUser(Param);

            Assert.Equal(result.Status, data.Status);
            Assert.Equal(result.Message, data.Message);
        }
    }
}