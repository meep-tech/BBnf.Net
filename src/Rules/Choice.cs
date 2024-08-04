using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Choice
    : Rule,
      IRule<Choice>,
      Rule.IPart {

    public static new Choice Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);
      Contract.Requires(parent is not null);

      cursor.SkipWhiteSpace();
      if(cursor.Read('|')) {
        List<Rule> rules = [];
        Choice choice = new(parent!, rules);

        Rule right = Rule.Parse(cursor, context with {
          Sequence = null,
          Parent = choice
        });
        if(right is Choice cRight) {
          rules.AddRange(cRight.Rules);
        }
        else {
          rules.Add(right);
        }

        if(seq![^1] is Choice cLeft) {
          rules.AddRange(cLeft.Rules);
        }
        else {
          rules.Add(seq[^1]);
        }

        return new Choice(
          parent!,
          rules
        );
      }
      else {
        throw new InvalidDataException("Expected a pipe to indicate a choice between rules.");
      }
    }

    public IReadOnlyList<Rule> Rules { get; }

    public Rule Parent { get; }

    private Choice(Rule parent, IReadOnlyList<Rule> rules)
      => (Parent, Rules) = (parent, rules);

    public override string ToSExpression()
      => $"(__choice__\n{string.Join("\n", Rules.Select(r => r.ToSExpression().Indent()))})";

    public override string ToBbnf()
      => Rules.Count >= 3
        || Rules.Any(r => r is Choice or Sequence or Tagged)
          ? Rules.Select(r => $"| {r.ToBbnf()}".Indent()).Join('\n')
          : Rules.Join(" | ", r => r.ToBbnf());
  }
}