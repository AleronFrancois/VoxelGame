# Voxel Game

![VoxelGameScreenshot#1](https://github.com/user-attachments/assets/05748e06-26ee-4ec8-b53d-9f60ed1f6f8f)

This is a simple voxel-based game, inspired by Minecraft, created using C# and OpenTK/OpenGL. The game features terrain generation with perlin noise for an interesting environment, player movement, texture mapping, and efficient chunk rendering with advanced optimization techniques such as back face culling, adjacent block face culling, chunk meshing and a dictionary lookup system for block locating.

## Features

- **Player**: The game includes a basic player that can move around and interact with the world.
- **Texture Mapping**: Textures are applied to blocks to give the world a visually appealing look.
- **Terrain Generation**: The game generates a dynamic terrain using noise functions to create realistic landscapes.

- **Optimization Techniques**: 
  - **Adjacent Block Face Culling**: Unnecessary block faces are not rendered when adjacent to other blocks which is handled by a bitmask, optimizing performance.
  - **Back Face Culling**: Faces of blocks that are not visible to the camera are culled, reducing the number of rendered faces.
  - **Chunk Meshing**: The chunk geometry is prepared as a chunk mesh then rendered as a single buffer object for efficient draw calling.

## How to Run

1. Clone this repository:
   ```bash
   git clone https://github.com/AleronFrancois/VoxelGame.git
