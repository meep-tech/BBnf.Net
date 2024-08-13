namespace BBnf.Rules {
  public class Special
  : Literal {
    public string Key { get; }
    internal Special(Rule parent, string key, string pattern)
      : base(parent, pattern)
      => Key = key;

    public override string ToBbnf()
      => $"`{"{"}{Key}{"}"}{Text}`";
  }
}