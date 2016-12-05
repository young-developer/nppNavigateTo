#pragma once
#include "Command.h"
#include <string>
#include <vector>
#include <algorithm>

class CommandParser
{
private:
    std::string m_Value;
    std::vector<Command> m_registredCommands;
    std::vector<Command> m_commandsToExec;
public:
    CommandParser(void);
    virtual ~CommandParser(void);
public:
    //gt
    std::vector<Command> RegistredCommands() const {return m_registredCommands;};
    std::vector<Command> ExecuteCommands() const {return m_commandsToExec;};   

//public functions
public:
    void registerCommand(std::string name, Command cmd);
    void parse(std::string value);
    void execute();
    bool cmdDetected();
};

