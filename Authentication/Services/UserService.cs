using Authentication.Entities;
using Authentication.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace Authentication.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }


    public class UserService : IUserService
    {
        private readonly AuthenticationSettings _authSettings;
        private readonly ILogger<UserService> _logger;

        public UserService(IOptions<AuthenticationSettings> authSettings, ILogger<UserService> logger)
        {
            _logger = logger;
            _logger.LogInformation("UserService constructor called");


            _authSettings = authSettings.Value;

            _logger.LogInformation("UserService constructor ended");
        }

        public User ValidateCredentials(string userName, string password)
        {
            try
            {
                _logger.LogInformation("ValidateCredentials UserService started");
                User user_temp = new User();

                user_temp.FirstName = userName;
                user_temp.Id = 1;
                user_temp.LastName = "";
                if (userName == "jimmywoe")
                {
                    user_temp.Roles = new Role[] { new Role() { roletype = "HeroesWriter" }, new Role() { roletype = "HeroesReader" } };
                }
                else
                {
                    user_temp.Roles = new Role[] { new Role() { roletype = "HeroesReader" } };

                }
                _logger.LogInformation("ValidateCredentials UserService ended");

                return user_temp;
                ////when connecting to a DC : new PrincipalContext(ContextType.Domain, "ESTAGIOIT", "CN=Users,DC=estagioit,DC=local");
                ////optionally a container (as an LDAP path - a "distinguished" name, full path but without any LDAP:// prefix)
                //using (var adContext = new PrincipalContext(ContextType.Machine, null))
                //{
                //    if (adContext.ValidateCredentials(userName, password))
                //    {
                //        //user
                //        UserPrincipal usr = new UserPrincipal(adContext);
                //        usr.SamAccountName = userName;
                //        var searcher = new PrincipalSearcher(usr);
                //        usr = searcher.FindOne() as UserPrincipal;
                //        User user = new User();
                //        user.WithoutPassword(usr);

                //        //roles
                //         PrincipalSearchResult<Principal> groups = usr.GetAuthorizationGroups();
                //        user.Roles = GetRoles(groups);
                //        return user;
                //    }
                //}
                //return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        private Role[] GetRoles(PrincipalSearchResult<Principal> roles)
        {
            List<Role> roleslist = new List<Role>();
            foreach (Principal roleprincipal in roles)
            {
                if (!String.IsNullOrEmpty(_authSettings.AllowedRoles.Where(role => role == roleprincipal.Name).FirstOrDefault()))
                {
                    Role role = new Role();
                    role.roletype = roleprincipal.Name;
                    roleslist.Add(role);
                }
            }
            return roleslist.ToArray();
        }


        public User Authenticate(string username, string password)
        {
            _logger.LogInformation("Authenticate UserService started");

            // hook on to AD here...
            User user = ValidateCredentials(username, password);
            // return null if user not found
            if (user == null)
                return null;
            List<Claim> claimlist = new List<Claim>();
            foreach (Role role in user.Roles)
            {
                claimlist.Add(new Claim(ClaimTypes.Role, role.roletype));
            }
            claimlist.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimlist.ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            _logger.LogInformation("Authenticate UserService ended");

            return user.WithoutPassword();
        }

        public IEnumerable<User> GetAll()
        {
            // just a stub to get some data here..
            List<User> _users = new List<User>();

            List<Role> rolesdoe = new List<Role>();
            rolesdoe.Add(new Role() { roletype = "def" });
            User doe = new User { Id = 1, FirstName = "Does", LastName = "not matter", Username = "doeviinotmatter", Password = "doe", Roles = rolesdoe.ToArray() };

            List<Role> roleswoe = new List<Role>();
            roleswoe.Add(new Role() { roletype = "abc" });
            User woe = new User { Id = 1, FirstName = "Does", LastName = "not matter as well", Username = "woeviinotmatter", Password = "woe", Roles = roleswoe.ToArray() };

            _users.Add(doe);
            _users.Add(woe);
            return _users.WithoutPasswords();
        }

        public User GetById(int id)
        {
            // just a stub to get some data here..

            List<Role> roleswoe = new List<Role>();
            roleswoe.Add(new Role() { roletype = "fgh" });
            roleswoe.Add(new Role() { roletype = "klm" });
            User woe = new User { Id = 1, FirstName = "Does", LastName = "not matter", Username = "doenotmatter", Password = "woe", Roles = roleswoe.ToArray() };
            return woe;
        }
    }
}