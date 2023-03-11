using Cledev.OpenAI.Playground.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace Cledev.OpenAI.Playground.Tests;

public class FormatCodeTests
{
    [Test]
    public void GivenContentWithoutCode_ThenResultWithoutTagsIsReturned()
    {
        const string content = "Blah blah blah Foo foo";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah Foo foo");
    }

    [Test]
    public void GivenContentWithCode_WhenIncludesSingleValue_ThenResultWithOpenTagIsReturned()
    {
        const string content = "Blah blah blah ```Foo foo";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah <pre>Foo foo");
    }

    [Test]
    public void GivenContentWithCode_WhenIncludesTwoValues_ThenResultWithOpenAndCloseTagsIsReturned()
    {
        const string content = "Blah blah blah ```Foo foo``` blah blah blah";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah <pre>Foo foo</pre> blah blah blah");
    }

    [Test]
    public void GivenContentWithCode_WhenIncludesOddValues_ThenResultWithAnOddOpenTagIsReturned()
    {
        const string content = "Blah blah blah ```Foo foo``` blah ```blah blah";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah <pre>Foo foo</pre> blah <pre>blah blah");
    }

    [Test]
    public void GivenContentWithCode_WhenIncludesMultipleMatchingValues_ThenResultWithMultipleOpenAndCloseTagsIsReturned()
    {
        const string content = "Blah blah blah ```Foo foo``` blah ```blah``` blah";
        var actual = content.FormatCode();
        actual.Should().Be("Blah blah blah <pre>Foo foo</pre> blah <pre>blah</pre> blah");
    }
}
