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

class NavigateToDlg : public DockingDlgInterface
{
public :
	NavigateToDlg() : DockingDlgInterface(IDD_PLUGINGOLINE_NAVTO)
    {
        nppManager = new NppManager();
    };

    virtual ~NavigateToDlg()
    {
        delete nppManager;
    }

    virtual void display(bool toShow = true) const {
        DockingDlgInterface::display(toShow);
        if (toShow)
            ::SetFocus(::GetDlgItem(_hSelf, ID_GOLINE_EDIT));
    };

	void setParent(HWND parent2set){
		_hParent = parent2set;
	};

protected :
	virtual INT_PTR CALLBACK run_dlgProc(UINT message, WPARAM wParam, LPARAM lParam);
private :
	void loadFileNamesToList(const std::string &filter);
	void openSelectedFile();
	int getSelectedFileId();
    int getLine() const {
        BOOL isSuccessful;
        int line = ::GetDlgItemInt(_hSelf, ID_GOLINE_EDIT, &isSuccessful, FALSE);
        return (isSuccessful?line:-1);
    };

	std::string getFilterEditValue() const 
	{
		TCHAR buf[MAX_PATH];
        GetDlgItemText(_hSelf, ID_GOLINE_EDIT, buf, MAX_PATH);
        return NppManager::wStrToStr(&buf[0]);
    };
private:
    NppManager *nppManager;
};
#endif //GOTILINE_DLG_H
