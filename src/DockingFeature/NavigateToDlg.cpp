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
					return TRUE;
				}
			}
			
			if((HIWORD(wParam) == EN_CHANGE) && (LOWORD(wParam) == ID_GOLINE_EDIT))   //your edit control ID
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
	SendMessage(hwndListBox, LB_RESETCONTENT, 0, 0);
    if(fileNameList.size() == 0)
    {
        return;
    }
    else
    {
        //add filenames
        for(std::vector<std::string>::iterator it = fileNameList.begin(); it != fileNameList.end(); ++it) 
        {
            std::wstring fileName = NppManager::strToWStr(*it);
            //add to list
            if(!fileName.empty())
            {
                SendMessage(hwndListBox, LB_ADDSTRING, 0, (LPARAM)fileName.c_str());
            }
        }
    }
}

int NavigateToDlg::getSelectedFileId()
{
	HWND hListBox = GetDlgItem(_hSelf, IDC_RESULTS_LIST);
	return (int)SendMessage(hListBox, LB_GETCURSEL, 0, 0);
}

void NavigateToDlg::openSelectedFile()
{
	TCHAR filePath[MAX_PATH];
	HWND hListBox = GetDlgItem(_hSelf, IDC_RESULTS_LIST);
	int pos = getSelectedFileId();
	SendMessage(hListBox, LB_GETTEXT, pos, (LPARAM)filePath);
    nppManager->switchToFile(NppManager::wStrToStr(filePath));
	::SetFocus(::GetDlgItem(_hSelf, IDC_RESULTS_LIST));
}