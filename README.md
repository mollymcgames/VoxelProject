# VoxelProject
## Introduction
VoxelProject visualises MRI scan outputs in NiFti (NII) format. A Nifti file represents an MRI scan in the form of Voxels of data. The visualisation is achieved by rendering the Voxels as 'Minecraft' style Voxels on screen that allow the user to close in, and dig through to see the inner layers of the scan.

## Pre-requisites
- Unity 2023.1.20f1+
- Visual Studio Community / Visual Studio Code
- The Unity packages listed in `packages-lock.json`

### External unity packages
A Nuget package is needed for this project, which is best installed, by first installing this external Unity plugin:  https://github.com/GlitchEnzo/NuGetForUnity

Once installed the following Nuget packages are required:
- [NIFTI.Net](https://www.nuget.org/packages/Nifti.NET) (latest version)

## Building
**Unity Editor**:  Running the code just from the editor will work. Make sure the "Model" scene is loaded as this sets up various "world" settings in a Singleton class that are used by other scenes.

**Binary Executable**: The project already has the scenes listed as required for a project build. So using the Editor to build a project executable will work. (This includes the sample NII files, which have to be located in the `streaming assets` folder within the project.)

## Usage
With the application running, the splash screen provides essential details on using the application. You are then taken to the main menu, where a model of a Human Body is displayed.  Currently this has three clickable areas, where there are three pre-defined Voxel models to explore.

### Navigation
Hovering over a clickable area will load the associated Nifti file from disk and then, after a few seconds, display the essential details of the Nifti file such as the filename, and the Voxel dimensions of the model.

Clicking one of the areas will then switch scenes, load and render the initial view of the selected Nifti file.

Controls:
- Move: `ASDW` keys + mouse
- Fly: `E` and `SPACE`
- Force refresh: Press `0`

Distance from the Voxel model will cause different Voxel chunks to be rendered.

### Other controls
On the top right of the display area are additional controls and options.
- **Greyscale Mode**: Toggling this will switch the rendering from a colour to greyscale rendering. (Don't forget to press `0` to reload!)
- **Visibility Threshold**: Moving this slider will progressively hide Voxels that fall below a certain colour visibility threshold. This helps to see different details in the Voxel model.  (Don't forget to press `0` to reload!)
- **Visibility Inversion**: Toggling this will invert the visibility threshold, providing an alternative way to see different details in the Voxel model.  (Don't forget to press `0` to reload!)
- **Field of View**: This slider changes the Field of View of the camera. This provides an alternative view of seeing more of the selected Voxel model.

### Supported files
- NIFTI files that follow the [NIFTI standard](https://nifti.nimh.nih.gov/).
- Voxel "text" files, that list out voxels in text format. (See an example in the `Streaming Assets` folder.)

### Changing voxel models
For now, changing a Voxel model is a manual task and has to be done in the code.

1. Place a new `*.nii` file in the `Assets\StreamingAssets` folder.
1. In the `MenuHandler.cs` file, find the associated clickable area loader, e.g. `LoadHeartFile` and change the `WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName=` value to the full filename (no path required) of your new `nii` file.
1. Run the code and click on your clickable area!

## Future enhancements
### Features to add
- Can't choose what voxel models to load.
- Support for Nifti files that have embedded segment data.
- Support for Nifti files that have embedded label tags.
- Having Voxels "fly" away as the user closes in and digs through the outer layers of Voxels.
- Add a 'refresh now' button (that appears when auto-refresh is disabled.)

### Performance
  - Updating the visible voxel selection code so that voxels at the back of the selected voxels are hidden will help.
  - Using `parallel.ForEach` might help, although there are challenges making sure the Unity Mesh is used in a thread safe manner that doesn't just create additional code locks/waits.
  - Investigating if an alternative way of selecting and looping through the voxels is possible, as a `foreach` loop over thousands to millions of Voxels is demonstrably slow.
- Nii file loading time, currently starts to get slow in proportion to the size of the MRI scan area. Can the file be loaded on demand perhaps as the model is rendered?

## Licensing
This project source code and documentation is licensed to Kingston University and the project author [Molly McConaghy](K2367574@kingston.ac.uk).

All images, graphics, MRI data files and other non-code resources are freely available and opensourced on the internet and not covered by any license restrictions.

All 3rd-party assets and libraries used in this project retain all rights under their respective licenses.
