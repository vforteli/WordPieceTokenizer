namespace vforteli.WordPieceTokenizer;

public class Tests
{
    private string GetWordFromWord(string text, Word word) => text[word.Start..word.End];

    [Test]
    public void TestPreTokenizer_Split()
    {
        var text = "lol. wat";

        var tokens = new PreTokenizer().Split(text).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(3));
            Assert.That(GetWordFromWord(text, tokens[0]), Is.EqualTo("lol"));
            Assert.That(GetWordFromWord(text, tokens[1]), Is.EqualTo("."));
            Assert.That(GetWordFromWord(text, tokens[2]), Is.EqualTo("wat"));
        });
    }

    [Test]
    public void TestPreTokenizer_Split_Scandinavian()
    {
        var text = "löl. wüt";

        var tokens = new PreTokenizer().Split(text).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(3));
            Assert.That(GetWordFromWord(text, tokens[0]), Is.EqualTo("löl"));
            Assert.That(GetWordFromWord(text, tokens[1]), Is.EqualTo("."));
            Assert.That(GetWordFromWord(text, tokens[2]), Is.EqualTo("wüt"));
        });
    }

    [Test]
    public void TestPreTokenizer_Split_dots()
    {
        var text = ". .";

        var tokens = new PreTokenizer().Split(text).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(2));
            Assert.That(GetWordFromWord(text, tokens[0]), Is.EqualTo("."));
            Assert.That(GetWordFromWord(text, tokens[1]), Is.EqualTo("."));
        });
    }

    [Test]
    public void TestPreTokenizer_Split_starting_whitespace()
    {
        var text = " bla. ";

        var tokens = new PreTokenizer().Split(text).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(2));
            Assert.That(GetWordFromWord(text, tokens[0]), Is.EqualTo("bla"));
            Assert.That(GetWordFromWord(text, tokens[1]), Is.EqualTo("."));
        });
    }

    [Test]
    public void TestPreTokenizer_Split_other_punctuation()
    {
        var text = "(bla)";

        var tokens = new PreTokenizer().Split(text).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(3));
            Assert.That(GetWordFromWord(text, tokens[0]), Is.EqualTo("("));
            Assert.That(GetWordFromWord(text, tokens[1]), Is.EqualTo("bla"));
            Assert.That(GetWordFromWord(text, tokens[2]), Is.EqualTo(")"));
        });
    }
}