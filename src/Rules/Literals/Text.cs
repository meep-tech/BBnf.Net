namespace BBnf.Rules {
  public class Text
  : Literal {
    internal Text(Rule parent, string text)
      : base(parent, text) { }

    public override string ToBbnf()
      => $"\"{Text}\"";
  }
}