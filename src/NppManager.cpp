#include "stdafx.h"      //Pre-compiled header for compiler
#include "NppManager.h"
#include "File.h"

NppManager::NppManager(const NppData& pNppData)
{
    nppData = pNppData;
    editor.SetScintillaInstance(nppData._scintillaMainHandle);
}

NppManager::~NppManager(void)
{
}

void NppManager::showMessageBox(const std::wstring& text, const std::wstring &msgTitle)
{
	::MessageBox(nppData._nppHandle, text.c_str(), msgTitle.c_str(), MB_OK);
}

bool NppManager::switchToFile(const File* file)
{
    return (file != nullptr)?switchToFile(file->getBufferId(),file->getView()):false;
}

bool NppManager::switchToFile(const std::wstring& filePath)
{
    bool res = false;
    TCHAR currentPath[MAX_PATH];
    ::SendMessage(nppData._nppHandle, NPPM_GETFULLCURRENTPATH, 0, (LPARAM)currentPath);
    std::wstring strCurrentFilePath(&currentPath[0]);
    std::transform(strCurrentFilePath.begin(), strCurrentFilePath.end(), strCurrentFilePath.begin(), ::tolower);
    if(filePath.compare(strCurrentFilePath) != 0 && filePath.empty() != true)
	{
        ::SendMessage(nppData._nppHandle, NPPM_SWITCHTOFILE, 0, (LPARAM)filePath.c_str());
        res = true;
	}

    return res;
}

bool NppManager::switchToFile(const INT_PTR bufferId, const INT_PTR view)
{
	return ::SendMessage(nppData._nppHandle, NPPM_ACTIVATEDOC, view, ::SendMessage(nppData._nppHandle, NPPM_GETPOSFROMBUFFERID, bufferId, MAIN_VIEW)) != -1;
}

bool NppManager::openContextMenu(const INT_PTR bufferId, const INT_PTR view)
{
	return ::SendMessage(nppData._nppHandle, NPPM_TRIGGERTABBARCONTEXTMENU, view, ::SendMessage(nppData._nppHandle, NPPM_GETPOSFROMBUFFERID, bufferId, MAIN_VIEW)) != -1;
}

bool NppManager::goToLine(const INT_PTR& line)
{
    if (line > 0)
	{
		// Get the current scintilla
		int which = -1;
		::SendMessage(nppData._nppHandle, NPPM_GETCURRENTSCINTILLA, 0, (LPARAM)&which);
		if (which == -1)
			return false;
		HWND curScintilla = (which == 0)?nppData._scintillaMainHandle:nppData._scintillaSecondHandle;

		::SendMessage(curScintilla, SCI_ENSUREVISIBLE, INT_PTR(line - 1), 0);
		::SendMessage(curScintilla, SCI_GOTOLINE, INT_PTR(line - 1), 0);
        return true;
	}
    return false;
}


void NppManager::setFocus()
{
	// Get the current scintilla
	int which = -1;
	::SendMessage(nppData._nppHandle, NPPM_GETCURRENTSCINTILLA, 0, (LPARAM)&which);
	if (which == -1)
		return;
	HWND curScintilla = (which == 0)?nppData._scintillaMainHandle:nppData._scintillaSecondHandle;
    //::SendMessage(curScintilla, SCI_ENSUREVISIBLE, 0, 0);
	::SendMessage(curScintilla, SCI_SETFOCUS, 0, (LPARAM)true);
    ::SetFocus(nppData._nppHandle);
}


