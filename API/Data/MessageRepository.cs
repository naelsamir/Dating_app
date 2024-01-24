

using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Intefaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        public readonly IMapper _Mapper;
        public MessageRepository(DataContext context,IMapper mapper)
        {
            _Mapper = mapper;
            _context = context;
            
        }
        public void AddMessage(Message message)
        {
            _context.Message.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Message.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Message.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Message.OrderByDescending(x=>x.MessageSent).AsQueryable();

            query=messageParams.Container switch
            {
                "Inbox"=>query.Where(u=>u.RecipientUsername == messageParams.Username && u.RecipientDelete == false),
                "Outbox"=>query.Where(u=>u.SenderUsername == messageParams.Username && u .SenderDelete == false),
                _=>query.Where(u=>u.RecipientUsername== messageParams.Username&& u.RecipientDelete == false && u.DateRead==null)

            };
            var message = query.ProjectTo<MessageDto>(_Mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(message,messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserUsername, string recipientUsername)
        {
            var messages = await _context.Message.Include(u=>u.Sender).ThenInclude(p=>p.Photos)
            .Include(u=>u.Recipient).ThenInclude(p=>p.Photos)
            .Where(
                m=>m.RecipientUsername == currentUserUsername&& m.RecipientDelete == false &&
                m.SenderUsername == recipientUsername ||
                m.RecipientUsername ==recipientUsername&& m.SenderDelete == false &&
                m.SenderUsername == currentUserUsername
                ).OrderByDescending(m=>m.MessageSent).ToListAsync();

            var unreadMessage = messages.Where(m=>m.DateRead == null 
            && m.RecipientUsername == currentUserUsername).ToList();
            if(unreadMessage.Any())
            {
                foreach(var message in unreadMessage){
                    message.DateRead = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
            return _Mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}