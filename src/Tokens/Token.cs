namespace BBnf.Tokens {

  public abstract record Token(
    string Name,
    ReadOnlySet<string> Aliases,
    ReadOnlySet<string> Tags,
    TextCursor.Location Location,
    string Text
  ) {
    private Lazy<ReadOnlySet<string>> _names = new(()
      => new HashSet<string> ([Name, ..Aliases]).AsReadOnly());

    public ReadOnlySet<string> Names
      => _names.Value;
  }
}