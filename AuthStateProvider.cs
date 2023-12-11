using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Courses.Models.Database;

namespace Courses
{
    struct UserCredentials
    {
        public string Usermane {get; set;}
        public byte[] PasswordHash {get; set;}
    }

    class AuthStateProvider : AuthenticationStateProvider
    {
        private ProtectedSessionStorage _storage;
        private DatabaseService _database;
        private AuthenticationState _unauthorized = new AuthenticationState(new System.Security.Claims.ClaimsPrincipal());

        public AuthStateProvider(ProtectedSessionStorage storage, DatabaseService database) : base()
        {
            _storage = storage;
            _database = database;                
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedCreadentialsResult = await _storage.GetAsync<UserCredentials>("userCredentials");
                if(savedCreadentialsResult.Success)
                {
                    UserCredentials credentials = savedCreadentialsResult.Value;
                    Models.Database.Account user = await _database.GetAccountByName(credentials.Usermane);
                    if(user == null)
                        throw new Exception("Unknown username");
                    if(user.PasswordHash.SequenceEqual(credentials.PasswordHash))
                    {
                        ClaimsIdentity identity = null;
                        switch(user.Role)
                        {
                            case "Admin":
                                identity = new ClaimsIdentity(new[]
                                {
                                    new Claim(ClaimTypes.Name, "Admin"),
                                    new Claim(ClaimTypes.Role, user.Role), 
                                });
                            break;
                            case "Student":
                                identity = new ClaimsIdentity(new[]
                                {
                                    new Claim(ClaimTypes.Name, user.Student.FullName),
                                    new Claim(ClaimTypes.Role, user.Role),
                                });
                            break;
                        }
                        Account = user;
                        return new AuthenticationState(new ClaimsPrincipal(identity));
                    }
                    else
                        throw new Exception("Password has changed since previous login");
                }
                else
                    return _unauthorized;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return _unauthorized;
            }
        }

        public async Task MakeAuth(string username, string password)
        {
            byte[] hashedBytes = null;
            using (SHA512 sha512 = SHA512.Create())
                hashedBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
            Models.Database.Account user = await _database.GetAccountByName(username);
            if(user == null)
                throw new Exception("Unknown username");
            if(user.PasswordHash.SequenceEqual(hashedBytes))
            {
                ClaimsIdentity identity = null;
                switch(user.Role)
                {
                    case "Admin":
                        identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, "Admin"),
                            new Claim(ClaimTypes.Role, user.Role), 
                        });
                    break;
                    case "Student":
                        identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, user.Student.FullName),
                            new Claim(ClaimTypes.Role, user.Role),
                        });
                    break;
                }
                Account = user;
                await _storage.SetAsync("userCredentials", new UserCredentials(){Usermane = username, PasswordHash = hashedBytes});
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
            }
            else{
                throw new Exception("Incorrect password");
            }
        }

        public Account Account { get; private set; }
    }
}