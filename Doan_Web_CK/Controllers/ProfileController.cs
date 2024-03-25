﻿using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doan_Web_CK.Controllers
{
    [Authorize(Roles = "Member, Admin")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountRepository _accountRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFriendShipRepository _friendShipRepository;
        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IAccountRepository accountRepository,
            IBlogRepository blogRepository,
            ICommentRepository commentRepository,
            ILikeRepository likeRepository,
            ICategoryRepository categoryRepository,
            IFriendShipRepository friendShipRepository
        )
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
            _blogRepository = blogRepository;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
            _categoryRepository = categoryRepository;
            _friendShipRepository = friendShipRepository;
        }
        public bool IsCurrentUserLiked(int blogId, string userId)
        {
            var task = IsCurrentUserLikedAsync(blogId, userId);
            task.Wait();
            return task.Result;
        }
        public async Task<bool> IsCurrentUserLikedAsync(int blogId, string userId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            var like = await _likeRepository.GetAllLikeAsync();
            var userLike = like.SingleOrDefault(p => p.ApplicationUserId == userId && p.BlogId == blogId);
            if (blog != null || blog.Likes != null)
            {
                if (userLike != null)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public async Task<string> GetUserNameByBlogIdAsync(int blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog != null)
            {
                var author = await _accountRepository.GetByIdAsync(blog.AccountId);
                return author.UserName;
            }
            return "No Blog was Found";
        }
        public string GetUserNameByBlogId(int blogId)
        {
            var task = GetUserNameByBlogIdAsync(blogId);
            task.Wait();
            return task.Result;
        }
        public async Task<IActionResult> Index(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _accountRepository.GetByIdAsync(user.Id);

            if (id != null)
            {
                account = await _accountRepository.GetByIdAsync(id);
            }

            var blogs = await _blogRepository.GetAllAsync();

            var accountBlogs = blogs.Where(p => p.AccountId == account.Id).ToList();
            ViewBag.Account = account;

            ViewBag.blogList = accountBlogs;
            ViewBag.currentUser = user;
            ViewBag.GetPhotoById = new Func<string, string>(GetPhotoById);
            ViewBag.GetUserName = new Func<string, string>(GetUserName);
            ViewBag.GetAllBlogComments = new Func<int, IEnumerable<Comment>>(GetAllBlogComments);
            ViewBag.IsCurrentUserLiked = new Func<int, string, bool>(IsCurrentUserLiked);
            ViewBag.GetUserNameByBlogId = new Func<int, string>(GetUserNameByBlogId);
            ViewBag.GetBlogLikesCount = new Func<int, int>(GetBlogLikesCount);
            ViewBag.GetBlogCommentsCount = new Func<int, int>(GetBlogCommentsCount);
            ViewBag.IsRequested = new Func<string, string, bool>(IsRequested);
            return View();
        }
        public async Task<bool> IsRequestedAsync(string userId, string friendId)
        {
            var friendships = await _friendShipRepository.GetAllAsync();
            var finded = friendships.SingleOrDefault(p => p.UserId == userId && p.FriendId == friendId && p.IsConfirmed == false);
            if (finded != null)
            {
                return true;
            }
            return false;
        }
        public bool IsRequested(string userId, string friendId)
        {
            var task = IsRequestedAsync(userId, friendId);
            task.Wait();
            return task.Result;
        }
        public int GetBlogCommentsCount(int blogId)
        {
            var task = GetBlogCommentsCountAsync(blogId);
            task.Wait();
            return task.Result; ;
        }
        public async Task<int> GetBlogCommentsCountAsync(int blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            var comments = await _commentRepository.GetAllComments();
            int count = comments.Where(p => p.BlogId == blogId).Count();
            if (blog.Comments == null)
            {
                blog.Comments = new List<Comment>();
            }
            return count;
        }
        public int GetBlogLikesCount(int blogId)
        {
            var task = GetBlogLikesCountAsync(blogId);
            task.Wait();
            return task.Result; ;
        }
        public async Task<int> GetBlogLikesCountAsync(int blogId)
        {
            var blog = await _blogRepository.GetByIdAsync(blogId);
            if (blog.Likes == null)
            {
                blog.Likes = new List<Like>();
            }
            return blog.Likes.Count();
        }
        public async Task<IEnumerable<Comment>?> GetAllBlogCommentsAsync(int blogId)
        {
            var comments = await _commentRepository.GetAllComments();
            if (comments != null)
            {
                var filered = comments.Where(p => p.BlogId == blogId).OrderByDescending(p => p.CommentDate).ToList().Take(3);
                return filered;
            }
            return comments;
        }
        public IEnumerable<Comment>? GetAllBlogComments(int blogId)
        {
            var task = GetAllBlogCommentsAsync(blogId);
            task.Wait();
            return task.Result;

        }
        [HttpPost]
        public async Task<IActionResult> Edit(IFormFile profile_photo, string user_name)
        {
            var user = await _userManager.GetUserAsync(User);
            if (profile_photo != null)
            {
                user.ImageUrl = await SaveImage(profile_photo);
            }
            user.UserName = user_name;
            await _accountRepository.UpdateAsync(user);

            var account = await _accountRepository.GetByIdAsync(user.Id);
            ViewBag.Account = account;
            return RedirectToAction("Index");
        }
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var savePath = Path.Combine("wwwroot/images", imageFile.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            Console.WriteLine("/images/" + imageFile.FileName);
            return "/images/" + imageFile.FileName;
        }
        public async Task<string> GetUserNameByIdAsync(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            return "@" + user.UserName;
        }
        public string GetUserName(string id)
        {
            var task = GetUserNameByIdAsync(id);
            task.Wait();
            return task.Result;
        }
        public async Task<string> GetPhotoByIdAsync(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            return user.ImageUrl;
        }
        public string GetPhotoById(string id)
        {
            var task = GetPhotoByIdAsync(id);
            task.Wait();
            return task.Result;
        }
    }
}
