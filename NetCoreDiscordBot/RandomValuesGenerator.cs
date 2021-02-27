using System;
using System.Security.Cryptography;
using System.Text;

namespace NetCoreDiscordBot
{
    /// <summary>
    /// Class for generating random values using <see cref="System.Random"/> 
    /// <para> String generator uses <see cref="RNGCryptoServiceProvider"/> as random provider.</para>
    /// </summary>
    public static class RandomValuesGenerator
    {
        private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        private static readonly Random random = new Random();

        /// <summary>
        /// Gets random <see cref="Int32"/> value between specified values
        /// </summary>
        /// <param name="minValue">Minimum value</param>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>Random <see cref="Int32"/> value</returns>
        public static int GetInt(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public static int GetRandomInt()
        {
            return random.Next();
        }

        /// <summary>
        /// Gets random <see cref="Double"/> value between specified values
        /// </summary>
        /// <param name="minValue">Minimum value</param>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>Random <see cref="Double"/> value</returns>
        public static double GetDouble(double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
        /// <summary>
        /// Get random alphanumeric <see cref="String"/> of specified length
        /// </summary>
        /// <param name="length">String lenth</param>
        /// <returns>Random alphanumeric <see cref="String"/></returns>
        public static string GetString(int length)
        {
            byte[] data = new byte[4 * length];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }
            return result.ToString();
        }

        public static bool GetBool(double chance) => random.NextDouble() < chance;
    }
}
