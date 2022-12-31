namespace VRT.Downloaders.Tests;

public class StringExtensionsTests
{
    [Theory()]
    [InlineData(@"ąęłćźćĄĄŹĆÓó.txt", @"ąęłćźćĄĄŹĆÓó.txt")]
    [InlineData(@" Wyjaśnione 🔴 15 Zagrożeń.mp4", @"Wyjaśnione __ 15 Zagrożeń.mp4")]    
    public void SanitizeAsFileNameTests(string fileName, string expectedFileName)
    {
        var result = fileName.SanitizeAsFileName();
        result.Should().Be(expectedFileName);
    }
}