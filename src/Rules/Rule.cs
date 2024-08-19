using System.Diagnostics.Contracts;

namespace BBnf.Rules {

  public abstract partial class Rule
    : IRule<Rule> {

    /// <summary>
    /// If this rule not generate a complete expression, but instead
    ///   should be used as part of a the larger, containing, named rule.
    /// </summary>
    public virtual bool IsPartial
      => true;

    protected Rule() { }
    public abstract string ToSExpression();
    public abstract string ToBbnf();

    public static Rule Parse(TextCursor cursor, Parser.Context context) {
      IReadOnlyList<Rule>? seq = context.Sequence;
      Rule? parent = context.Parent;
      Contract.Requires(seq is null);
      Contract.Requires(parent is not null);

      List<Rule> rules = [];
      context = context with { Sequence = rules };

      cursor.SkipWhiteSpace();
      while(!cursor.IsAtEnd && cursor.Current is not ';') {
        switch(cursor.Current) {
          case '!':
            rules.Add(Not.Parse(cursor, context));
            break;
          case '.':
            rules.Add(Immediate.Parse(cursor, context));
            break;
          case '(':
            rules.Add(Group.Parse(cursor, context));
            break;
          case '[':
            rules.Add(Optional.Parse(cursor, context));
            break;
          case '?' when !cursor.Previous.IsWhiteSpaceOrNull():
            rules[^1] = Optional.Parse(cursor, context);
            break;
          case '?' when cursor.Previous.IsWhiteSpaceOrNull():
            rules.Add(Optional.Parse(cursor, context));
            break;
          case '*' when cursor.Previous.IsWhiteSpaceOrNull():
            rules.Add(Repeat.Parse(cursor, context));
            break;
          case '*' when !cursor.Previous.IsWhiteSpaceOrNull():
            rules[^1] = NoneOrMore.Parse(cursor, context);
            break;
          case '+' when cursor.Previous.IsWhiteSpaceOrNull():
            rules.Add(OneOrMore.Parse(cursor, context));
            break;
          case '+' when !cursor.Previous.IsWhiteSpaceOrNull():
            rules[^1] = OneOrMore.Parse(cursor, context);
            break;
          case '|':
            rules[^1] = Choice.Parse(cursor, context);
            break;
          case '/' when cursor.Next is '*':
            _ = ParseTagsAndComments(cursor);
            break;
          case '/' when cursor.Next is '/':
            _ = ParseTagsAndComments(cursor);
            break;
          case '"' or '/' or '\'' or '`':
            rules.Add(Literal.Parse(cursor, context));
            break;
          case '#' when cursor.Next.IsWhiteSpaceOrNull():
            _ = ParseTagsAndComments(cursor);
            break;
          case '#' when !cursor.Next.IsWhiteSpaceOrNull():
            rules[^1] = Tagged.Parse(cursor, context);
            break;
          case '_':
            int peek = 0;
            while(cursor.Peek(peek) is '_') {
              peek++;
            }

            if(cursor.Peek(peek).IsUpper()) {
              rules.Add(Token.Parse(cursor, context));
            }
            else {
              rules.Add(Reference.Parse(cursor, context));
            }

            break;
          case char c when c.IsLetter() && c.IsUpper():
            rules.Add((Rule)IToken.Parse(cursor, context));
            break;
          case char c when c.IsLetter() && c.IsLower():
            rules.Add(Reference.Parse(cursor, context));
            break;
          case char c when c.IsDigit():
            var i = 1;
            while(cursor.Peek(i).IsDigit()) {
              i++;
            }

            if(cursor.Peek(i) is '*') {
              rules.Add(NoneOrMore.Parse(cursor, context));
            }
            else {
              rules.Add(Number.Parse(cursor, context));
            }

            break;
          case char c when c.IsWhiteSpaceOrNull():
            cursor.Skip();
            continue;
          default:
            throw new InvalidDataException($"Unexpected token: {cursor.Current}");
        }
      }

      // if the first rule is a tagged rule without a target;
      // then we apply the tags to the entire rule/sequence
      if(rules.First() is Tagged ownTags && ownTags.Rule is null) {
        rules.RemoveAt(0);
        if(rules.Count == 1) {
          ownTags.Rule = rules[0];
        }
        else {
          Rule rule = Sequence.Parse(
            cursor,
            context with {Sequence = rules.Skip(1).ToList() }
          );
          ownTags.Rule = rule;
        }

        return ownTags;
      } // return either the single rule or a sequence (without applying own-tags)
      else {
        return rules.Count > 1
          ? Sequence.Parse(cursor, context)
          : rules[0];
      }
    }

    protected static string[] ParseTagsAndComments(TextCursor cursor) {
      List<string> tags = [];

      cursor.SkipWhiteSpace();
      while(cursor.Current is '#') {
        // skip comment
        if(cursor.Next.IsWhiteSpace()) {
          cursor.Skip();
          cursor.SkipWhiteSpace();
          cursor.Read(While.Not.Char.IsNewLine);
          cursor.SkipWhiteSpace();
        } // read tag
        else {
          cursor.Skip();
          if(cursor.ReadWhile(out string? tag, While.Not.Char.IsWhiteSpaceOrNull)) {
            tags.Add(tag);
          }
          cursor.SkipWhiteSpace();
        }
      }

      while(!cursor.IsAtEnd && cursor.Current is '/' && cursor.Next is '*') {
        cursor.Skip(2);
        while(!cursor.IsAtEnd && !(cursor.Current is '*' && cursor.Next is '/')) {
          cursor.Skip();
        }
        cursor.Skip(2);
        cursor.SkipWhiteSpace();
      }

      while(!cursor.IsAtEnd && cursor.Current is '/' && cursor.Next is '/') {
        cursor.Skip(2);
        cursor.Read(While.Not.Char.IsNewLine);
        cursor.SkipWhiteSpace();
      }

      return [.. tags];
    }
  }
}