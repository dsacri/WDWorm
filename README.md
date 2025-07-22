# WDWorm
WDWorm: A runtime-efficient and user-friendly GUI-based toolbox for experimenting with the nerve net of C. elegans

## Overview

This is the official implementation of paper ["WDWorm: A runtime-efficient and user-friendly GUI-based toolbox for experimenting with the nerve net of C. elegans"](https://www.ssrn.com/abstract=5291111) (submitted to Neurocomputing).

WDWorm is a nerve net simulator of *C. elegans* is modeled using electrical circuits that simulate the behavior of individual neurons and their interconnections. The goal of this work is to provide a user-friendly, runtime-efficient, and highly flexible simulation toolbox for the *C. elegans* nervous system, enabling easy modification of neurons and their connections. To this end, we have developed WDWorm with the following most important features: 
1. it is directly executable without requiring an installation, 
2. it offers a simple GUI well usable by non-programmers, 
3. it can generate artificial calcium concentration traces comparable to fluorescence traces obtained by calcium imaging methods, supporting comparison with real measurement data, 
4. it allows the user to modify the simulation during runtime, such that also transient behavior due to the changes can be observed.

This project contains the source code and project files necessary to build the application using [Unity version 6000.0.32f1](https://unity.com/releases/editor/whats-new/6000.0.32). The graphical user interface (GUI) was designed in Unity, and the code was written in C#. The 3D model is based on a Blender model created by Chris Grove, PhD, for WormBase (www.wormbase.org) at the California Institute of Technology.

A compiled Windows version of the application is available here: [WDWorm-compiled](https://github.com/dsacri/WDWorm-compiled). It can be run directly without installation.


## System Requirements
The application is optimized to run on most Windows personal computers. Versions for iOS and Android can be compiled directly from the provided source code and Unity project.

Minimum System Requirements:
 - Operating System: Windows 7 (SP1+) or later, or macOS High Sierra 10.13+ or later
 - CPU: X64 architecture with SSE2 instruction set support.
 - Graphics API: DX10, DX11, or DX12-capable GPUs for Windows, or Metal-capable GPUs for macOS
 - RAM: 8GB


## Installation
An installation is not required. The Unity project files can be downloaded and opened in [Unity version 6000.0.32f1](https://unity.com/releases/editor/whats-new/6000.0.32) to run or compile the application. Alternatively, a precompiled version is available at [WDWorm-compiled](https://github.com/dsacri/WDWorm-compiled).

## Usage
WDWorm can be launched by executing CElegans.exe located in the build folder, or from within the Unity development environment when using the source code.

Upon startup, the default view displays the body model of C. elegans. The simulation begins by clicking the Play button at the bottom of the screen. Neurons are color-coded according to their type, as indicated in the legend in the bottom-left corner. The size of each neuron reflects its activity level at a given time. The camera view can be rotated by clicking and dragging with the left mouse button, and moved by clicking and dragging with the right mouse button.
<img width="1919" height="1079" alt="default view" src="https://github.com/user-attachments/assets/d61db563-8841-44e2-82cd-ff9f386833da" />

Three control buttons are located in the top-right corner of the interface:
- **Graph**: Toggles the display of individual values for activated neurons. The graph window, which shows up to 20 seconds of simulation data, can be moved by dragging it.
  <img width="3838" height="2158" alt="grafik" src="https://github.com/user-attachments/assets/cebaf28c-34a7-459b-b7c1-b2e1b6e106dd" />
- **Menu**: Provides options to exit the application (Close App), reset the camera view (Reset Camera), and toggle the display of all neurons (Show All Neurons). When this option is enabled, all neurons are shown at a fixed size, regardless of their activity.
         <img width="218" height="799" alt="grafik" src="https://github.com/user-attachments/assets/9c5b2ecb-6510-48a5-9b0a-bfbd35f61d1e" />
- **Settings**: Opens a configuration menu containing multiple tabs:
  - **General Settings**: Allows saving the current configuration to a .json file, loading settings from a .json file, and exporting simulation results as .csv files.
    <img width="3838" height="2158" alt="grafik" src="https://github.com/user-attachments/assets/400c5bcf-ba57-47e9-8b90-2fcec9c5bd23" />
  - **Graph Settings**: Provides options to switch between a line graph and a heatmap, select the parameter displayed (either membrane potential or calcium concentration), and choose which neurons from the list on the left are shown in the graph.
    <img width="3838" height="2158" alt="grafik" src="https://github.com/user-attachments/assets/95e70d28-e4d8-46b5-a825-4020d5c84478" />
    <img width="405" height="1705" alt="grafik" src="https://github.com/user-attachments/assets/5c1a262d-f30b-4d73-9e93-344dbed28f6c" /> <img width="400" height="1707" alt="grafik" src="https://github.com/user-attachments/assets/647c9e28-0174-4636-8404-ac06f4c79c74" />
    <img width="400" height="1708" alt="grafik" src="https://github.com/user-attachments/assets/db84ae8a-d481-4a28-8b4d-1e760236ebd9" /> <img width="403" height="1702" alt="grafik" src="https://github.com/user-attachments/assets/fd1c3665-8d64-4994-9cbb-866b279d1411" /> 
  - **Toggle Neuron**: Enables or disables individual neurons. Entire neural circuits can also be toggled using the selector in the bottom-right corner.
    <img width="3838" height="2158" alt="grafik" src="https://github.com/user-attachments/assets/6407b93c-0347-46c2-ae8a-67c2c4adfa2e" />
  - **External Current Stimuli**: Allows selection of neurons to be stimulated with external current, along with configuration of current amplitude and pulse width.
    <img width="3838" height="2158" alt="grafik" src="https://github.com/user-attachments/assets/91241201-15c9-468d-a425-419ba1831835" />
  - **Modify Connectome**: The connectome data from the 3D atlas by *Skuhersky et al. (2022)* is used by default. This tab allows adding new connections between neurons. To define a connection, click one neuron with the left mouse button and another with the right mouse button. Parameters for individual connections can be adjusted by selecting a connection and clicking "Change Connection Values". The chemical layers of connections can also be enabled or disabled in the bottom-right corner.
    <img width="3838" height="2158" alt="grafik" src="https://github.com/user-attachments/assets/a7b1f3aa-a6df-46ca-8b0d-2b9e45d49f04" />
  - **Neuron Model Parameters**: Allows modification of wave-digital model parameters for each neuron. Select a neuron from the list on the left to adjust its parameters on the right. A reset button is available to restore default parameter values.
    <img width="3838" height="2158" alt="grafik" src="https://github.com/user-attachments/assets/7c7170c7-dff4-4d9a-8452-f07014c81aaf" />



## Acknowledgement
This work was funded by the Deutsche Forschungsgemeinschaft (DFG, German Research Foundation) - Project-ID 434434223 - SFB 1461.

## License
The code in this repository is under the BY-NC-SA Creative Commons license as specified by the LICENSE file.

The 3D-Model used is based on the blender model built by Chris Grove, PhD for WormBase (www.wormbase.org) at the California Institute of Technology and is available for dissemination, with appropriate attribution, under the MIT license: [LICENSE](https://opensource.org/license/mit).
