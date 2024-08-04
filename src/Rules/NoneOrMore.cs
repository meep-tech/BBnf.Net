using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class NoneOrMore
    : Rule,
      IRule<NoneOrMore>,
      Rule.IPart {

    public static new NoneOrMore Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);
      Contract.Requires(seq!.Any());
      Contract.Requires(parent is not null);

      cursor.SkipWhiteSpace();
      if(!cursor.Previous.IsWhiteSpaceOrNull() && cursor.Read('*')) {
        return new NoneOrMore(parent!, seq![^1]);
      }
      else {
        throw new InvalidDataException("Expected a star to indicate zero or more repetitions of a precceding rule.");
      }
    }

    public Rule Rule { get; private set; }

    public Rule Parent { get; }

    private NoneOrMore(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override string ToSExpression()
      => $"(__nom__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}*";
  }
}