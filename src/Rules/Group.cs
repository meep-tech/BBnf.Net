using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Group
    : Rule,
      IRule<Group>,
      Rule.IPart {

    public static new Group Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is null);
      Contract.Requires(parent is not null);

      cursor.SkipWhiteSpace();
      if(cursor.Read('(')) {
        Group group = new(parent!, null!);
        Rule rule = Rule.Parse(cursor, context with {
          Sequence = null,
          Parent = group
        });

        cursor.SkipWhiteSpace();
        if(cursor.Read(')')) {
          cursor.Skip();
          group.Rule = rule;

          return group;
        }
        else {
          throw new InvalidDataException("Expected a right parenthesis to end a grouped rule.");
        }
      }
      else {
        throw new InvalidDataException("Expected a left parenthesis to start a grouped rule.");
      }
    }

    public Rule Rule { get; private set; }

    public Rule Parent { get; }

    private Group(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override string ToSExpression()
      => $"(__group__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"({Rule.ToBbnf()})";
  }
}