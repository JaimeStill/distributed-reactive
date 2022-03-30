namespace Topics.Identity;

using Topics.Core.Extensions;
using Topics.Identity.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

public class AdUserProvider : IUserProvider
{
    public AdUser CurrentUser { get; set; }
    public bool Initialized { get; set; }

    public async Task Create(HttpContext context, IConfiguration config)
    {
        CurrentUser = await GetAdUser(context.User.Identity);
        Initialized = true;
    }

    public Task AddIdentity(HttpContext context) => throw new NotImplementedException("Only used for MockIdentity implementation");

    public Task Create(string samAccountName) => throw new NotImplementedException("Use Create(HttpContext context) for UserProvider");

    public Task<AdUser> GetAdUser(IIdentity identity)
    {
        return Task.Run(() =>
        {
            try
            {
                PrincipalContext context = new(ContextType.Domain);
                UserPrincipal principal = new(context);

                if (context != null)
                {
                    principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, identity.Name);
                }

                return AdUser.CastToAdUser(principal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetExceptionChain());
            }
        });
    }

    public Task<AdUser> GetAdUser(string samAccountName)
    {
        return Task.Run(() =>
        {
            try
            {
                PrincipalContext context = new(ContextType.Domain);
                UserPrincipal principal = new(context);

                if (context != null)
                {
                    principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, samAccountName);
                }

                return AdUser.CastToAdUser(principal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetExceptionChain());
            }
        });
    }

    public Task<AdUser> GetAdUser(Guid guid)
    {
        return Task.Run(() =>
        {
            try
            {
                PrincipalContext context = new(ContextType.Domain);
                UserPrincipal principal = new(context);

                if (context != null)
                {
                    principal = UserPrincipal.FindByIdentity(context, IdentityType.Guid, guid.ToString());
                }

                return AdUser.CastToAdUser(principal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetExceptionChain());
            }
        });
    }

    public Task<List<AdUser>> GetDomainUsers()
    {
        return Task.Run(() =>
        {
            PrincipalContext context = new(ContextType.Domain);
            UserPrincipal principal = new(context);
            principal.UserPrincipalName = "*@*";
            principal.Enabled = true;
            PrincipalSearcher searcher = new(principal);

            var users = searcher
                .FindAll()
                .AsQueryable()
                .Cast<UserPrincipal>()
                .FilterUsers()
                .SelectAdUsers()
                .OrderBy(x => x.Surname)
                .ToList();

            return users;
        });
    }

    public Task<List<AdUser>> SearchDomainUsers(string search)
    {
        return Task.Run(() =>
        {
            PrincipalContext context = new(ContextType.Domain);
            UserPrincipal principal = new(context);
            principal.SamAccountName = $"*{search}*";
            principal.Enabled = true;
            PrincipalSearcher searcher = new(principal);

            var users = searcher
                .FindAll()
                .AsQueryable()
                .Cast<UserPrincipal>()
                .FilterUsers()
                .SelectAdUsers()
                .OrderBy(x => x.Surname)
                .ToList();

            return users;
        });
    }
}