std::vector<std::wstring> NppManager::getOpenedFileNames(std::wstring filterPath)
{
	bool inverseLogic = false;
	std::vector<std::wstring> fileNamesList;
	size_t nbFile[2];
	nbFile[0] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, 0, PRIMARY_VIEW);
	nbFile[1] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, 0, SECOND_VIEW);

	INT_PTR selectedTab[2];
	selectedTab[0] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETCURRENTDOCINDEX, 0, MAIN_VIEW);
	selectedTab[1] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETCURRENTDOCINDEX, 0, SUB_VIEW);

	if ((nbFile[0] + nbFile[1]) == 0)
		return fileNamesList;

	INT_PTR currentView;
	::SendMessage(nppData._nppHandle, NPPM_GETCURRENTSCINTILLA, 0, reinterpret_cast<LPARAM>(&currentView));

	//invert logic search string trimming
	if (filterPath[0] == '|')
	{
		inverseLogic = true;
		filterPath.erase(0, filterPath.find_first_not_of('|'));       //prefixing spaces
		filterPath.erase(0, filterPath.find_first_not_of(' '));       //prefixing spaces
		filterPath.erase(filterPath.find_last_not_of(' ') + 1);         //surfixing spaces
	}

	for (size_t view = 0; view < 2; view++)
	{
		if (nbFile[view] && selectedTab[view] >= 0)
		{
			TCHAR **fileNames = (TCHAR **)new TCHAR*[nbFile[view]];
			for (size_t i = 0; i < nbFile[view]; ++i)
			{
				fileNames[i] = new TCHAR[MAX_PATH];
			}

			if (::SendMessage(nppData._nppHandle, view ? NPPM_GETOPENFILENAMESSECOND : NPPM_GETOPENFILENAMESPRIMARY,
				reinterpret_cast<WPARAM>(fileNames), static_cast<LPARAM>(nbFile[view])))
			{
				for (size_t position = 0; position < nbFile[view]; ++position)
				{
					std::wstring fileName(fileNames[position]);
					std::transform(fileName.begin(), fileName.end(), fileName.begin(), ::tolower);
					std::transform(filterPath.begin(), filterPath.end(), filterPath.begin(), ::tolower);
					std::size_t found = fileName.find(filterPath);
					bool subStringLogic = inverseLogic ? (found == std::wstring::npos) : (found != std::wstring::npos);
					if (filterPath.empty() || subStringLogic)
					{
						fileNamesList.push_back(fileName);
					}
				}
			}
			for (size_t i = 0; i < nbFile[view]; ++i)
			{
				delete[] fileNames[i];
			}
			delete[] fileNames;
		}
	}
	return fileNamesList;
}

INT_PTR NppManager::getNumberOfFiles()
{
	size_t nbFile[2];
	nbFile[0] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, 0, PRIMARY_VIEW);
	nbFile[1] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, 0, SECOND_VIEW);

	return nbFile[0] + nbFile[1];
}

std::vector<File> NppManager::getOpenedFiles(std::wstring filterPath)
{
    bool inverseLogic = false;
    std::vector<File> fileNamesList;
    

	INT_PTR selectedTab[2];
	selectedTab[0] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETCURRENTDOCINDEX, 0, MAIN_VIEW);
	selectedTab[1] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETCURRENTDOCINDEX, 0, SUB_VIEW);

	size_t nbFile[2];
	nbFile[0] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, 0, PRIMARY_VIEW);
	nbFile[1] = (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, 0, SECOND_VIEW);

    if(getNumberOfFiles() == 0)
		return fileNamesList;

	INT_PTR currentView;
	::SendMessage(nppData._nppHandle, NPPM_GETCURRENTSCINTILLA, 0, reinterpret_cast<LPARAM>(&currentView));

    //invert logic search string trimming
    if(filterPath[0] == '|')
    {
        inverseLogic = true;
        filterPath.erase(0, filterPath.find_first_not_of('|'));       //prefixing spaces
        filterPath.erase(0, filterPath.find_first_not_of(' '));       //prefixing spaces
        filterPath.erase(filterPath.find_last_not_of(' ')+1);         //surfixing spaces
    }

	for(size_t view = 0; view < 2; view++)
	{
		if (nbFile[view] && selectedTab[view] >= 0)
		{
			TCHAR **fileNames = (TCHAR **)new TCHAR*[nbFile[view]];
			for (size_t i = 0; i < nbFile[view]; ++i)
			{
				fileNames[i] = new TCHAR[MAX_PATH];
			}

			if (::SendMessage(nppData._nppHandle, view ? NPPM_GETOPENFILENAMESSECOND : NPPM_GETOPENFILENAMESPRIMARY, 
							reinterpret_cast<WPARAM>(fileNames), static_cast<LPARAM>(nbFile[view])))
			{
				for(size_t position = 0; position < nbFile[view]; ++position)
				{
                    std::wstring fileName(fileNames[position]);
			        std::transform(fileName.begin(), fileName.end(), fileName.begin(), ::tolower);
			        std::transform(filterPath.begin(), filterPath.end(), filterPath.begin(), ::tolower);
			        std::size_t found = fileName.find(filterPath);
                    bool subStringLogic = inverseLogic?(found==std::wstring::npos):(found!=std::wstring::npos);
                    if(filterPath.empty() || subStringLogic)
			        {
						auto bufId = getBufferIdByFilePath(fileName);
						File file(fileNames[position], position, bufId, view);
						fileNamesList.push_back(file);                        
			        }
				}
			}
			for (size_t i = 0; i < nbFile[view]; ++i)
	        {
		        delete [] fileNames[i];
	        }
	        delete [] fileNames;
		}
	}
    return fileNamesList;
}

HWND NppManager::getCurrentHScintilla(INT_PTR which)
{
	return (which == 0)?nppData._scintillaMainHandle:nppData._scintillaSecondHandle;
}

