﻿#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

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
