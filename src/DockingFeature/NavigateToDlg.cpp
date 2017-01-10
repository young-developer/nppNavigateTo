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
#include "stdafx.h"
#include "NavigateToDlg.h"
#include "../PluginDefinition.h"

#include <shlwapi.h>
#include <string>
#include <algorithm>


#ifdef UNICODE 
	#define generic_itoa _itow
	typedef std::string String; 
#else
	#define generic_itoa itoa
	typedef std::wstring String; 
#endif

extern NppData nppData;

#define NAME_COLUMN 0
#define PATH_COLUMN 1

WNDPROC lpfnEditWndProcCombo; //  Original wndproc for the combo box 
HWND ghwndListView;

INT_PTR CALLBACK NavigateToDlg::run_dlgProc(UINT message, WPARAM wParam, LPARAM lParam)
{
    //close on Escape
	if (GetAsyncKeyState(VK_ESCAPE) < 0) {
	    hide();
	}
	    
	switch (message) 
	{
        /*case WM_ACTIVATE:
        {
            if (LOWORD(wParam) == WA_INACTIVE && isVisible())
			{
                hide();
			}
			else
			{
               // display();
			}
            return TRUE;
        }
        break;*/
		case WM_COMMAND : 
		{
			switch (wParam)
			{
				case IDOK :
				{
                    std::string editValue = getFilterEditValue();
					openSelectedFile();
                    //save history
                    if(!editValue.empty())
                    {
                        updateHistory(editValue);
                    }
					return TRUE;
				}
                break;
			}
            switch(LOWORD(wParam))
            {
                case ID_GOLINE_EDIT: // If the combo box sent the message,
                {
                    switch(HIWORD(wParam)) // Find out what message it was
                    {
                    case CBN_DROPDOWN: // This means that the list is about to display
                        isDropDownOpened = true;
                        return TRUE;
                        break;
                    case CBN_CLOSEUP:
                        isDropDownOpened = false;
                        return TRUE;
                        break;
                    case CBN_SELCHANGE:
                    {
                        int idx_row;
                        TCHAR strText[MAX_PATH];
	                    idx_row = SendMessage(hwndGoLineEdit , CB_GETCURSEL, 0, 0 );
	                    SendMessage(hwndGoLineEdit,CB_GETLBTEXT, idx_row,(LPARAM)strText);
                        loadFileNamesToList(NppManager::wStrToStr(&strText[0]));
                        selectFirstRowInList();
                    }
                    break;
                    case CBN_EDITCHANGE:
                        refreshResultsList();
                        return TRUE;
                    }
                }
                break;
                case IDC_RESULTS_LIST:
				{
					switch (HIWORD(wParam))
					 {
						case LBN_DBLCLK: 
						{
							openSelectedFile();
                            return TRUE;
						}
						break;
					 }
				}
            }
			return FALSE;
		}
        break;
        case WM_INITDIALOG:
        {
            ghwndListView = hwndListView = ::GetDlgItem(_hSelf, IDC_RESULTS_LIST);
            hwndGoLineEdit = ::GetDlgItem(_hSelf, ID_GOLINE_EDIT);
            if (NULL != hwndGoLineEdit)
			{
                //  Get the edit window handle to each combo box. 
                HWND hwndEdit1 = GetWindow(hwndGoLineEdit, GW_CHILD);
				lpfnEditWndProcCombo = (WNDPROC) SetWindowLong(hwndEdit1, GWL_WNDPROC, (DWORD) ComboBoxProc); 
			}
            LVCOLUMN LvCol;
            memset(&LvCol,0,sizeof(LvCol));                  // Zero Members
            LvCol.mask=LVCF_FMT | LVCF_WIDTH | LVCF_TEXT | LVCF_SUBITEM;  // Type of mask
            LvCol.cx=0x85;                                   // width between each coloum
            LvCol.pszText=TEXT("Name");                            // First Header Text
            SendMessage(hwndListView,LVM_SETEXTENDEDLISTVIEWSTYLE,
               0,LVS_EX_FULLROWSELECT); // Set style
            SendMessage(hwndListView,LVM_INSERTCOLUMN,NAME_COLUMN,(LPARAM)&LvCol); // Insert/Show the coloum
            LvCol.pszText=TEXT("Path");    
            LvCol.cx=0x115;                                   // width of column
            SendMessage(hwndListView,LVM_INSERTCOLUMN,PATH_COLUMN,(LPARAM)&LvCol); // ...

            SendMessage(hwndGoLineEdit,CB_SETMINVISIBLE,8,0); // Visible items = 8
            //ListView_SetExtendedListViewStyle(hwndListView,LVS_EX_HEADERDRAGDROP); //now we can drag&drop headers :)
            refreshResultsList();
			return TRUE;
        }
        break;
        case WM_NOTIFY:
        {
            switch(LOWORD(wParam))
            {
                case IDC_RESULTS_LIST:
                {
                    NMHDR* pNMHDR = (NMHDR*)lParam;
                    if(pNMHDR->code == NM_DBLCLK)
                    {
                        openSelectedFile();
                        return TRUE;
                    }

                    if (pNMHDR->code == LVN_COLUMNCLICK) // sort columns
				    {
                        SortOnColumnClick((LPNMLISTVIEW)lParam);
				    }
                }
            }
        }
        break;
		default :
			return DockingDlgInterface::run_dlgProc(message, wParam, lParam);
	}
}

