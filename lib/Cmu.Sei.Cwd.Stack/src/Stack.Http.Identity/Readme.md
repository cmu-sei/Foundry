# Stack.Http.Identity

This library provides common http identity components

## Usage

`dotnet add package Cwd.Stack.Http.Identity -s https://nuget.cwd.local/v3/index.json`

You should also add a `NuGet.Config` file to your relying project so that everybody restores from the same repo.

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.cwd.local" value="https://nuget.cwd.local/v3/index.json" />
  </packageSources>
</configuration>
```

Create a new HttpIdentityResolver in your project

...
public class CustomIdentityResolver : HttpIdentityResolver
    {
        DbContext _db;
        ILogger<CustomIdentityResolver> Logger { get; }        

        public UserIdentityResolver(IHttpContextAccessor httpContextAccessor, DbContext db, ILogger<CustomIdentityResolver> logger)
            : base(httpContextAccessor)
        {
            _db = db;
            Logger = logger;
        }

        public override async Task<IStackIdentity> GetIdentityAsync()
        {
            var claimsPrincipal = HttpContextAccessor.HttpContext.User;

            string subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            if (string.IsNullOrWhiteSpace(clientId))
                return null;

            return Get(claimsPrincipal) ?? Add(claimsPrincipal) ?? Update(claimsPrincipal);
        }

        IStackIdentity Add(ClaimsPrincipal claimsPrincipal)
        {
            string subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            // create a new account based on claims

            return ConvertToIdentity(user, claimsPrincipal);
        }

        IStackIdentity Get(ClaimsPrincipal claimsPrincipal)
        {
            string subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            // get account based on claims

            return ConvertToIdentity(user, claimsPrincipal);
        }

        IStackIdentity Update(ClaimsPrincipal claimsPrincipal)
        {
            string subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            // update account based on claims

            return ConvertToIdentity(user, claimsPrincipal);
        }

        IStackIdentity ConvertToIdentity(ClaimsPrincipal claimsPrincipal)
        {
            var data = new List<object>() { claimsPrincipal };

            return new StackIdentity { Id = 'Some Id', Data = data };
        }
    }
	...

In Startup.cs

...
services.AddScoped<IStackIdentityResolver, CustomIdentityResolver>();
...