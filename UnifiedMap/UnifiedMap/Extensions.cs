namespace fivenine.UnifiedMaps
{
    public static class Extensions
    {
        public static bool EqualsSafe(this object a, object b)
        {
            if (a != null)
            {
                return a.Equals(b);
            }

            if (b != null)
            {
                return b.Equals(a);
            }

            // Both are null -> so equals
            return true;
        }
    }
}
