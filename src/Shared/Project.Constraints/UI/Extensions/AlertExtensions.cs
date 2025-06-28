namespace Project.Constraints.UI.Extensions;

public static class AlertExtensions
{
    public static void AlertSuccess(this IUIService service, string title, string message)
    {
        service.Alert(MessageType.Success, title, message);
    }

    public static void AlertError(this IUIService service, string title, string message)
    {
        service.Alert(MessageType.Error, title, message);
    }

    public static void AlertWarning(this IUIService service, string title, string message)
    {
        service.Alert(MessageType.Warning, title, message);
    }

    public static void AlertInfo(this IUIService service, string title, string message)
    {
        service.Alert(MessageType.Information, title, message);
    }
}