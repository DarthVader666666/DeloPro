using AutoMapper;

using Delopro.Data.Interfaces;
using Delopro.Data.Entities;
using Delopro.Data.Enums;
using Delopro.Server.Enums;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Delopro.Bll.Services;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    [Authorize(Roles = "Owner, Admin")]
    public class AdministrationController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;

        public AdministrationController(IRepository<User> userRepository, IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUsers()
        { 
            var users = await _userRepository.GetListAsync();
            var userShortResponse = _mapper.Map<IEnumerable<UserShortResponse>>(users);

            return Ok(userShortResponse);
        }

        [HttpGet]
        [Route("[action]/{userId:int}")]
        public async Task<IActionResult> GetUser([FromRoute] int userId)
        {
            var user = await _userRepository.GetAsync(userId);

            if (user == null)
            {
                return StatusCode(500, new { errorText = "Ошибка базы данных" });
            }

            var userLongResponse = _mapper.Map<UserLongResponse>(user);
            var roles = (await _roleRepository.GetListAsync(user.UserId))
                .Select(r => Enum.TryParse(typeof(UserRoleType), r?.RoleName, out object? role)
                ? (int)role - 1 : -1).ToArray();

            userLongResponse.Roles = roles;

            return Ok(userLongResponse);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest userUpdateRequest)
        {
            var userRolesToDelete = await _userRoleRepository.GetListAsync(userUpdateRequest?.UserId);
            var userRolesToCreate = userUpdateRequest?.Roles?.Select(x => (UserRole?)new UserRole { UserId = userUpdateRequest.UserId, RoleId = x + 1 }) ?? [];

            await _userRoleRepository.DeleteRangeAsync(userRolesToDelete);

            foreach (var userRole in userRolesToCreate ?? [])
            {
                await _userRoleRepository.CreateAsync(userRole);
            }

            var user = await _userRepository.GetAsync(userUpdateRequest?.UserId);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка базы данных" });
            }

            user.DeletionDate = userUpdateRequest?.DeletionDate;
            user.IsConfirmed = userUpdateRequest?.Status == (int)UserStatus.Confirmed;
            user.IsDeleted = userUpdateRequest?.Status == (int)UserStatus.Deleted;

            await _userRepository.UpdateAsync(user);

            return Ok(new { okText = "Пользователь успешно обновлен" });
        }

        [HttpDelete]
        [Route("[action]/{userId:int}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {
            var user = await _userRepository.GetAsync(userId);

            if (user is null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
            else
            {
                await _userRepository.DeleteAsync(userId);
                return Ok(new { okText = $"Пользователь {user.Nickname} успешно удалён" });
            }
        }
    }
}
