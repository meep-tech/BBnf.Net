namespace BBnf.Rules {
  public class Character
: Literal {
    public char Char { get; }
    override public string Text
      => Char.ToString();
    internal Character(Rule parent, char character)
      : base(parent, string.Empty)
      => Char = character;

    public override string ToBbnf()
      => $"'{Char}'";
  }
}