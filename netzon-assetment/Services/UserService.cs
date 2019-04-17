using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using netzon_assetment.Models;
using netzon_assetment.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace netzon_assetment.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int value);
        User AddRole(User user, string role);
        IEnumerable<User> Search(string q, int? limit, int? offset);
        User FindByEmail(string email);
        User Register(string firstname, string lastname, string email, string password);
        User UpdateProfile(User user, string firstName, string lastName);
        void Delete(object user);
    }

    public class UserService : IUserService
    {
        private NetzonDbContext _dbContext;
        public UserService(NetzonDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User Authenticate(string email, string password)
        {
            var passwordHasher = new PasswordHasher();
            var hashedPassword = passwordHasher.HashPassword(password);
            var uu = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == email && x.Password == hashedPassword);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
            //var roles = _dbContext.Roles.Where(r => r.ID = )
            var roles = _dbContext.Roles
                .Include(r => r.Users)
                .Where(r => r.Users.Any(u => u.UserID == user.ID));
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.ID.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r.Code)));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            

            return user;
        }

        public User Get(int id)
        {
            var user =  _dbContext.Users.FirstOrDefault(u => u.ID == id);
            
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            // return users without passwords
            return _dbContext.Users
                .AsEnumerable()
                .Select(u =>
                {
                    u.Password = null;
                    return u;
                });
        }

        public User GetById(int id)
        {
            var user = _dbContext.Users.Find(id);
            //
            return user;
        }

        public User AddRole(User user, string roleCode)
        {
            var matchedRole = _dbContext.Roles.First(r => r.Code == roleCode);
            var matchedUserRole = _dbContext.UserRoles.FirstOrDefault(ur => ur.RoleID == matchedRole.ID && ur.UserID == user.ID);
            if (matchedUserRole != null)
            {
                return user;
            }

            _dbContext.UserRoles.Add(new UserRole { UserID = user.ID, RoleID = matchedRole.ID, User = user, Role = matchedRole });
            _dbContext.SaveChanges();
            //
            return user;
        }

        public IEnumerable<User> Search(string q, int? limit, int? offset)
        {
            var users = _dbContext.Users.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                users = users.Where(u => u.FirstName.StartsWith(q) || u.LastName.StartsWith(q));
            }

            if (limit.HasValue)
            {
                users = users.OrderBy(r => r.ID).Skip(offset ?? 0).Take(limit.Value);
            }

            return users.Select(u => { u.Password = null; return u; });
        }

        public User FindByEmail(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return null;
            }

            //
            return user;
        }

        public User Register(string firstname, string lastname, string email, string password)
        {
            var passwordHasher = new PasswordHasher();
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                Password = passwordHasher.HashPassword(password)
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            
            return user;
        }

        public User UpdateProfile(User user, string firstName, string lastName)
        {
            _dbContext.Attach(user);
            user.FirstName = firstName;
            user.LastName = lastName;
            _dbContext.Entry(user).State = EntityState.Modified;
            _dbContext.SaveChanges();
            
            return user;
        }

        public void Delete(object user)
        {
            _dbContext.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}
