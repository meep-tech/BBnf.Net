using System.Diagnostics.CodeAnalysis;

namespace BBnf.Tokens {
  public static partial class Parser {
    public static Result Parse(string source)
      => File.Exists(source)
        ? ParseFromFile(source)
        : Directory.Exists(source)
          ? ParseFromDirectory(source)
          : ParseCode(source);

    public static Result ParseFromFile(string path)
      => ParseCode(File.ReadAllText(path));

    public static Result ParseFromDirectory(string path) {
      List<Token> tokens = [];

      try {
        foreach(var file in Directory.GetFiles(path, "*.tokens", SearchOption.AllDirectories)
          .DebugEach($"Loading all rules from file: {0}.")
          .Select(File.ReadAllText)) {
          Result result = ParseCode(file);
          if(result is Success success) {
            tokens.AddRange(success.Tokens);
          }
          else {
            return result;
          }
        }

        return new Success([.. tokens]);
      }
      catch(Exception e) {
        Log.Debug($"Failed to parse tokens from directory: {path}.");
        Log.Debug(e.Message);

        return new Failure(e, [.. tokens]);
      }
    }

    public static Result ParseCode(string src) {
      List<Token> tokens = [];
      TextCursor cursor = new(src);

      if(cursor.SourceIsEmpty) {
        return new Success([]);
      }

      try {
        while(!cursor.IsAtEnd) {
          if(TryParse(cursor, out Token? token, out Exception? exception)) {
            tokens.Add(token!);
            cursor.SkipWhiteSpace();
          }
          else {
            return new Failure(exception!, [.. tokens]);
          }
        }

        return new Success([.. tokens]);
      }
      catch(Exception e) {
        Log.Debug($"Failed to parse tokens from source code.");
        Log.Debug(e.Message);

        return new Failure(e, [.. tokens]);
      }
    }

    public static bool TryParse(
      TextCursor cursor,
      [NotNullWhen(true)] out Token? token,
      [NotNullWhen(false)] out Exception? exception
    ) {
      TextCursor.Location start;
      IEnumerable<char>? name = null;
      HashSet<string> tags = [];

      read_CommentsAndTags(cursor);

      // Parse for the key of the token.
      start = cursor.Position;
      cursor.ReadWhile(out name, c => char.IsLetterOrDigit(c) || c == '_');
      if(name is null) {
        token = null;
        exception = new Exception($"Unexpected end of input in name at position: {cursor.Position}.");
        return false;
      }

      // Parse for the assigner.
      cursor.SkipWhiteSpace();
      while(!cursor.IsAtEnd) {
        switch(cursor.Current) {
          case '#':
            read_CommentsAndTags(cursor);
            continue;
          // assigner
          case ':' when cursor.Read("::="):
            break;
          // no value?
          case ';':
            token = new_Empty(start);
            exception = null;

            return true;
          // unknown
          default:
            token = null;
            exception = new($"Unexpected character: '{cursor.Current}' at position: {cursor.Position}."
            + " Expected either # for a tag or ::= for a Token definition after a Name.");

            return false;
        }
      }

      read_CommentsAndTags(cursor);

      // Parse the value of the token.
      cursor.SkipWhiteSpace();
      switch(cursor.Current) {
        case '"':
          if(cursor.ReadWhile(out IEnumerable<char>? value, c => c is not '"')) {
            if(!value.Any()) {
              token = null;
              exception = new($"Cannot make a Static Token with an empty value at position: {cursor.Position}.");
              return false;
            }

            token = new_Static(start, value);
            exception = null;
            break;
          }
          else {
            token = null;
            exception = new($"Unexpected end of input in string literal at position: {cursor.Position}.");
            return false;
          }
        case '\'':
          if(cursor.ReadWhile(out IEnumerable<char>? text, c => c is not '\'')) {
            if(text.Count() != 1) {
              token = null;
              exception = new($"Expected a single character for a Lexeme Token within single quotes at position: {cursor.Position}.");
              return false;
            }

            token = new_Static(start, text);
            exception = null;
            break;
          }
          else {
            token = null;
            exception = new($"Unexpected end of input in char at position: {cursor.Position}.");
            return false;
          }
        case '`':
          if(cursor.ReadWhile(out IEnumerable<char>? pattern, c => c is not '`')) {
            token = new_Lexeme(start, pattern);
            exception = null;
            break;
          }
          else {
            token = null;
            exception = new($"Unexpected end of input in pattern at position: {cursor.Position}.");
            return false;
          }
        case ';':
          token = new_Empty(start);
          exception = null;
          break;
        default:
          token = null;
          exception = new($"Unexpected character: '{cursor.Current}' at position: {cursor.Position}."
          + " Expected either \" for a Static Token or ' for a Lexeme Token definition after a Name.");

          return false;
      }

      // Skip comments.
      read_CommentsAndTags(cursor);

      // Make sure it ends with a semicolon.
      if(cursor.Read(';')) {
        return true;
      }
      else {
        token = null;
        exception = new($"Unexpected character: '{cursor.Current}' at position: {cursor.Position}."
        + " Expected a semicolon to end the Token definition.");

        return false;
      }

      #region Local Helper Methods

      Empty new_Empty(TextCursor.Location at)
        => new(name.Join(), [], tags.AsReadOnly(), at);

      Static new_Static(TextCursor.Location at, IEnumerable<char> value)
        => new(name.Join(), [], tags.AsReadOnly(), at, value.Join());

      Lexeme new_Lexeme(TextCursor.Location at, IEnumerable<char> pattern)
        => new(name.Join(), [], tags.AsReadOnly(), at, new(pattern.Join()), null!);

      void read_CommentsAndTags(TextCursor cursor) {
        cursor.SkipWhiteSpace();
        while(cursor.Current is '#') {
          // skip comment
          if(cursor.Next.IsWhiteSpaceOrNull()) {
            cursor.Skip(c => c is not '\n');
            cursor.SkipWhiteSpace();
          } // read tag
          else if(cursor.ReadWhile(out IEnumerable<char>? tag, c => !c.IsWhiteSpaceOrNull())) {
            tags.Add(tag.Join());
            cursor.SkipWhiteSpace();
          }
        }
      }

      #endregion
    }
  }
}
