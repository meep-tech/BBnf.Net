namespace BBnf {

  public record Source(
    string Path,
    string Key
  ) {
    public static implicit operator Source(string source)
      => new(source);

    public static implicit operator Source(FormattableString source) {
      string uri = source.Format.Replace("{0}", "");
      string key = source.GetArgument(0)!.ToString()!;

      return new(uri, key);
    }

    public static implicit operator Source((FormattableString key, string source) tuple)
      => new(Key: tuple.key.Format, Path: tuple.source);
  }
}