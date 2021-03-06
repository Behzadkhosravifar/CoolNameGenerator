﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CoolNameGenerator.GeneticWordProcessing;

namespace CoolNameGenerator.Helper
{
    /// <summary>String extensions.</summary>
    public static class StringExtensions
    {
        private static readonly Regex SInsertUnderscoreBeforeUppercase = new Regex("(?<!_|^)([A-Z])",
            RegexOptions.Compiled);

        private static readonly Regex SCapitalizeRegex = new Regex("((\\s|^)\\S)(\\S+)", RegexOptions.Compiled);

        /// <summary>Gets the word in the specified index.</summary>
        /// <returns>The word from index.</returns>
        /// <param name="source">The source string.</param>
        /// <param name="index">The index.</param>
        public static string GetWordFromIndex(this string source, int index)
        {
            int wordStartIndex;
            return source.GetWordFromIndex(index, out wordStartIndex);
        }

        /// <summary>Gets the word in the specified index.</summary>
        /// <returns>The word from index.</returns>
        /// <param name="source">The source string.</param>
        /// <param name="index">The index.</param>
        /// <param name="wordStartIndex">Word start index.</param>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters",
            Justification = "It's necessary for the method purpose.", MessageId = "2#")]
        public static string GetWordFromIndex(this string source, int index, out int wordStartIndex)
        {
            var strArray = source.Split(' ');
            var length = strArray.Length;
            var num1 = -1;
            for (var index1 = 0; index1 < length; ++index1)
            {
                var str = strArray[index1];
                var num2 = num1 + str.Length;
                if (num2 >= index)
                {
                    wordStartIndex = num2 - (str.Length - 1);
                    return str;
                }
                num1 = num2 + 1;
                if (num1 == index)
                {
                    wordStartIndex = num1;
                    return " ";
                }
            }
            wordStartIndex = 0;
            return source;
        }

        /// <summary>Counts the words.</summary>
        /// <returns>The words.</returns>
        /// <param name="source">The source string.</param>
        public static int CountWords(this string source)
        {
            return source.Split(new char[1] {' '}, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>Removes the accents.</summary>
        /// <returns>The accents.</returns>
        /// <param name="source">The source string.</param>
        public static string RemoveAccents(this string source)
        {
            var str = source.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            for (var index = 0; index < str.Length; ++index)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(str[index]) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(str[index]);
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>Removes the non alphanumeric chars.</summary>
        /// <returns>The non alphanumeric.</returns>
        /// <param name="source">The source string.</param>
        public static string RemoveNonAlphanumeric(this string source)
        {
            return Regex.Replace(source, "[^0-9A-Za-záàãâäéèêëíìîïóòõôöúùûüñ]*", string.Empty);
        }

        /// <summary>Removes the non numeric chars.</summary>
        /// <returns>The non numeric.</returns>
        /// <param name="source">The source string.</param>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Ok.",
            MessageId = "NonNumeric")]
        public static string RemoveNonNumeric(this string source)
        {
            return Regex.Replace(source, "[^0-9]*", string.Empty);
        }

        /// <summary>Removes the specified string from borders.</summary>
        /// <returns>The from borders.</returns>
        /// <param name="source">The source string.</param>
        /// <param name="remove">The string to remove.</param>
        public static string RemoveFromBorders(this string source, string remove)
        {
            remove = Regex.Escape(remove);
            return Regex.Replace(source,
                string.Format(CultureInfo.InvariantCulture, "(^{0}|{1}$)", (object) remove, (object) remove),
                string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>Removes the chars "remove" from source borders.</summary>
        /// <returns>The from borders.</returns>
        /// <param name="source">The source string.</param>
        /// <param name="remove">The chars to remove.</param>
        public static string RemoveFromBorders(this string source, params char[] remove)
        {
            var str = Regex.Escape(new string(remove));
            return Regex.Replace(source,
                string.Format(CultureInfo.InvariantCulture, "(^[{0}]|[{1}]$)", (object) str, (object) str), string.Empty);
        }

        /// <summary>Removes the punctuations.</summary>
        /// <returns>The clean string.</returns>
        /// <param name="source">The source string.</param>
        public static string RemovePunctuations(this string source)
        {
            return Regex.Replace(source, "[!\\(\\)\\[\\]{}\\:;\\.,?'\"]*", string.Empty);
        }

        /// <summary>Escapes the accents to hexadecimal equivalent.</summary>
        /// <returns>The accents to hex.</returns>
        /// <param name="source">The source string.</param>
        public static string EscapeAccentsToHex(this string source)
        {
            var stringBuilder = new StringBuilder(source.Length);
            var length = source.Length;
            for (var index = 0; index < length; ++index)
            {
                if (source[index].HasAccent())
                    stringBuilder.Append(Uri.HexEscape(source[index]));
                else
                    stringBuilder.Append(source[index]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>Escapes the accents to html entities.</summary>
        /// <returns>The accents to html entities.</returns>
        /// <param name="source">The source string.</param>
        public static string EscapeAccentsToHtmlEntities(this string source)
        {
            var length = source.Length;
            var stringBuilder = new StringBuilder();
            for (var index = 0; index < length; ++index)
            {
                var ch = source[index];
                if (ch >= 160 && ch < 256)
                    stringBuilder.AppendFormat("&#{0};", ((int) ch).ToString(NumberFormatInfo.InvariantInfo));
                else
                    stringBuilder.Append(ch);
            }
            return stringBuilder.ToString();
        }

        /// <summary>Verify if source ends the with punctuation.</summary>
        /// <returns><c>true</c>, if with punctuation was the end, <c>false</c> otherwise.</returns>
        /// <param name="source">The source string.</param>
        public static bool EndsWithPunctuation(this string source)
        {
            return Regex.IsMatch(source, "[!\\(\\)\\[\\]{}\\:;\\.,?'String.Empty]$");
        }

        /// <summary>Determines if has accent.</summary>
        /// <returns><c>true</c> if has accent the specified source; otherwise, <c>false</c>.</returns>
        /// <param name="source">The source string.</param>
        public static bool HasAccent(this string source)
        {
            var length = source.Length;
            for (var index = 0; index < length; ++index)
            {
                if (source[index].HasAccent())
                    return true;
            }
            return false;
        }

        /// <summary>Inserts the underscore before every upper case char.</summary>
        /// <returns>The result string.</returns>
        /// <param name="input">The source string.</param>
        public static string InsertUnderscoreBeforeUppercase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return SInsertUnderscoreBeforeUppercase.Replace(input, "_$1");
        }

        /// <summary>
        ///     Format the specified string. Is a String.Format(CultureInfo.InvariantCulture,..) shortcut.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The formatted string.</returns>
        public static string With(this string source, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, source, args);
        }

        /// <summary>Capitalize the string.</summary>
        /// <param name="source">The source string.</param>
        /// <param name="ignoreWordsLowerThanChars">The words lower than specified chars will be ignored.</param>
        /// <returns>The capitalized string.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public static string Capitalize(this string source, int ignoreWordsLowerThanChars = 3)
        {
            return SCapitalizeRegex.Replace(source.ToLowerInvariant(), m =>
            {
                if (m.Value.Trim().Length < ignoreWordsLowerThanChars)
                    return m.Value;
                return m.Groups[1].Value.ToUpperInvariant() + m.Groups[3].Value;
            });
        }

        /// <summary>
        ///     Returns a value indicating whether the specified substring occurs within this string
        ///     <remarks>
        ///         Based on http://stackoverflow.com/a/444818/956886.
        ///     </remarks>
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="substring">The substring.</param>
        /// <returns>True se if substring occurs, otherwise false.</returns>
        public static bool ContainsIgnoreCase(this string source, string substring)
        {
            return source.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static Dictionary<string, string> GetPairUniqueWords(this IEnumerable<string> source)
        {
            var res = source.SelectMany(x => new string(x.Where(w => w != '\t' && w != ' ').ToArray())
                .Split(Words.NotIgnoreChars.Concat(Words.NumericLetters)
                    .ToArray())).Where(word => word.Length > 1).Distinct();

            var pairWords = res.Select(x => new
            {
                persian = x.Contains(':') ? x.Substring(x.IndexOf(':') + 1).ToLower() : x.ToLower(),
                finglish = x.Contains(':') ? x.Substring(0, x.IndexOf(':')).ToLower() : null
            }).SkipWhile(x => x.finglish?.Length < 3 || x.persian?.Length < 3);

            var uniqueWords =
                pairWords.Distinct((a, b) => a.persian == b.persian, c => c.persian.GetHashCode())
                    .ToDictionary(p => p.persian, p => p.finglish);

            return uniqueWords;
        }

        public static HashSet<string> GetUniqueWords(this IEnumerable<string> source)
        {
            var res = source.SelectMany(x => new string(x.Where(w => w != '\t' && w != ' ').ToArray())
                .Split(Words.NotIgnoreChars.Concat(Words.NumericLetters)
                    .ToArray())).Where(word => word.Length > 1).Distinct();

            var pairWords = res.Select(x => x.Contains(':') ? x.Substring(0, x.IndexOf(':')).ToLower() : x.ToLower());

            var uniqueWords = new HashSet<string>(pairWords.Distinct());

            return uniqueWords;
        }

        /// <summary>
        ///     Gets the sub words.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="includeMiddleSubWords">include middle sub words? by default is true.</param>
        /// <returns>Sub words.</returns>
        public static IList<string> GetSubWords(this string word, bool includeMiddleSubWords = true)
        {
            // word:    a b c d
            //          0 1 2 3
            //
            // sub words:        
            //          a b , a b c , a b c d
            //          0 1   0 1 2   0 1 2 3   
            //
            //          b c , b c d
            //          1 2   1 2 3
            //
            //          c d
            //          2 3
            //
            var subWords = new List<string>();

            for (var i = 0; i < word.Length - 1; i++)
            {
                for (var j = 2; j <= word.Length - i; j++)
                {
                    subWords.Add(word.Substring(i, j));
                }

                if (!includeMiddleSubWords) break; // just sub words which stated by the original word, first chars
            }

            return subWords;
        }

        /// <summary>
        ///     Gets the sub words.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="includeMiddleSubWords">include middle sub words? by default is true.</param>
        /// <returns>Sub words.</returns>
        public static IList<string> GetSubWords(this WordChromosome word, bool includeMiddleSubWords = true)
        {
            return word.ToString().GetSubWords();
        }

        /// <summary>
        ///     Gets the sub words.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="includeMiddleSubWords">include middle sub words? by default is true.</param>
        /// <returns>Sub words.</returns>
        public static Dictionary<string, double> GetSubWordsByCoverage(this string word,
            bool includeMiddleSubWords = true)
        {
            // word:    a b c d
            //          0 1 2 3
            //          
            // sub words:        
            //          a b ,    a b c ,     a b c d
            //          0 1      0 1 2       0 1 2 3   
            //          50%      75%         100%
            //
            //          b c ,    b c d
            //          1 2      1 2 3
            //          50%      75%
            //
            //          c d
            //          2 3
            //          50%
            //
            var subWords = new Dictionary<string, double>();
            var len = word.Length;
            for (var i = 0; i < len - 1; i++)
            {
                for (var j = 2; j <= len - i; j++)
                {
                    var wordBuffer = word.Substring(i, j);
                    subWords[wordBuffer] = (double) wordBuffer.Length/len;
                }

                if (!includeMiddleSubWords) break; // just sub words which stated by the original word, first chars
            }

            return subWords;
        }

        public static Dictionary<string, Dictionary<string, double>> GetWordsBySubWordsCoverage(
            this IEnumerable<string> words, bool includeMiddleSubWords = true)
        {
            var result = new Dictionary<string, Dictionary<string, double>>();
            foreach (var word in words)
            {
                result[word] = word.GetSubWordsByCoverage(includeMiddleSubWords);
            }

            return result;
        }

        public static Dictionary<string, double> GetUniqueSubWordsCoverage(this HashSet<string> words,
            bool includeMiddleSubWords = true)
        {
            var result = new Dictionary<string, double>();

            foreach (var word in words)
            {
                foreach (var subwordsCoverage in word.GetSubWordsByCoverage(includeMiddleSubWords))
                {
                    if (result.ContainsKey(subwordsCoverage.Key))
                    {
                        result[subwordsCoverage.Key] += subwordsCoverage.Value;
                    }
                    else
                    {
                        result[subwordsCoverage.Key] = subwordsCoverage.Value;
                    }
                }
            }

            return result;
        }

        public static int CountOverlap(this string word, IEnumerable<string> matchableWords)
        {
            var overlapCount = 0;
            //
            //          01234567
            // word:    abookeye
            // [book]:   1234
            // [okey]:     3456
            // [eye]:        567
            //
            // [book] overlapped by [okey] 
            // [okey] overlapped by [eye]
            //
            // overlapCount: 2
            //

            for (var i = 0; i < matchableWords.Count(); i++)
            {
                var match = matchableWords.ElementAt(i);
                var i1 = word.IndexOf(match, StringComparison.Ordinal);
                var i2 = i1 + match.Length - 1;

                for (var j = i + 1; j < matchableWords.Count(); j++)
                {
                    var oMatch = matchableWords.ElementAt(j);
                    var j1 = word.IndexOf(oMatch, StringComparison.Ordinal);
                    var j2 = j1 + oMatch.Length - 1;

                    //
                    //    j1------j2  
                    //        i1--------i2
                    //             j1------j2
                    //    j1------------------j2
                    //           j1---j2
                    //
                    if ((i1 <= j1 && i2 >= j1) ||
                        (i1 >= j1 && i1 <= j2))
                    {
                        overlapCount++;
                    }
                }
            }

            return overlapCount;
        }

        public static int CountOverlap(this WordChromosome word, IEnumerable<string> matchableWords)
        {
            return word.ToString().CountOverlap(matchableWords);
        }
    }
}