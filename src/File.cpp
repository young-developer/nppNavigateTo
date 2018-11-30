#include <stdafx.h>
#include "File.h"
#include "Shlwapi.h"

File::File(std::wstring fileFullPath, INT_PTR indexOfFile, INT_PTR bufferIndex, INT_PTR fileView)
{
    fullPath = fileFullPath;
    fileName = (LPWSTR)PathFindFileName(fullPath.c_str());
    bufferId = bufferIndex;
    index = indexOfFile;
    view = fileView;
    showViewString = false;
}

std::wstring File::getFullPath() const
{
    return fullPath;
}

void File::showView()
{
    showViewString = true;
}

std::wstring File::getFileName()
{
    return fileName;
}

std::wstring File::getFileNameWithView() const
{
    if(showViewString)
    {
        std::wstring strView = view ? TEXT(" (2)") : TEXT(" (1)");
        return fileName + strView;
    }
    else
    {
        return fileName;
    }
}

bool File::operator==(const File &other) const {
   return equals(other);
}

bool File::operator!=(const File &other) const {
    return !(*this == other);
}

bool File::equals(const File& other) const
{
    if(this == &other)
        return true;

    return this->fileName == other.fileName && this->fullPath == other.fullPath && this->bufferId == other.bufferId && this->view == other.view && this->index == other.index;
}

size_t File::getHash() const
{
	return std::hash<std::wstring>()(this->fullPath) ^ std::hash<INT_PTR>()(this->view) ^ std::hash<INT_PTR>()(this->index);
}

INT_PTR File::getView() const
{
    return view;
}

INT_PTR File::getBufferId() const
{
    return bufferId;
}

INT_PTR File::getIndex() const
{
    return index;
}

void File::setIndex(INT_PTR anIndex)
{
	index = anIndex;
}

bool File::isValid() const
{
    return !fileName.empty() && !fullPath.empty() && bufferId >= 0 && index >= 0;
}

INT_PTR File::decrementIndex()
{
	index = index - 1;
	return getIndex();
}

INT_PTR File::incrementIndex()
{
	index = index + 1;
	return getIndex();
}

File::~File()
{
}