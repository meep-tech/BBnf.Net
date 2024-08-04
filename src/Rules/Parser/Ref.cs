namespace BBnf.Rules {
  public partial class Parser {
    public record Ref(
      string Target,
      Custom Source,
      TextCursor.Location Position,
      Source? Context
    );
  }
}