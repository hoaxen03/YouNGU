namespace YouNGU.Data.Repositories
{
    public interface ISubscriptionRepository
    {
            Task<bool> IsSubscribedAsync(string subscriberId, string publisherId);
            Task SubscribeAsync(string subscriberId, string publisherId);
            Task UnsubscribeAsync(string subscriberId, string publisherId);
            Task<int> GetSubscriberCountAsync(string publisherId);
            Task<int> GetSubscriptionCountAsync(string subscriberId);

    }
}
