namespace BBnf.Tokens {
  public record Empty(
    string Name,
    ReadOnlySet<string> Aliases,
    ReadOnlySet<string> Tags,
    TextCursor.Location Location
  ) : Token(Name, Aliases, Tags, Location, string.Empty);
}