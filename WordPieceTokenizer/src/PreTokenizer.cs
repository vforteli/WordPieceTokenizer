using System.Text.RegularExpressions;

namespace NerAnonymizer;

public record Word(string Text, int Start, int End);

public class PreTokenizer
{
    /// <summary>
    /// Split text into words according to.. and.. um.. bertish
    /// </summary>
    public IEnumerable<Word> Split(ReadOnlySpan<char> text)
    {
        // todo maybe switch to something that just reads characters from a span instead...
        var matches = Regex.Matches(text.ToString(), @"(\b\w+\b)|\p{P}");
        return matches.Select(o => new Word(o.Value, o.Index, o.Index + o.Length));
    }
}