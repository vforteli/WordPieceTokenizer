using System.Text.RegularExpressions;

namespace vforteli.WordPieceTokenizer;

public partial class PreTokenizer
{
    /// <summary>
    /// Split text into words according to.. and.. um.. bertish
    /// </summary>
    public IEnumerable<Word> Split(ReadOnlySpan<char> text)
    {
        // todo maybe switch to something that just reads characters from a span instead...
        var matches = WordBoundaryPattern().Matches(text.ToString());
        return matches.Select(o => new Word(o.Value, o.Index, o.Index + o.Length));
    }

    [GeneratedRegex(@"(\b\w+\b)|\p{P}")]
    private static partial Regex WordBoundaryPattern();
}