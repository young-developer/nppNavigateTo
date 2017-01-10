#pragma once
#include "PluginInterface.h"
#include <string>
#include <vector>
#include <algorithm>

class NppManager
{
public:
    LangType detectLanguageFromTextBegining(const unsigned char *data, size_t dataLen);
public:
    NppManager(void);
    virtual ~NppManager(void);
//public functions
public:
    std::vector<std::string> getOpenedFileNames(std::string filterPath = "");
    bool goToLine(const int& line);
    bool switchToFile(const std::string& filePath);
    void showMessageBox(const std::string& text);
    int getBufferIdByFilePath(const std::string& filePath);
//static string helper //todo: should be moved from here
public:
    static std::string wStrToStr(TCHAR *wStr)
    {
        std::wstring temp(wStr); //convert to wstring
	    std::string resString(temp.begin(), temp.end()); //and convert to string.
        return resString;
    }

    static std::wstring strToWStr(const std::string& str)
    {
	    std::wstring resString(str.begin(), str.end()); //and convert to wstring.
        return resString;
    }
};

