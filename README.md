## Tab Organization Extension for Visual Studio

One new command is exposed:

- `Tools.SortDocumentTabs`
  - Sort visible document tabs in active window
  - Default shortcut: CTRL+R, CTRL+S

This shortcut can be remapped in Tools > Options > Environment > Keyboard.

### Compatibility

- Visual Studio 2022

### Build

Visual Studio 2022 is required to build the solution. Note that `Microsoft.VisualStudio.SDK` should have major version corresponding to the version of Visual Studio where the extension will run (e.g. nuget version 16.x for VS2019) and `Microsoft.VSSDK.BuildTools` should have major version corresponding to the version of Visual Studio that is used to _build_ the extension.

### Additional Information

This is an alpha release meant for testing.

Copyright (c) 2022 Matt Mower
