namespace WebStruct.RateLimit
{
    public class RateLimitRule
    {
        public int Limit { get; set; } = 100;
        public TimeSpan Period { get; set; } = TimeSpan.FromMinutes(1);
    }

    public interface IRateLimitService
    {
        bool IsAllowed(string clientId, string endpoint);
    }

    public class RateLimitService : IRateLimitService
    {
        private readonly Dictionary<string, Dictionary<string, List<DateTime>>> _requests = new();
        private readonly RateLimitRule _rule = new();
        private readonly object _lock = new();

        public bool IsAllowed(string clientId, string endpoint)
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                var key = $"{clientId}_{endpoint}";

                if (!_requests.ContainsKey(key))
                {
                    _requests[key] = new Dictionary<string, List<DateTime>>();
                }

                if (!_requests[key].ContainsKey(clientId))
                {
                    _requests[key][clientId] = new List<DateTime>();
                }

                // Удаляем старые запросы
                _requests[key][clientId].RemoveAll(t => t < now - _rule.Period);

                // Проверяем лимит
                if (_requests[key][clientId].Count >= _rule.Limit)
                {
                    return false;
                }

                // Добавляем текущий запрос
                _requests[key][clientId].Add(now);
                return true;
            }
        }
    }
}
