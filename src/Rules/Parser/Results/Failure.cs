namespace BBnf.Rules {
  public static partial class Parser {
    public record Failure
    : Result {

      public override bool IsSuccess
        => false;

      public Exception Exception { get; }

      public TextCursor.Location? Location { get; }

      internal Failure(
        Exception exception,
        TextCursor.Location? location,
        IReadOnlyList<Custom>? rules,
        IEnumerable<Ref>? refs = null
      ) : base(rules, refs)
        => (Exception, Location)
        = (exception, location);
    }
  }
}