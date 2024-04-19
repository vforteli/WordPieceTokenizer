using NerAnonymizer;

namespace NerAnonymizerTests;

public class Tests
{
    [Test]
    public void TestPreTokenizer_Split()
    {
        var text = "lol. wat";

        var tokens = new PreTokenizer().Split(text).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(tokens, Has.Count.EqualTo(3));
            Assert.That(tokens[0].Text, Is.EqualTo("lol"));
            Assert.That(tokens[1].Text, Is.EqualTo("."));
            Assert.That(tokens[2].Text, Is.EqualTo("wat"));
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
            Assert.That(tokens[0].Text, Is.EqualTo("."));
            Assert.That(tokens[1].Text, Is.EqualTo("."));
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
            Assert.That(tokens[0].Text, Is.EqualTo("bla"));
            Assert.That(tokens[1].Text, Is.EqualTo("."));
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
            Assert.That(tokens[0].Text, Is.EqualTo("("));
            Assert.That(tokens[1].Text, Is.EqualTo("bla"));
            Assert.That(tokens[2].Text, Is.EqualTo(")"));
        });
    }
}
