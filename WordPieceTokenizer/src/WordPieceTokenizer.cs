using System.Collections.Frozen;

namespace vforteli.WordPieceTokenizer;

/// <summary>
/// WordPieceTokenizer
/// </summary>
public class WordPieceTokenizer
{
    private readonly FrozenDictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> _prefixTokensToIds;
    private readonly FrozenDictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> _suffixTokensToIds;
    private readonly int _longestPrefixLength;
    private readonly int _longestSuffixLength;

    private readonly PreTokenizer _preTokenizer;

    private readonly string[] _idsToTokens;
    private readonly int _unkTokenId;

    /// <summary>
    /// Create a new WordPieceTokenizer with a vocabulary and specified UNK token
    /// </summary>    
    public WordPieceTokenizer(string vocabulary, string unkToken = "[UNK]", PreTokenizer? preTokenizer = null)
    {
        _idsToTokens = vocabulary.Split(["\n", "\r\n"], StringSplitOptions.None);
        _prefixTokensToIds = _idsToTokens.Select((t, i) => (t, i)).Where(o => !o.t.StartsWith("##"))
            .Select(o => new KeyValuePair<string, int>(o.t, o.i)).ToFrozenDictionary()
            .GetAlternateLookup<ReadOnlySpan<char>>();

        _suffixTokensToIds = _idsToTokens.Select((t, i) => (t, i)).Where(o => o.t.StartsWith("##"))
            .Select(o => new KeyValuePair<string, int>(o.t[2..], o.i)).ToFrozenDictionary()
            .GetAlternateLookup<ReadOnlySpan<char>>();

        _unkTokenId = _prefixTokensToIds[unkToken];

        _longestPrefixLength = _prefixTokensToIds.Dictionary.Max(o => o.Key.Length);
        _longestSuffixLength = _suffixTokensToIds.Dictionary.Max(o => o.Key.Length);
        _preTokenizer = preTokenizer ?? new PreTokenizer();
    }

    public string IdToToken(int id) => _idsToTokens[id];

    public int TokenToId(string token) =>
        token.StartsWith("##")
            ? _suffixTokensToIds[token.AsSpan()[2..]]
            : _prefixTokensToIds[token];


    /// <summary>
    /// Tokenize text into word pieces
    /// </summary>
    public IEnumerable<Token> Tokenize(string text) => _preTokenizer.Split(text).SelectMany(w => TokenizeWord(w, text));


    /// <summary>
    /// Split word into.. eh word pieces
    /// </summary>
    public IEnumerable<Token> TokenizeWord(Word word, ReadOnlySpan<char> text)
    {
        if (word.Start == word.End)
        {
            throw new ArgumentException("uh, should probably not feed this with empty words?");
        }

        var wordText = text[word.Start..word.End];

        // perfect match 
        if (_prefixTokensToIds.TryGetValue(wordText, out var id))
        {
            return [new Token(id, word.Start, word.End)];
        }

        // 10 is just a basic heuristic here... most words will probably fit and wont allocate unnecessary or need to reallocate more
        var tokens = new List<Token>(10);
        var index = 0;

        for (var i = Math.Min(wordText.Length, _longestPrefixLength - 1); i > 0; i--)
        {
            if (_prefixTokensToIds.TryGetValue(wordText[index..i], out var pieceId))
            {
                tokens.Add(new Token(pieceId, word.Start, word.Start + i));
                index = i;
                break;
            }
        }

        // no matching prefix found, give up
        if (tokens.Count == 0)
        {
            return [new Token(_unkTokenId, word.Start, word.End)];
        }

        while (index < wordText.Length)
        {
            var unk = true;
            for (var i = Math.Min(wordText.Length - index, _longestSuffixLength - 1); i > 0; i--)
            {
                if (_suffixTokensToIds.TryGetValue(wordText[index..(index + i)], out var pieceId))
                {
                    unk = false;
                    tokens.Add(new Token(pieceId, word.Start + index, word.Start + index + i));
                    index += i;
                    break;
                }
            }

            // if we stumble upon an unknown token in the middle of a word, the whole word becomes unknown
            if (unk)
            {
                return [new Token(_unkTokenId, word.Start, word.End)];
            }
        }

        return tokens;
    }
}