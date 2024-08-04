
namespace BBnf.Tokens {
  public static partial class Parser {

    public abstract record Result {

      public abstract bool IsSuccess { get; }

      public IReadOnlyList<Token>? Tokens { get; }

      internal Result(IReadOnlyList<Token>? tokens)
        => Tokens = tokens;
    }
  }
}