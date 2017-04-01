//this file is part of notepad++
//Copyright (C)2003 Don HO <donho@altern.org>
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
#include "PluginDefinition.h"
#include "menuCmdID.h"
#include <ShellAPI.h>
#include "AboutDialog.h"
#include "ScintillaGateway.h"

//
// put the headers you need here
//
#include <stdlib.h>
#include <time.h>
#include <shlwapi.h>
#include "DockingFeature/NavigateToDlg.h"

NavigateToDlg _navigateToForm;
AboutDialog _aboutDlg;
static HANDLE _hModule; // For dialog initialization

#ifdef UNICODE 
	#define generic_itoa _itow
#else
	#define generic_itoa itoa
#endif

FuncItem funcItem[nbFunc];

//
// The data of Notepad++ that you can use in your plugin commands
//
NppData nppData;

#define NAVIGATETO_FORM_INDEX 0
#define FIRST_SEPARATOR 1
#define CHECK_UPDATES_INDEX 2
#define ABOUT_FORM_INDEX 3
//
// Initialize your plugin data here
// It will be called while plugin loading   
void pluginInit(HANDLE hModule)
{
    _hModule = hModule;
}

//
// Here you can do the clean up, save the parameters (if any) for the next session
//
void pluginCleanUp()
{
    
}

//
// Initialization of your plugin commands
// You should fill your plugins commands here
void commandMenuInit()
{ 
    //ShortCut CTRL+,
    ShortcutKey *dialogShKey = new ShortcutKey;
    dialogShKey->_isAlt = false;
    dialogShKey->_isCtrl = true;
    dialogShKey->_isShift = false;
    dialogShKey->_key = VK_OEM_COMMA;

    setCommand(NAVIGATETO_FORM_INDEX, TEXT("Tabs"), NavigateToDlgForm, dialogShKey, false);
    setCommand(FIRST_SEPARATOR, TEXT("---"), NULL, NULL, false);
    setCommand(CHECK_UPDATES_INDEX, TEXT("Check for update"), CheckUpdates, 0, false);
    setCommand(ABOUT_FORM_INDEX, TEXT("About"), ShowAboutDlgForm, 0, false);
}


//
// Here you can do the clean up (especially for the shortcut)
//
void commandMenuCleanUp()
{
	// Don't forget to deallocate your shortcut here
	delete funcItem[NAVIGATETO_FORM_INDEX]._pShKey;
}

//
// This function help you to initialize your plugin commands
//
bool setCommand(size_t index, TCHAR *cmdName, PFUNCPLUGINCMD pFunc, ShortcutKey *sk, bool check0nInit) 
{
    if (index >= nbFunc)
        return false;

    if (!pFunc)
        return false;

    lstrcpy(funcItem[index]._itemName, cmdName);
    funcItem[index]._pFunc = pFunc;
    funcItem[index]._init2Check = check0nInit;
    funcItem[index]._pShKey = sk;

    return true;
}

// NavigateTo Dlg
// 
// - please see NavigateToDlg.h and NavigateToDlg.cpp to have more informations.
void NavigateToDlgForm()
{
    _navigateToForm.doDialog();
}

void ShowAboutDlgForm()
{
    _aboutDlg.doDialog();
}

void CheckUpdates()
{
    //open releases
    ShellExecute(0, 0, L"https://github.com/young-developer/nppNavigateTo/releases", 0, 0 , SW_SHOW );
}

void setNppInfo(NppData notpadPlusData)
{
    nppData = notpadPlusData;
    _navigateToForm.init((HINSTANCE)_hModule, nppData);
    _aboutDlg.init((HINSTANCE)_hModule, nppData._nppHandle);
	commandMenuInit();
}