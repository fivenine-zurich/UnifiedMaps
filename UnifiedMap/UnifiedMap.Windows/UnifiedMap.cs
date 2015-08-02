// ReSharper disable once CheckNamespace

namespace fivenine
{
    public static class UnifiedMap
    {
        private static bool _initialized;

        internal static string ApplicationId { get; set; }

        internal static string AuthenticationToken { get; set; }

        public static void Init()
        {
            if (_initialized)
                return;

            _initialized = true;
        }

        public static void Init(string applicationId, string authenticationToken)
        {
            ApplicationId = applicationId;
            AuthenticationToken = authenticationToken;

            Init();
        }
    }
}
