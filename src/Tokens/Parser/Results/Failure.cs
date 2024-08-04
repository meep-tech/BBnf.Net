namespace BBnf.Tokens {
  public static partial class Parser {
    public record Failure
    : Result {

      public override bool IsSuccess
        => false;

      public Exception Exception { get; }

      internal Failure(Exception exception, IReadOnlyList<Token>? tokens)
        : base(tokens)
        => Exception = exception;
    }
  }
}