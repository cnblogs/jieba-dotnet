using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using jieba.NET;
using JiebaNet.Segmenter;
using Xunit;

namespace Test
{
    public class StopwordsTests
    {
        private readonly Stopwords _stopwords = Stopwords.Instance;

        [Fact]
        public void Stopwords_loading_test()
        {
            _stopwords.Dictionary.Count().Should().BeGreaterThan(10);
            _stopwords.Dictionary.Remove("this");
            _stopwords.Dictionary.ContainsKey("this").Should().BeFalse();
        }

        [Fact]
        public void Stopwords_removing_test()
        {
            var words = new[] { "this", "as" };
            _stopwords.RemoveWords(words);

            foreach (var word in words)
            {
                _stopwords.Dictionary.ContainsKey(word).Should().BeFalse();
            }
        }

        [Fact]
        public void Stopwords_adding_test()
        {
            var word = "jieba";
            _stopwords.Dictionary.ContainsKey(word).Should().BeFalse();
            _stopwords.AddWords(word);
            _stopwords.Dictionary.ContainsKey(word).Should().BeTrue();
        }

        [Fact]
        public void Stopwords_AddAsciiSpecialChars_test()
        {
            _stopwords.AddAsciiSpecialChars();
            _stopwords.Dictionary.ContainsKey(".").Should().BeTrue();
            _stopwords.Dictionary.ContainsKey("c").Should().BeFalse();
        }

        [Fact]
        public void Stopwords_AddChineseSpecialChars_test()
        {
            _stopwords.AddChineseSpecialChars();
            _stopwords.Dictionary.ContainsKey("，").Should().BeTrue();
        }

        [Fact]
        public void Stopwords_RemoveEnglishWords_test()
        {
            var regexLetters = new Regex(@"^[a-zA-Z]+$");
            _stopwords.RemoveEnglishWords();
            _stopwords.Dictionary
                .Select(kv => kv.Key).Any(regexLetters.IsMatch)
                .Should().BeFalse();
            _stopwords.Dictionary.Count.Should().BeGreaterThan(10);
        }
    }
}
