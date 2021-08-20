using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandHandler : MonoBehaviour
{
    [SerializeField]
    Text commandList;
    [SerializeField]
    Text commandInput;
    [SerializeField]
    Text currentCommandField;

    [SerializeField]
    GameObject playerShip;

    List<Command> queuedCommands = new List<Command>();
    List<Command> runningCommands = new List<Command>();
    Command currentCommand;

    private void Update() {
        if (currentCommand != null) {
            bool finished = currentCommand.Execute(playerShip);
            if (finished) {
                currentCommand = null;
            }
        } else if (runningCommands.Count > 0) {
            currentCommand = runningCommands[0];
            runningCommands.RemoveAt(0);
            currentCommandField.text = currentCommand.AsString();
        } else {
            currentCommandField.text = "No command running";
        }
    }

    public void ParseCommand() {
        ParseCommand(commandInput.text);
    }

    public void ParseCommand(string commandString) {
        if (commandString.Length > 0) {
            Command command = CommandParser.parse(commandString);
            queuedCommands.Add(command);
            commandList.text += command.AsString() + "\n";
        }
    }

    public void ExecuteCommands() {
        runningCommands.Clear();
        currentCommand = null;
        runningCommands.AddRange(queuedCommands);
        queuedCommands.Clear();
    }
}
