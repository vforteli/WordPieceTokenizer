#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace vforteli.WordPieceTokenizer;

public class WordPieceTokenizerTests
{
    private readonly WordPieceTokenizer _tokenizer = new WordPieceTokenizer(VocabStrings.Vocab);

    [Test]
    public async Task TokenToId()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_tokenizer.TokenToId("ta"), Is.EqualTo(153));
            Assert.That(_tokenizer.TokenToId("##mm"), Is.EqualTo(169));
            Assert.That(() => _tokenizer.TokenToId("doesntexist"), Throws.InstanceOf<KeyNotFoundException>());
        });
    }

    [Test]
    public async Task IdToToken()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_tokenizer.IdToToken(153), Is.EqualTo("ta"));
            Assert.That(_tokenizer.IdToToken(169), Is.EqualTo("##mm"));
            Assert.That(() => _tokenizer.IdToToken(int.MaxValue), Throws.InstanceOf<IndexOutOfRangeException>());
        });
    }

    [Test]
    public async Task TokenizeWord_wtf()
    {
        var text = "wtf";
        var word = new Word(text, 0, text.Length);

        var tokens = _tokenizer.TokenizeWord(word).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(3));

            Assert.That(tokens[0].Id, Is.EqualTo(1349));
            Assert.That(tokens[0].Start, Is.EqualTo(0));
            Assert.That(tokens[0].End, Is.EqualTo(1));

            Assert.That(tokens[1].Id, Is.EqualTo(50008));
            Assert.That(tokens[1].Start, Is.EqualTo(1));
            Assert.That(tokens[1].End, Is.EqualTo(2));

            Assert.That(tokens[2].Id, Is.EqualTo(50050));
            Assert.That(tokens[2].Start, Is.EqualTo(2));
            Assert.That(tokens[2].End, Is.EqualTo(3));
        });
    }

    [Test]
    public async Task TokenizeWord_long()
    {
        var text = "wtf kokemuksistaankö wtf";

        var tokens = _tokenizer.Tokenize(text).ToList();
        var expected = "w, ##t, ##f, kokemuksistaan, ##kö, w, ##t, ##f";

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(8));
            Assert.That(string.Join(", ", tokens.Select(o => _tokenizer.IdToToken(o.Id))), Is.EqualTo(expected));
        });
    }

    [Test]
    public async Task TokenizeWord_long_2()
    {
        var text = "kokemuksistaan";

        var tokens = _tokenizer.Tokenize(text).ToList();
        var expected = "kokemuksistaan";

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(1));
            Assert.That(string.Join(", ", tokens.Select(o => _tokenizer.IdToToken(o.Id))), Is.EqualTo(expected));
        });
    }

    [Test]
    public async Task TokenizeWord_sentence()
    {
        var text = "onko pallo vai kalakukko";

        var tokens = _tokenizer.Tokenize(text).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(6));

            Assert.That(tokens[0].Id, Is.EqualTo(1435));
            Assert.That(tokens[0].Start, Is.EqualTo(0));
            Assert.That(tokens[0].End, Is.EqualTo(4));

            Assert.That(tokens[1].Id, Is.EqualTo(12371));
            Assert.That(tokens[1].Start, Is.EqualTo(5));
            Assert.That(tokens[1].End, Is.EqualTo(10));

            Assert.That(tokens[2].Id, Is.EqualTo(1317));
            Assert.That(tokens[2].Start, Is.EqualTo(11));
            Assert.That(tokens[2].End, Is.EqualTo(14));

            Assert.That(tokens[3].Id, Is.EqualTo(7445));
            Assert.That(tokens[3].Start, Is.EqualTo(15));
            Assert.That(tokens[3].End, Is.EqualTo(19));

            Assert.That(tokens[4].Id, Is.EqualTo(225));
            Assert.That(tokens[4].Start, Is.EqualTo(19));
            Assert.That(tokens[4].End, Is.EqualTo(21));

            Assert.That(tokens[5].Id, Is.EqualTo(760));
            Assert.That(tokens[5].Start, Is.EqualTo(21));
            Assert.That(tokens[5].End, Is.EqualTo(24));
        });
    }

    [Test]
    public async Task TokenizeWord_wtf_with_offset()
    {
        var text = "wtf";
        var word = new Word(text, 10, text.Length);

        var tokens = _tokenizer.TokenizeWord(word).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(3));

            Assert.That(tokens[0].Id, Is.EqualTo(1349));
            Assert.That(tokens[0].Start, Is.EqualTo(10));
            Assert.That(tokens[0].End, Is.EqualTo(11));

            Assert.That(tokens[1].Id, Is.EqualTo(50008));
            Assert.That(tokens[1].Start, Is.EqualTo(11));
            Assert.That(tokens[1].End, Is.EqualTo(12));

            Assert.That(tokens[2].Id, Is.EqualTo(50050));
            Assert.That(tokens[2].Start, Is.EqualTo(12));
            Assert.That(tokens[2].End, Is.EqualTo(13));
        });
    }

    [Test]
    public async Task TokenizeWord_SingleUNK()
    {
        var text = "^";
        var word = new Word(text, 0, text.Length);

        var token = _tokenizer.TokenizeWord(word).Single();

        Assert.Multiple(() =>
        {
            Assert.That(token.Id, Is.EqualTo(_tokenizer.TokenToId("[UNK]")));
            Assert.That(token.Start, Is.EqualTo(0));
            Assert.That(token.End, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task TokenizeWord_ContainUNK()
    {
        var text = "wt^";
        var word = new Word(text, 0, text.Length);

        var token = _tokenizer.TokenizeWord(word).Single();

        Assert.Multiple(() =>
        {
            Assert.That(token.Id, Is.EqualTo(_tokenizer.TokenToId("[UNK]")));
            Assert.That(token.Start, Is.EqualTo(0));
            Assert.That(token.End, Is.EqualTo(3));
        });
    }
}
