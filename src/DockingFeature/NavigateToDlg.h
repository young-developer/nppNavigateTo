//this file is part of notepad++
//Copyright (C)2003 Don HO ( donho@altern.org )
//Copyright (C)2016 Oleksii Maryshchenko ( http://omaryshchenko.info )
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either
//version 2 of the License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

#ifndef GOTILINE_DLG_H
#define GOTILINE_DLG_H

#pragma once

#include "../SizeableFeature/SizeableDlg.h"
#include "resource.h"
#include "../NppManager.h"
#include "../File.h"
#include <string>
#include <forward_list>
#include <unordered_map>
#include <Windowsx.h>
#include <CommCtrl.h>

void LOG(const char* errorType, const char *ErrorString);

class NavigateToDlg : public SizeableDlg
{
public :
    NavigateToDlg();
    void doDialog();
    void selectAllInputChars();

    bool IsDropDownOpened() const
    {
        return isDropDownOpened;
    }
    bool DropDownIsNotOpened() const
    {
        return !isDropDownOpened;
    }
    
    void beNotified(SCNotification *notifyCode);

    virtual ~NavigateToDlg()
    {
        delete nppManager;
    }

    virtual void init(HINSTANCE hInst, NppData nppData)
	{
        if(nppManager == nullptr)
            nppManager = new NppManager(nppData);
        
        SizeableDlg::init(hInst, nppData._nppHandle);
    }

    virtual void display(bool toShow = true) const {
        SizeableDlg::display(toShow);
        if (toShow)
            ::SetFocus(hwndGoLineEdit);
    };

	void setParent(HWND parent2set){
		_hParent = parent2set;
	};

    virtual void onSize(UINT nType, int cx, int cy)
    {
	    SizeableDlg::onSize(nType, cx, cy);
	    fitColumnsToSize();
    }

    void fitColumnsToSize()
    {
	    RECT rc;
	    if (GetClientRect(hwndListView, &rc))
	    {
			LONG_PTR len = ptrdiff_t(rc.right - rc.left);
			len -= static_cast<LONG_PTR>(SendMessage(hwndListView, LVM_GETCOLUMNWIDTH, 0, 0));
		    len -= 2;
		    SendMessage(hwndListView, LVM_SETCOLUMNWIDTH, 1, len);
	    }
    }

    void hide()
    {
        if(isVisible())
            display(false);
    }

protected :
	virtual INT_PTR CALLBACK run_dlgProc(UINT message, WPARAM wParam, LPARAM lParam);
    static INT_PTR CALLBACK CompareListItems(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
private :
    //File list functions
	void loadFileNamesToList(const std::wstring &filter);
	void updateFileBufferStatus(INT_PTR bufferID, FileStatus status);
    void updateCurrentFileStatus(FileStatus status);
    void addFileToListView(const File& file);
	void addFileToListView(INT_PTR bufferID);
	void removeFileFromListView(INT_PTR bufferID);
    //

	void openSelectedFile();
    void previewHighlightedFile();
    void updateHistory(const std::wstring &value);
	INT_PTR getSelectedFileId();
    File* NavigateToDlg::getSelectedFile();
	INT_PTR getLine() const {
        BOOL isSuccessful;
		INT_PTR line = ::GetDlgItemInt(_hSelf, ID_GOLINE_EDIT, &isSuccessful, FALSE);
        return (isSuccessful?line:-1);
    };

    void refreshResultsList(bool selectFirstRow = true)
    {
        loadFileNamesToList(getFilterEditValue());
        if(selectFirstRow)
            selectFirstRowInList();
    }

    void selectFirstRowInList()
    {
        //select first row in grid
        ListView_SetItemState(hwndListView, 0, LVIS_SELECTED, LVIS_SELECTED);
        ListView_SetItemState(hwndListView, 0, LVIS_FOCUSED, LVIS_FOCUSED);
    }

    bool isListViewFocused()
    {
       return ::GetFocus() != hwndListView;
    }

	void NavigateToDlg::addFileToList(const File& file)
	{
		if(file.isValid()) 
        {
			fileList.insert(std::make_pair(file.getBufferId(), file));
        }
	}

	void NavigateToDlg::removeFileFromList(INT_PTR bufferID)
	{
		fileList.erase(bufferID);
	}

	void NavigateToDlg::clearFileList()
	{
		fileList.clear();
	}

	std::wstring getFilterEditValue() const 
	{
		TCHAR buf[MAX_PATH];
        ComboBox_GetText(hwndGoLineEdit, buf, MAX_PATH);
        return std::wstring(&buf[0]);
    };
private:
    //Events
    void SortOnColumnClick(LPNMLISTVIEW pLVInfo);
private:
    HWND hwndListView;
    HWND hwndGoLineEdit;
    NppManager *nppManager;
    bool isDropDownOpened;
    std::forward_list<std::wstring> history; //TODO: should be changed to HistoryManager class
	std::unordered_map<INT_PTR, File> fileList;
};

LRESULT ProcessCustomDraw (LPARAM lParam);

#endif //GOTILINE_DLG_H
