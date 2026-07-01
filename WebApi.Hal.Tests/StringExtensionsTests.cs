#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace WebApi.Hal.Tests;

/// <summary>
/// Tests whether `StringExtensions.Replace` behaves identically to `string.Replace`.
/// </summary>
public class StringExtensionsTests
{
    // It's all AI from here.
    
    public static TheoryData<string, string, string?, StringComparison> ReplacementCases => new()
    {
        { "hello world", "world", "there", StringComparison.Ordinal },
        { "hello world", "missing", "there", StringComparison.Ordinal },
        { "hello hello hello", "hello", "hi", StringComparison.Ordinal },
        { "aaaa", "aa", "b", StringComparison.Ordinal },
        { "abababab", "ab", "x", StringComparison.Ordinal },
        { "abcabcabc", "abc", "", StringComparison.Ordinal },
        { "abcabcabc", "abc", null, StringComparison.Ordinal },
        { "abc", "a", null, StringComparison.Ordinal },
        { "abc", "b", null, StringComparison.Ordinal },
        { "abc", "c", null, StringComparison.Ordinal },
        { "abc", "z", null, StringComparison.Ordinal },

        { "", "a", "b", StringComparison.Ordinal },
        { "a", "a", "b", StringComparison.Ordinal },
        { "a", "A", "b", StringComparison.Ordinal },
        { "a", "A", "b", StringComparison.OrdinalIgnoreCase },
        { "ABC abc AbC", "abc", "x", StringComparison.OrdinalIgnoreCase },
        { "ABC abc AbC", "abc", "x", StringComparison.Ordinal },
        { "The Quick Brown Fox", "quick", "slow", StringComparison.OrdinalIgnoreCase },
        { "The Quick Brown Fox", "quick", "slow", StringComparison.Ordinal },

        { "foo.bar.foo", ".", "-", StringComparison.Ordinal },
        { "foo/bar/foo", "/", "\\", StringComparison.Ordinal },
        { "one\ntwo\nthree", "\n", "|", StringComparison.Ordinal },
        { "one\r\ntwo\r\nthree", "\r\n", "|", StringComparison.Ordinal },
        { "\tindent\tindent", "\t", "    ", StringComparison.Ordinal },
        { " spaced ", " ", "_", StringComparison.Ordinal },

        { "banana", "ana", "X", StringComparison.Ordinal },
        { "mississippi", "iss", "X", StringComparison.Ordinal },
        { "aaaaa", "aa", "X", StringComparison.Ordinal },
        { "11111", "11", "X", StringComparison.Ordinal },
        { "abcabc", "bcab", "X", StringComparison.Ordinal },

        { "résumé RESUME", "résumé", "cv", StringComparison.CurrentCulture },
        { "résumé RESUME", "résumé", "cv", StringComparison.CurrentCultureIgnoreCase },
        { "résumé RESUME", "résumé", "cv", StringComparison.InvariantCulture },
        { "résumé RESUME", "résumé", "cv", StringComparison.InvariantCultureIgnoreCase },

        { "straße STRASSE", "straße", "road", StringComparison.CurrentCulture },
        { "straße STRASSE", "straße", "road", StringComparison.CurrentCultureIgnoreCase },
        { "straße STRASSE", "straße", "road", StringComparison.InvariantCulture },
        { "straße STRASSE", "straße", "road", StringComparison.InvariantCultureIgnoreCase },

        { "İstanbul istanbul", "istanbul", "x", StringComparison.CurrentCulture },
        { "İstanbul istanbul", "istanbul", "x", StringComparison.CurrentCultureIgnoreCase },
        { "İstanbul istanbul", "istanbul", "x", StringComparison.InvariantCulture },
        { "İstanbul istanbul", "istanbul", "x", StringComparison.InvariantCultureIgnoreCase },

        { "emoji 😀 emoji 😀", "😀", ":)", StringComparison.Ordinal },
        { "symbols !@#$%^&*()", "!@#", "X", StringComparison.Ordinal },
        { "中文中文", "中文", "x", StringComparison.Ordinal },
        { "добро добро", "добро", "x", StringComparison.Ordinal },
        { "العربية العربية", "العربية", "x", StringComparison.Ordinal },
    };

