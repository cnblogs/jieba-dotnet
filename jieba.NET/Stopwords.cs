using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using JiebaNet.Segmenter.Common;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.Collections.Concurrent;

namespace JiebaNet.Segmenter
{
    public class Stopwords
    {
        private const string _stropwordsPath = "Resources/stopwords.txt";
        private static readonly string[] _chineseSpecialChars = new[]
        {
            "、", "，", "。", "；", "！", "￥", "（", "）", "【", "】", "‘", "’", "“", "”", "：", "？", "《", "》"
        };
        private static readonly Lazy<Stopwords> lazy = new Lazy<Stopwords>(() => new Stopwords());
        private readonly IDictionary<string, bool> _dict = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, bool> Dictionary => _dict;
        public static Stopwords Instance => lazy.Value;

        private Stopwords()
        {
            LoadDict();

            Debug.WriteLine("{0} stopwords have been loaded", _dict.Count);
        }

        public Stopwords AddWords(params string[] words)
        {
            if (words?.Length > 0)
            {
                foreach (var word in words)
                {
                    _dict.TryAdd(word, true); ;
                }
            }

            return this;
        }

        public Stopwords RemoveWords(params string[] words)
        {
            if (words?.Length > 0)
            {
                foreach (var word in words)
                {
                    _dict.Remove(word);
                }
            }

            return this;
        }

        public Stopwords AddAsciiSpecialChars()
        {
            foreach (char @char in Enumerable.Range('\x1', 127))
            {
                if (!char.IsLetterOrDigit(@char))
                {
                    AddWords(@char.ToString());
                }
            }

            return this;
        }

        public Stopwords AddChineseSpecialChars()
        {
            return AddWords(_chineseSpecialChars);
        }

        public Stopwords RemoveEnglishWords()
        {
            foreach (var k in _dict.Keys)
            {
                if (!k.Any(c => !char.IsLetter(c)))
                {
                    _dict.Remove(k);
                }
            }

            return this;
        }

        private void LoadDict()
        {
            var fileProvider = new EmbeddedFileProvider(GetType().GetTypeInfo().Assembly);
            var fileInfo = fileProvider.GetFileInfo(_stropwordsPath);

            using var reader = new StreamReader(fileInfo.CreateReadStream());
            var s = "";
            while ((s = reader.ReadLine()) != null)
            {
                s = s.Trim();
                if (string.IsNullOrWhiteSpace(s))
                    continue;
                if (_dict.ContainsKey(s))
                    continue;
                _dict.Add(s, true);
            }
        }
    }
}