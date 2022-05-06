//this file is part of notepad++
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
#include "stdafx.h"      //Pre-compiled header for compiler
#include "NavigateToDlg.h"
#include "../PluginDefinition.h"
#include "../File.h"
#include "Windowsx.h"
#include <string>
#include <algorithm>

//////////////////
// Window map tells CWinMgr how to position dialog controls
//
BEGIN_WINDOW_MAP(WindowsDlgMap)
	BEGINROWS(WRCT_REST,0,RCMARGINS(2,2))
		BEGINCOLS(WRCT_REST,0,0)                       // Begin list control column
			BEGINROWS(WRCT_REST,0,0)
                RCSPACE(12)
                RCTOFIT(ID_GOLINE_EDIT)
                RCSPACE(12)
				RCREST(IDC_RESULTS_LIST)	
			ENDGROUP()
		ENDGROUP()
	ENDGROUP()
END_WINDOW_MAP()
//END OF MAP

#ifdef UNICODE 
	#define generic_itoa _itow
	typedef std::wstring String; 
#else
	#define generic_itoa itoa
	typedef std::string String; 
#endif

extern NppData nppData;

#define NAME_COLUMN 0
#define PATH_COLUMN 1

WNDPROC g_oldComboBoxProc = 0; //  Original wndproc for the combo box 
WNDPROC g_oldListProc = 0; //  Original wndproc for the list of files
WNDPROC g_oldComboProc = 0;
HWND ghwndListView;
std::wstring searchString;


LRESULT CALLBACK listProc (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK ComboBoxProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK ComboProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

bool isDropDownOpened = false;

std::string GetErrorAsString(DWORD errorMessageID)
{
	//Get the error message, if any.
	if (errorMessageID == 0)
		return std::string(); //No error message has been recorded

	LPSTR messageBuffer = nullptr;
	size_t size = FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL, errorMessageID, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&messageBuffer, 0, NULL);

	std::string message(messageBuffer, size);

	//Free the buffer.
	LocalFree(messageBuffer);

	return message;
}

void moveSelectionUp(BOOL wrap)
{
	
	int currentItem = (int)SendMessage(ghwndListView, LVM_GETNEXTITEM, WPARAM(-1), LVNI_SELECTED);
	if (currentItem == -1)
		currentItem = 0;

	if (currentItem > 0)
	{
	
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, currentItem - 1, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, currentItem - 1, 0);
	}
	else if (wrap)
	{
		int itemCount = (int)::SendMessage(ghwndListView, LVM_GETITEMCOUNT, 0, 0);
	
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, itemCount - 1, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, itemCount - 1, 0);
	}

}

void moveSelectionPageUp()
{
	
	int currentItem = (int)SendMessage(ghwndListView, LVM_GETNEXTITEM, WPARAM(-1), LVNI_SELECTED);
	if (currentItem == -1)
		currentItem = 0;

	int pageItems   = (int)::SendMessage(ghwndListView, LVM_GETCOUNTPERPAGE, 0, 0); 

	if (currentItem >= pageItems)
	{
	
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, currentItem - pageItems, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, currentItem - pageItems, 0);
	}
	else
	{
		
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, 0, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, 0, 0);
	}

}

void moveSelectionDown(BOOL wrap)
{
	
	int currentItem = (int)::SendMessage(ghwndListView, LVM_GETNEXTITEM, WPARAM(-1), LVNI_SELECTED);
	int itemCount   = (int)::SendMessage(ghwndListView, LVM_GETITEMCOUNT, 0, 0);

	if (currentItem < itemCount - 1)
	{
	
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, currentItem + 1, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, currentItem + 1, 0);
	}
	else if (wrap)
	{
		
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, 0, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, 0, 0);
	}


}


void moveSelectionPageDown()
{
	int currentItem = (int)::SendMessage(ghwndListView, LVM_GETNEXTITEM, WPARAM(-1), LVNI_SELECTED);
	int itemCount   = (int)::SendMessage(ghwndListView, LVM_GETITEMCOUNT, 0, 0);
	int pageItems   = (int)::SendMessage(ghwndListView, LVM_GETCOUNTPERPAGE, 0, 0);

	if (currentItem < itemCount - pageItems)
	{
	
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, currentItem + pageItems, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, currentItem + pageItems, 0);
	}
	else 
	{
		
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, itemCount - 1, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, itemCount - 1, 0);
	}
}

void moveSelectionBottom(void)
{
	int itemCount = (int)::SendMessage(ghwndListView, LVM_GETITEMCOUNT, 0, 0);
	
	if (itemCount > 0)
	{
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, itemCount - 1, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, itemCount - 1, 0);
	}


}

