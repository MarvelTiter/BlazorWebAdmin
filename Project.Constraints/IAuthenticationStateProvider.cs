using Project.Common.Attributes;
using Project.Constraints.Models;
namespace Project.Constraints;

public interface IAuthenticationStateProvider
{
    UserInfo? Current {  get; }
    Task IdentifyUser(UserInfo info);
    Task ClearState();
}
