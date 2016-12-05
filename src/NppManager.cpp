#include "NppManager.h"

extern NppData nppData;

NppManager::NppManager(void)
{
}


NppManager::~NppManager(void)
{
}

void showMessageBox(const std::string& text)
{
    ::MessageBox(nppData._nppHandle, NppManager::strToWStr(text).c_str() , TEXT("NppManager::logInfo"), MB_OK);
}

bool NppManager::switchToFile(const std::string& filePath)
{
    bool res = false;
    TCHAR currentPath[MAX_PATH];
    ::SendMessage(nppData._nppHandle, NPPM_GETFULLCURRENTPATH, 0, (LPARAM)currentPath);
    std::string strCurrentFilePath = wStrToStr(&currentPath[0]);
    std::transform(strCurrentFilePath.begin(), strCurrentFilePath.end(), strCurrentFilePath.begin(), ::tolower);
    if(filePath.compare(strCurrentFilePath) != 0)
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

	if (::SendMessage(nppData._nppHandle, NPPM_GETOPENFILENAMES, (WPARAM)fileNames, (LPARAM)nbFile))
	{ 
		for (int i = 0 ; i < nbFile ; i++)
		{
			std::string fileName = wStrToStr(&fileNames[i][0]);
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