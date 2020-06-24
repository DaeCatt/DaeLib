# Dae's Library (DaeLib)

This is a library mod for Terraria that I wrote to de-duplicate code I need in
several mods.

## DaeLib.Geometry.Rect

A simple class that mimics Microsoft.XNA.Framework.Rectangle but using floats
instead of ints for sub-pixel positioning.

## DaeLib.Graphics.ScalableTexture2D

A helper class the cuts a Texture2D into four corners, four sides, and a
middle texture. A generalization of how UIPanel is drawn in Terraria.

## DaeLib.UI

Several UI elements. Primarily focused on input elements missing from vanilla
Terraria.
