using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class OneOrMore
    : Rule,
      IRule<OneOrMore>,
      Rule.IPart {

    public static new OneOrMore Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);
      Contract.Requires(seq!.Any());
      Contract.Requires(parent is not null);

      cursor.SkipWhiteSpace();
      if(!cursor.Previous.IsWhiteSpaceOrNull() && cursor.Read('+')) {
        cursor.Skip();
        return new OneOrMore(parent!, seq![^1]);
      }
      else {
        throw new InvalidDataException("Expected a plus to indicate one or more of a rule.");
      }
    }

    public Rule Rule { get; }

    public Rule Parent { get; }

    private OneOrMore(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override string ToSExpression()
      => $"(__oom__\n\t{Rule.ToSExpression()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}+";
  }
}