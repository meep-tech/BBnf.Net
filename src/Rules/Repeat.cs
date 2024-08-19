using System.Diagnostics.Contracts;

using BBnf.Cursors;

namespace BBnf.Rules {

  public class Repeat
    : Rule,
      IRule<Repeat>,
      Rule.IPart {

    public static new Repeat Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(parent is not null);

      cursor.SkipWhiteSpace();
      if(cursor.Current is '*') {
        if(seq?.Any() ?? false) { // rule*X[-Y]
          if(cursor.Next.IsDigit()) {
            cursor.ReadNumber(out int? min);
            int? max = min;

            // rule*X-Y
            if(cursor.Read('-') || cursor.Read("..")) {
              cursor.ReadNumber(out max);
            }

            Repeat repeat = new(parent!, seq![^1], min!.Value, max);
            return repeat;
          }
          else if(cursor.Previous.IsWhiteSpaceOrNull()) { // rule *X-Y
            throw new InvalidDataException("Expected the star touching the preceding rule to indicate a number of repetitions of a given rule.");
          }
          else { // rule*
            return NoneOrMore.Parse(cursor, context);
          }
        }
        else if(cursor.Next.IsWhiteSpaceOrNull()) { // * rule
          throw new InvalidDataException("Expected a rule to be after the star to indicate a number of repetitions of a given rule.");
        }
        else if(cursor.Next.IsDigit()) { // *X[-Y][*]rule
          cursor.Next();
          cursor.ReadNumber(out int? min);
          int? max = min;

          if(cursor.Read('-') || cursor.Read("..")) {
            // *X-Y[*]rule
            cursor.ReadNumber(out max);
          }

          cursor.Skip('*');

          Repeat repeat = new(parent!, null!, min!.Value, min);
          Rule rule = Rule.Parse(cursor, context with {
            Sequence = null,
            Parent = repeat
          });

          repeat.Rule = rule;
          return repeat;
        }
        else { // *rule
          return NoneOrMore.Parse(cursor, context);
        }
      }
      else if(cursor.Current is '+') {
        if(seq?.Any() ?? false) { // rule+X
          if(cursor.Next.IsDigit()) {
            cursor.ReadNumber(out int? min);
            Repeat repeat = new(parent!, seq[^1], min!.Value, null);
            return repeat;
          }
          else if(cursor.Previous.IsWhiteSpaceOrNull()) { // rule +
            throw new InvalidDataException("Expected the plus touching the preceding rule to indicate a number of repetitions of a given rule.");
          }
          else { // rule+
            return OneOrMore.Parse(cursor, context);
          }
        }
        else if(cursor.Next.IsWhiteSpaceOrNull()) { // + rule
          throw new InvalidDataException("Expected a rule to be after the plus to indicate a number of repetitions of a given rule.");
        }
        else if(cursor.Next.IsDigit()) { // +X[*]rule
          cursor.Next();
          cursor.ReadNumber(out int? min);
          cursor.Skip('*');

          Repeat repeat = new(parent!, null!, min!.Value, null);
          Rule rule = Rule.Parse(cursor, context with {
            Sequence = null,
            Parent = repeat
          });

          repeat.Rule = rule;
          return repeat;
        }
        else { // +rule
          return OneOrMore.Parse(cursor, context);
        }
      }
      else if(cursor.Current.IsDigit()) { // X[-][Y][*]rule
        cursor.ReadNumber(out int? min);
        int? max = min;

        // X-Y[*]rule
        if(cursor.Read('-') || cursor.Read("..")) {
          cursor.ReadNumber(out max);
          // X-*rule
          if(max is null && cursor.Next is not '*') {
            throw new InvalidDataException("Expected a star or number to indicate a maximum number of repetitions of a given rule.");
          }
        }

        cursor.Skip('*');

        Repeat repeat = new(parent!, null!, min!.Value, max);
        Rule rule = Rule.Parse(cursor, context with {
          Sequence = null,
          Parent = repeat
        });

        repeat.Rule = rule;
        return repeat;
      }
      else {
        throw new InvalidDataException("Expected a star or plus along with numbers to indicate a number of repetitions of a given rule.");
      }
    }

    public int Min { get; }

    public int? Max { get; }

    public Rule Rule { get; private protected set; }

    public Rule Parent { get; }

    private protected Repeat(Rule parent, Rule rule, int min, int? max)
      => (Parent, Rule, Min, Max)
      = (parent, rule, min, max);

    public override string ToSExpression()
      => $"(__repeat__ {Min}{(Max is null ? '+' : $"-{Max}")} \n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}{{{Min}{(Max is null ? "" : $"-{Max}")}}}";
  }
}