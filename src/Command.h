#pragma once
#include <string>
#include <vector>
#include <algorithm>
#include "NppManager.h"

class Command
{
    NppManager *mgr;
    std::string cmd;
    void(NppManager:: *method)();
public:
    Command(NppManager *obj, std::string strName, std::string strCMD, void(NppManager:: *meth)() = 0)
    {
        mgr = obj; // the argument's name is "meth"
        method = meth;
        cmd = strCMD;
    }
    void execute()
    {
        (mgr->*method)(); // invoke the method on the object
    }
    ~Command(void);
};

