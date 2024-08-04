
namespace BBnf.Rules {
  public static partial class Parser {
    public readonly record struct Context {
      private static readonly List<Ref> _refs = [];
      public Grammar? Grammar { get; }
      public IReadOnlyList<Rule>? Sequence { get; internal init; }
      public Rule? Parent { get; internal init; }
      public IEnumerable<Ref> Refs { get; }
        = _refs;

      internal Context(
        Grammar? grammar,
        Rule? parent = null,
        IReadOnlyList<Rule>? currentSeq = null
      ) => (Grammar, Parent, Sequence)
        = (grammar, parent, currentSeq);

      internal void _registerReference(Ref @ref) {
        if(Parent is not null) {
          _refs.Add(@ref);
        }
      }
    }
  }
}