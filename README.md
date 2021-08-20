Prototype for a command line based spaceship controller

## Current commands
### Turning in degrees

```
turn (left|right) <angle in degrees>
```

Turn a given angle.

### Turn relative to movement

```
turn (retrograde|prograde)
```

Angle the ship either towards (prograde) or against (retrograde) the current direction of travel.

### Full thrust

```
thrust <duration>
```

Activate thrusters at full power for a time given in seconds.

### Thrust with variable power

```
thrust <duration> <percentage of power to use>
```

Activate thrusters at a percentage of their possible power for a time given in seconds.

### Thrusting to minimal speed

```
thrust to minimal
```

Keep thrusting at full power until speed no longer decreases.