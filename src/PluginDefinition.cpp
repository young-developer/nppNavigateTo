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

#include "PluginDefinition.h"
#include "menuCmdID.h"

//
// put the headers you need here
//
#include <stdlib.h>
#include <time.h>
#include <shlwapi.h>
#include "DockingFeature/NavigateToDlg.h"

const TCHAR sectionName[] = TEXT("Bool Param Section");
const TCHAR keyName[] = TEXT("boolParam");
const TCHAR configFileName[] = TEXT("navigateTo.ini");

NavigateToDlg _navigateToForm;

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


TCHAR iniFilePath[MAX_PATH];
bool doCloseTag = false;

#define NAVIGATETO_FORM_INDEX 0

//
// Initialize your plugin data here
// It will be called while plugin loading   
void pluginInit(HANDLE hModule)
{
    // Initialize navigateto dialog
    _navigateToForm.init((HINSTANCE)hModule, NULL);
}

//
// Here you can do the clean up, save the parameters (if any) for the next session
//
void pluginCleanUp()
{
     ::WritePrivateProfileString(sectionName, keyName, doCloseTag?TEXT("1"):TEXT("0"), iniFilePath);
}

//
// Initialization of your plugin commands
// You should fill your plugins commands here
void commandMenuInit()
{
    //
    // Firstly we get the parameters from your plugin config file (if any)
    //
    // get path of plugin configuration
    ::SendMessage(nppData._nppHandle, NPPM_GETPLUGINSCONFIGDIR, MAX_PATH, (LPARAM)iniFilePath);

    // if config path doesn't exist, we create it
    if (PathFileExists(iniFilePath) == FALSE)
    {
            ::CreateDirectory(iniFilePath, NULL);
    }

    // make your plugin config file full file path name
    PathAppend(iniFilePath, configFileName);

    // get the parameter value from plugin config
    doCloseTag = (::GetPrivateProfileInt(sectionName, keyName, 0, iniFilePath) != 0);

    //--------------------------------------------//
    //-- STEP 3. CUSTOMIZE YOUR PLUGIN COMMANDS --//
    //--------------------------------------------//
    // with function :
    // setCommand(int index,                      // zero based number to indicate the order of command
    //            TCHAR *commandName,             // the command name that you want to see in plugin menu
    //            PFUNCPLUGINCMD functionPointer, // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
    //            ShortcutKey *shortcut,          // optional. Define a shortcut to trigger this command
    //            bool check0nInit                // optional. Make this menu item be checked visually
    //            );
   
    //ShortCut CTRL+,
    ShortcutKey *dialogShKey = new ShortcutKey;
    dialogShKey->_isAlt = false;
    dialogShKey->_isCtrl = true;
    dialogShKey->_isShift = false;
    dialogShKey->_key = VK_OEM_COMMA;

    setCommand(NAVIGATETO_FORM_INDEX, TEXT("Show form"), NavigateToDlgForm, dialogShKey, false);
}


//
// Here you can do the clean up (especially for the shortcut)
//
void commandMenuCleanUp()
{
	// Don't forget to deallocate your shortcut here
	delete funcItem[NAVIGATETO_FORM_INDEX]._pShKey;
}

//----------------------------------------------//
//-- STEP 4. DEFINE YOUR ASSOCIATED FUNCTIONS --//
//----------------------------------------------//
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

// Dockable NavigateTo Dlg
// 
// You have to create your dialog by inherented DockingDlgInterface class in order to make your dialog dockable
// - please see NavigateToDlg.h and NavigateToDlg.cpp to have more informations.
void NavigateToDlgForm()
{
        _navigateToForm.setParent(nppData._nppHandle);
        tTbData	data = {0};

        if (!_navigateToForm.isCreated())
        {
                _navigateToForm.create(&data);

                // define the default floating behaviour
                data.uMask = DWS_DF_FLOATING;

                data.pszModuleName = _navigateToForm.getPluginFileName();

                // the dlgDlg should be the index of funcItem where the current function pointer is
                // in this case is NAVIGATETO_FORM_INDEX
                data.dlgID = NAVIGATETO_FORM_INDEX;
                ::SendMessage(nppData._nppHandle, NPPM_DMMREGASDCKDLG, 0, (LPARAM)&data);
        }
        _navigateToForm.display();
}

