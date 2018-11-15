using System;
using System.Collections.Generic;
using System.Security.Claims;
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
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);

            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            var users = await this._repo.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

    
            var (currentPage, totalCount, pageSize, totalPages) = users;
            
            Response.AddPagination(currentPage, pageSize, totalCount, totalPages);



            return Ok(usersToReturn);
        }
        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await this._repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        [ServiceFilter(typeof(UpdateEntityAccessor))]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto) 
        {
             
            var userFromRepo = await _repo.GetUser(id);
            //from to
            _mapper.Map(userForUpdateDto, userFromRepo);
            
            if (await _repo.SaveAll())
                return NoContent();
            throw new Exception($"Updating user {id} failed on save");

        }
        
        [HttpPost("{id}/like/{recipientId}")]
        [ServiceFilter(typeof(UpdateEntityAccessor))]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            var like = await _repo.GetLike(id, recipientId);
            if (like != null) 
                return BadRequest("You already like this user");
            if (await _repo.GetUser(recipientId) == null)
                return NotFound();
            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);

            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("Failed to like user");
        }

    }
}
