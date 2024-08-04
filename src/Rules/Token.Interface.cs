using System.Diagnostics.Contracts;

namespace BBnf.Rules {

  public partial class Token {
    public class Interface
      : Rule,
        IRule<Interface>,
        Rule.IPart,
        IToken {

      /// <inheritdoc/>
      public static new Interface Parse(TextCursor cursor, Parser.Context context) {
        Contract.Requires(context.Grammar is not null);

        string key = ParseKey(cursor).ToLowerInvariant();
        if(!context.Grammar!.Tokens.Tags.ContainsKey(key)) {
          throw new InvalidDataException($"Unknown token tag interface key: {key}.");
        }

        return new Interface(context.Parent!, key);
      }

      /// <inheritdoc/>
      public Rule Parent { get; }

      public string Key { get; }

      internal Interface(Rule parent, string key)
        => (Parent, Key) = (parent, key);

      public override string ToSExpression()
        => $"(__token_interface__ {Key})";

      public override string ToBbnf()
        => Key.ToSnakeCase().ToUpperInvariant();
    }
  }
}