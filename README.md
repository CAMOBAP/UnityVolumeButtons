# Unity3d VolumeButtonsPlugin

This plugin was developed for Unity3d Android & iOS platforms to be able listen for Volume up/down button clicks, it doesn't override them

Implemented buttons:
 - [x] Volume Up
 - [x] Volume Down
 - [ ] Mute

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

```
Unity3d 2019.x
```

### Installing

To install this pulgin:
 - Copy `Assets/Plugin/VolumeButtonsPlugin` into your project
 - Make sure that you updated activity name to `org.camobap.unity3d.VolumeButtonsActivity` in your `Assets/Plugin/Android/AndroidManifest.xml`*
 - For iOS add `MediaPlayer.framework`

* If you already have another custom activity there, please change parent `UnityPlayerActivity` in `VolumeButtonsActivity.java` to yours one or vice-versa

## Running the tests

Test app have two blinking Panels

## Usage in your apps

For your propose you can define fuction with signature below:

```
    ...
    
    public void HandleVolumeButton(VolumeButtonsEventType e)
    {
        ...
    }
```

And attach it to `VolumeButtons.cs` component

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://opensource.org/licenses/MIT) file for details