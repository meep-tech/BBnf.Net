using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Range
    : Rule,
      IRule<Range>,
      Rule.IPart {

    public static new Range Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(parent is not null);
      Contract.Requires(seq is not null);
      Contract.Requires(seq!.Any());

      cursor.SkipWhiteSpace();
      if(cursor.Read("-") || cursor.Read("..")) {
        Rule start = seq![^1];
        Rule end;

        Range range = new(parent!, start, null!);
        if(cursor.Next.IsDigit()) {
          end = Number.Parse(cursor, context with {
            Sequence = null,
            Parent = range
          });
        }
        else if(cursor.Next.IsUpper()) {
          end = Token.Parse(cursor, context with {
            Sequence = null,
            Parent = range
          });
        }
        else if(cursor.Next is '_') {
          end = Reference.Parse(cursor, context with {
            Sequence = null,
            Parent = range
          });

          if(end is not Token) {
            throw new InvalidDataException("Expected a token reference for the end of a range.");
          }
        }
        else {
          throw new InvalidDataException("Expected a number or token reference for the end of a range.");
        }

        range.End = end;
        return range;
      }
      else {
        throw new InvalidDataException("Expected a range operator (`-` or `..`) between two rules.");
      }
    }

    public Rule Start { get; }

    public Rule End { get; private set; }

    public Rule Parent { get; }

    private Range(Rule parent, Rule start, Rule end)
      => (Parent, Start, End)
      = (parent, start, end);

    public override string ToSExpression()
      => $"(__range__\n{Start.ToSExpression().Indent()}\n{End.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"{Start.ToBbnf()}..{End.ToBbnf()}";
  }
}