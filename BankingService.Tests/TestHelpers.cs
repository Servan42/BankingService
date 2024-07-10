namespace BankingService.Tests
{
    internal static class TestHelpers
    {
        internal static bool IsEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            CollectionAssert.AreEqual(expected, actual);
            return true;
        }
    }
}
