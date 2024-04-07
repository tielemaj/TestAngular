using System;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int ReceipientId { get; set; }
        public string ReceipientUsername { get; set; }
        public AppUser Receipient { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSend { get; set; } = DateTime.UtcNow;
        public bool SenderDeleted { get; set; }
        public bool ReceipientDeleted { get; set; }
    }
}