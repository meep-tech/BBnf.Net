using System.Text.RegularExpressions;

namespace BBnf.Tests.Unit;

public partial class Tags {
    public partial class NL {

        [Theory]
        /// <summary>
        /// Test the default pattern for newlines.
        /// </summary>
        [InlineData("\n", 1),
            InlineData("\r", 1),
            InlineData("\f", 1),
            InlineData("\r\n", 1),
            InlineData("\n\r", 1),
            InlineData("\f\r", 1),
            InlineData("\r\f", 1),
            InlineData("\r\r", 2),
            InlineData("\f\f", 2),
            InlineData("\f\n", 2),
            InlineData("\n\f", 2),
            InlineData("\n\n", 2),
            InlineData("\r\n\f", 2),
            InlineData("\f\r\n", 2),
            InlineData("\r\f\n", 2),
            InlineData("\f\r\f", 2),
            InlineData("\r\f\r", 2),
            InlineData("\r\r\n", 2),
            InlineData("\n\n\r", 2),
            InlineData("\r\n\r\n", 2),
            InlineData("\n\r\n\r", 2),
            InlineData("\r\n\n\r", 2),
            InlineData("\f\f\n", 3),
            InlineData("\n\f\f", 3),
            InlineData("\n\n\n", 3),
            InlineData("\r\r\r", 3),
            InlineData("\f\f\f", 3),
            InlineData("\f\n\f", 3),
            InlineData("\n\f\n", 3),
            InlineData("\r\n\f\n", 3),
            InlineData("\f\r\n\f", 3),
            InlineData("\r\n\f\r\n", 3),
            InlineData("\r\n\r\n\r\n", 3),
            InlineData("\n\r\n\r\n\r", 3),
            InlineData("\r\n\r\f\f\r", 3)]
        public void DefaultPattern(string input, int expected_NewLines) {
            Regex regex = Tokens.Tags.NL.DefaultPattern;
            MatchCollection matches = regex.Matches(input);

            Assert.Equal(expected_NewLines, matches.Count);
        }
    }
}