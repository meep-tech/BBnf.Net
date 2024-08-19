using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class OneOrMore
    : Repeat,
      IRule<OneOrMore>,
      Rule.IPart {

    public static new OneOrMore Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(parent is not null);
      cursor.SkipWhiteSpace();

      if(seq?.Any() ?? false) { // rule+
        if(!cursor.Previous.IsWhiteSpaceOrNull() && cursor.Read('+')) {
          cursor.Skip();
          return new OneOrMore(parent!, seq![^1]);
        }
        else {
          throw new InvalidDataException("Expected a plus to indicate one or more of a rule.");
        }
      }
      else { // +rule
        if(cursor.Previous.IsWhiteSpaceOrNull() && cursor.Read('+')) {
          OneOrMore oneOrMore = new(parent!, null!);
          Rule rule = Rule.Parse(cursor, context with {
            Sequence = null,
            Parent = oneOrMore
          });

          oneOrMore.Rule = rule;
          return oneOrMore;
        }
        else {
          throw new InvalidDataException("Expected a plus to indicate one or more of a rule.");
        }
      }
    }

    private OneOrMore(Rule parent, Rule rule)
      : base(parent, rule, 1, null) { }

    public override string ToSExpression()
      => $"(__oom__\n\t{Rule.ToSExpression()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}+";
  }
}