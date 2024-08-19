namespace BBnf.Rules {
  public class Not
    : Rule,
      IRule<Not>,
      Rule.IPart {
    public static new Not Parse(TextCursor cursor, Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;

      cursor.SkipWhiteSpace();
      if(cursor.Read('!')) {
        if(seq?.Any() ?? false) {
          if(cursor.Back(2).IsWhiteSpaceOrNull()) {
            throw new InvalidDataException("Expected a rule to be preceeding the not exclamation mark; but no rule was found.");
          }
          else {
            Not not = new(parent!, null!);
            Rule rule = Rule.Parse(cursor, context with {
              Sequence = null,
              Parent = not
            });

            not.Rule = rule;
            return not;
          }
        }
        else {
          if(cursor.Current.IsWhiteSpaceOrNull()) {
            throw new InvalidDataException("Unexpected padding after the exclamation mark.");
          }
          else {
            Not not = new(parent!, null!);
            Rule rule = Rule.Parse(cursor, context with {
              Sequence = null,
              Parent = not
            });

            not.Rule = rule;
            return not;
          }
        }
      }
      else {
        throw new InvalidDataException("Expected an exclamation mark to indicate a not rule.");
      }
    }

    public Rule Rule { get; private set; }
    public Rule Parent { get; }

    private Not(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override string ToSExpression()
      => $"(__not__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"!{Rule.ToBbnf()}";
  }
}