using Demarbit.Shared.Domain.Extensions;
using FluentAssertions;

namespace Demarbit.Shared.Domain.Tests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("Café au Lait", "cafe-au-lait")]
    [InlineData("Ürban Köln", "urban-koln")]
    [InlineData("  multiple   spaces  ", "multiple-spaces")]
    [InlineData("Special!@#Characters$%^", "special-characters")]
    [InlineData("Already-slugified", "already-slugified")]
    [InlineData("trailing---hyphens---", "trailing-hyphens")]
    [InlineData("---leading-hyphens", "leading-hyphens")]
    [InlineData("MiXeD CaSe", "mixed-case")]
    public void ToSlug_should_produce_expected_output(string input, string expected)
    {
        input.ToSlug().Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToSlug_should_return_empty_for_null_or_whitespace(string? input)
    {
        (input ?? "").ToSlug().Should().BeEmpty();
    }
}