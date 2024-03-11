namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkPushMessageReadCommand : PushMessagesCommand
    {
        public string NotificationId { get; set; }
    }
}
