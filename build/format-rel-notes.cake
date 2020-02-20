using System.Text.RegularExpressions;

string FormatReleaseNotes(string text) {
    if (string.IsNullOrEmpty(text)) throw new Exception("Release notes are empty.");

    return MakeSentence(RemoveIncrementFlag(text).Trim());
}

string RemoveIncrementFlag(string text) =>
    Regex.Replace(text, @"\+semver:\s?(breaking|major|feature|minor|fix|patch|none|skip)", string.Empty);

string MakeSentence(string text) {
    var withUpperFirst = text.Skip(1).Prepend(char.ToUpper(text.First()));
    var withPeriod = text.Last() == '.' ? withUpperFirst : withUpperFirst.Append('.');

    return string.Concat(withPeriod);
}
