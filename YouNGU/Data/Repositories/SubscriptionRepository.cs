using Microsoft.EntityFrameworkCore;
using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsSubscribedAsync(string subscriberId, string publisherId)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.SubscriberId == subscriberId && s.PublisherId == publisherId);
        }

        public async Task SubscribeAsync(string subscriberId, string publisherId)
        {
            // Kiểm tra xem đã đăng ký chưa
            var isSubscribed = await IsSubscribedAsync(subscriberId, publisherId);

            if (!isSubscribed)
            {
                var subscription = new Subscription
                {
                    SubscriberId = subscriberId,
                    PublisherId = publisherId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Subscriptions.AddAsync(subscription);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnsubscribeAsync(string subscriberId, string publisherId)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.SubscriberId == subscriberId && s.PublisherId == publisherId);

            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetSubscriberCountAsync(string publisherId)
        {
            return await _context.Subscriptions
                .CountAsync(s => s.PublisherId == publisherId);
        }

        public async Task<int> GetSubscriptionCountAsync(string subscriberId)
        {
            return await _context.Subscriptions
                .CountAsync(s => s.SubscriberId == subscriberId);
        }
    }
}