FileStatus NppManager::getFileStatus(const File& file)
{
    FileStatus fileStatus = SAVED;
    BOOL readonly = ::SendMessage(getCurrentHScintilla(VIEW(file.getIndex())), SCI_GETREADONLY, 0, 0);
	
	if (readonly)
		fileStatus = READONLY;
	else
	{
		INT_PTR modified = ::SendMessage(getCurrentHScintilla(VIEW(file.getIndex())), SCI_GETMODIFY, 0, 0);
		if (modified)
			fileStatus = UNSAVED;
	}
    return fileStatus;
}


INT_PTR NppManager::getIndexByFilePath(const std::wstring& filePath)
{
    std::vector<std::wstring> fileNamesList = getOpenedFileNames();
	if(fileNamesList.size() == 0)
		return -1;
    auto iter = std::find(fileNamesList.begin(), fileNamesList.end(), filePath);
    if(iter == fileNamesList.end())
        return -1;
    auto index = (INT_PTR)(iter - fileNamesList.begin());

    return index;
}

INT_PTR NppManager::getBufferIdByFilePath(const std::wstring& filePath)
{
    return (INT_PTR)::SendMessage(nppData._nppHandle, NPPM_GETBUFFERIDFROMPOS, (LPARAM)getIndexByFilePath(filePath), 0);
}

 LangType NppManager::detectLanguageFromTextBegining(const unsigned char *data, size_t dataLen)
{
	struct FirstLineLanguages
	{
		std::string pattern;
		LangType lang;
	};

	// Is the buffer at least the size of a BOM?
	if (dataLen <= 3)
		return L_TEXT;

	// Eliminate BOM if present
	size_t i = 0;
	if ((data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF) || // UTF8 BOM
		(data[0] == 0xFE && data[1] == 0xFF && data[2] == 0x00) || // UTF16 BE BOM
		(data[0] == 0xFF && data[1] == 0xFE && data[2] == 0x00))   // UTF16 LE BOM
		i += 3;

	// Skip any space-like char
	for (; i < dataLen; ++i)
	{
		if (data[i] != ' ' && data[i] != '\t' && data[i] != '\n' && data[i] != '\r')
			break;
	}

	// Create the buffer to need to test
	const size_t longestLength = 40; // shebangs can be large
	std::string buf2Test = std::string((const char *)data + i, longestLength);

	// Is there a \r or \n in the buffer? If so, truncate it
	auto cr = buf2Test.find("\r");
	auto nl = buf2Test.find("\n");
	auto crnl = min(cr, nl);
	if (crnl != std::string::npos && crnl < longestLength)
		buf2Test = std::string((const char *)data + i, crnl);

	// First test for a Unix-like Shebang
	// See https://en.wikipedia.org/wiki/Shebang_%28Unix%29 for more details about Shebang
	std::string shebang = "#!";

	size_t foundPos = buf2Test.find(shebang);
	if (foundPos == 0)
	{
		// Make a list of the most commonly used languages
		const size_t NB_SHEBANG_LANGUAGES = 6;
		FirstLineLanguages ShebangLangs[NB_SHEBANG_LANGUAGES] = {
			{ "sh",		L_BASH },
			{ "python", L_PYTHON },
			{ "perl",	L_PERL },
			{ "php",	L_PHP },
			{ "ruby",	L_RUBY },
			{ "node",	L_JAVASCRIPT }
		};

		// Go through the list of languages
		for (i = 0; i < NB_SHEBANG_LANGUAGES; ++i)
		{
			if (buf2Test.find(ShebangLangs[i].pattern) != std::string::npos)
			{
				return ShebangLangs[i].lang;
			}
		}

		// Unrecognized shebang (there is always room for improvement ;-)
		return L_TEXT;
	}

	// Are there any other patterns we know off?
	const size_t NB_FIRST_LINE_LANGUAGES = 5;
	FirstLineLanguages languages[NB_FIRST_LINE_LANGUAGES] = {
		{ "<?xml",			L_XML },
		{ "<?php",			L_PHP },
		{ "<html",			L_HTML },
		{ "<!DOCTYPE html",	L_HTML },
		{ "<?",				L_PHP } // MUST be after "<?php" and "<?xml" to get the result as accurate as possible
	};

	for (i = 0; i < NB_FIRST_LINE_LANGUAGES; ++i)
	{
		foundPos = buf2Test.find(languages[i].pattern);
		if (foundPos == 0)
		{
			return languages[i].lang;
		}
	}

	// Unrecognized first line, we assume it is a text file for now
	return L_TEXT;
}