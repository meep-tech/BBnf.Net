using System.Text.RegularExpressions;

namespace BBnf.Tokens {
  public record Lexeme(
    string Name,
    ReadOnlySet<string> Aliases,
    ReadOnlySet<string> Tags,
    TextCursor.Location Location,
    Regex Pattern,
    string Text
  ) : Token(Name, Aliases, Tags, Location, Text);
}