using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Intefaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(LogUserActivity))]
[Authorize]
public class UserController:ControllerBase
{
    
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

    public UserController(IUserRepository userRepository, IMapper mapper,
    IPhotoService photoService)
    {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
    
    }

[HttpGet]
public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams){

    //  var username = User.GetUserName();
    var currentUser= await _userRepository.GetUserByUsernameAsync(User.GetUserName());
    userParams.CurrentUsername = currentUser.UserName;
    if(string.IsNullOrEmpty(userParams.Gender)){
        userParams.Gender = currentUser.Gender=="male"? "female":"male";
    }

    var users = await _userRepository.GetMembersAsync(userParams);
    Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
    return Ok(users);
}

[HttpGet("{username}")]
public async Task<ActionResult<MemberDto>> GetUser(string username){
    return await _userRepository.GetMemberAsync(username);
}

[HttpPut]
public async Task<ActionResult> UpdateUser(MemberUpdatedDto memberUpdatedDto){
    var username = User.GetUserName();
    var user = await _userRepository.GetUserByUsernameAsync(username);
    if(user==null) return NotFound();
    _mapper.Map(memberUpdatedDto,user);
    if(await _userRepository.SaveAllAsync()) return NoContent();
    return BadRequest("faild to update user");
}

[HttpPost("add-photo")]
public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){
var username = User.GetUserName();
    var user = await _userRepository.GetUserByUsernameAsync(username);
    if(user == null) return NotFound();
    var result = await _photoService.AddPhotoAsync(file);
    if(result.Error !=null) return BadRequest(result.Error.Message);
    var photo = new Photos{
        Url= result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId
    };
    if(user.Photos.Count == 0) photo.IsMain = true;
    user.Photos.Add(photo);
    if(await _userRepository.SaveAllAsync())
    {
        return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
    };
    return BadRequest("faild adding photo");
    
}

    [HttpDelete("delete-photo/{photoId}")]
     public async Task<ActionResult> deletePhoto( int photoId)
     {
        var username = User.GetUserName();
        var user = await _userRepository.GetUserByUsernameAsync(username);
        var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);
        if(photo == null) return NotFound();
        if(photo.IsMain) return BadRequest("you cannot delete main photo");
        if(photo.PublicId !=null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }
        user.Photos.Remove(photo);
        if(await _userRepository.SaveAllAsync()) return Ok();
        return BadRequest("problem in deleting photo");
     }
}
