using System.Diagnostics.Contracts;

namespace BBnf.Rules {

  public partial class Token
    : Terminal<Token>,
      IToken {

    public static new Token Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      Contract.Requires(parent is not null);

      string key = ParseKey(cursor);
      return new(parent!, key);
    }

    public static string ParseKey(TextCursor cursor) {
      cursor.SkipWhiteSpace();
      if(cursor.Read(out string? key, c =>
        (c.IsLetter() && c.IsUpper())
        || c.IsDigit()
        || c == '_')
      ) {
        return key;
      }
      else {
        throw new InvalidDataException(
          "Expected an upper-case word for a terminal token rule.");
      }
    }

    public string Key { get; }
    public override Rule Parent { get; internal init; }
    internal Token(Rule parent, string type)
      => (Parent, Key) = (parent, type);

    public override string ToSExpression()
      => $"({ToBbnf()})";

    public override string ToBbnf()
      => Key.ToSnakeCase().ToUpperInvariant();
  }
}