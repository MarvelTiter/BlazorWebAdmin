namespace Project.Constraints.UI.Extensions
{
    public static class MessageExtensions
    {
        public static void Success(this IUIService service, string message)
        {
            service.Message(MessageType.Success, message);
        }

        public static void Info(this IUIService service, string message)
        {
            service.Message(MessageType.Information, message);
        }

        public static void Error(this IUIService service, string message)
        {
            service.Message(MessageType.Error, message);
        }
    }
}
