namespace Briefcase
{
    public class Message
    {
        public Message(string sender, string receiver, string content, string conversationId = null)
        {
            Sender = sender;
            Receiver = receiver;
            Content = content;
            ConversationId = conversationId;
        }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Content { get; set; }

        public string ConversationId { get; set; }
    }
}