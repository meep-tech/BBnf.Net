using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Not
    : Rule,
      IRule<Not>,
      Rule.IPart {
    public static new Not Parse(TextCursor cursor, Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);

      cursor.SkipWhiteSpace();
      if(cursor.Read('!')) {
        if(cursor.Current.IsWhiteSpaceOrNull()) {
          throw new InvalidDataException("Unexpected padding after the exclamation mark.");
        }

        Rule rule = Rule.Parse(cursor, context);

        return new Not(parent!, rule);
      }
      else {
        throw new InvalidDataException("Expected an exclamation mark to indicate a not rule.");
      }
    }

    public Rule Rule { get; }
    public Rule Parent { get; }

    private Not(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override string ToSExpression()
      => $"(__not__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"!{Rule.ToBbnf()}";
  }
}