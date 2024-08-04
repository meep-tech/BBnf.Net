using System.Diagnostics.CodeAnalysis;

namespace BBnf.Rules {
  public static partial class Parser {

    public static Result Parse(string path, Grammar? grammar = null)
      => File.Exists(path)
        ? ParseFile(path, grammar)
        : Directory.Exists(path)
          ? ParseDirectory(path, grammar)
          : ParseCode(path, grammar);

    public static Result ParseFile(string path, Grammar? grammar = null)
      => ParseCode(File.ReadAllText(path), grammar);

    public static Result ParseDirectory(string path, Grammar? grammar = null) {
      List<Custom> rules = [];
      List<Ref> refs = [];

      try {
        foreach(string? file in Directory
          .GetFiles(path, "*.rule", SearchOption.AllDirectories)
          .DebugEach($"Loading all rules from file: {0}.")
          .Select(File.ReadAllText)
        ) {
          Result result = ParseCode(file, grammar);
          if(result is Success success) {
            rules.AddRange(success.Rules);
            refs.AddRange(success.Refs);
          }
          else {
            return result;
          }
        }

        return new Success(rules, refs);
      }
      catch(Exception e) {
#if DEBUG
        Console.WriteLine($"Failed to parse rules from directory: {path}.");
        Console.WriteLine(e.Message);
#endif

        return new Failure(
          e,
          null,
          rules
        );
      }
    }

    public static Result ParseCode(string src, Grammar? grammar = null) {
      List<Custom> rules = [];
      List<Ref> refs = [];

      TextCursor cursor = new(src);
      cursor.SkipWhiteSpace();

      while(!cursor.IsAtEnd) {
        Context context = new(grammar);
        if(TryParse(cursor, context, out Custom? rule, out Exception? exception)) {
          rules.Add(rule!);
          refs.AddRange(context.Refs);
          cursor.SkipWhiteSpace();
        }
        else {
          return new Failure(
            exception!,
            cursor.Position,
            rules,
            refs
          );
        }
      }

      return new Success(rules, refs);
    }

    public static bool TryParse(
      TextCursor cursor,
      Context context,
      [NotNullWhen(true)] out Custom? rule,
      [NotNullWhen(false)] out Exception? exception
    ) {
      try {
        rule = Custom.Parse(cursor, context);
        exception = null;
        return true;
      }
      catch(Exception e) {
        rule = null;
        exception = e;
        return false;
      }
    }
  }
}