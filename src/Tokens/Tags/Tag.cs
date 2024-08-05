namespace BBnf.Tokens.Tags {

  public record Tag(
  params string[] Keys
) : BBnf.Tags.Tag(Keys) {

    public record Context(
      string? Prev,
      string? Next,
      IReadOnlySet<string> Tags
    );

    /// <summary>
    /// Used to modify the raw value of the tagged token when it's value is set/initailized.
    /// </summary>
    /// <param name="value">The new/next value being assigned to the token</param>
    /// <param name="context">The context of the lexer and the token being set</param>
    /// <returns>The new value to be assigned to the token.</returns> 
    protected internal virtual Func<string?, Context, string?> Set
      => (value, _) => value;
  }
}