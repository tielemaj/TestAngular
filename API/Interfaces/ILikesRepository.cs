﻿using API.Dtos;
using API.Entities;
using API.Helpers;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);
    }
}