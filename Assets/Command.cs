using System;
using UnityEngine;

internal abstract class Command
{
    CommandType type;

    internal Command(CommandType type) {
        this.type = type;
    }

    internal abstract string AsString();
    internal abstract bool Execute(GameObject playerShip);
}

class TurnCommand : Command
{
    TurnDirection direction;
    float rotation;

    internal TurnCommand(TurnDirection direction, float rotation) : base(CommandType.TURN) {
        this.direction = direction;
        if (direction.Equals(TurnDirection.LEFT) || direction.Equals(TurnDirection.RIGHT)) {
            this.rotation = rotation;
        } else {
            this.rotation = 0;
        }
    }

    internal override string AsString()
    {
        if (direction.Equals(TurnDirection.LEFT) || direction.Equals(TurnDirection.RIGHT)) {
            return String.Format("Turning {0} degrees {1}", rotation, direction);
        } else {
            return "Turning somewhere";
        }
    }

    internal override bool Execute(GameObject playerShip)
    {
        if (rotation > 0) {
            ShipData data = playerShip.GetComponent<ShipData>();
            float rotationStep = data.RotationSpeed * Time.deltaTime;
            playerShip.transform.rotation *= Quaternion.AngleAxis(rotationStep * (direction.Equals(TurnDirection.LEFT) ? -1 : 1), playerShip.transform.up);
            rotation -= rotationStep;
            return false;
        } else {
            return true;
        }
    }
}

class TurnRelativeCommand : Command
{
    TurnDirection direction;

    Quaternion targetRotation = Quaternion.identity;

    internal TurnRelativeCommand(TurnDirection direction) : base(CommandType.TURN_RELATIVE) {
        this.direction = direction;
    }

    internal override string AsString()
    {
        if (direction.Equals(TurnDirection.PROGRADE) || direction.Equals(TurnDirection.RETROGRADE)) {
            return String.Format("Turning to {0}", direction);
        } else {
            return "Turning somewhere";
        }
    }

    internal override bool Execute(GameObject playerShip)
    {
        if (targetRotation == Quaternion.identity) {
            Vector3 movementDirection = playerShip.GetComponent<Rigidbody>().velocity.normalized;
            if (direction.Equals(TurnDirection.RETROGRADE)) {
                movementDirection *= -1;
            }
            targetRotation = Quaternion.LookRotation(movementDirection, playerShip.transform.up);
        }
        if (playerShip.transform.rotation != targetRotation) {
            ShipData data = playerShip.GetComponent<ShipData>();
            float rotationStep = data.RotationSpeed * Time.deltaTime;
            playerShip.transform.rotation = Quaternion.RotateTowards(playerShip.transform.rotation, targetRotation, rotationStep);
            return false;
        } else {
            return true;
        }
    }
}

internal class ThrustCommand : Command
{
    float power;
    float duration;

    float timeRan = 0;

    internal ThrustCommand(float power, float duration) : base(CommandType.THRUST) {
        this.power = power;
        this.duration = duration;
    }

    internal override string AsString()
    {
        return String.Format("Thrusting for {0} seconds at {1}% power", duration, power);
    }

    internal override bool Execute(GameObject playerShip)
    {
        if (timeRan <= duration) {
            ShipData data = playerShip.GetComponent<ShipData>();
            playerShip.GetComponent<Rigidbody>().AddForce(playerShip.transform.forward * (power / 100) * Time.deltaTime * data.ThrusterPower);
            timeRan += Time.deltaTime;
            return false;
        } else {
            return true;
        }
    }
}

internal class ThrustRelativeCommand : Command
{
    ThrustValue stopCriterion;

    Vector3 previousVelocity = Vector3.zero;


    internal ThrustRelativeCommand(ThrustValue stopCriterion) : base(CommandType.THRUST_RELATIVE) {
        this.stopCriterion = stopCriterion;
    }

    internal override string AsString()
    {
        return String.Format("Thrusting until {0}", stopCriterion);
    }

    internal override bool Execute(GameObject playerShip)
    {
        Vector3 currentVelocity = playerShip.GetComponent<Rigidbody>().velocity;
        bool result = stopCriterion switch
        {
            ThrustValue.SPEED_MINIMAL => MinimizeSpeed(playerShip),
            _ => true,
        };
        previousVelocity = currentVelocity;
        return result;
    }

    internal bool MinimizeSpeed(GameObject playerShip) {
        if (previousVelocity.Equals(Vector3.zero) || previousVelocity.magnitude >= playerShip.GetComponent<Rigidbody>().velocity.magnitude) {
            ShipData data = playerShip.GetComponent<ShipData>();
            playerShip.GetComponent<Rigidbody>().AddForce(playerShip.transform.forward * Time.deltaTime * data.ThrusterPower);
            return false;
        } else {
            return true;
        }
    }
}

internal enum TurnDirection
{
    LEFT,
    RIGHT,
    RETROGRADE,
    PROGRADE
}

internal enum ThrustValue
{
    SPEED_MINIMAL,
    UNKNOWN
}

internal enum CommandType
{
    TURN,
    TURN_RELATIVE,
    THRUST,
    THRUST_RELATIVE,
    TARGET,
    FIRE
}