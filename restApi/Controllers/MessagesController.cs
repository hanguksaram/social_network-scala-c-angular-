using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Helpers;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ServiceFilter(typeof(UpdateEntityAccessor))]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDatingRepository _repo;

        public MessagesController(IMapper mapper, IDatingRepository repo)
        {
            this._mapper = mapper;
            this._repo = repo;
        }
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id) {

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
                return NotFound();
            
            return Ok(messageFromRepo);

        }
        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, 
            [FromQuery]MessageParams messageParams){
            
            messageParams.UserId = userId;
            
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
            
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            var (currentPage, totalCount, pageSize, totalPages) = messagesFromRepo;
            
            Response.AddPagination(currentPage, pageSize, totalCount, totalPages);

            return Ok(messages);

        }
        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId) {
            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto msg){
            
            msg.SenderId = userId;
            var sender = await _repo.GetUser(userId);
            var recipient = await _repo.GetUser(msg.RecipientId);// it allows message entity fill map null fields with info fetched from users what a magical conventions omg

            if (recipient == null) 
                return BadRequest("Could not find user");
            
            var message = _mapper.Map<Message>(msg);

            _repo.Add(message);

            

            if (await _repo.SaveAll()) {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }

            throw new Exception("Creating message failed on save");
            
        }
    }
}