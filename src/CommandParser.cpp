#include "CommandParser.h"

CommandParser::CommandParser(void)
{
}


CommandParser::~CommandParser(void)
{
}

bool CommandParser::cmdDetected()
{
    return ExecuteCommands().size() > 0; //there are command to execute
}

void CommandParser::execute()
{
    ExecuteCommands().clear();
}