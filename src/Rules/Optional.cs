using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Optional
    : Rule,
      IRule<Optional>,
      Rule.IPart {
    public static new Optional Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(parent is not null);

      if(seq is null) {
        cursor.SkipWhiteSpace();
        if(cursor.Read('[')) {
          Optional optional = new(parent!, null!);
          Rule rule = Rule.Parse(cursor, context with {
            Sequence = null,
            Parent = optional
          });

          cursor.SkipWhiteSpace();
          if(cursor.Read(']')) {
            cursor.Skip();
            optional.Rule = rule;

            return optional;
          }
          else {
            throw new InvalidDataException("Expected a right bracket to end an optional rule.");
          }
        }
        else if(cursor.Read('?')) {
          Optional optional = new(parent!, null!);
          Rule rule = Rule.Parse(cursor, context with {
            Sequence = null,
            Parent = optional
          });

          optional.Rule = rule;
          return optional;
        }
        else {
          throw new InvalidDataException("Expected a left bracket to start an optional rule.");
        }
      }
      else if(cursor.Read('?')) {
        if(seq.Count > 0) {
          Rule prev = seq[^1];

          return prev is Optional opt
            ? new Optional(parent!, opt.Rule)
            : new Optional(parent!, prev);
        }
        else {
          if(cursor.Current.IsWhiteSpaceOrNull()) {
            throw new InvalidDataException("Expected a rule to be preceeding the optional question mark; but no rule was found.");
          }

          Optional opt = new(parent!, null!);
          Rule rule = Rule.Parse(cursor, context with {
            Sequence = null,
            Parent = opt
          });

          opt.Rule = rule;
          return opt;
        }
      }
      else {
        throw new InvalidDataException("Expected a question mark to indicate an optional preceeding rule; but no preceeding rule was found.");
      }
    }

    public Rule Rule { get; private set; }

    public Rule Parent { get; }

    private Optional(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override string ToSExpression()
      => $"(__optional__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => Rule is Sequence or Choice or Tagged
        ? $"[\n{Rule.ToBbnf().Indent()}]"
        : $"{Rule.ToBbnf()}?";
  }
}