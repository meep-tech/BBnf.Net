using System.Diagnostics.Contracts;

namespace BBnf.Rules {

  public abstract class Literal
  : Terminal<Literal> {

    public new static Literal Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      Contract.Requires(parent is not null);

      if(cursor.Read(['"', '\'', '`'])) {
        string text = "";
        char quote = cursor.Current;
        do {
          if(cursor.Read(out string? chars, c => c != quote)) {
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
          '`' => new Pattern(parent!, text),
          _ => throw new InvalidDataException(
            "Expected a quote of some kind to start a literal rule.")
        };
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