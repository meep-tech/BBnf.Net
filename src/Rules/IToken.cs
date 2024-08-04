using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public interface IToken
    : IRule {

    string Key { get; }

    public static IToken Parse(TextCursor cursor, Parser.Context context) {
      Contract.Requires(context.Parent is not null);
      string key = Token.ParseKey(cursor);

      if(context.Grammar?.Tokens.Tags.ContainsKey(key.ToLowerInvariant()) ?? false) {
        return new Token.Interface(context.Parent!, key.ToLowerInvariant());
      }
      else if(context.Grammar?.Tokens.ContainsKey(key.ToUpperInvariant()) ?? true) {
        return new Token(context.Parent!, key.ToUpperInvariant());
      }
      else {
        throw new InvalidDataException($"Unknown token or tag interface key: {key}.");
      }
    }
  }
}