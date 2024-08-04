namespace BBnf.Rules {
  public static partial class Parser {
    public record Success
    : Result {

      public override bool IsSuccess
        => true;

      public new IReadOnlyList<Custom> Rules
        => base.Rules!;

      public new IEnumerable<Ref> Refs
        => base.Refs!;

      internal Success(IReadOnlyList<Custom> rules, IEnumerable<Ref> refs)
        : base(rules, refs) { }
    }
  }
}