using System;
using System.Text.RegularExpressions;
using UnityEngine;

internal class CommandParser
{


    static Regex thrustRegex = new Regex(@"thrust ([0-9]+)( [0-9]+)?");
    static Regex thrustRelativeRegex = new Regex(@"thrust to (minimal)");
    static Regex turnRegex = new Regex(@"turn (left|right)( [0-9]+)");
    static Regex turnRelativeRegex = new Regex(@"turn (prograde|retrograde)");

    internal static Command parse(string commandString)
    {
        string command = commandString.ToLower();

        Match thrustMatch = thrustRegex.Match(command);
        if (thrustMatch.Success)
        {
            float duration = float.Parse(thrustMatch.Groups[1].Value);
            float power;
            if (thrustMatch.Groups[2].Value.Length > 0)
            {
                power = float.Parse(thrustMatch.Groups[2].Value);
            }
            else
            {
                power = 100;
            }
            return new ThrustCommand(power, duration);
        }

        Match thrustRelativeMatch = thrustRelativeRegex.Match(command);
        if (thrustRelativeMatch.Success)
        {
            string value = thrustRelativeMatch.Groups[1].Value;
            ThrustValue stopCriterion = value switch {
                "minimal" => ThrustValue.SPEED_MINIMAL,
                _ => ThrustValue.UNKNOWN,
            };
            return new ThrustRelativeCommand(stopCriterion);
        }

        Match turnMatch = turnRegex.Match(command);
        if (turnMatch.Success)
        {
            TurnDirection direction = (TurnDirection)Enum.Parse(typeof(TurnDirection), turnMatch.Groups[1].Value.ToUpper());
            float rotation = float.Parse(turnMatch.Groups[2].Value);
            return new TurnCommand(direction, rotation);
        }

        Match turnRelativeMatch = turnRelativeRegex.Match(command);
        if (turnRelativeMatch.Success)
        {
            TurnDirection direction = (TurnDirection)Enum.Parse(typeof(TurnDirection), turnRelativeMatch.Groups[1].Value.ToUpper());
            return new TurnRelativeCommand(direction);
        }
        return null;
    }
}