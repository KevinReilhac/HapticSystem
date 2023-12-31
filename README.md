
# Haptic System

Make your gamepad vibrate.


## Features

- Create HapticClips
- Test it in inspector
- Play it in game


## Authors

- [@Kévin "Kebab" Reilhac](https://www.github.com/KevinReilhac)


## Installation

Install HapticSystem with Unity Package Manager

```bash
https://github.com/KevinReilhac/HapticSystem.git#upm
```
    
## Usage/Examples

### Create Haptic Clip
![Create Clip](https://i.ibb.co/NpzF4C0/Capture-d-cran-2023-06-28-164824.jpg)
### Test it
![Setup Clip](https://i.ibb.co/3dDm8qm/Haptic-Clip-Setup.gif)
### Play it
```csharp
    [SerializedField] private HapticClip hapticClip;
    [SerializedField] private int targetGamepad;
    ...
    HapticManager.PlayClip(hapticClip, targetGamepad);
```

### Stop it
```csharp
    HapticClipInstance instance = HapticManager.PlayClip(hapticClip, targetGamepad);
    HapticManager.StopClipInstance(instance);

    //OR
    HapticManager.StopAllClipInstances()
```
## Documentation

[Read Documentation](https://kevinreilhac.github.io/HapticSystem/)
____________________


I am not responsible for any use of this module, find a better one.