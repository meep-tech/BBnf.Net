using System.Diagnostics.Contracts;

namespace BBnf.Rules {

  public abstract class Literal
  : Terminal<Literal> {

    public new static Literal Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      Contract.Requires(parent is not null);

      if(cursor.Read(['"', '\'', '/', '`'])) {
        string text = "";
        string? key = null;
        char quote = cursor.Previous;

        // read key if it is a special literal
        if(quote is '`') {
          if(cursor.Read('{')) {
            cursor.ReadWhile(out key, c => c != '}');
            cursor.Skip();
          }
        }

        // read all chars until the closing quote (except if it is escaped)
        do {
          if(cursor.ReadWhile(out string? chars, c => c != quote)) {
            text += chars;
          }
          else {
            throw new InvalidDataException(
              "Expected a closing quote for a literal terminal rule.");
          }
        } while(cursor.Previous == '\\');

        cursor.Skip();
        return quote switch {
          '\'' => new Character(parent!, text.Length != 1
            ? throw new InvalidDataException(
              "Expected a single character for a character literal rule using single quotes.")
            : text[0]),
          '"' => new Text(parent!, text),
          '/' => new Pattern(parent!, text),
          '`' => new Special(parent!, key!, text),
          _ => throw new InvalidDataException(
            "Expected a quote of some kind to start a literal rule.")
        };
      }
      else if(cursor.Current.IsDigit()) {
        return Number.Parse(cursor, context);
      }
      else {
        throw new InvalidDataException(
          "Expected a single or double quote to start a literal terminal rule.");
      }
    }

    /// <inheritdoc/>
    public override Rule Parent { get; internal init; }
    /// <inheritdoc />
    public virtual string Text { get; }

    internal Literal(Rule parent, string text)
      => (Parent, Text) = (parent, text);

    public override string ToSExpression()
      => $"(__{GetType().Name.ToLowerInvariant()}__\n\t\"{Text.Indent()}\")";
  }
}