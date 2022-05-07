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
	File(std::wstring fileFullPath, INT_PTR indexOfFile, INT_PTR bufferIndex, INT_PTR fileView);
    ~File();
    //getters
    std::wstring getFullPath() const;
    std::wstring getFileName();
    std::wstring getFileNameWithView() const;
    FileStatus getFileStatus() const {return fileStatus;}
    void setFileStatus(FileStatus newStatus){ fileStatus = newStatus;}
	INT_PTR getView() const;
	INT_PTR getIndex() const;
	void setIndex(INT_PTR anIndex);
	INT_PTR decrementIndex();
	INT_PTR incrementIndex();
	INT_PTR getBufferId() const;
    bool isValid() const;
    void showView();
    bool equals(const File& other) const;
    bool operator==(const File &other) const;
    bool operator!=(const File &other) const;
    size_t getHash() const;
private:
    std::wstring fullPath;
    std::wstring fileName;
	INT_PTR bufferId;
	INT_PTR index;
	INT_PTR view;
    bool showViewString;
    FileStatus fileStatus;
};