void moveSelectionTop(void)
{
	int itemCount = (int)::SendMessage(ghwndListView, LVM_GETITEMCOUNT, 0, 0);
	
	if (itemCount > 0)
	{
		LVITEM lvi;
		lvi.stateMask = LVIS_SELECTED | LVIS_FOCUSED;
		lvi.state     = LVIS_SELECTED | LVIS_FOCUSED;
		::SendMessage(ghwndListView, LVM_SETITEMSTATE, 0, reinterpret_cast<LPARAM>(&lvi));
		::SendMessage(ghwndListView, LVM_ENSUREVISIBLE, 0, 0);
	}
}

NavigateToDlg::NavigateToDlg() : SizeableDlg(WindowsDlgMap)
{
    history.resize(10);
    isDropDownOpened = false;
};

void NavigateToDlg::doDialog()
{
    if (!isCreated())
    {
        create(IDD_NAVIGATETO_FORM);
    }
    display();
    refreshResultsList();
}

INT_PTR CALLBACK NavigateToDlg::run_dlgProc(UINT message, WPARAM wParam, LPARAM lParam)
{    
	switch (message) 
	{
        case WM_SETFOCUS:
        {
            HWND hwndEdit = GetWindow(hwndGoLineEdit, GW_CHILD);
            PostMessage(hwndEdit, EM_SETSEL, 0, -1);

            return TRUE;
        }

        case WM_ACTIVATE:
        {
#ifndef _DEBUG
            if (LOWORD(wParam) == WA_INACTIVE && isVisible() && !isDropDownOpened)
			{
                hide();
			}
            isDropDownOpened = false;
#endif
            return TRUE;
        }

        case WM_KEYDOWN :
		{
			switch (wParam)
			{
                case VK_ESCAPE:
				{
					hide();
					return TRUE;
				}
				case VK_DOWN:
				{
					moveSelectionDown(TRUE);
					return TRUE;
				}
				case VK_UP:
				{
					moveSelectionUp(TRUE);
					return TRUE;
				}
                case VK_LEFT:
                case VK_RIGHT:
                {
                    /*if(wParam == VK_LEFT)
                    {
                        //Open in first view
                        File* selFile = getSelectedFile();
                        if(selFile!=nullptr)
                            nppManager->switchToFile(selFile->getIndex(),MAIN_VIEW);
                    }
                    else if(wParam == VK_RIGHT)
                    {
                         //open in second view
                        File* selFile = getSelectedFile();
                        if(selFile!=nullptr)
                            nppManager->switchToFile(selFile->getIndex(),SECOND_VIEW);
                    }*/
                }
                break;
				case VK_NEXT:
				{
					moveSelectionPageDown();
					return TRUE;
				}
				
				case VK_PRIOR:
				{
					moveSelectionPageUp();
					return TRUE;
				}
				case VK_HOME:
				{
					moveSelectionTop();
					return TRUE;
				}
				case VK_END:
				{
					moveSelectionBottom();
					return TRUE;
				}

				case VK_TAB:
				{
					return TRUE;
					break;
				}
                case VK_RETURN:
                {
                    openSelectedFile();
                    break;
                }

				default:
                {
                    int result;
				    BYTE keyState[256];
						
				    ::GetKeyboardState((PBYTE)&keyState);
				    keyState[VK_CONTROL] = 0;
				    WORD buffer;
						
				    result = ::ToAscii((UINT)wParam, (lParam & 0x00FF0000) >> 16, (const BYTE *)&keyState, &buffer, 0);
				    if (1 == result)
				    {
					    char temp[2];
					    temp[0] = (char)buffer;
                        temp[1] = '\0';
						::SetDlgItemTextA(_hSelf, ID_GOLINE_EDIT, temp);
						::SendDlgItemMessage(_hSelf, ID_GOLINE_EDIT, EM_SETSEL, 1, 1);
						::SetFocus(GetDlgItem(_hSelf, ID_GOLINE_EDIT));
                    }
                }
				break;
			}
			break;
        }
		case WM_COMMAND : 
		{
			switch (wParam)
			{
				case IDOK :
				{
					openSelectedFile();
					return TRUE;
				}
                case IDCANCEL:
                {
                    if(!::isDropDownOpened)
                    {
                        hide();
                        return TRUE;
                    }
                }
                break;
			}
            switch(LOWORD(wParam))
            {
                case ID_GOLINE_EDIT: // If the combo box sent the message,
                {
                    switch(HIWORD(wParam))
                    {
                        case CBN_DROPDOWN: // This means that the list is about to display
                        {
                            isDropDownOpened = ::isDropDownOpened = true;
                            return TRUE;
                        }
                        break;
                        case CBN_CLOSEUP:
                        {
                            ::isDropDownOpened = isDropDownOpened = false;
                            return TRUE;
                        }
                        break;
                        case CBN_SELCHANGE:
                        {
                            int idx_row;
                            TCHAR strText[MAX_PATH];
	                        idx_row = (int)SendMessage(hwndGoLineEdit , CB_GETCURSEL, 0, 0 );
	                        SendMessage(hwndGoLineEdit,CB_GETLBTEXT, idx_row,(LPARAM)strText);
                            loadFileNamesToList(std::wstring(&strText[0]));
                            selectFirstRowInList();
                        }
                        break;
                        case CBN_EDITCHANGE:
                        {
                            refreshResultsList();
                            return TRUE;
                        }
                        break;
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
            _winMgr.InitToFitSizeFromCurrent(_hSelf);
            ghwndListView = hwndListView = ::GetDlgItem(_hSelf, IDC_RESULTS_LIST);
            hwndGoLineEdit = ::GetDlgItem(_hSelf, ID_GOLINE_EDIT);
            if (NULL != hwndGoLineEdit)
			{
                //  Get the edit window handle to each combo box. 
                HWND hwndEdit = GetWindow(hwndGoLineEdit, GW_CHILD);
                Edit_SetCueBannerTextFocused(hwndEdit,TEXT(" Filter by Name, Path. Put a | at the beginning of the line to invert the whole filter condition"), true);
                g_oldComboProc = (WNDPROC) SetWindowLongPtr(hwndEdit, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(ComboProc));
                g_oldComboBoxProc = (WNDPROC) SetWindowLongPtr(hwndEdit, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(ComboBoxProc));
			}
            
            if(NULL != hwndListView)
            {
                g_oldListProc = (WNDPROC) ::SetWindowLongPtr(hwndListView, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(listProc));
            }

            _winMgr.CalcLayout(_hSelf);
	        _winMgr.SetWindowPositions(_hSelf);
			if (NULL != hwndListView)
			{
				RECT rc;
				GetClientRect(hwndListView, &rc);
				LONG width = rc.right - rc.left;
				LVCOLUMN LvCol;
				memset(&LvCol, 0, sizeof(LvCol));                  // Zero Members
				LvCol.mask = LVCF_FMT | LVCF_WIDTH | LVCF_TEXT | LVCF_SUBITEM;  // Type of mask
				LvCol.cx = static_cast<LONG>(width*0.4);                                   // width between each coloum
				LvCol.pszText = TEXT("Name");                            // First Header Text
				::SendMessage(hwndListView, LVM_SETEXTENDEDLISTVIEWSTYLE, WPARAM(0), LVS_EX_FULLROWSELECT); // Set style
				::SendMessage(hwndListView, LVM_INSERTCOLUMN, NAME_COLUMN, (LPARAM)&LvCol); // Insert/Show the coloum
				LvCol.pszText = TEXT("Path");
				LvCol.cx = static_cast<LONG>(width*0.6);                                   // width of column
				SendMessage(hwndListView, LVM_INSERTCOLUMN, PATH_COLUMN, (LPARAM)&LvCol); // ...

				SendMessage(hwndGoLineEdit, CB_SETMINVISIBLE, 8, 0); // Visible items = 8
				//ListView_SetExtendedListViewStyle(hwndListView,LVS_EX_HEADERDRAGDROP); //now we can drag&drop headers :)
				refreshResultsList();
				goToCenter();
				fitColumnsToSize();
				SetFocus(hwndListView);
			}
			else
			{
				nppManager->showMessageBox(TEXT("init dialog list is nullpointer"));
			}
            
			return SizeableDlg::run_dlgProc(message, wParam, lParam);
        }
        break;
        case WM_DESTROY:
		{
			destroy();
			return TRUE;
		}
        case WM_CLOSE:
        {
            hide();
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
                    if(pNMHDR->code == NM_CLICK)
                    {
                        ::SetFocus(GetDlgItem(_hSelf, ID_GOLINE_EDIT));
                        SendMessage(GetWindow(hwndGoLineEdit, GW_CHILD),EM_SETSEL,WPARAM(0),-1);
                        SendMessage(GetWindow(hwndGoLineEdit, GW_CHILD),EM_SETSEL,WPARAM(-1),-1);
                    }
                    if(pNMHDR->code == NM_RCLICK)
                    {
                        isDropDownOpened = true;
                        File* selFile = getSelectedFile();
                        if(selFile !=nullptr)
                            nppManager->openContextMenu(selFile->getIndex(), selFile->getView());
                        if(DropDownIsNotOpened())
                        {
                            ::SetFocus(GetDlgItem(_hSelf, ID_GOLINE_EDIT));
                            SendMessage(GetWindow(hwndGoLineEdit, GW_CHILD),EM_SETSEL,WPARAM(0),-1);
                            SendMessage(GetWindow(hwndGoLineEdit, GW_CHILD),EM_SETSEL,WPARAM(-1),-1);
                        }
                    }
                    
                    if(pNMHDR->code == LVN_ITEMCHANGED)
                    {
                        LPNMLVODSTATECHANGE lpStateChange = (LPNMLVODSTATECHANGE) lParam;
                        if ((lpStateChange->uNewState ^  lpStateChange->uOldState) & LVIS_SELECTED) 
                        {
                            SHORT shiftState = ::GetKeyState(VK_SHIFT) & 0x80;
                            if(shiftState)
                            {
                                isDropDownOpened = true;
                                nppManager->switchToFile(getSelectedFile());
                            }
                        }
                        if(DropDownIsNotOpened())
                        {
                            ::SetFocus(GetDlgItem(_hSelf, ID_GOLINE_EDIT));
                            SendMessage(GetWindow(hwndGoLineEdit, GW_CHILD),EM_SETSEL,WPARAM(0),-1);
                            SendMessage(GetWindow(hwndGoLineEdit, GW_CHILD),EM_SETSEL,WPARAM(-1),-1);
                        }
                        return TRUE;
                    }
                   
                    if (pNMHDR->code == LVN_COLUMNCLICK) // sort columns
				    {
                        SortOnColumnClick((LPNMLISTVIEW)lParam);
				    }
                    if(pNMHDR->code == HDN_ITEMCHANGING)
                    {
                        NMHEADER *pHdr = (NMHEADER*) pNMHDR;
                        if ((pHdr->iItem == NAME_COLUMN) &&
                            (pHdr->pitem) &&
                            (pHdr->pitem->mask & HDI_WIDTH))
                        {
                            fitColumnsToSize();
                        }
                    }
                }

                if(((LPNMHDR)lParam)->code == NM_CUSTOMDRAW)
                {
                    SetWindowLongPtr(_hSelf, DWLP_MSGRESULT, 
                        static_cast<LONG_PTR>(ProcessCustomDraw(lParam)));
                   return TRUE;
                }
            }
        }
        break;
		default :
            return SizeableDlg::run_dlgProc(message, wParam, lParam);
	}
    return SizeableDlg::run_dlgProc(message, wParam, lParam);
}

LRESULT ProcessCustomDraw (LPARAM lParam)
{
    LPNMLVCUSTOMDRAW lplvcd = (LPNMLVCUSTOMDRAW)lParam;
    static bool bIsHighlighted = false;
    switch(lplvcd->nmcd.dwDrawStage) 
    {
        case CDDS_PREPAINT : //Before the paint cycle begins
            //request notifications for individual listview items
            return CDRF_NOTIFYITEMDRAW;
            
        case CDDS_ITEMPREPAINT: //Before an item is drawn
        {
            return CDRF_NEWFONT | CDRF_NOTIFYSUBITEMDRAW;
        }
        break;

        //Before a subitem is drawn
        case CDDS_SUBITEM | CDDS_ITEMPREPAINT:
            {
                if (lplvcd->iSubItem == NAME_COLUMN ||  lplvcd->iSubItem == PATH_COLUMN) //detect which subitem is being drawn
                { 
                    File* rowData = (File*)lplvcd->nmcd.lItemlParam;
                    if(rowData != NULL && rowData->isValid())
                    {
                        RECT iR = { 0 };
                        SetBkMode(lplvcd->nmcd.hdc, OPAQUE);
                        ListView_GetSubItemRect(lplvcd->nmcd.hdr.hwndFrom, lplvcd->nmcd.dwItemSpec, lplvcd->iSubItem, LVIR_BOUNDS, &iR);
                        SIZE sz = { 0 };
                        GetTextExtentPoint32(lplvcd->nmcd.hdc, (lplvcd->iSubItem == PATH_COLUMN?rowData->getFullPath().c_str():rowData->getFileName().c_str()), (int)(lplvcd->iSubItem == PATH_COLUMN?rowData->getFullPath():rowData->getFileName()).length(), &sz);
                        if(ListView_GetItemState(ghwndListView,lplvcd->nmcd.dwItemSpec,LVIS_FOCUSED|LVIS_SELECTED) != (LVIS_FOCUSED|LVIS_SELECTED))
                        {
                            switch(rowData->getFileStatus())
                            {
                                case UNSAVED:
                                    SetTextColor(lplvcd->nmcd.hdc, RGB(193, 56, 1));
                                    break;
                                case READONLY:
                                    SetTextColor(lplvcd->nmcd.hdc, RGB(104,104,104));
                                    break;
                                default:
                                    SetTextColor(lplvcd->nmcd.hdc, RGB(0, 0, 0));
                                break;
                            }
                            if (((int)lplvcd->nmcd.dwItemSpec%2)==0) {
                                HBRUSH brush = CreateSolidBrush(RGB(239, 240, 242));
                                FillRect(lplvcd->nmcd.hdc,&iR,brush);
                                SetBkColor(lplvcd->nmcd.hdc, RGB(239, 240, 242));
                                DeleteObject(brush);
                            } else {
                                HBRUSH brush = CreateSolidBrush(RGB(255,255,255));
                                FillRect(lplvcd->nmcd.hdc,&iR,brush);
                                SetBkColor(lplvcd->nmcd.hdc, RGB(255, 255, 255));
                                DeleteObject(brush);
                            }
                        }
                        else
                        {
                            //highlight according to file status
                            switch(rowData->getFileStatus())
                            {
                                case UNSAVED:
                                    SetTextColor(lplvcd->nmcd.hdc, RGB(255, 116, 61));
                                    break;
                                case READONLY:
                                    SetTextColor(lplvcd->nmcd.hdc, RGB(240,240,240));
                                    break;
                                default:
                                    SetTextColor(lplvcd->nmcd.hdc,  RGB(255, 255, 255));
                                break;
                            }
                            SetBkColor(lplvcd->nmcd.hdc, ::GetSysColor(COLOR_HIGHLIGHT));
                            HBRUSH brush = CreateSolidBrush(::GetSysColor(COLOR_HIGHLIGHT));
                            FillRect(lplvcd->nmcd.hdc,&iR,brush);
                            DeleteObject(brush);
                        }
                        
                        iR.left +=5;
                        //draw text according to search
                        std::wstring columnText = (lplvcd->iSubItem == PATH_COLUMN?rowData->getFullPath():rowData->getFileName());
                        std::wstring lowerColumnText = columnText;
                        std::transform(lowerColumnText.begin(), lowerColumnText.end(), lowerColumnText.begin(), ::tolower);
                        std::transform(searchString.begin(), searchString.end(), searchString.begin(), ::tolower);
                        std::size_t found = lowerColumnText.find(searchString);
                        if (found!=std::string::npos && !searchString.empty())
                        {
                            std::wstring start;
                            if(found != 0)
                            {
                                start = columnText.substr (0, found);
                                DrawText(lplvcd->nmcd.hdc, start.c_str(), -1, &iR,  DT_LEFT | DT_NOCLIP | DT_VCENTER | DT_SINGLELINE | DT_END_ELLIPSIS);
                                GetTextExtentPoint32(lplvcd->nmcd.hdc, start.c_str(), (int)start.length(), &sz);
                                iR.left += sz.cx;
                            }
                            //if it is highlighted item
                            if(ListView_GetItemState(ghwndListView,lplvcd->nmcd.dwItemSpec,LVIS_FOCUSED|LVIS_SELECTED) != (LVIS_FOCUSED|LVIS_SELECTED))
                            {
                                SetBkColor(lplvcd->nmcd.hdc, RGB(255,160,122));
                            }
                            else
                            {
                                switch(rowData->getFileStatus())
                                {
                                    case UNSAVED:
                                        SetTextColor(lplvcd->nmcd.hdc, RGB(0, 0, 0));
                                        break;
                                    case READONLY:
                                        SetTextColor(lplvcd->nmcd.hdc, RGB(104,104,104));
                                        break;
                                    default:
                                        SetTextColor(lplvcd->nmcd.hdc, RGB(0, 0, 0));
                                    break;
                                }
                                SetBkColor(lplvcd->nmcd.hdc, RGB(255,160,122));
                            }
                            std::wstring mathString = columnText.substr(found, searchString.length());
                            DrawText(lplvcd->nmcd.hdc, (mathString).c_str(), -1, &iR, DT_LEFT | DT_NOCLIP | DT_VCENTER | DT_SINGLELINE | DT_END_ELLIPSIS);
                            GetTextExtentPoint32(lplvcd->nmcd.hdc, (std::wstring(mathString)).c_str(), (int)(std::wstring(mathString)).length(), &sz);

                            if(found+searchString.length()-1 != columnText.length()-1)
                            {
                                iR.left += sz.cx;
                                std::wstring end = columnText.substr (found+searchString.length());
                                 if(ListView_GetItemState(ghwndListView,lplvcd->nmcd.dwItemSpec,LVIS_FOCUSED|LVIS_SELECTED) != (LVIS_FOCUSED|LVIS_SELECTED))
                                 {
                                    switch(rowData->getFileStatus())
                                    {
                                        case UNSAVED:
                                            SetTextColor(lplvcd->nmcd.hdc, RGB(193, 56, 1));
                                            break;
                                        case READONLY:
                                            SetTextColor(lplvcd->nmcd.hdc, RGB(104,104,104));
                                            break;
                                        default:
                                            SetTextColor(lplvcd->nmcd.hdc, RGB(0, 0, 0));
                                        break;
                                    }
                                    if (((int)lplvcd->nmcd.dwItemSpec%2)==0) {
                                        SetBkColor(lplvcd->nmcd.hdc, RGB(247, 247, 247));
                                    } else {
                                        SetBkColor(lplvcd->nmcd.hdc, ::GetSysColor(COLOR_WINDOW));
                                    }
                                 }
                                 else
                                 {
                                    switch(rowData->getFileStatus())
                                    {
                                        case UNSAVED:
                                            SetTextColor(lplvcd->nmcd.hdc,  RGB(193, 56, 1));
                                            break;
                                        case READONLY:
                                            SetTextColor(lplvcd->nmcd.hdc, RGB(104,104,104));
                                            break;
                                        default:
                                            SetTextColor(lplvcd->nmcd.hdc,  RGB(255, 255, 255));
                                        break;
                                    }
                                     SetBkColor(lplvcd->nmcd.hdc, ::GetSysColor(COLOR_HIGHLIGHT));
                                 }
                                DrawText(lplvcd->nmcd.hdc, end.c_str(), -1, &iR,  DT_LEFT | DT_NOCLIP | DT_VCENTER | DT_SINGLELINE | DT_END_ELLIPSIS);
                            }
                         }
                         else
                         {
                             DrawText(lplvcd->nmcd.hdc, columnText.c_str(), -1, &iR,  DT_LEFT | DT_NOCLIP | DT_VCENTER | DT_SINGLELINE | DT_END_ELLIPSIS);
                         }
                        return CDRF_SKIPDEFAULT;
                    }
                }
                return CDRF_DODEFAULT;
            }
    }
    return CDRF_DODEFAULT;
}

LRESULT CALLBACK ComboBoxProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) 
{ 
    switch (msg) 
    { 
        case WM_KEYDOWN:
            switch (wParam) 
            { 
                case VK_UP:
                case VK_DOWN:
                case VK_HOME:
                case VK_END:
                case VK_TAB:
                case VK_ESCAPE:
                    if(!::isDropDownOpened)
                    {
                        ::SendMessage(GetParent(GetParent(hwnd)), WM_KEYDOWN, wParam, 0);
                        return TRUE;
                    }
                case VK_LEFT:
                case VK_RIGHT:
                {
                    SHORT ctrlState = ::GetKeyState(VK_CONTROL) & 0x80;
                    if(ctrlState)
                    {
                        ::SendMessage(GetParent(GetParent(hwnd)), WM_KEYDOWN, wParam, 0);
                        return TRUE;
                    }
                }  
            }
            break;
        case WM_CHAR:
            switch (wParam) 
            { 
                case VK_ESCAPE:
                    if(!::isDropDownOpened)
                    {
                        ::SendMessage(GetParent(GetParent(hwnd)), WM_KEYDOWN, wParam, 0);
                        return TRUE;
                    }
            }
    } 
 
    //  Call the original window procedure for default processing. 
     return CallWindowProc(g_oldComboBoxProc, hwnd, msg, wParam, lParam); 
}

