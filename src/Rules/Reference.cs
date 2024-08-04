using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Reference
    : Rule,
      IRule<Reference>,
      Rule.IPart {

    public static new Reference Parse(TextCursor cursor, Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is null);
      TextCursor.Location start = cursor.Position;

      cursor.SkipWhiteSpace();
      if(cursor.Read(out string? key, c =>
        (c.IsLetter() && c.IsLower())
        || c == '_'
        || c == '.'
        || c.IsDigit()
      )) {
        // register the reference in the parser
        Rule named = context.Parent!;
        while(named is Rule.IPart part) {
          named = part.Parent;
        }

        Parser.Ref @ref = new(
          key!,
          (Custom)named,
          start,
          grammar!.Context
        );

        context._registerReference(@ref);

        // create and return the reference rule
        Reference reference = new(parent!, key!);

        return reference;
      }
      else {
        throw new InvalidDataException("Expected a lowercase identifier for a rule reference.");
      }
    }

    /// <summary>
    /// The name of the rule to reference.
    /// </summary>
    public string Key { get; }

    public Rule Parent { get; }

    private Reference(Rule parent, string key)
      => (Parent, Key) = (parent, key);

    public override string ToSExpression()
      => $"({Key.ToSnakeCase().ToLowerInvariant()})";

    public override string ToBbnf()
      => Key.ToSnakeCase().ToLowerInvariant();
  }
}