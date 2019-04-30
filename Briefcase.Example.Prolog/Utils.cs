namespace Briefcase.Example.Prolog
{
    static class Utils
    {
        public static bool IsIn(this int number, int lowerBound, int upperBound)
            => lowerBound <= number && number < upperBound;
    }
}
