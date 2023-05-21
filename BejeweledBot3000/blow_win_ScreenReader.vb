Imports System.Drawing.Imaging

Public Class blow_win_ScreenReader
    Public LatestBitmap As New Bitmap(1, 1)

    Function DrawAndGetblow_win_BoardFromScreen() As blow_win_Board
        Return DrawAndGetblow_win_BoardFromScreen(0.33, False)
    End Function

    Function DrawAndGetblow_win_BoardFromScreen(m As Double, useMode As Boolean) As blow_win_Board
        TileCount = 7
        TileSize = 85

        Dim bFull As New Bitmap(TileCount * TileSize, TileCount * TileSize)
        Dim gFull As Graphics = Graphics.FromImage(bFull)
        gFull.CopyFromScreen(GetTopLeftOfblow_win_Board_DiamondMine, New Point(0, 0), bFull.Size)
        bFull = bFull.Clone(New Rectangle(0, 0, bFull.Width, bFull.Height), PixelFormat.Format8bppIndexed)
        bFull = bFull.Clone(New Rectangle(0, 0, bFull.Width, bFull.Height), PixelFormat.Format16bppRgb555)


        gFull = Graphics.FromImage(bFull)
        LatestBitmap = bFull

        Dim blow_win_Board As New blow_win_Board(TileCount)
        For x = 0 To TileCount - 1
            For y = 0 To TileCount - 1
                Dim sampleRectangle = New Rectangle((TileSize * m) + (x * TileSize), (TileSize * m) + (y * TileSize),
                                                    TileSize * (1 - m * 2), TileSize * (1 - m * 2))
                ' MsgBox(sampleRectangle.Left, MsgBoxStyle.YesNo)
                Dim colorBucket As New ColorBucket()
                For i = sampleRectangle.Left To sampleRectangle.Right
                    For j = sampleRectangle.Top To sampleRectangle.Bottom
                        colorBucket.Colors.Add(bFull.GetPixel(i, j))
                    Next
                Next
                Dim tileCode
                If useMode Then
                    tileCode = colorBucket.GetColorCode_MODE
                Else
                    tileCode = colorBucket.GetColorCode()
                End If
                gFull.DrawRectangle(New Pen(Color.FromArgb(tileCode), 2), sampleRectangle)
                blow_win_Board.SetTile(x, y, tileCode)
            Next
        Next
        blow_win_Board.Normalise()
        Return blow_win_Board
    End Function

    Function DrawAndGetblow_win_BoardFromScreen_VeryQuick() As blow_win_Board
        Return DrawAndGetblow_win_BoardFromScreen(0.25, True)
    End Function

    Function GetTopLeftOfblow_win_Board_DiamondMine()
        Dim b As Rect = Getblow_win_WindowRect()
        Return New Point(b.Left + 525 + 3.9 * TileSize, b.Top + 163 + 1.1 * TileSize)
    End Function

    Function GetTopLeftOfblow_win_Board_Classic()
        Dim b As Rect = Getblow_win_WindowRect()
        Return New Point(b.Left + 525 + 4.15 * TileSize, b.Top + 160 + 0.65 * TileSize)
    End Function

    Function Getblow_win_WindowRect() As Rect
        'Dim handle As IntPtr = Process.GetProcessesByName("123")(0).MainWindowHandle
        Dim rect As New Rect
        'GetWindowRect(handle, rect)
        Return rect
    End Function

End Class
