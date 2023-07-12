# NavigateTo v2 Plugin for Notepad++ - Documentation [![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md) 


[![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/banner2-direct.svg)](https://vshymanskyy.github.io/StandWithUkraine/)

## "Navigate To v2"

![NavigateTo_Search](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/NavigateTo_Search.png) 

![NavigateTo_Main](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/NavigateTo_Main.png)

![NavigateTo_Main](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/NavigateTo_Main_Dock.png)

### Basic Tabs Search
Examples:
![NavigateTo_With_Fuzzy](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/Without_fuzzy.png)

### Multi-glob syntax

Multi-glob syntax lets the user search space-separated words or [globs](https://en.wikipedia.org/wiki/Glob_(programming)). __This syntax is case-insensitive.__ The rules are as follows:

1. `foo bar txt` matches anything that contains the words `foo`, `bar`, and `txt`, for example `foobar.txt` *but not `bar.txt` or `foo.bar`*
    * __Note that this is the same as the old behavior of NavigateTo before the introduction of glob syntax, so the old search behavior is retained.__
2. `foo | bar` matches anything that contains *`foo` OR `bar`*, for example `foo.txt` and `bar.txt`
3. `foo | <bar baz>` matches *`foo` OR (`bar` AND `baz`)*, like `foo.txt` and `bar.baz` but not `bar.txt`. That is, `<>` are grouping parentheses, although *`<` does not require a closing `>`*. 
4. `foo !bar` matches *`foo` AND NOT `bar`*, that is, `!` before a search term represents logical negation. So `foo !bar` matches `foo.txt` but not `foo.bar` 
5. `*.foo` uses *glob syntax* and matches anything that has the `.foo` extension.
    * __Note that anything using the `*`, `[]`, and `{}` characters is considered a glob, and the end of the glob must match the end of the search target.__
    * For example, while `foo` matches `foo.txt` or `txt.foo`, __`*.foo` matches `txt.foo` *but not `foo.txt`*__
6. `*` matches any number of characters (including zero) *other than the path separator `\`.* Thus `r.c*` matches `bar.c` and `gor.cpp` *but not `jir.c\foo.bar`*
7. `?` matches *any one character* other than the path separator `\`. Thus `foo.?` matches `foo.h` and `foo.c` but not `foo.cpzp` or `foo.h\bar.py`
8. `[chars]` matches any one character inside the square brackets. It also matches *character ranges*, specifically `a-z`, `A-Z`, and `0-9` or any subset thereof.
    * `*.a[0-2]` matches `foo.a0`, `foo.a1`, and `foo.a1`
    * `*.[x-y1][7-8]` matches `foo.x7`, `foo.x8`, `foo.y7`, `foo.y8`, `foo.17`, and `foo.18`
    * `big[ _]dog.txt` matches `big dog.txt` and `big_dog.txt`
9. `[!chars]` matches *any one character __not__ inside the square brackets other than `\`.* Just as with `[chars]`, character groups can be negated.
    *  `*.[!1-2]` matches `foo.a` and `foo.g` and `foo.3` *but not `foo.1` or `foo.2` or `foo.\bar.txt`*
10. `\xNN` matches a unicode character at codepoint `NN`. For example, `\x21` matches the exclamation point `!`.
    * `\xNN` escapes can be used in character classes. For example, `[\x61-\x6a]` matches the letters `a-z`.
11. `\uNNNN` matches a unicode character at codepoint `NNNN`
    * `\u0021` also matches `!`
    * `\u0434` matches the Cyrillic character `д`
    * `\uNNNN` escapes can be used in character classes. For example, `[\u03B1-\u03C9]` matches the lower-case Greek letters `α` through `ω`.
12. `*.{foo,bar,baz}` matches `a.foo`, `a.bar`, and `a.baz`. That is, `{OPTION1,OPTION2,...,OPTIONN}` matches each comma-separated option within the `{}`
    * You can use multiple `{}` alternations within the same glob. For example, `{foo,bar}.{txt,md}` matches `foo.txt`, `foo.md`, `bar.txt`, and `bar.md`

#### Glob syntax "kitchen sink" example, `foo**[!c-p]u\u0434ck*.{tx?,cpp} !fgh | <ba\x72 baz`

##### Matches
1. `foo\boo\quдcked.txt` (recall that `\u0434` is `д`)
2. `foo\boo\quдcked.cpp`
3. `foozuдck.txb`
4. `bar.baz` (note that `\x72` is `r`)
5. `baz.bar`
6. `fgh\bar.baz` (`fgh` is forbidden if the glob is being matched, but not if `bar baz` is being matched)

##### Non-matches
1. `foo\boo\duдcked.txt` (the `d` in `duдcked` is in the character class `c-p`, which was negated)
2. `foozuдck.xml` (the `xml` extension does not match `tx?`)
3. `foo.baz` (does not match the fancy glob, and it doesn't contain both `bar` and `baz`)
4.  `foo\boo\quдcked.txto` (`txto` does not match `tx?` __because globs are always anchored at the end__ and `txto` has an `o` after the matching portion)
7. `foo\fgh\quдcked.txt` (contains the forbidden search term `fgh`)
8. `foo\boo\quдcked\bad.txt` (`uдck*` matches *any characters after* `uдck` *other than `\`*)


### Fuzzy Tabs Search
Fuzzy search is based on: https://en.wikipedia.org/wiki/Longest_common_subsequence_problem
Highlight is based on character.

Examples:
![NavigateTo_With_Fuzzy](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/With_fuzzy.png)
![NavigateTo_With_Fuzzy2](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/With_fuzzy_ex2.png)

### Top Directory Search
![NavigateTo_With_Fuzzy2](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/NavigateTo_Main_Search_Top_Dir.png)

## Settings

![NavigateTo_Settings](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/NavigateTo_Settings.png)
#### General
* Keep dialog visible - after user navigate to tab/file dialog will not be hidden. Should be enabled when you use it as dockable window.
* Clear search input - after user navigate to tab clear search input
* Keep First Row Selected - after specifing search input first row of the list will be selected otherwise form keep privious selection options.
#### Search behavior
* Search in current file folder - search files in Top directory of current file (fuzzy search only affect tabs search)
* Search in sub directories too - include subdirectory files (fuzzy search only affect tabs search)
* Search menu commands - experimental menu items search
* Prefer filename over path - filename matches are shown first and than filepath matches
* Fuzzy search - Tabs only fuzzy search based on (Longest common subsequence). Tolerance reduces search result restrictions based on length of input search.
#### File List
* N(px) Min Grid Width - Size in pixels identifing minimum grid size to hide path, source columns.
* N Min Char Search - start filter after specifing N chars
* Sort after search by [COLUMN] [ORDER] - Sort automaticly results based on predifined params
#### Appearance
* Highlight Background and Text - color of highlighted text and background
* Selected Row Background and Text - colors of selected row
* Grid Background - background of grid without rows
* Row Background - color of rows



