namespace BBnf.Rules {
  public class Pattern
  : Literal {
    internal Pattern(Rule parent, string pattern)
      : base(parent, pattern) { }

    public override string ToBbnf()
      => $"/{Text}/";
  }
}