LRESULT CALLBACK NavigateToDlg::ComboBoxProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) 
{ 
    switch (msg) 
    { 
        case WM_KEYDOWN: 
            switch (wParam) 
            { 
                case VK_UP:
                    //if(isListViewFocused() && DropDownIsNotOpened())
                    ::SetFocus(ghwndListView);
                    return 0;
                case VK_DOWN:
                    ::SetFocus(ghwndListView);
                     return 0;
                case VK_TAB: 
                    //SendMessage(hwndMain, WM_TAB, 0, 0); 
                    return 0; 
                case VK_ESCAPE: 
                    //SendMessage(hwndMain, WM_ESC, 0, 0); 
                    return 0; 
                case VK_RETURN: 
                    //SendMessage(hwndMain, WM_ENTER, 0, 0); 
                    return 0; 
            } 
            break; 
 
        case WM_KEYUP: 
        case WM_CHAR: 
            switch (wParam) 
            { 
                case VK_TAB: 
                case VK_ESCAPE: 
                case VK_RETURN: 
                    return 0; 
            } 
    } 
 
    //  Call the original window procedure for default processing. 
     return CallWindowProc(lpfnEditWndProcCombo, hwnd, msg, wParam, lParam); 
} 

int CALLBACK NavigateToDlg::CompareListItems(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
    BOOL bSortAscending = (lParamSort > 0);
    int nColumn = abs(lParamSort) - 1;

    fileData* pr1= (fileData*)lParam1;
    fileData* pr2= (fileData*)lParam2;
    
    if ( !bSortAscending ) 
    { // sort descending, just swap
        pr2 = (fileData*)lParam1;
        pr1 = (fileData*)lParam2;
    }
    std::wstring s1,s2;
    switch(nColumn)
    {
        case 1:
            {
                s1 = pr1->fullPath;
                s2 = pr2->fullPath;
            }
            break;
        default:
            s1 = pr1->fileName;
            s2 = pr2->fileName;
    }

    int nRet= 0;
    if ( s1 > s2 ) nRet=  1;
    if ( s1 < s2 ) nRet= -1;
    return( nRet );
}

// sortOrder - 0 neither, 1 ascending, 2 descending
void setListViewSortIcon(HWND listView, int col, int sortOrder)
{
    HWND headerWnd;
    char headerText[MAX_PATH];
    HD_ITEM item;
    int numColumns, curCol;

    headerWnd = ListView_GetHeader(listView);
    numColumns = Header_GetItemCount(headerWnd);

    for (curCol=0; curCol<numColumns; curCol++)
    {
        item.mask = HDI_FORMAT | HDI_TEXT;
        item.pszText = (LPWSTR)headerText;
        item.cchTextMax = MAX_PATH - 1;
        SendMessage(headerWnd, HDM_GETITEM, curCol, (LPARAM)&item);

        if ((sortOrder != 0) && (curCol==col))
        switch (sortOrder)
        {
            case 1:
                item.fmt &= !HDF_SORTUP;
                item.fmt |= HDF_SORTDOWN;
                break;
            case 2:
                item.fmt &= !HDF_SORTDOWN;
                item.fmt |= HDF_SORTUP;
                break;
        }
        else
        {
            item.fmt &= !HDF_SORTUP & !HDF_SORTDOWN;
        }
        item.fmt |= HDF_STRING;
        item.mask = HDI_FORMAT | HDI_TEXT;
        SendMessage(headerWnd, HDM_SETITEM, curCol, (LPARAM)&item);
    }
}

void NavigateToDlg::SortOnColumnClick(LPNMLISTVIEW pLVInfo)
{
    static int nSortColumn = 0;
    static BOOL bSortAscending = TRUE;
    LPARAM lParamSort;
    // get new sort parameters
    if (pLVInfo->iSubItem == nSortColumn)
        bSortAscending = !bSortAscending;
    else
    {
        nSortColumn = pLVInfo->iSubItem;
        bSortAscending = TRUE;
    }

    // combine sort info into a single value we can send to our sort function
    lParamSort = 1 + nSortColumn;
    if (!bSortAscending)
    {
        lParamSort = -lParamSort;
    }

    // sort list
    ListView_SortItems(pLVInfo->hdr.hwndFrom, CompareListItems, lParamSort);
    setListViewSortIcon(pLVInfo->hdr.hwndFrom, nSortColumn, bSortAscending+1);
}

