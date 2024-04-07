using API.Dtos;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessge(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(x => x.MessageSend).AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.ReceipientUsername == messageParams.UserName && !x.ReceipientDeleted),
                "Outbox" => query.Where(x => x.SenderUsername == messageParams.UserName && !x.SenderDeleted ),
                _ => query.Where(x => x.ReceipientUsername == messageParams.UserName && x.DateRead == null && !x.ReceipientDeleted)
            };
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string receipientUsername)
        {
            var messages = await _context.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Receipient).ThenInclude(x => x.Photos)
                .Where(x =>
                    (x.ReceipientUsername == currentUsername && x.SenderUsername == receipientUsername && !x.ReceipientDeleted) ||
                    (x.ReceipientUsername == receipientUsername && x.SenderUsername == currentUsername && !x.SenderDeleted)
                 )
                .OrderBy(m => m.MessageSend)
                .ToListAsync();

            var unreadMessages = messages.Where(x => x.DateRead == null && x.ReceipientUsername == currentUsername).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<Message> GetMessge(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}