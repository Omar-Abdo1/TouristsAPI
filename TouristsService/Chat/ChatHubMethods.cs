namespace TouristsAPI.Hubs;

public static class ChatHubMethods
{
    //Client Invoke These
    public const string UserTyping = "Typing";
    public const string StopTyping = "StopTyping"; 

    //Client Listen on These
    public const string ReceiveMessage = "ReceiveNewMessage";
    public const string UserIsOnline = "UserCameOnline";
    public const string UserIsOffline = "UserWentOffline";
    public const string OnUserTyping = "UserIsTyping";
    public const string OnUserStoppedTyping = "UserStoppedTyping";
    public const string MessageDeleted = "MessageWasDeleted";
    public const string MessagesReadUpdated = "MessagesReadStatusUpdated";
}