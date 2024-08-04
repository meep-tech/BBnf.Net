using System.Collections.ObjectModel;

namespace BBnf.Tokens {
  public class TokenDictionary(
  IDictionary<string, Token> tokens,
  IDictionary<string, ISet<string>> tags
) : ReadOnlyDictionary<string, Token>(tokens) {
    public ReadOnlyDictionary<string, ReadOnlySet<string>> Tags { get; }
      = new(tags.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value.AsReadOnly()
      ));
  }
}