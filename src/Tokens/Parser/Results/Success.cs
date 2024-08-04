namespace BBnf.Tokens {
  public static partial class Parser {
    public record Success
    : Result {

      public override bool IsSuccess
        => true;

      public new IReadOnlyList<Token> Tokens
        => base.Tokens!;

      internal Success(IReadOnlyList<Token> tokens)
        : base(tokens) { }
    }
  }
}