using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Unordered
    : Rule,
      IRule<Unordered>,
      Rule.IPart {

    public static new Unordered Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is null);
      Contract.Requires(parent is not null);

      cursor.SkipWhiteSpace();
      if(cursor.Read('{')) {
        Unordered unordered = new(parent!, null!);
        Rule rule = Rule.Parse(cursor, context with {
          Sequence = null,
          Parent = unordered
        });

        cursor.SkipWhiteSpace();
        if(cursor.Read('}')) {
          cursor.Skip();
          unordered.Rule = rule;

          return unordered;
        }
        else {
          throw new InvalidDataException("Expected a right parenthesis to end a unordereded rule.");
        }
      }
      else {
        throw new InvalidDataException("Expected a left parenthesis to start a unordereded rule.");
      }
    }

    public Rule Rule { get; private set; }

    public Rule Parent { get; }

    private Unordered(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override string ToSExpression()
      => $"(__unordered__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"({Rule.ToBbnf()})";
  }
}