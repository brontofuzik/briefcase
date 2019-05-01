namespace Briefcase.Utils
{
    public static class NumberExtensions
    {
        public static bool IsWithinInterval(this int number, int lowerBound, int upperBound)
            => lowerBound <= number && number < upperBound;
    }
}
