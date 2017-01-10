#include "stdafx.h"
#include "NppManager.h"

extern NppData nppData;

NppManager::NppManager(void)
{
}


NppManager::~NppManager(void)
{
}

void NppManager::showMessageBox(const std::string& text)
{
    ::MessageBox(nppData._nppHandle, NppManager::strToWStr(text).c_str() , TEXT("Info"), MB_OK);
}

bool NppManager::switchToFile(const std::string& filePath)
{
    bool res = false;
    TCHAR currentPath[MAX_PATH];
    ::SendMessage(nppData._nppHandle, NPPM_GETFULLCURRENTPATH, 0, (LPARAM)currentPath);
    std::string strCurrentFilePath = wStrToStr(&currentPath[0]);
    std::transform(strCurrentFilePath.begin(), strCurrentFilePath.end(), strCurrentFilePath.begin(), ::tolower);
    if(filePath.compare(strCurrentFilePath) != 0 && filePath.empty() != true)
	{
        ::SendMessage(nppData._nppHandle, NPPM_SWITCHTOFILE, 0, (LPARAM)NppManager::strToWStr(filePath).c_str());
        res = true;
	}
    return res;
}

bool NppManager::goToLine(const int& line)
{
    if (line > 0)
	{
		// Get the current scintilla
		int which = -1;
		::SendMessage(nppData._nppHandle, NPPM_GETCURRENTSCINTILLA, 0, (LPARAM)&which);
		if (which == -1)
			return false;
		HWND curScintilla = (which == 0)?nppData._scintillaMainHandle:nppData._scintillaSecondHandle;

		::SendMessage(curScintilla, SCI_ENSUREVISIBLE, line-1, 0);
		::SendMessage(curScintilla, SCI_GOTOLINE, line-1, 0);
        return true;
	}
    return false;
}

std::vector<std::string> NppManager::getOpenedFileNames(std::string filterPath)
{
    int nbFile = (int)::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, 0, 0);
    std::vector<std::string> fileNamesList;
	if(nbFile == 0)
		return fileNamesList;
	
	TCHAR **fileNames = (TCHAR **)new TCHAR*[nbFile];
	for (int i = 0 ; i < nbFile ; i++)
	{
		fileNames[i] = new TCHAR[MAX_PATH];
	}

	if (::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, (WPARAM)fileNames, (LPARAM)nbFile))
	{ 
		for (int i = 0 ; i < nbFile ; i++)
		{
			std::string fileName = wStrToStr(&fileNames[i][0]);
            //hard coded 'new  0' file name because it is not availiable
            if(fileName == std::string("new  0"))
                continue;

			std::transform(fileName.begin(), fileName.end(), fileName.begin(), ::tolower);
			std::transform(filterPath.begin(), filterPath.end(), filterPath.begin(), ::tolower);
			std::size_t found = fileName.find(filterPath);

			if(filterPath.empty() || found!=std::string::npos)
			{
                fileNamesList.push_back(fileName);
			}
		}
	}

	for (int i = 0 ; i < nbFile ; i++)
	{
		delete [] fileNames[i];
	}
	delete [] fileNames;

    return fileNamesList;
}


int NppManager::getBufferIdByFilePath(const std::string& filePath)
{
    std::vector<std::string> fileNamesList = getOpenedFileNames();
	if(fileNamesList.size() == 0)
		return -1;
    auto iter = std::find(fileNamesList.begin(), fileNamesList.end(), filePath);
    if(iter == fileNamesList.end())
        return -1;
    int index = iter - fileNamesList.begin();
    int pos = ::SendMessage(nppData._nppHandle, NPPM_GETBUFFERIDFROMPOS, (LPARAM)index, 0);

    if (pos)
    {
        return pos;
    }
    return -1;
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