LRESULT CALLBACK ComboProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) 
{ 
    switch (msg) 
    { 
        case WM_KEYDOWN: 
            switch (wParam) 
            { 
                case VK_UP:
                case VK_DOWN:
                case VK_HOME:
                case VK_END:
                case VK_TAB:
                case VK_ESCAPE:
                    if(!::isDropDownOpened)
                    {
                        ::SendMessage(GetParent(hwnd), WM_KEYDOWN, wParam, 0);
                        return TRUE;
                    }
                case VK_LEFT:
                case VK_RIGHT:
                {
                    SHORT ctrlState = ::GetKeyState(VK_CONTROL) & 0x80;
                    if(ctrlState)
                    {
                        ::SendMessage(GetParent(GetParent(hwnd)), WM_KEYDOWN, wParam, 0);
                        return TRUE;
                    }
                }  
            }
            break;
            case WM_CHAR:
            switch (wParam) 
            { 
                case VK_ESCAPE:
                    if(!::isDropDownOpened)
                    {
                        ::SendMessage(GetParent(GetParent(hwnd)), WM_KEYDOWN, wParam, 0);
                        return TRUE;
                    }
            }
            break;
    }
 
    //  Call the original window procedure for default processing. 
     return CallWindowProc(g_oldComboProc, hwnd, msg, wParam, lParam); 
}

