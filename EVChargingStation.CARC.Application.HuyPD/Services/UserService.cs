using EVChargingStation.CARC.Application.HuyPD.Utils;
using EVChargingStation.CARC.Domain.HuyPD.DTOs.AuthDTO;
using EVChargingStation.CARC.Domain.HuyPD.Entities;
using EVChargingStation.CARC.Infrastructure.HuyPD.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVChargingStation.CARC.Application.HuyPD.Services
{
    public interface IUserService
    {
        public Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO, IConfiguration configuration);
        public Task<LoginResponseDTO> RefreshTokenAsync(String refreshToken, IConfiguration configuration);
        public Task<bool> Logout(Guid userId);
        public Task RegisterAsync(RegisterDTO registerDTO);
        public Task<List<User>> GetAllUserAsync();
        public Task<User> GetUserById(Guid id);
        public Task<bool> DeleteUser(Guid id);
    }
    public class UserService : IUserService
    {
        IUnitOfWork _unitOfWork;
        private readonly ICurrentTime currentTime;
        private readonly PasswordHasher passwordHasher;

        public UserService(IUnitOfWork unitOfWork, ICurrentTime currentTime, PasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            this.currentTime = currentTime;
            this.passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO, IConfiguration configuration)
        {
            User user = (await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email && u.IsDeleted == false))
                            ?? throw new UnauthorizedAccessException("Account is not exited or Wrong username/password");
            if (passwordHasher.VerifyPassword(loginDTO.Password, user.PasswordHash, out string? newHash))
            {
                var accessToken = JwtUtils.GenerateJwtToken(user.HuyPDID, user.Email, user.Role.ToString(), configuration, TimeSpan.FromMinutes(15));
                var refreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return new LoginResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
        }
        public async Task<LoginResponseDTO> RefreshTokenAsync(String refreshToken, IConfiguration configuration)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsDeleted == false);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }
            var newAccessToken = JwtUtils.GenerateJwtToken(user.HuyPDID, user.Email, user.Role.ToString(), configuration, TimeSpan.FromMinutes(15));
            var newRefreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return new LoginResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }
        public Task<bool> Logout(Guid userId)
        {
         var user=  _unitOfWork.Users.FirstOrDefaultAsync(u => u.HuyPDID == userId && u.IsDeleted == false).Result
                        ?? throw new UnauthorizedAccessException("User not found.");
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            _unitOfWork.Users.Update(user);
            _unitOfWork.SaveChangesAsync();
            return Task.FromResult(true);
        }
        public async Task RegisterAsync(RegisterDTO registerDTO)
        {
            String password = passwordHasher.HashPassword(registerDTO.Password);
            User newUser = new User
            {
                HuyPDID = Guid.NewGuid(),
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                PasswordHash = password,
                DateOfBirth = registerDTO.DateOfBirth,
                Address = registerDTO.Address,
                Gender = registerDTO.Gender,
                Phone = registerDTO.Phone,
                IsDeleted = false,
                CreatedAt = currentTime.GetCurrentTime(),
                Role = Domain.HuyPD.Enums.RoleType.Driver,
                Status= Domain.HuyPD.Enums.UserStatus.Active
            };
            newUser.CreatedBy = newUser.HuyPDID;
            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<User>> GetAllUserAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }
        public async Task<User> GetUserById(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null || user.IsDeleted) throw new KeyNotFoundException("User not found.");
            return user;
        }
        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null || user.IsDeleted) throw new KeyNotFoundException("User not found.");
            user.IsDeleted = true;
            bool result = await _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
