namespace TouristsAPI.Hubs;

public static class ChatHubMethods
{
    //Client Invoke These
    public const string SendMessage = "SendMessage";
    public const string UserTyping = "Typing";
    public const string MarkMessagesRead = "MarkRead";
    public const string DeleteMessage = "DeleteMessage";

    //Client Listen on These
    public const string ReceiveMessage = "ReceiveNewMessage";
    public const string UserIsOnline = "UserCameOnline";
    public const string UserIsOffline = "UserWentOffline";
    public const string OnUserTyping = "UserIsTyping";
    public const string MessageDeleted = "MessageWasDeleted";
    public const string MessagesReadUpdated = "MessagesReadStatusUpdated";
}