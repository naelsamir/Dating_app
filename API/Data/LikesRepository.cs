

using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context){
            _context = context;

        }
        public async Task<UserLike> GetUserLike(int SourceUserId, int TargetUserId)
        {
            return await _context.Likes.FindAsync(SourceUserId,TargetUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u=>u.UserName).AsQueryable();
            var Likes = _context.Likes.AsQueryable();
            if(likesParams.predicate =="liked"){
                Likes = Likes.Where(like=>like.SourceUserId == likesParams.UserId);
                users = Likes.Select(like=>like.TargetUser);
            }
            if(likesParams.predicate =="likedBy"){
                Likes = Likes.Where(like=>like.TargetUserId == likesParams.UserId);
                users = Likes.Select(like=>like.SourceUser);
            }

            var likesUsers = users.Select(user=> new LikeDTO{
                UserName = user.UserName,
                Age = user.DateOfBirth.CalculateAge(),
                KnownAs = user.KnownAs,
                City = user.City,
                Id = user.Id,
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain).Url
            });
            return await PagedList<LikeDTO>.CreateAsync(likesUsers,likesParams.PageNumber,likesParams.PageSize);
        
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x=>x.LikedUsers).FirstOrDefaultAsync(x=> x.Id == userId);
            
        }
    }
}