# Intro

Othello is an implementation of the board-game Othello (aka Reversi). It was created in Unity with C# as the programming language. It features:

* 3D animation of plays
* Computer player (with opening book, transposition table, alpha-beta pruning negamax searching)
* Undo/redo and replay of games
* Displays archived game stats for each square
* Displays potential plays
* Save and load games

# Setup Instructions

## Visual Studio

The source code can be compiled in either Visual Studio 2019, VS Code, or MonoDevelop. Build in Visual Studio (or equivalent) first so the class libraries can go into the folders for Unity to pick-up. The class libraries are:
* Othello.Core
* Othello.Model

If you don't build in Visual Studio (or equivalent) you'll get compile errors in Unity.

## Unity

The graphics engine currently runs on Unity 2020.3.12f1. It should be updatable to newer versions. The unity project file is stored in the Assets folder. See http://unity3d.com/. The Unity scene you need to open is called *Othello.unity*, in the root of the repo.

# Additional Information

More information about the project can be found at http://ledpup.blogspot.com.au/search/label/Reversi.
Opening book games are from WThor archive (games recorded yearly from 1977): https://www.ffothello.org/informatique/la-base-wthor/

# History

* 2021 update : Abandoned XBox Live integration but merged the rest of the branch into master. Added WThor 2020 archive into opening book. Fixed all the Unity files so it's properly ready for cloning of repo (thought I'd done that years ago!)
* 2020 update: refactored the WThor file reader. Added the games from 2012-2019. Started on online multiplayer using XBox Live.
* 2019 update: published to the Microsoft Store at https://www.microsoft.com/store/apps/9WZDNCRDWZZ3.
