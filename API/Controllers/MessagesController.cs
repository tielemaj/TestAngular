using API.Dtos;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessagesController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUserName();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You can not send messages to yourself");

            var sender = await _uow.UserRepository.GetUserByUserNameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message()
            {
                Sender = sender,
                Receipient = recipient,
                SenderUsername = sender.UserName,
                ReceipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _uow.MessageRepository.AddMessage(message);

            if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();
            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
            return messages;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();
            var message = await _uow.MessageRepository.GetMessge(id);
            if (message == null) return NotFound();

            if (message.SenderUsername != username && message.ReceipientUsername != username)
                return Unauthorized();

            if (message.SenderUsername == username)                message.SenderDeleted = true;   
            if (message.ReceipientUsername == username) message.ReceipientDeleted = true;
            if (message.SenderDeleted && message.ReceipientDeleted)
            {
                _uow.MessageRepository.DeleteMessge(message);
            }
            if (await _uow.Complete()) return Ok();
            return BadRequest("Problem deleting message");
        }
    }
}