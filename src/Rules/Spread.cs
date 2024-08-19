namespace BBnf.Rules {
  public class Spread
    : Rule,
     IRule<Spread>,
     Rule.IPart {

    public static Spread Parse(TextCursor cursor, Parser.Context context) {
      cursor.SkipWhiteSpace();
      if(cursor.Read("...")) {
        Rule? parent = context.Parent;
        Rule rule = Rule.Parse(cursor, context with {
          Sequence = null,
          Parent = null
        });

        Spread spread = new(parent!, rule);
        return spread;
      }
      else {
        throw new InvalidDataException("Expected a spread operator (`...` as a prefix).");
      }
    }


    public Rule Parent { get; }
    public Rule Rule { get; }
    private Spread(Rule parent, Rule rule)
      => (Parent, Rule)
      = (parent, rule);
  }
}