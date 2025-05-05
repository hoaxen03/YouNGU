using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouNGU.Data.Repositories;
using YouNGU.Models.ViewModels;

namespace YouNGU.Controllers
{
    public class ChannelController : Controller
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ChannelController(
            IVideoRepository videoRepository,
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _videoRepository = videoRepository;
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<IActionResult> Index(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var videos = await _videoRepository.GetByUserIdAsync(id);
            var subscriberCount = await _subscriptionRepository.GetSubscriberCountAsync(id);

            bool isSubscribed = false;
            if (User.Identity.IsAuthenticated)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                isSubscribed = await _subscriptionRepository.IsSubscribedAsync(currentUserId, id);
            }

            var model = new ChannelViewModel
            {
                User = user,
                Videos = videos,
                SubscriberCount = subscriberCount,
                IsSubscribed = isSubscribed
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> MyVideos()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var videos = await _videoRepository.GetByUserIdAsync(userId);
            return View(videos);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleSubscription(string publisherId)
        {
            string subscriberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isSubscribed = await _subscriptionRepository.IsSubscribedAsync(subscriberId, publisherId);

            if (isSubscribed)
            {
                await _subscriptionRepository.UnsubscribeAsync(subscriberId, publisherId);
            }
            else
            {
                await _subscriptionRepository.SubscribeAsync(subscriberId, publisherId);
            }

            return Json(new { success = true, isSubscribed = !isSubscribed });
        }
    }
}
