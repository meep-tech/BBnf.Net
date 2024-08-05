namespace BBnf.Tags {
  /// <summary>
  /// Represents a type of known tag
  /// </summary>
  public interface ITag {
    string[] Keys { get; }
  }

  /// <summary>
  /// Represents a type of known tag for either a token or a rule
  /// </summary>
  public abstract record Tag(
     params string[] Keys
  ) : ITag;
}
