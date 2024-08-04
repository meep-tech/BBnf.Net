using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Immediate
    : Rule,
      IRule<Immediate>,
      Rule.IPart {

    public static new Immediate Parse(TextCursor cursor, Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      Contract.Requires(parent is not null);

      cursor.SkipWhiteSpace();
      if(cursor.Current is '.' && cursor.Next.IsWhiteSpaceOrNull()) {
        cursor.Skip();

        Immediate immediate = new(parent!, null!);
        immediate.Rule = Rule.Parse(cursor, context with {
          Sequence = null,
          Parent = immediate
        });

        return immediate;
      }
      else {
        throw new InvalidDataException("Expected a dot to indicate a concatenation of rules.");
      }
    }

    public Rule Rule { get; private set; }

    public Rule Parent { get; }

    private Immediate(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override string ToSExpression()
      => $"(__immediate__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $". {Rule.ToBbnf()}";
  }
}