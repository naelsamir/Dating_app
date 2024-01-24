


using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Intefaces
{
    public interface ILikesRepository 
    {
        Task<UserLike> GetUserLike(int SourceUserId, int TargetUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
        
    }
}