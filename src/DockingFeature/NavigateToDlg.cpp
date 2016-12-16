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

typedef HRESULT (WINAPI *SHAUTOCOMPLETEFN) 
    (HWND hTarget, DWORD dwFlags);
static HINSTANCE hSHLWAPIDLL = NULL;

bool SetupForAutoComplete(HWND hTarget, DWORD dwFlags)
{
    if (hTarget == NULL)
    {
        return false;
    }

    bool ok = false;

    if (hSHLWAPIDLL == NULL)
    {
        hSHLWAPIDLL= LoadLibrary(TEXT("SHLWAPI.DLL"));
        if (hSHLWAPIDLL== NULL)
        {
            return false;
        }
    }

    SHAUTOCOMPLETEFN pSHAC = 
        (SHAUTOCOMPLETEFN)GetProcAddress(hSHLWAPIDLL, 
        "SHAutoComplete");

    if (pSHAC != NULL)
    {
        ok = SUCCEEDED(pSHAC(hTarget, dwFlags));
    }

    return ok;
}

INT_PTR CALLBACK NavigateToDlg::run_dlgProc(UINT message, WPARAM wParam, LPARAM lParam)
{
	if (GetAsyncKeyState(VK_ESCAPE) < 0) {
		display(false);
	}
	
	if (((GetAsyncKeyState(VK_DOWN) < 0) || (GetAsyncKeyState(VK_UP) < 0)) && ::GetFocus() != ::GetDlgItem(_hSelf, IDC_RESULTS_LIST))
	{
		::SetFocus(::GetDlgItem(_hSelf, IDC_RESULTS_LIST));
	}
    
	switch (message) 
	{
		case WM_COMMAND : 
		{
			switch (wParam)
			{
				case IDOK :
				{
                    std::string editValue = getFilterEditValue();
                    /*
                    
                    cmdParser.parse(editValue);
                    if(cmdParser.cmdDetected())
                    {
                        cmdParser.execute();
                    }
                    else
                    */
					openSelectedFile();
                    //save history
                    SendMessage(::GetDlgItem(_hSelf, ID_GOLINE_EDIT),(UINT) CB_ADDSTRING,(WPARAM) 0,(LPARAM)NppManager::strToWStr(editValue).c_str()); 
					return TRUE;
				}
			}
			
			if((HIWORD(wParam) == CBN_EDITCHANGE || HIWORD(wParam) == CBN_SELCHANGE) && (LOWORD(wParam) == ID_GOLINE_EDIT))   //your edit control ID
			{
				loadFileNamesToList(getFilterEditValue());
				HWND hwndListBox = ::GetDlgItem(_hSelf, IDC_RESULTS_LIST);  
				SendMessage(hwndListBox, LB_SETCURSEL, 0, 0);
			}
			
			switch (LOWORD(wParam))
			{
				case IDC_RESULTS_LIST:
				{
					switch (HIWORD(wParam))
					 {
						case LBN_DBLCLK: 
						{
							openSelectedFile();
						 }
						 break;
					 }
					
				}
			}
			return FALSE;
		}
        case WM_INITDIALOG:
        {
            memset(&LvCol,0,sizeof(LvCol));                  // Zero Members
            LvCol.mask=LVCF_TEXT|LVCF_WIDTH|LVCF_SUBITEM;    // Type of mask
            LvCol.cx=0x28;                                   // width between each coloum
            LvCol.pszText=TEXT("Name");                            // First Header Text
            LvCol.cx=0x81;                                   // width of column
            SendMessage(::GetDlgItem(_hSelf, IDC_RESULTS_LIST),LVM_SETEXTENDEDLISTVIEWSTYLE,
               0,LVS_EX_FULLROWSELECT); // Set style
            SendMessage(::GetDlgItem(_hSelf, IDC_RESULTS_LIST),LVM_INSERTCOLUMN,0,(LPARAM)&LvCol); // Insert/Show the coloum
            LvCol.pszText=TEXT("Path");                            // Next coloum
            SendMessage(::GetDlgItem(_hSelf, IDC_RESULTS_LIST),LVM_INSERTCOLUMN,1,(LPARAM)&LvCol); // ...
            LvCol.pszText=TEXT("Type");                            //
            SendMessage(::GetDlgItem(_hSelf, IDC_RESULTS_LIST),LVM_INSERTCOLUMN,2,(LPARAM)&LvCol); //

            if (cmdComboBoxEdit)
            {
                // enable auto complete for m_myCombo
                SetupForAutoComplete(cmdComboBoxEdit->m_hWnd, SHACF_USETAB);
            }
        }
        case WM_NOTIFY:
        {
            switch(LOWORD(wParam))
            {
                case IDC_RESULTS_LIST: 
                if(((LPNMHDR)lParam)->code == NM_DBLCLK)
                {
                    openSelectedFile();
                }
            }
        }
		default :
			return DockingDlgInterface::run_dlgProc(message, wParam, lParam);
	}
}

void NavigateToDlg::loadFileNamesToList(const std::string &filter)
{
    std::vector<std::string> fileNameList = nppManager->getOpenedFileNames(filter);
    TCHAR toto[10];
    //show count
    ::SetDlgItemText(_hSelf, ID_UGO_RESULTS_CNT_LABEL, generic_itoa(fileNameList.size(), toto, 10));

    //reset list
    HWND hwndListBox = ::GetDlgItem(_hSelf, IDC_RESULTS_LIST);  
	SendMessage(hwndListBox, LVM_DELETEALLITEMS,0,0);
    if(fileNameList.size() == 0)
    {
        return;
    }
    //add filenames
    for(std::vector<std::string>::iterator it = fileNameList.begin(); it != fileNameList.end(); ++it) 
    {
        std::wstring fileName = NppManager::strToWStr(*it);
        //add to list
        if(!fileName.empty())
        {
            memset(&LvItem,0,sizeof(LvItem)); // Zero struct's Members

            //  Setting properties Of members:

            LvItem.mask=LVIF_TEXT;   // Text Style
            LvItem.cchTextMax = 256; // Max size of test
            LvItem.iItem=0;          // choose item  
            LvItem.iSubItem=0;       // Put in first coluom
            LvItem.pszText=(LPWSTR)fileName.c_str(); // Text to display (can be from a char variable) (Items)
            SendMessage(hwndListBox,LVM_INSERTITEM,0,(LPARAM)&LvItem); // Send info to the Listview
            LvItem.iSubItem=1;
            LvItem.pszText=(LPWSTR)fileName.c_str();
            SendMessage(hwndListBox,LVM_SETITEM,0,(LPARAM)&LvItem); // Enter text to SubItems
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
        nppManager->showMessageBox("Sorry... There is no files to open.");
    }
	return iSelect;
}

void NavigateToDlg::openSelectedFile()
{
    char filePath[MAX_PATH]={0};
	HWND hListBox = GetDlgItem(_hSelf, IDC_RESULTS_LIST);
	int pos = getSelectedFileId();
    memset(&LvItem,0,sizeof(LvItem));
    LvItem.mask=LVIF_TEXT;
    LvItem.iSubItem=0;
    LvItem.pszText=(LPWSTR)filePath;
    LvItem.cchTextMax=256;
    LvItem.iItem=pos;

    SendMessage(hListBox,LVM_GETITEMTEXT, 
                pos, (LPARAM)&LvItem);
    bool isSwitched = nppManager->switchToFile(NppManager::wStrToStr(LvItem.pszText));
	::SetFocus(::GetDlgItem(_hSelf, IDC_RESULTS_LIST));
    
    if(isSwitched)
        display(false);
}