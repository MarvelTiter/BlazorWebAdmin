namespace BlazorTemplate.ClientCore.UI;

public interface IJsComponent
{
    Lazy<string> Id { get; }
}