using System.Collections.Frozen;

namespace vforteli.WordPieceTokenizer;

/// <summary>
/// WordPieceTokenizer
/// </summary>
public class WordPieceTokenizer
{
    private readonly FrozenDictionary<string, int> _prefixTokensToIds;
    private readonly FrozenDictionary<string, int> _suffixTokensToIds;
    private readonly int _longestPrefixLength;
    private readonly int _longestSuffixLength;

    private readonly PreTokenizer _preTokenizer;

    private readonly string[] _idsToTokens;
    private readonly int _unkTokenId;

    /// <summary>
    /// Create a new WordPieceTokenizer with a vocabulary and specified UNK token
    /// </summary>    
    public WordPieceTokenizer(string vocabulary, string unkToken)
    {
        _idsToTokens = vocabulary.Split(["\n", "\r\n"], StringSplitOptions.None);
        _prefixTokensToIds = _idsToTokens.Select((t, i) => (t, i)).Where(o => !o.t.StartsWith("##")).Select(o => new KeyValuePair<string, int>(o.t, o.i)).ToFrozenDictionary();
        _suffixTokensToIds = _idsToTokens.Select((t, i) => (t, i)).Where(o => o.t.StartsWith("##")).Select(o => new KeyValuePair<string, int>(o.t[2..], o.i)).ToFrozenDictionary();
        _unkTokenId = _prefixTokensToIds[unkToken];

        _longestPrefixLength = _prefixTokensToIds.Max(o => o.Key.Length);
        _longestSuffixLength = _suffixTokensToIds.Max(o => o.Key.Length);
        _preTokenizer = new PreTokenizer();
    }

    /// <summary>
    /// Create a new WordPieceTokenizer with a vocabulary default [UNK] token
    /// </summary>    
    public WordPieceTokenizer(string vocabulary) : this(vocabulary, "[UNK]") { }

    public string? IdToToken(int id) => _idsToTokens[id];

    public int? TokenToId(string token) =>
        token.StartsWith("##")
            ? _suffixTokensToIds[token[2..]]
            : _prefixTokensToIds[token];


    /// <summary>
    /// Tokenize text into word pieces
    /// </summary>
    public IEnumerable<Token> Tokenize(string text) => _preTokenizer.Split(text).SelectMany(TokenizeWord);


    /// <summary>
    /// Split word into.. eh word pieces
    /// </summary>
    public IEnumerable<Token> TokenizeWord(Word word)
    {
        if (word.Start == word.End)
        {
            throw new ArgumentException("uh, should probably not feed this with empty words?");
        }

        // perfect match 
        if (_prefixTokensToIds.TryGetValue(word.Text, out var id))
        {
            return [new Token(id, word.Start, word.End)];
        }

        var tokens = new List<Token>();
        var index = 0;

        for (int i = Math.Min(word.Text.Length, _longestPrefixLength - 1); i > 0; i--)
        {
            if (_prefixTokensToIds.TryGetValue(word.Text[index..i], out var pieceId))
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

        while (index < word.Text.Length)
        {
            var unk = true;
            for (int i = Math.Min(word.Text.Length - index, _longestSuffixLength - 1); i > 0; i--)
            {
                if (_suffixTokensToIds.TryGetValue(word.Text[index..(index + i)], out var pieceId))
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
