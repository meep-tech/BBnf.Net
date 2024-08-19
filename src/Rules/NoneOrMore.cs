using System.Diagnostics.Contracts;

namespace BBnf.Rules {

  public class NoneOrMore
    : Repeat,
      IRule<NoneOrMore>,
      Rule.IPart {

    public static new NoneOrMore Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;

      Contract.Requires(parent is not null);

      cursor.SkipWhiteSpace();

      if(seq?.Any() ?? false) {
        if(!cursor.Previous.IsWhiteSpaceOrNull() && cursor.Read('*')) {
          return new NoneOrMore(parent!, seq![^1]);
        }
        else {
          throw new InvalidDataException("Expected a star to indicate zero or more repetitions of a precceding rule.");
        }
      }
      else {
        if(cursor.Previous.IsWhiteSpaceOrNull() && cursor.Read('*')) {
          NoneOrMore noneOrMore = new(parent!, null!);
          Rule rule = Rule.Parse(cursor, context with {
            Sequence = null,
            Parent = noneOrMore
          });

          noneOrMore.Rule = rule;
          return noneOrMore;
        }
        else {
          throw new InvalidDataException("Expected a star to indicate zero or more repetitions of a rule.");
        }
      }
    }

    private NoneOrMore(Rule parent, Rule rule)
      : base(parent, rule, 0, null) { }

    public override string ToSExpression()
      => $"(__nom__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}*";
  }
}