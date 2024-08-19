
namespace BBnf.Rules {
  public class Number
    : Literal,
      IRule<Number> {

    public new static Number Parse(TextCursor atCursor, Parser.Context context) {
      string val = "";

      if(atCursor.Read("-")) {
        val += atCursor.Current;
      }

      while(atCursor.Current.IsDigit()) {
        val += atCursor.Current;
        atCursor.Skip();
        while(atCursor.Current is '_') {
          atCursor.Skip();
        }
      }

      return new Number(context.Parent!, decimal.Parse(val));
    }

    public bool IsNegative
      => Text.StartsWith('-');

    public bool IsDecimal
      => Text.Contains('.');

    public bool IsInteger
      => !IsDecimal;

    public bool IsPositive
      => !IsNegative;

    public decimal Value
      => decimal.Parse(Text);

    internal Number(Rule parent, decimal value)
      : base(parent, value.ToString()) { }

    public override string ToBbnf()
      => $"{Text}";
  }
}