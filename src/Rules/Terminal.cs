namespace BBnf.Rules {

  public interface ITerminal;

  public abstract class Terminal<T>
    : Rule,
      IRule<T>,
      Rule.IPart,
      ITerminal
      where T : Terminal<T> {
    public abstract Rule Parent { get; internal init; }
    static T IRule<T>.Parse(TextCursor cursor, Parser.Context context)
      => cursor.Current is '"' or '\'' or '/'
        ? Literal.Parse(cursor, context) is T literal
          ? literal
          : throw new InvalidDataException("Expected a literal terminal rule.")
        : Token.Parse(cursor, context)
          is T token
          ? token
          : throw new InvalidDataException("Expected a token terminal rule.");
  }
}