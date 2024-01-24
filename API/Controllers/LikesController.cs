

using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController:ControllerBase
    {
        private readonly ILikesRepository _LikesRepository;
        private readonly IUserRepository _UserRepository;
        public LikesController(ILikesRepository likesRepository, IUserRepository userRepository)
        {
            _UserRepository = userRepository;
            _LikesRepository = likesRepository;
            
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = int.Parse(User.GetUserId());
            var likedUser = await _UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _LikesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();
            if(sourceUser.UserName == username) return BadRequest("you cannot like yourself");
             var userLike = await _LikesRepository.GetUserLike(sourceUserId,likedUser.Id);
             if(userLike !=null) return BadRequest("you already liked this user");

             userLike = new UserLike
             {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
             };
             sourceUser.LikedUsers.Add(userLike);
             if(await _UserRepository.SaveAllAsync()) return Ok();
             return BadRequest("you failed to like this user");
        }

        [HttpGet()]
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = int.Parse(User.GetUserId());
            var users = await _LikesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
            return Ok(users);
        }
    }
}