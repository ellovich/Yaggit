using Models.Extensions;

namespace Yaggit.Test.Infrastructure
{
    [TestFixture]
    public class FormattingTests
    {
        // -----------------------
        // NormalizeName
        // -----------------------
        [Test]
        public void NormalizeName_ShouldReplaceSpacesAndRemoveInvalidChars()
        {
            var result = "User Name!@#".NormalizeName();
            Assert.That(result, Is.EqualTo("User_Name"));
        }

        [Test]
        public void NormalizeName_ShouldAddUnderscoreIfStartsWithDigit()
        {
            var result = "123Name".NormalizeName();
            Assert.That(result, Is.EqualTo("_123Name"));
        }

        [Test]
        public void NormalizeName_ShouldReturnEmptyString_WhenNull()
        {
            string? value = null;
            var result = value.NormalizeName();
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NormalizeName_ShouldReturnUnderscore_WhenAllInvalid()
        {
            var result = "!!!".NormalizeName();
            Assert.That(result, Is.EqualTo("_"));
        }

        // -----------------------
        // ToPascalCase
        // -----------------------
        [TestCase("user_name", ExpectedResult = "UserName")]
        [TestCase("User Name", ExpectedResult = "UserName")]
        [TestCase("user-name", ExpectedResult = "UserName")]
        [TestCase("  multi_word_value ", ExpectedResult = "MultiWordValue")]
        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        public string ToPascalCase_ShouldFormatProperly(string? input)
            => input.ToPascalCase();

        // -----------------------
        // ToLowerFirstLetter / ToUpperFirstLetter
        // -----------------------
        [Test]
        public void ToLowerFirstLetter_ShouldConvertOnlyFirstChar()
        {
            var result = "UserName".ToLowerFirstLetter();
            Assert.That(result, Is.EqualTo("userName"));
        }

        [Test]
        public void ToLowerFirstLetter_ShouldHandleNullOrEmpty()
        {
            string? s1 = null;
            string s2 = "";
            Assert.That(s1.ToLowerFirstLetter(), Is.EqualTo(""));
            Assert.That(s2.ToLowerFirstLetter(), Is.EqualTo(""));
        }

        [Test]
        public void ToUpperFirstLetter_ShouldConvertOnlyFirstChar()
        {
            var result = "userName".ToUpperFirstLetter();
            Assert.That(result, Is.EqualTo("UserName"));
        }

        [Test]
        public void ToUpperFirstLetter_ShouldHandleEmpty()
        {
            var result = "".ToUpperFirstLetter();
            Assert.That(result, Is.EqualTo(""));
        }

        // -----------------------
        // GetPrefixFromPascalCase
        // -----------------------
        [TestCase("UserName", ExpectedResult = "User")]
        [TestCase("OrderId", ExpectedResult = "Order")]
        [TestCase("X", ExpectedResult = "X")]
        [TestCase("lowercase", ExpectedResult = "lowercase")]
        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        public string GetPrefixFromPascalCase_ShouldExtractPrefix(string? input)
            => input.GetPrefixFromPascalCase();

        // -----------------------
        // ReplaceFirst
        // -----------------------
        [Test]
        public void ReplaceFirst_ShouldReplaceOnlyFirstOccurrence()
        {
            var result = "apple_apple_apple".ReplaceFirst("apple", "pear");
            Assert.That(result, Is.EqualTo("pear_apple_apple"));
        }

        [Test]
        public void ReplaceFirst_ShouldReturnOriginal_WhenNoMatch()
        {
            var result = "banana".ReplaceFirst("apple", "pear");
            Assert.That(result, Is.EqualTo("banana"));
        }

        [Test]
        public void ReplaceFirst_ShouldHandleNull()
        {
            string? text = null;
            var result = text.ReplaceFirst("x", "y");
            Assert.That(result, Is.EqualTo(""));
        }

        // -----------------------
        // SplitPascalCase
        // -----------------------
        [TestCase("UserName", ExpectedResult = "User Name")]
        [TestCase("OrderId", ExpectedResult = "Order Id")]
        [TestCase("XMLHttpRequest", ExpectedResult = "XMLHttp Request")]
        [TestCase("lowercase", ExpectedResult = "lowercase")]
        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        public string SplitPascalCase_ShouldInsertSpaces(string? input)
            => input.SplitPascalCase();

        // -----------------------
        // IsValidIdentifier
        // -----------------------
        [TestCase("ValidName", ExpectedResult = true)]
        [TestCase("_privateVar", ExpectedResult = true)]
        [TestCase("123abc", ExpectedResult = false)]
        [TestCase("space name", ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase(null, ExpectedResult = false)]
        [TestCase("snake_case", ExpectedResult = true)]
        [TestCase("CamelCase", ExpectedResult = true)]
        public bool IsValidIdentifier_ShouldValidateCorrectly(string? input)
            => input.IsValidIdentifier();
    }
}
