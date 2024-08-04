using System.Diagnostics.CodeAnalysis;
using System.Text;

using BBnf.Rules;

using Rule = BBnf.Rules.Rule;
using RuleDictionary = BBnf.Rules.RuleDictionary;
using Token = BBnf.Tokens.Token;
using TokenDictionary = BBnf.Tokens.TokenDictionary;

using ReadOnlyRuleDictionary
  = System.Collections.ObjectModel
    .ReadOnlyDictionary<
      string,
      System.Collections.ObjectModel
        .ReadOnlyDictionary<string, BBnf.Rules.Rule>
    >;

namespace BBnf {

  public partial class Grammar
  : ICloneable {

    /// <summary>
    /// The available variants of the grammer via the 
    ///   different styntax contexts available to it.
    /// </summary>
    public IReadOnlyList<Source> Contexts { get; private init; }

    /// <summary>
    /// The current style of the grammar. 
    /// Determines which syntax to use as the default
    ///   overriding context for rule resolution.
    /// </summary>
    public Source Context { get; set; }

    /// <summary>
    /// The tokens given the current context.
    /// </summary>
    public TokenDictionary Tokens { get; private init; }

    /// <summary>
    /// The rules given the current context.
    /// </summary>
    public RuleDictionary Rules { get; private init; }

    #region Initialization

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Grammar() { } // for cloning
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Grammar(
      Source tokens,
      Source rules,
      params Source[] contexts
    ) : this() {
      // init context
      Contexts = [.. rules.Enumerate().Concat(contexts)];
      Context = Contexts[0];

      // load tokens and rules
      Tokens = new TokenDictionary(loadTokens(tokens, out var tags), tags);
      Rules = new RuleDictionary(this, loadRules(out IEnumerable<Parser.Ref> refs));

      if(!validateRefs(refs, out string? message)) {
        throw new ArgumentException(message);
      }

      #region Local Helper Methods

      /// <summary>
      /// load all tokens from all contexts, storing them in a Dictionary by key
      /// </summary>
      Dictionary<string, Token> loadTokens(Source source, out IDictionary<string, ISet<string>> tags) {
        Log.Debug("Loading all tokens from source: " + source);

        Tokens.Parser.Result result
          = BBnf.Tokens.Parser.Parse(source.Path);

        if(result is Tokens.Parser.Failure failure) {
          throw new ArgumentException(
            $"Failed to load tokens from source: '{source.Key}'.\n{failure.Exception.Message}",
            failure.Exception
          );
        }
        else {
          Log.Debug("All tokens loaded successfully!");
          Dictionary<string, Token> tokens = [];
          tags = new Dictionary<string, ISet<string>>();

          foreach(Token token in result.Tokens!) {
            string key = token.Name.ToSnakeCase().ToUpperInvariant();
            if(!tokens.TryAdd(key, token)) {
              throw new ArgumentException(
                $"Token with key: '{key}' already exists.");
            }

            foreach(string? tag in token.Tags
              .Select(t => t
                .Split(':')
                .First()
                .ToLower())
            ) {
              if(tags.TryGetValue(tag, out ISet<string>? value)) {
                value.Add(key);
              }
              else {
                Log.Debug($"New tag registered: {tag}");
                tags.Add(tag, new HashSet<string> { key });
              }
            }
          }

          return tokens;
        }
      }

      /// <summary> 
      /// load all rules from all contexts, storing them in a Dictionary by key
      /// </summary>
      ReadOnlyRuleDictionary loadRules(out IEnumerable<Rules.Parser.Ref> refs) {
        Dictionary<string, Dictionary<string, Rule>> rules
          = [];
        refs = [];

        Console.WriteLine("Loading all rules from all contexts:"
          + string.Join("\n  - ", Contexts.Select(c =>
            $"{c.Key}: {c.Path}")));

        // load each context
        foreach(Source source in Contexts) {
          // load each rule from the context
          Dictionary<string, Rule> ctx = [];
          if(!rules.TryAdd(source.Key, ctx)) {
            throw new ArgumentException(
              $"Context with key: '{source.Key}' already exists.");
          }

          Context = source;
          Rules.Parser.Result result = BBnf.Rules.Parser.Parse(source.Path, this);
          if(result is Rules.Parser.Failure failure) {
            throw new ArgumentException(
              $"Failed to load rules from context: '{source.Key}', at: {failure.Location}, in {source.Path}.\n{failure.Exception.Message}",
              failure.Exception
            );
          }
          else if(result is Rules.Parser.Success success) {
            Console.WriteLine("All rules loaded successfully!");
            foreach(Custom rule in success.Rules) {
              if(!ctx.TryAdd(rule.Key, rule)) {
                throw new ArgumentException(
                  $"Rule with key: '{rule.Key}' already exists in context: '{source.Key}'.");
              }
            }

            refs = refs.Concat(success.Refs);
          }
        }

        Context = Contexts[0];
        return rules.ToDictionary(
          c => c.Key,
          c => c.Value.AsReadOnly()
        ).AsReadOnly();
      }

      /// <summary>
      /// validate all references within the provided grammar rules
      /// </summary>
      bool validateRefs(IEnumerable<Parser.Ref> refs, [NotNullWhen(false)] out string? message) {
        foreach(Parser.Ref @ref in refs) {
          string? ctx, name = null!;

          if(@ref.Target.Contains('.')) {
            string[] parts = @ref.Target.Split('.');
            ctx = parts[0];
            name = parts[1];

            if(!Rules.ContainsKey(ctx)) {
              message = error(
                $"The context:'{ctx}' does not exist in the current grammar.");

              return false;
            }
            else if(!Rules.Contexts[ctx].ContainsKey(name)) {
              message = error(
                $"The rule:'{name}' does not exist in the context:'{ctx}'.");

              return false;
            }
          }
          else {
            if(!Rules.ContainsKey(@ref.Target)) {
              message = error(
                $"The rule:'{@ref.Target}' does not exist in the current grammar.");

              return false;
            }
          }

          #region Local Helper Methods

          string error(string message) {
            Rule named = @ref.Source;
            while(named is Rule.IPart part) {
              named = part.Parent;
            }

            return $"""
              [ERROR]: Failed to validate all references within the provided grammar rules.
                [Message]: {message.Indent(inline: true)}
                [Location]: [{@ref.Position.Line}, {@ref.Position.Column}]
                  [file]: {@ref.Context?.Path ?? "..."}
                  [rule]: {((Named)named).Key}
                  [token]: {@ref.Target}
                  [line]: {@ref.Position.Line}
                  [column]: {@ref.Position.Column}
                  [Index]: {@ref.Position.Index}
              """;
          }

          #endregion
        }

        message = null;
        return true;
      }

      #endregion
    }

    #endregion

    public string ToSExpression(params string[] contexts) {
      StringBuilder sb = new($"(grammar #ctx:{Context.Key}\n");
      if(contexts.Length == 0) {
        foreach(Rule rule in Rules.Values) {
          sb.Append(rule.ToSExpression().Indent());
          sb.AppendLine();
        }
      }
      else {
        foreach(Source context
        in contexts.Select(c => Contexts.First(c => c.Key == c))) {
          sb.Append($"\t\n(@{context.Key} #ctx");
          foreach(Rule rule in Rules.From(context).Values) {
            sb.Append(rule.ToSExpression().Indent(2));
            sb.AppendLine();
          }
          sb.Append(')');
        }
      }

      return sb.Append(')').ToString();
    }

    public object Clone()
      => new Grammar {
        Contexts = Contexts,
        Context = Context,
        Rules = Rules,
        Tokens = Tokens
      };
  }
}