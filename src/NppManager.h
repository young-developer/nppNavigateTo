#pragma once
#include "PluginInterface.h"
#include "ScintillaGateway.h"
#include <string>
#include <vector>
#include <algorithm>

#define INDEX(x)           (x & 0x3FFFFFFF)
#define VIEW(x)			   ((x & 0xC0000000) >> 30)

class File;
class ScintillaGateway;
enum FileStatus;
class NppManager
{
private:
    NppData nppData;
    ScintillaGateway editor;
public:
    LangType detectLanguageFromTextBegining(const unsigned char *data, size_t dataLen);
public:
    NppManager(const NppData& pNppData);
    virtual ~NppManager(void);
//public functions
public:
    std::vector<std::wstring> getOpenedFileNames(std::wstring filterPath = TEXT(""));
    std::vector<File> getOpenedFiles(std::wstring filterPath = TEXT(""));
    bool goToLine(const int& line);
    bool switchToFile(const std::wstring& filePath);
    bool switchToFile(const File* file);
    bool switchToFile(const int index, const int view);
    bool openContextMenu(const int index, const int view);
	void showMessageBox(const std::wstring& text, const std::wstring &msgTitle = TEXT("Info"));
    int getBufferIdByFilePath(const std::wstring& filePath);
    int getIndexByFilePath(const std::wstring& filePath);
    HWND getCurrentHScintilla(int which);
    void setFocus();
    FileStatus getFileStatus(const File& file);
//static string helper //todo: should be moved from here
public:
    static std::wstring strToWStr(const std::string& str)
    {
	    std::wstring resString(str.begin(), str.end()); //and convert to wstring.
        return resString;
    }
};

