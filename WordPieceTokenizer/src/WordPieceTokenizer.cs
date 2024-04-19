using System.Collections.Frozen;

namespace NerAnonymizer;

/// <summary>
/// Raw token, this is the output from the model
/// </summary>
public record struct Token(long Id, int Start, int End);


/// <summary>
/// Internal raw prediction with only score and index
/// </summary>
public record Prediction(string EntityGroup, double Score, int Start, int End);


/// <summary>
/// Word piece model for tokenizing using existing vocabularies
/// </summary>
public class WordPieceTokenizer
{
    private readonly FrozenDictionary<string, int> _prefixTokensToIds;
    private readonly FrozenDictionary<string, int> _suffixTokensToIds;

    private readonly string[] _idsToTokens;


    /// <summary>
    /// Create a new WordPiece model with a vocabulary
    /// </summary>
    /// <param name="vocabulary"></param>
    public WordPieceTokenizer(string vocabulary)
    {
        _idsToTokens = vocabulary.Split(["\n", "\r\n"], StringSplitOptions.None);
        _prefixTokensToIds = _idsToTokens.Select((t, i) => (t, i)).Where(o => !o.t.StartsWith("##")).Select(o => new KeyValuePair<string, int>(o.t, o.i)).ToFrozenDictionary();
        _suffixTokensToIds = _idsToTokens.Select((t, i) => (t, i)).Where(o => o.t.StartsWith("##")).Select(o => new KeyValuePair<string, int>(o.t[2..], o.i)).ToFrozenDictionary();
    }


    public IReadOnlyDictionary<string, int> GetVocab() => _prefixTokensToIds;

    public int GetVocabSize() => _idsToTokens.Length;

    public string? IdToToken(int id) => _idsToTokens[id];

    public int? TokenToId(string token) =>
        token.StartsWith("##")
            ? _suffixTokensToIds[token]
            : _prefixTokensToIds[token];


    /// <summary>
    /// Tokenize text into word pieces
    /// </summary>
    public IReadOnlyList<Token> Tokenize(ReadOnlySpan<char> text) => new PreTokenizer().Split(text).SelectMany(TokenizeWord).ToList();


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

        // start chopping the word into longest possible prefix from vocabulary
        for (int i = word.Text.Length; i > 0; i--)
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
            return [new Token(_prefixTokensToIds["[UNK]"], word.Start, word.End)];
        }

        while (index < word.Text.Length)
        {
            var unk = true;
            for (int i = word.Text.Length - index; i > 0; i--)
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
                return [new Token(_prefixTokensToIds["[UNK]"], word.Start, word.End)];
            }
        }

        return tokens.Count != 0
            ? tokens
            : [new Token(_prefixTokensToIds["[UNK]"], word.Start, word.End)];
    }
}
