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
	INT_PTR getNumberOfFiles();
    std::vector<std::wstring> getOpenedFileNames(std::wstring filterPath = TEXT(""));
    std::vector<File> getOpenedFiles(std::wstring filterPath = TEXT(""));
	bool goToLine(const INT_PTR& line);
    bool switchToFile(const std::wstring& filePath);
    bool switchToFile(const File* file);
	bool switchToFile(const INT_PTR bufferId, const INT_PTR view);
	bool openContextMenu(const INT_PTR bufferId, const INT_PTR view);
	void showMessageBox(const std::wstring& text, const std::wstring &msgTitle = TEXT("Info"));
	INT_PTR getBufferIdByFilePath(const std::wstring& filePath);
	INT_PTR getIndexByFilePath(const std::wstring& filePath);
	HWND getCurrentHScintilla(INT_PTR which);
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

