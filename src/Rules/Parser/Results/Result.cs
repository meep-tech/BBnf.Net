
namespace BBnf.Rules {
  public static partial class Parser {

    public abstract record Result {

      public abstract bool IsSuccess { get; }

      public IReadOnlyList<Custom>? Rules { get; }

      public IEnumerable<Ref>? Refs { get; }

      internal Result(
        IReadOnlyList<Custom>? rules,
        IEnumerable<Ref>? refs
      ) => (Rules, Refs)
        = (rules, refs);
    }
  }
}