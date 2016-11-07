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

void NavigateToDlg::loadFileNamesToList(std::string filter)
{
	int nbFile = (int)::SendMessage(nppData._nppHandle, NPPM_GETNBOPENFILES, 0, 0);
	if(nbFile == 0)
		return;
	
	TCHAR **fileNames = (TCHAR **)new TCHAR*[nbFile];
	for (int i = 0 ; i < nbFile ; i++)
	{
		fileNames[i] = new TCHAR[MAX_PATH];
	}

	if (::SendMessage(nppData._nppHandle, NPPM_GETOPENFILENAMES, (WPARAM)fileNames, (LPARAM)nbFile))
	{ 
		HWND hwndListBox = ::GetDlgItem(_hSelf, IDC_RESULTS_LIST);  
		SendMessage(hwndListBox, LB_RESETCONTENT, 0, 0);
		int filesCount = nbFile;
		for (int i = 0 ; i < nbFile ; i++)
		{
			std::wstring temp(&fileNames[i][0]); //convert to wstring
			std::string fileName(temp.begin(), temp.end()); //and convert to string.
			std::transform(fileName.begin(), fileName.end(), fileName.begin(), ::tolower);
			std::transform(filter.begin(), filter.end(), filter.begin(), ::tolower);
			std::size_t found = fileName.find(filter);

			if(filter.empty() || found!=std::string::npos)
			{
				SendMessage(hwndListBox, LB_ADDSTRING, 0, (LPARAM)fileNames[i]);
			}
			else
			{
				--filesCount;
			}
		}
		TCHAR toto[10];
		::SetDlgItemText(_hSelf, ID_UGO_RESULTS_CNT_LABEL, generic_itoa(filesCount, toto, 10));
	}

	for (int i = 0 ; i < nbFile ; i++)
	{
		delete [] fileNames[i];
	}
	delete [] fileNames;
}

void NavigateToDlg::gotToLine(int line)
{
	if (line != -1)
	{
		// Get the current scintilla
		int which = -1;
		::SendMessage(nppData._nppHandle, NPPM_GETCURRENTSCINTILLA, 0, (LPARAM)&which);
		if (which == -1)
			return;
		HWND curScintilla = (which == 0)?nppData._scintillaMainHandle:nppData._scintillaSecondHandle;

		::SendMessage(curScintilla, SCI_ENSUREVISIBLE, line-1, 0);
		::SendMessage(curScintilla, SCI_GOTOLINE, line-1, 0);
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
	TCHAR currentPath[MAX_PATH];
	HWND hListBox = GetDlgItem(_hSelf, IDC_RESULTS_LIST);
	int pos = getSelectedFileId();
	SendMessage(hListBox, LB_GETTEXT, pos, (LPARAM)filePath);	
	::SendMessage(nppData._nppHandle, NPPM_GETFULLCURRENTPATH, 0, (LPARAM)currentPath);
	if(_tcscmp(filePath, currentPath) != 0)
	{
		//::MessageBox(nppData._nppHandle, filePath , TEXT("selected file path"), MB_OK);
		::SendMessage(nppData._nppHandle, NPPM_SWITCHTOFILE, 0, (LPARAM)filePath);
	}
	::SetFocus(::GetDlgItem(_hSelf, IDC_RESULTS_LIST));
}