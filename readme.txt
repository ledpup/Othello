Othello is an implementation of the board-game Othello (aka Reversi). It was created in Unity with C# as the programming language. It features:

* 3D animation of plays
* Computer opponent (with opening book, alpha-beta pruning negamax searching)
* Undo/redo and replay of games
* Archived game stats for each square
* Save/load of games
* Displays potential plays

The source code can be compile in either Visual Studio or MonoDevelop. You will need Unity to be able to compile and run the GUI for the game. The unity project file is stored in the Assets folder. See http://unity3d.com/.

More information about the project can be found at http://ledpup.blogspot.com.au/search/label/Reversi.

What I'm working on:

* Improving the evaluation function of the computer opponent
* Removing memory leaks in the depth-first search