void NavigateToDlg::updateHistory(const std::string &value)
{
    if(!value.empty())
    {
        history.remove(value);
        history.push_front(value);
        //clear history
        SendMessage(hwndGoLineEdit,CB_RESETCONTENT,0,0);
        for (auto& item: history)
        {
            if(!item.empty())
                SendMessage(hwndGoLineEdit,(UINT) CB_ADDSTRING,
                    (WPARAM)SendDlgItemMessage(_hSelf, ID_GOLINE_EDIT, CB_GETCOUNT, 0, 0) ,(LPARAM)NppManager::strToWStr(item).c_str());
        }
    }
}

fileData* NavigateToDlg::getFileByFullPath(std::wstring text)
{
    for (auto& item: fileList)
    {
        if(item.fileName == text || item.fullPath == text)
        {
            return &item;
        }
    }
}

void NavigateToDlg::AddListViewItem(int item, const std::wstring &text)
{
    lis.pszText = (LPWSTR)text.c_str();
    lis.iItem = 0;
    lis.cchTextMax = MAX_PATH;
    lis.iSubItem = item;
    if (item == 0)
    {
        lis.mask = LVIF_TEXT | LVIF_PARAM;
        lis.lParam = (LPARAM)getFileByFullPath(text);
        SendMessage(hwndListView,LVM_INSERTITEM,0,(LPARAM)&lis);// Send info to the Listview)
    }
    else
    {
        lis.mask = LVIF_TEXT;
        SendMessage(hwndListView, LVM_SETITEM, 0, (LPARAM)&lis);
    }
}

void NavigateToDlg::loadFileNamesToList(const std::string &filter)
{
    std::vector<std::string> fileNameList = nppManager->getOpenedFileNames(filter);
    TCHAR toto[10];
    //show count
    ::SetDlgItemText(_hSelf, ID_UGO_RESULTS_CNT_LABEL, generic_itoa(fileNameList.size(), toto, 10));

    //reset list
	SendMessage(hwndListView, LVM_DELETEALLITEMS,0,0);
    fileList.clear();
    if(fileNameList.size() == 0)
    {
        return;
    }
    //add filenames
    for(std::vector<std::string>::iterator it = fileNameList.begin(); it != fileNameList.end(); ++it) 
    {
        std::wstring fullPath = NppManager::strToWStr(*it);
        fileData file;
        file.fullPath = fullPath;
        file.fileName = (LPWSTR)PathFindFileName(fullPath.c_str());
        //add to list
        if(!file.fullPath.empty() && !file.fileName.empty())
        {
            fileList.push_back(file);
            AddListViewItem(NAME_COLUMN, file.fileName);
            AddListViewItem(PATH_COLUMN, file.fullPath);
        }
    }
}

int NavigateToDlg::getSelectedFileId()
{
    HWND hListBox = GetDlgItem(_hSelf, IDC_RESULTS_LIST);
    int iSelect = SendMessage(hListBox,LVM_GETNEXTITEM,
        -1,LVNI_FOCUSED); // return item selected

    if(iSelect==-1) // no items
    {
        nppManager->showMessageBox("Please, select file from the list.");
    }
	return iSelect;
}

void NavigateToDlg::openSelectedFile()
{
    LVITEM LvItem;
    char filePath[MAX_PATH]={0};
	HWND hListBox = GetDlgItem(_hSelf, IDC_RESULTS_LIST);
	int pos = getSelectedFileId();
    if(pos == -1)
        return;
    memset(&LvItem,0,sizeof(LvItem));
    LvItem.mask=LVIF_TEXT;
    LvItem.iSubItem=1;
    LvItem.pszText=(LPWSTR)filePath;
    LvItem.cchTextMax=MAX_PATH;
    LvItem.iItem=pos;

    SendMessage(hListBox,LVM_GETITEMTEXT, 
                pos, (LPARAM)&LvItem);
    nppManager->switchToFile(NppManager::wStrToStr(LvItem.pszText));
	::SetFocus(hwndListView);
    //save history
    updateHistory(NppManager::wStrToStr(PathFindFileName(LvItem.pszText)));
    hide();
}

void NavigateToDlg::beNotified(SCNotification *notifyCode)
{
    switch (notifyCode->nmhdr.code) 
	{
		case NPPN_FILEOPENED:
        case NPPN_FILECLOSED:
        case NPPN_FILERENAMED:
        case NPPN_FILEDELETED:
            refreshResultsList();
		break;
            break;
		default:
			return;
	}
}