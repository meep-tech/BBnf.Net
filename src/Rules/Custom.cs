using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public abstract class Custom
    : Rule,
      IRule<Custom> {

    public static new Custom Parse(
      TextCursor cursor,
      Parser.Context context
    ) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(parent is null);
      Contract.Requires(seq is null);

      string key = null!;
      try {
        cursor.SkipWhiteSpace();
        IEnumerable<string> tags
          = ParseTagsAndComments(cursor);

        // get the key
        cursor.SkipWhiteSpace();
        if(!cursor.Read(out key!, c => c.IsLetterOrDigit() || c is '_')) {
          throw new InvalidDataException(
            "Expected an alphanumeric identifier/word as the rule key.");
        }

        cursor.SkipWhiteSpace();
        tags = tags.Concat(ParseTagsAndComments(cursor));
        cursor.SkipWhiteSpace();

        // parse for the assigner
        if(!cursor.Read("::=")) {
          throw new InvalidDataException(
            "Expected `::=` as an assigner following the rule's key (and any optional tags or comments).");
        }

        // make the parent for the custom rule content
        Custom custom = key.StartsWith('_')
          ? new Hidden(key, null!)
          : new Named(key, null!);

        // parse the rule content
        custom.Rule = Rule.Parse(cursor, context with { Parent = custom });

        return custom;
      }
      catch(Exception e) {
        throw new InvalidDataException($"""
        [ERROR]: Failed to parse custom rule with key: {key ?? "???"}.
          [Message]: {e.Message.Indent(inline: true)}
          [Location]: [{cursor.Position.Line}, {cursor.Position.Column}]
            [file]: {grammar?.Context.Path ?? "???"}
            [line]: {cursor.Position.Line}
            [column]: {cursor.Position.Column} 
            [Index]: {cursor.Position.Index}
        """, e);
      }
    }

    public Rule Rule { get; private set; }

    public string Key { get; }

    protected Custom(string key, Rule rule) {
      Key = key;
      Rule = rule;
    }

    public override string ToSExpression()
      => $"({Key} ::= \n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => Rule is Tagged tagged
      ? $"{Key} ::= {tagged.TagsToBbnf()}\n{tagged.Rule.ToBbnf().Indent()};"
      : $"{Key} ::=\n{Rule.ToBbnf().Indent()};";
  }
}