    public static TheoryData<string, string, string?, StringComparison> AllStringComparisonCases
    {
        get
        {
            var data = new TheoryData<string, string, string?, StringComparison>();

            foreach (var comparison in Enum.GetValues<StringComparison>())
            {
                data.Add("hello HELLO hello", "hello", "hi", comparison);
                data.Add("abc ABC Abc aBc", "abc", "x", comparison);
                data.Add("nothing to replace", "zzz", "x", comparison);
                data.Add("aaaa", "aa", "b", comparison);
                data.Add("banana", "ana", "x", comparison);
                data.Add("prefix-middle-suffix", "-", "_", comparison);
                data.Add("CASE case Case", "case", null, comparison);
                data.Add("", "x", "y", comparison);
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(ReplacementCases))]
    [MemberData(nameof(AllStringComparisonCases))]
    public void Replace_matches_string_replace(
        string input,
        string oldValue,
        string? newValue,
        StringComparison comparison)
    {
#pragma warning disable CS0618
        var actual = WebApi.Hal.StringExtensions.Replace(input, oldValue, newValue!, comparison);
#pragma warning restore CS0618

        var expected = input.Replace(oldValue, newValue, comparison);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    [InlineData(StringComparison.InvariantCulture)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase)]
    public void Replace_matches_string_replace_when_replacement_is_longer_than_old_value(
        StringComparison comparison)
    {
        var input = "a b c b a";
        var oldValue = "b";
        string? newValue = "[replacement]";

#pragma warning disable CS0618
        var actual = WebApi.Hal.StringExtensions.Replace(input, oldValue, newValue!, comparison);
#pragma warning restore CS0618

        var expected = input.Replace(oldValue, newValue, comparison);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    [InlineData(StringComparison.InvariantCulture)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase)]
    public void Replace_matches_string_replace_when_replacement_contains_old_value(
        StringComparison comparison)
    {
        var input = "aaa";
        var oldValue = "a";
        string? newValue = "aa";

#pragma warning disable CS0618
        var actual = WebApi.Hal.StringExtensions.Replace(input, oldValue, newValue!, comparison);
#pragma warning restore CS0618

        var expected = input.Replace(oldValue, newValue, comparison);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Replace_matches_string_replace_for_null_receiver_exception()
    {
        string input = null!;
        var oldValue = "x";
        string? newValue = "y";
        var comparison = StringComparison.Ordinal;

#pragma warning disable CS0618
        var actual = Record.Exception(() =>
            WebApi.Hal.StringExtensions.Replace(input, oldValue, newValue!, comparison));
#pragma warning restore CS0618

        var expected = Record.Exception(() =>
            input.Replace(oldValue, newValue, comparison));

        Assert.NotNull(actual);
        Assert.NotNull(expected);
        Assert.Equal(expected.GetType(), actual.GetType());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Replace_matches_string_replace_for_invalid_old_value_exception(string? oldValue)
    {
        var input = "abc";
        string? newValue = "x";
        var comparison = StringComparison.Ordinal;

#pragma warning disable CS0618
        var actual = Record.Exception(() =>
            WebApi.Hal.StringExtensions.Replace(input, oldValue!, newValue!, comparison));
#pragma warning restore CS0618

        var expected = Record.Exception(() =>
            input.Replace(oldValue!, newValue, comparison));

        Assert.NotNull(actual);
        Assert.NotNull(expected);
        Assert.Equal(expected.GetType(), actual.GetType());
    }

    [Fact]
    public void Replace_matches_string_replace_for_invalid_string_comparison_exception()
    {
        var input = "abc";
        var oldValue = "a";
        string? newValue = "x";
        var comparison = (StringComparison)999;

#pragma warning disable CS0618
        var actual = Record.Exception(() =>
            WebApi.Hal.StringExtensions.Replace(input, oldValue, newValue!, comparison));
#pragma warning restore CS0618

        var expected = Record.Exception(() =>
            input.Replace(oldValue, newValue, comparison));

        Assert.NotNull(actual);
        Assert.NotNull(expected);
        Assert.Equal(expected.GetType(), actual.GetType());
    }

    [Fact]
    public void Replace_matches_string_replace_under_turkish_culture()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
            CultureInfo.CurrentUICulture = new CultureInfo("tr-TR");

            var input = "I i İ ı";
            var oldValue = "i";
            string? newValue = "x";
            var comparison = StringComparison.CurrentCultureIgnoreCase;

#pragma warning disable CS0618
            var actual = WebApi.Hal.StringExtensions.Replace(input, oldValue, newValue!, comparison);
#pragma warning restore CS0618

            var expected = input.Replace(oldValue, newValue, comparison);

            Assert.Equal(expected, actual);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    [Fact]
    public void Replace_matches_string_replace_under_german_culture()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            CultureInfo.CurrentUICulture = new CultureInfo("de-DE");

            var input = "straße STRASSE Strasse";
            var oldValue = "strasse";
            string? newValue = "road";
            var comparison = StringComparison.CurrentCultureIgnoreCase;

#pragma warning disable CS0618
            var actual = WebApi.Hal.StringExtensions.Replace(input, oldValue, newValue!, comparison);
#pragma warning restore CS0618

            var expected = input.Replace(oldValue, newValue, comparison);

            Assert.Equal(expected, actual);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    [Fact]
    public void Replace_matches_string_replace_for_many_generated_ordinal_cases()
    {
        var inputs = new[]
        {
            "",
            "a",
            "aa",
            "aaa",
            "aaaa",
            "abc",
            "abcabc",
            "ABCabcABC",
            "hello world hello",
            "one two three two one",
            "----",
            "😀😀😀",
            "line1\nline2\nline3",
        };

        var oldValues = new[]
        {
            "a",
            "aa",
            "abc",
            "ABC",
            "hello",
            "two",
            "-",
            "😀",
            "\n",
            "missing",
        };

        string?[] newValues =
        {
            null,
            "",
            "x",
            "replacement",
            "a",
            "aa",
            "😀",
            "\n",
        };

        foreach (var input in inputs)
        foreach (var oldValue in oldValues)
        foreach (var newValue in newValues)
        {
#pragma warning disable CS0618
            var actual = WebApi.Hal.StringExtensions.Replace(
                input,
                oldValue,
                newValue!,
                StringComparison.Ordinal);
#pragma warning restore CS0618

            var expected = input.Replace(
                oldValue,
                newValue,
                StringComparison.Ordinal);

            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void Replace_matches_string_replace_for_randomized_cases()
    {
        var random = new Random(12345);
        var alphabet = new[] { "a", "b", "c", "A", "B", "C", " ", "-", "_" };
        string?[] replacements = { null, "", "x", "XX", "a", "abc", "-" };

        for (var i = 0; i < 1_000; i++)
        {
            var input = CreateRandomString(random, alphabet, maxLength: 50);

            var oldValue = random.Next(0, 4) switch
            {
                0 => "a",
                1 => "A",
                2 => "ab",
                _ => alphabet[random.Next(alphabet.Length)]
            };

            var newValue = replacements[random.Next(replacements.Length)];
            var comparison = random.Next(2) == 0
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

#pragma warning disable CS0618
            var actual = WebApi.Hal.StringExtensions.Replace(
                input,
                oldValue,
                newValue!,
                comparison);
#pragma warning restore CS0618

            var expected = input.Replace(
                oldValue,
                newValue,
                comparison);

            Assert.Equal(expected, actual);
        }
    }

    private static string CreateRandomString(
        Random random,
        IReadOnlyList<string> alphabet,
        int maxLength)
    {
        var length = random.Next(maxLength + 1);
        var parts = new string[length];

        for (var i = 0; i < length; i++)
        {
            parts[i] = alphabet[random.Next(alphabet.Count)];
        }

        return string.Concat(parts);
    }
}