

using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Intefaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
[ApiController]
[Route("api/[controller]")]
    public class MessageController:ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessageController(IUserRepository userRepository,IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();
            if(username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("you cannot send message to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
            if(recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient= recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            _messageRepository.AddMessage(message);
            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("message failed to sent");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessageForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();
            var message = await _messageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(message.CurrentPage,message.PageSize,message.TotalPages,message.TotalCount));
            return message;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUserUsername= User.GetUserName();
            return Ok(await _messageRepository.GetMessageThread(currentUserUsername,username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> deleteMessage(int id)
        {
            var username = User.GetUserName();
            var message = await _messageRepository.GetMessage(id);
            if(message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();

            if(message.SenderUsername == username) message.SenderDelete = true;
            if(message.RecipientUsername == username) message.RecipientDelete = true;
            if(message.SenderDelete && message.RecipientDelete)
            {
                 _messageRepository.DeleteMessage(message);
            }
            if(await _messageRepository.SaveAllAsync()) return Ok();
            return BadRequest("problem in deleting message");
        }
    }
}