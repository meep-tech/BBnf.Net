using System.Text.RegularExpressions;

namespace BBnf.Tokens.Tags {
  public partial record NL()
  : Tag("nl", "newline", "new-line") {

    public static string DefaultValue
      => DefaultPattern.ToString();

    public static readonly Regex DefaultPattern
      = _GetDefaultPatternRegex();

    protected internal override Func<string?, Context, string?> Set
      => (val, _) => val ?? DefaultValue;

    [GeneratedRegex(@"\r[\n\f]?|[\n\f]\r?")]
    private static partial Regex _GetDefaultPatternRegex();
  }
}