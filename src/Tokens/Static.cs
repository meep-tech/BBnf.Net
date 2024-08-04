namespace BBnf.Tokens {
  public record Static(
    string Name,
    ReadOnlySet<string> Aliases,
    ReadOnlySet<string> Tags,
    TextCursor.Location Location,
    string Value
  ) : Token(Name, Aliases, Tags, Location, Value);
}