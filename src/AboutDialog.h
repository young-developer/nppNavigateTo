#pragma once

#include "DockingFeature\StaticDialog.h"

class AboutDialog : public StaticDialog
{
public:
	AboutDialog(void);
	~AboutDialog(void);
	void init(HINSTANCE hInst, HWND parent);
    void doDialog();
protected:
    virtual INT_PTR CALLBACK run_dlgProc(UINT message, WPARAM wParam, LPARAM lParam);
};
