using AutoMapper;
using Mcce.SmartOffice.Core.Exceptions;
using Mcce.SmartOffice.Users.Entities;
using Mcce.SmartOffice.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcce.SmartOffice.Users.Managers
{
    public interface IUserManager
    {
        Task<UserModel[]> GetUsers();

        Task<UserModel> GetUser(int userId);

        Task DeleteUser(int userId);
    }

    public class UserManager : IUserManager
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserManager(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserModel[]> GetUsers()
        {
            var users = await _dbContext.Users
                .OrderBy(x => x.UserName)
                .ToListAsync();

            return users
                .Select(_mapper.Map<UserModel>)
                .ToArray();
        }

        public async Task<UserModel> GetUser(int userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return user == null ? throw new EntityNotFoundException<User>(userId) : _mapper.Map<UserModel>(user);
        }        

        public async Task DeleteUser(int userId)
        {
            using var tx = await _dbContext.Database.BeginTransactionAsync();

            await _dbContext.Users
                .Where(x => x.Id == userId)
                .ExecuteDeleteAsync();

            await _dbContext.SaveChangesAsync();

            await tx.CommitAsync();
        }
    }
}
