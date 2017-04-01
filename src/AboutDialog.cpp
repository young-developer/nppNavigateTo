#include "stdafx.h"
#include "AboutDialog.h"
#include "DockingFeature\resource.h" 

AboutDialog::AboutDialog(void):StaticDialog()
{
}

AboutDialog::~AboutDialog(void)
{
}

void AboutDialog::init(HINSTANCE hInst, HWND parent)
{
    Window::init(hInst, parent);
};

void AboutDialog::doDialog()
{
    if (!isCreated())
        create(IDD_ABOUTDIALOG);

	goToCenter();
    display();
}

INT_PTR CALLBACK AboutDialog::run_dlgProc(UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message) 
	{
        case WM_INITDIALOG :
		{
			return TRUE;
		}
		case WM_COMMAND : 
		{
				switch (wParam)
				{
					case IDOK :
					case IDCANCEL :
						display(FALSE);
						return TRUE;
					default :
						break;
				}
		}
            default :
                return FALSE;
	}
	return FALSE;
}