LRESULT CALLBACK listProc (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
 	switch (message)
	{
		case WM_KEYDOWN:
        {
		    ::SendMessage(GetParent(hwnd), WM_KEYDOWN, wParam, 0);
		    return TRUE;
        }
        break;
		case WM_CHAR:
        switch (wParam) 
        { 
            case VK_ESCAPE:
                if(!::isDropDownOpened)
                {
                    ::SendMessage(GetParent(GetParent(hwnd)), WM_KEYDOWN, wParam, 0);
                    return TRUE;
                }
        }
        break;
		case WM_KILLFOCUS:
			// Ignore the kill focus message, so the highlight bar stays blue
			return TRUE;

		case WM_GETDLGCODE :
		{
			LRESULT dlgCode = CallWindowProc(g_oldListProc, hwnd, message, wParam, lParam);
			dlgCode |= DLGC_WANTTAB;
			return dlgCode;
		}
		break;
		default:
			if (g_oldListProc) 
			{
				return ::CallWindowProc(g_oldListProc, hwnd, message, wParam, lParam);
			}
			else
			{
				return ::DefWindowProc(hwnd, message, wParam, lParam);
			}
	}
    return ::CallWindowProc(g_oldListProc, hwnd, message, wParam, lParam);
}

int CALLBACK NavigateToDlg::CompareListItems(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
    BOOL bSortAscending = (lParamSort > 0);
    int nColumn = (int)(abs(lParamSort)) - 1;

    File* pr1= (File*)lParam1;
    File* pr2= (File*)lParam2;
    
    if ( !bSortAscending ) 
    { // sort descending, just swap
        pr2 = (File*)lParam1;
        pr1 = (File*)lParam2;
    }
    std::wstring s1,s2;
    switch(nColumn)
    {
        case 1:
            {
                s1 = pr1->getFullPath();
                s2 = pr2->getFullPath();
            }
            break;
        default:
            s1 = pr1->getFileName();
            s2 = pr2->getFileName();
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

void NavigateToDlg::updateHistory(const std::wstring &value)
{
    if(!value.empty())
    {
        history.remove(value);
        history.push_front(value);
        //clear history but save old value in edit box
        std::wstring oldValue = getFilterEditValue();
        SendMessage(hwndGoLineEdit,CB_RESETCONTENT,0,0);
        for (auto& item: history)
        {
            if(!item.empty())
                SendMessage(hwndGoLineEdit,(UINT) CB_ADDSTRING,
                    (WPARAM)SendDlgItemMessage(_hSelf, ID_GOLINE_EDIT, CB_GETCOUNT, 0, 0) ,(LPARAM)item.c_str());
        }
        SetWindowText( hwndGoLineEdit, (LPCWSTR)oldValue.c_str() );
    }
}

void NavigateToDlg::addFileToListView(const File& file)
{
    if(file.isValid())
    {
        fileList.insert(std::make_pair(file.getBufferId(), file));
        LVITEM lis;
        lis.iItem = 0;
        lis.cchTextMax = MAX_PATH;
        lis.iSubItem = 0;
        lis.mask = LVIF_PARAM;
        lis.lParam = (LPARAM)&fileList[file.getBufferId()];
		lis.pszText = TEXT("empty");
		try
		{
			if (_hSelf != NULL)
			{
				ghwndListView = ::GetDlgItem(_hSelf, IDC_RESULTS_LIST);
				if (ghwndListView == NULL)
					throw GetLastError();
				int indexOfNewItem = SendMessage(ghwndListView, LVM_INSERTITEM, 0, (LPARAM)&lis);// Send info to the Listview
				if (indexOfNewItem == -1)
					throw GetLastError();
			}
			else
				throw GetLastError();
		}
		catch (DWORD aCause)
		{
			if (!GetErrorAsString(aCause).empty())
			{
				std::string error = GetErrorAsString(aCause).append(" Could not add file ");
				nppManager->showMessageBox(NppManager::strToWStr(error).append(fileList[file.getBufferId()].getFileName()), TEXT("Exception"));
			}
		}
    }
}

void NavigateToDlg::addFileToListView(int bufferID)
{
    TCHAR filePath[MAX_PATH];
	::SendMessage(nppData._nppHandle, NPPM_GETFULLPATHFROMBUFFERID, bufferID, reinterpret_cast<LPARAM>(filePath));
    int index = ::SendMessage(nppData._nppHandle, NPPM_GETPOSFROMBUFFERID, bufferID, MAIN_VIEW);
    if(index != -1)
    {
        File file(filePath, INDEX(index), bufferID, VIEW(index));
        addFileToListView(file);
    }
}

void NavigateToDlg::removeFileFromListView(int bufferID)
{
    ListView_DeleteItem(hwndListView, fileList[bufferID].getIndex());
    fileList.erase(bufferID);
}

bool customNonSensetiveComparator(std::wstring filterString, std::wstring filePath)
{
    bool inverseLogic = false;
    //invert logic search string trimming
    if(filterString[0] == '|')
    {
        inverseLogic = true;
        filterString.erase(0, filterString.find_first_not_of('|'));       //prefixing spaces
        filterString.erase(0, filterString.find_first_not_of(' '));       //prefixing spaces
        filterString.erase(filterString.find_last_not_of(' ')+1);         //surfixing spaces
    }

	std::transform(filePath.begin(), filePath.end(), filePath.begin(), ::tolower);
	std::transform(filterString.begin(), filterString.end(), filterString.begin(), ::tolower);
	std::size_t found = filePath.find(filterString);
    bool subStringFound = inverseLogic ? (found==std::wstring::npos) : (found!=std::wstring::npos);
    //int windowsSearchFound = PathMatchSpec((LPCWSTR)fileName.c_str(),(LPCWSTR)filterString.c_str());
    
    return filterString.empty() || subStringFound;
}

void NavigateToDlg::loadFileNamesToList(const std::wstring &filter)
{
    searchString = filter;
    std::vector<File> loadFileList;
    if(fileList.empty()) //first load from NPP
    {
        loadFileList = nppManager->getOpenedFiles();
    }
    else
    {
        typedef bool (*FilterFunction)(std::wstring filterString, std::wstring filePath);
        FilterFunction filterFunction = customNonSensetiveComparator;
        if(filter.empty())
        {
            for( auto it : fileList) {
                loadFileList.push_back( it.second );
            }
        }
        else
        {
            for(auto file : fileList)
            {
                if((*filterFunction)(filter, file.second.getFullPath()))
                {
                    loadFileList.push_back(file.second);
                }
            }
        }
    }
    //show count
    TCHAR toto[10];
    std::wstring lblResultCnt(TEXT("NavigateTo - Tabs - Found "));
    lblResultCnt.append(generic_itoa((int)loadFileList.size(), toto, 10));
    lblResultCnt.append(TEXT(" matching results"));
    SetWindowText(_hSelf, lblResultCnt.c_str());
    //reset list
	SendMessage(hwndListView, LVM_DELETEALLITEMS,0,0);
    if(loadFileList.size() == 0)
        return;
    
    for(auto file : loadFileList) 
    {
        addFileToListView(file);
    }
}

int NavigateToDlg::getSelectedFileId()
{
    LRESULT iSelect = SendMessage(hwndListView,LVM_GETNEXTITEM,
       WPARAM(-1),LVNI_FOCUSED); // return item selected
	return (int)iSelect;
}

File* NavigateToDlg::getSelectedFile()
{
	LVITEM item;
	item.mask = LVIF_PARAM;
	item.iItem = getSelectedFileId();
	
	if (item.iItem == -1) 
        return nullptr;

	::SendMessage(hwndListView, LVM_GETITEM, 0, reinterpret_cast<LPARAM>(&item));
	return reinterpret_cast<File*>(item.lParam);
}

void NavigateToDlg::openSelectedFile()
{
    File* selFile = getSelectedFile();
    if(selFile != nullptr)
    {
        nppManager->switchToFile(selFile);
        updateHistory(selFile->getFileName());
        hide();
    }
}

void NavigateToDlg::updateFileBufferStatus(int bufferID, FileStatus status)
{
    std::unordered_map<int,File>::iterator foundFile = fileList.find(bufferID);

    if ( foundFile != fileList.end() )
        foundFile->second.setFileStatus(status);
}

void NavigateToDlg::updateCurrentFileStatus(FileStatus status)
{
	int bufferID = ::SendMessage(nppData._nppHandle, NPPM_GETCURRENTBUFFERID, 0, 0);
	updateFileBufferStatus(bufferID, status);
}

void NavigateToDlg::beNotified(SCNotification *notifyCode)
{
	if (_hSelf == NULL)
		return;

    switch (notifyCode->nmhdr.code) 
	{
		case NPPN_FILEOPENED:
        case NPPN_FILERENAMED:
			addFileToListView(notifyCode->nmhdr.idFrom);
            if(isVisible())
                refreshResultsList(false);
		    break;
        case NPPN_FILECLOSED:
        case NPPN_FILEDELETED:
            removeFileFromListView(notifyCode->nmhdr.idFrom);
            if(isVisible())
                refreshResultsList(false);
            break;
        case NPPN_READONLYCHANGED:
		{
			FileStatus newStatus;
			if (notifyCode->nmhdr.idFrom & 1)
			{
				newStatus = READONLY;
			}
			else
			{
				if (notifyCode->nmhdr.idFrom & 2)
					newStatus = UNSAVED;
				else
					newStatus = SAVED;
			}
			updateFileBufferStatus(reinterpret_cast<int>(notifyCode->nmhdr.hwndFrom), newStatus);
		}
		break;
		case SCN_SAVEPOINTLEFT:
			if (notifyCode->nmhdr.hwndFrom == nppData._scintillaMainHandle 
				|| notifyCode->nmhdr.hwndFrom == nppData._scintillaSecondHandle)
			{
				updateCurrentFileStatus(UNSAVED);
                if(isVisible())
                    refreshResultsList(false);
			}
			break;
		case SCN_SAVEPOINTREACHED:
			if (notifyCode->nmhdr.hwndFrom == nppData._scintillaMainHandle 
				|| notifyCode->nmhdr.hwndFrom == nppData._scintillaSecondHandle)
			{
				updateCurrentFileStatus(SAVED);
                if(isVisible())
                    refreshResultsList(false);
			}
			break;
        break;
		default:
			return;
	}
}