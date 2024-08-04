using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace BBnf.Rules {
  public class Tagged
    : Rule,
      IRule<Tagged>,
      Rule.IPart {
    public static new Tagged Parse(TextCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);
      Contract.Requires(parent is not null);

      TextCursor.Location? start = cursor.Position;
      Dictionary<string, string?>? tags = null;

      cursor.SkipWhiteSpace();
      if(!cursor.Next.IsWhiteSpaceOrNull() && cursor.Read('#')) {
        while(!cursor.IsAtEnd) {
          // padding before the current token marks the end of a tag
          if(cursor.Current.IsWhiteSpaceOrNull()) {
            if(start is not null) {
              endTag();
              cursor.SkipWhiteSpace();
            }
          }

          // newlines mark the end of a tag list
          if(cursor.Current.IsNewLine()) {
            break;
          } // hash marks the start of a new tag
          else if(cursor.Read('#')) {
            startTag();
          }
        }

        if(start is not null) {
          endTag();
        }

        if(tags is null) {
          throw new InvalidDataException("Tags must be present.");
        }

        if(seq!.Count == 0) {
          return new Tagged(parent!, tags, null!);
        }
        else if(seq![^1] is Tagged tagged) {
          tags = tags.Concat(tagged.Tags).ToDictionary();
          return new Tagged(parent!, tags, tagged.Rule);
        }
        else {
          return new Tagged(parent!, tags, seq[^1]);
        }
      }
      else {
        throw new InvalidDataException("Expected a hash symbol and a space to start a tag after an existing rule.");
      }

      #region Local Helper Methods

      void startTag()
        => start
          = start is null
            ? (TextCursor.Location?)cursor.Position
            : throw new InvalidDataException("tags must be separated by whitespace.");

      void endTag() {
        string[] parts
          = cursor.Memory.Slice(start!.Value.Index..cursor.Position.Index)
            .Join()
            .ToString()
            .Split(':');

        string key = parts[0];
        string? value = parts.Length > 1
          ? parts[1..].Join(':')
          : null;

        (tags ??= []).Add(
            key ?? throw new InvalidOperationException("Tags must have a key"),
            value
        );

        start = null;
      }

      #endregion
    }

    public ReadOnlyDictionary<string, string?> Tags { get; }

    public Rule Rule { get; internal set; }

    public Rule Parent { get; }

    private Tagged(Rule parent, Dictionary<string, string?> tags, Rule rule)
      => (Parent, Tags, Rule) = (parent, new ReadOnlyDictionary<string, string?>(tags), rule);

    public override string ToSExpression()
      => $"(__tagged__\n{Rule.ToSExpression().Indent()}\n{TagsToBbnf().Indent()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()} #{TagsToBbnf()}";

    public string TagsToBbnf()
      => Tags.Join(' ', TagToBbnf);
    #region Static Helper Methods

    public static string TagToBbnf(string key, string? value)
      => $"#{key}{(value is not null ? $":{value}" : "")}";

    public static string TagToBbnf(KeyValuePair<string, string?> tag)
      => TagToBbnf(tag.Key, tag.Value);

    public static KeyValuePair<string, string?> TagFromBbnf(string tag) {
      string[] parts = tag.Split(':');
      return new KeyValuePair<string, string?>(parts[0], parts.Length > 1 ? parts[1..].Join(":") : null);
    }

    #endregion
  }
}