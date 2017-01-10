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

#include "DockingDlgInterface.h"
#include "resource.h"
#include "../NppManager.h"
#include <string>
#include <forward_list>
#include <Windowsx.h>

typedef struct{ std::wstring fullPath; std::wstring fileName; } fileData;

class NavigateToDlg : public DockingDlgInterface
{
public :
	NavigateToDlg() : DockingDlgInterface(IDD_PLUGINGOLINE_NAVTO)
    {
        nppManager = new NppManager();
        history.resize(10);
        isDropDownOpened = false;
    };

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

    virtual void display(bool toShow = true) const {
        DockingDlgInterface::display(toShow);
        if (toShow)
            ::SetFocus(hwndGoLineEdit);
    };

	void setParent(HWND parent2set){
		_hParent = parent2set;
	};

    void hide()
    {
        if(isVisible())
            display(false);
    }

protected :
	virtual INT_PTR CALLBACK run_dlgProc(UINT message, WPARAM wParam, LPARAM lParam);
    static int CALLBACK CompareListItems(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
    static LRESULT CALLBACK ComboBoxProc( HWND, UINT, WPARAM, LPARAM );
private :
	virtual void loadFileNamesToList(const std::string &filter);
	void openSelectedFile();
    void updateHistory(const std::string &value);
	int getSelectedFileId();
    fileData* getFileByFullPath(std::wstring text);
    void AddListViewItem(int item, const std::wstring &text);
    int getLine() const {
        BOOL isSuccessful;
        int line = ::GetDlgItemInt(_hSelf, ID_GOLINE_EDIT, &isSuccessful, FALSE);
        return (isSuccessful?line:-1);
    };

    void refreshResultsList()
    {
        loadFileNamesToList(getFilterEditValue());
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

	std::string getFilterEditValue() const 
	{
		TCHAR buf[MAX_PATH];
        ComboBox_GetText(hwndGoLineEdit, buf, MAX_PATH);
        return NppManager::wStrToStr(&buf[0]);
    };
private:
    //Events
    void SortOnColumnClick(LPNMLISTVIEW pLVInfo);
private:
    HWND hwndListView;
    HWND hwndGoLineEdit;
    NppManager *nppManager;
    bool isDropDownOpened;
    std::forward_list<std::string> history; //TODO: should be changed to HistoryManager class
    std::vector<fileData> fileList;
    
    LVITEM lis;
};

#endif //GOTILINE_DLG_H
