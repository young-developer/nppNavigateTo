# NavigateTo v2 Plugin for Notepad++ - Documentation [![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md) 


[![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/banner2-direct.svg)](https://vshymanskyy.github.io/StandWithUkraine/)

## "Navigate To v2"

![NavigateTo_Search](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/NavigateTo_Search.png) 

![NavigateTo_Main](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/NavigateTo_Main.png)

![NavigateTo_Main](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/NavigateTo_Main_Dock.png)

### Basic Tabs Search
Examples:
![NavigateTo_With_Fuzzy](https://github.com/young-developer/nppNavigateTo/blob/master/documentation/Without_fuzzy.png)

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



