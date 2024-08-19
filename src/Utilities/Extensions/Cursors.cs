using System.Diagnostics.CodeAnalysis;

namespace BBnf.Cursors {
  public static class CursorExtensions {
    public static bool ReadNumber(this TextCursor cursor, [NotNullWhen(true)] out decimal? value)
      => cursor.ReadNumber(out value, '_');

    public static bool ReadNumber(this TextCursor cursor, [NotNullWhen(true)] out decimal? value, params char[] skip) {
      string val = "";
      while(cursor.Current.IsDigit()) {
        val += cursor.Current;
        cursor.Skip();
        while(cursor.Current is '_') {
          cursor.Skip();
        }
      }

      value = decimal.Parse(val);
      return true;
    }

    public static bool ReadNumber(this TextCursor cursor, [NotNullWhen(true)] out int? value)
      => cursor.ReadNumber(out value, '_');

    public static bool ReadNumber(this TextCursor cursor, [NotNullWhen(true)] out int? value, params char[] skip) {
      string val = "";
      while(cursor.Current.IsDigit()) {
        val += cursor.Current;
        cursor.Skip();
        while(skip.Contains(cursor.Current)) {
          cursor.Skip();
        }
      }

      value = int.Parse(val);
      return true;
    }
  }
}