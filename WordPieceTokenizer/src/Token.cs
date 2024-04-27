namespace vforteli.WordPieceTokenizer;

/// <summary>
/// Token with the id from vocabulary and its start and end positions
/// </summary>
public record struct Token(int Id, int Start, int End);
