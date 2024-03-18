using System;

public class PlayerOutOfMazeException : Exception
{
    public PlayerOutOfMazeException() : base("The player is out of maze grid bounds...") { }
    public void ShowMessageWindow()
    {
        NativeWinAlert.Error("PlayerOutOfMazeException", Message);
    }
}
