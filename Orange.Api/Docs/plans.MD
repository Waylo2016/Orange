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
}
 ```

Asp core Identity also requires being able to send emails for password resets and/or account verification. <br>
Will make a dedicated email for this (Orange@Waylo.tech) and use SMTP to send emails on that email. <br>

* SMTP based email sending:
[`MailKit`]([https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-10.0&redirectedfrom=MSDN](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient.credentials?view=net-10.0#system-net-mail-smtpclient-credentials)](https://mimekit.net/docs/html/Introduction.htm)) creating and sending emails. <br>

  
### Authentication methods:

* Discord OAuth2
* Google OAuth2 (does require at least a discord OAuth2 login)
* username/password login (with email verification and password reset, does require at least a discord OAuth2 login)
* passkey login (with email verification and password reset, does require at least a discord OAuth2 login and will also require a username/password login to set up the passkey)
  * more to read for passkeys: [Microsoft Docs]([https://learn.microsoft.com/en-us/aspnet/core/security/authentication/passkey?view=aspnetcore-8.0](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-10.0&tabs=visual-studio#register-confirm-email-and-reset-password)) and [Andrew Lock's writeup]([https://andrewlock.net/series/aspnet-core-identity/](https://andrewlock.net/exploring-dotnet-10-preview-features-6-passkey-support-for-aspnetcore-identity/))