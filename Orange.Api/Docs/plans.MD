## Expanded TODO for the API

# write actual fucking documentation you scrub. Waylo you loser


## Authentication for Dashboard 

The dashboard will have an account for the user themselves. OAuth should be possible for maybe google, but definitely Discord. <br>
This will probably require me to do something with the Aspnet Core Identity system, password salting and all that beautiful jazz. <br>
the shape of the user model will be something like this:

```csharp
 public class User : IdentityUser
{
    /// <summary>
    /// Last Login
    /// </summary>
    public DateTime? LastLogin { get; set; }
    
    /// <summary>
    /// Discord role IDs that grant Orange moderator access, in addition
    /// to users with Manage Server. Configured by the guild owner.
    /// </summary>
    public List<ulong> ModeratorRoleIds { get; set; } = []; //will contain discord snowflake ID's
    
    public List<OrangeRoles> CustomRoles { get; set; } = []; //will contain custom roles that are set by the server owner, and can be assigned to users by the server owner or other users with the appropriate permissions
}
 ```

Asp core Identity also requires being able to send emails for password resets and/or account verification. <br>
Will make a dedicated email for this (Orange@Waylo.tech) and use SMTP to send emails on that email. <br>

* SMTP based email sending:
[`MailKit`]([https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-10.0&redirectedfrom=MSDN](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient.credentials?view=net-10.0#system-net-mail-smtpclient-credentials)](https://mimekit.net/docs/html/Introduction.htm)) creating and sending emails. <br>
  * Will require a form of rate limiting to prevent abuse. <br>
  
### Authentication methods:

* Discord OAuth2
* [Google OAuth2]([https://x.com/Bunch_1123/status/2098030974944854049?s=20](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-10.0#configure-google-authentication)) (does require at least a discord OAuth2 login)
* username/password login (with email verification and password reset, does require at least a discord OAuth2 login)
  * Only allow discord oAuth after the user has verified they exist, and check if discord user also is verified. Google account should also be verified
* passkey login (with email verification and password reset, does require at least a discord OAuth2 login and will also require a username/password login to set up the passkey)
  * more to read for passkeys: [Microsoft Docs]([https://learn.microsoft.com/en-us/aspnet/core/security/authentication/passkey?view=aspnetcore-8.0](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-10.0&tabs=visual-studio#register-confirm-email-and-reset-password)) and [Andrew Lock's writeup]([https://andrewlock.net/series/aspnet-core-identity/](https://andrewlock.net/exploring-dotnet-10-preview-features-6-passkey-support-for-aspnetcore-identity/))

### How to know what server a user is in

* Use the bot to know what server it is in, then use the [discord API](https://docs.discord.com/developers/resources/user#get-current-user-guild-member) to get the list of servers that user's discordID is in <br>
can do this probably via the discord OAuth API, using the `
/users/@me/guilds/{guild.id}/member` endpoint, which returns the user object for a specified guild ID, which contains an array of roles the user can have.
does require the `guilds.members.read` oauth scope. (privileged)


### Data storing??

* clearly state only discord IDs, roles, emails and passwords are stored, and nothing else. <br>
  * requests for data removal will be honored and the user will be removed from the database, and all data will be deleted after 2 weeks, to allow for users to change their mind. <br>


### Callbacks for OAuth logins

* Discord: `/signin-discord` <br>
* Google: `/signin-google` <br>

### Dashboard authentication extras

Once logged in, the front end should send a JWT token to the backend for authentication per server. User can be moderator on server A, but only admin on server B. Have to decide who can tweak what settings. <br>
This should be able to be done via roles in discord with certain permissions. (maybe even custom roles, that only exist in Orange, not in Discord) <br>

the custom roles should be able to be done with a simple enum 

```csharp
  public enum OrangeRoles
    {
        Admin,
        Verifier
    }
```
This can deliver more granularity to a server owner and allow them to delegate verification duties to other users without giving them full admin access. <br>
Also allows for easily expandable custom perms that should be able to be set through discord embeds AND the web dashboard. <br>

Would require custom roles to be stored in the database so they can be linked to the user. Would also allow me to do custom claims for the JWT token, so that the front end can know what permissions a user has on a server. <br>

example custom claims for back end auth:
```csharp
// omitted a lot of code for brevity, but this is the gist of it

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireClaim("role", "Admin"));
            options.AddPolicy("Verifier", policy => policy.RequireClaim("role", "Verifier"));
        });
    }
}
```

or something akin to that. Should check earlier implementations of this in [earlier codebases](https://github.com/MishaOpstal/LeafBid/blob/main/LeafBid/LeafBidAPI/Program.cs). <br>