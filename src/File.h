#pragma once

#include <string>

enum FileStatus
{
	SAVED,
	UNSAVED,
    READONLY
};

class File
{
public:
    File(){};
    File(std::wstring fileFullPath, int indexOfFile, int bufferIndex, int fileView);
    ~File();
    //getters
    std::wstring getFullPath() const;
    std::wstring getFileName();
    std::wstring getFileNameWithView() const;
    FileStatus getFileStatus() const {return fileStatus;}
    void setFileStatus(FileStatus newStatus){ fileStatus = newStatus;}
    int getView() const;
    int getIndex() const;
    int getBufferId() const;
    bool isValid() const;
    void showView();
    bool equals(const File& other) const;
    bool operator==(const File &other) const;
    bool operator!=(const File &other) const;
    size_t getHash() const;
private:
    std::wstring fullPath;
    std::wstring fileName;
    int bufferId;
    int index;
    int view;
    bool showViewString;
    FileStatus fileStatus;
};