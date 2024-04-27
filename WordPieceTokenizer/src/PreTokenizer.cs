namespace vforteli.WordPieceTokenizer;

public class PreTokenizer
{
    /// <summary>
    /// Split text into words according to.. and.. um.. bertish
    /// </summary>
    public IEnumerable<Word> Split(string text)
    {
        var insideWord = false;
        var currentWordStartIndex = 0;
        var i = 0;

        foreach (var c in text)
        {
            if (char.IsPunctuation(c))
            {
                if (insideWord)
                {
                    yield return new Word(text[currentWordStartIndex..i], currentWordStartIndex, i);
                }

                yield return new Word(text[i..(i + 1)].ToString(), i, i + 1);
                insideWord = false;
            }
            else if (char.IsWhiteSpace(c))
            {
                if (insideWord)
                {
                    insideWord = false;
                    yield return new Word(text[currentWordStartIndex..i], currentWordStartIndex, i);
                }
            }
            else
            {
                if (!insideWord)
                {
                    currentWordStartIndex = i;
                }

                insideWord = true;
            }

            i++;
        }

        if (insideWord)
        {
            yield return new Word(text[currentWordStartIndex..text.Length], currentWordStartIndex, text.Length);
        }
    }
}
