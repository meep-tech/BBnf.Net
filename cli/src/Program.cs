using Meep.Tech.Where;

namespace BBnf.Net.CLI {
  public class Program {

    public const string Version
      = "0.0.1";

    public const string Name
      = "BBnf.Net";

    public const string Command
      = "bbnf";

    public const string Usage
    = $"""
      Usage: bbnf [command] [options]

      Commands:
        [parse] [options] <source>   Parse the source file (Default).
        lex     [options] <source>   Lex the source file.
        grammar <command> [options]  Manage the grammar.
      """;

    public enum SubCommand {
      Lex,
      Parse,
      Grammar,
    }

    public enum GrammarCommand {
      Init,
      New = Init,
      Use,
      List,
      Rules,
    }

    public const string GrammarUsage
      = $"""
        Usage: bbnf grammar [command] [options]

        Commands:
          list|ls [-s|--source|--sources]            
            : List all known grammars.
          use [options] [grammar]    
            : Set the current grammar to use (initializing it first if it doesn't exist).
          rules|rs [options] [grammar]
            : List all rules in the current/given grammar.
          init|new [options] [[-n|--name] <name>] [[-s|--source|--sources] (<source> | <context>=<source>)+]
            : Initialize a new grammar.
        """;

    public static void Main(string[] args) {
      (string? cmd, args) = args;
      switch(cmd) {
        case "lex":
          Lex(args);
          break;
        case "grammar" or "grammars":
          Grammar(args);
          break;
        default:
          Parse([.. cmd.InEnumerable().Concat(args).WhereNotNull()]);
          break;
      }
    }

    public static void Lex(string[] args) {
      (string? source, args) = args;
      Console.WriteLine($"Lexing: {source}");
      throw new NotImplementedException();
    }

    public static void Parse(string[] args) {
      (string? source, args) = args;
      Console.WriteLine($"Parsing: {source}");
      throw new NotImplementedException();
    }

    public static void Grammar(string[] args) {
      (string? cmd, args) = args;
      GrammarCommand? gCmd = cmd?.ToLower() switch {
        "init" or "new" => GrammarCommand.Init,
        "use" => GrammarCommand.Use,
        "list" or "ls" => GrammarCommand.List,
        "rules" => GrammarCommand.Rules,
        _ => null,
      };

      switch(gCmd) {
        case GrammarCommand.Init:
          InitGrammar(args);
          break;
        case GrammarCommand.Use:
          UseGrammar(args);
          break;
        case GrammarCommand.Rules:
          ListGrammarRules(args);
          break;
        case GrammarCommand.List:
        default:
          ListGrammars(args);
          break;
      }
    }
  }
}