using System.Text;

namespace Mcce.SmartOffice.Api
{
    internal static class Make
    {
        private static readonly Random Random = new();

        #region String
        public static string String()
        {
            return String(5);
        }

        public static string String(int length)
        {
            return String(length, false);
        }

        public static string String(int length, bool lowerCase)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < length; i++)
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65))));

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        #endregion String

        #region Int
        public static int Int()
        {
            return Random.Next();
        }

        public static int Int(int min, int max)
        {
            return Random.Next(min, max);
        }
        #endregion Int

        #region Double
        public static double Double()
        {
            return Random.NextDouble();
        }
        #endregion Double

        #region Date
        public static DateTime DateTime()
        {
            var start = new DateTime(1995, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            int range = (System.DateTime.Today - start).Days;
            return start.AddDays(Random.Next(range));
        }
        #endregion Date

        #region Identifier
        public static string Identifier()
        {
            return Guid.NewGuid().GetHashCode().ToString("x8");
        }
        #endregion Identifier

        #region Bool
        public static bool Bool()
        {
            return Int(0, 1) == Int(0, 1);
        }
        #endregion Bool

        #region Enum
        public static TEnum Enum<TEnum>()
            where TEnum : struct
        {
            var values = System.Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToArray();
            var index = Random.Next(0, values.Length -1);

            return values[index];

        }
        #endregion
    }
}
