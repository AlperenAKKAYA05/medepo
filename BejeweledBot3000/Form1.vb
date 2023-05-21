Imports System.Drawing.Imaging
Imports System.Threading
Imports blow_win_Bot3000

Public Class Form1

    Dim moveCounter As Integer = 0
    Dim OscillationDetector As New OscillationDetector
    Dim blow_win_ScreenReader As New blow_win_ScreenReader

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        dgvBoard.RowCount = TileCount
        dgvBoard.ColumnCount = TileCount
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Console.Beep()
        Thread.Sleep(200)
        Console.Beep()
        Thread.Sleep(200)
        Console.Beep()
        Thread.Sleep(200)

        GetMoves().ForEach(Sub(m) PerformMoveUsingMouse(m))
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim blow_win_Location As Rect = blow_win_ScreenReader.Getblow_win_WindowRect()
        Me.Location = New Point(blow_win_Location.Right, blow_win_Location.Top)
        Dim moves = GetMoves()
        If Not cbWaitForStaticBoard.Checked OrElse blow_win_BoardIsStatic() Then
            PerformMovesUsingMouseIfCapsLock(moves)
            TryClickOnPlayAgainButtonIfCapsLock()
        Else
            LOG("Waiting for static board")
        End If

    End Sub

    Private Function blow_win_BoardIsStatic() As Boolean
        Dim blow_win_Board1 As blow_win_Board = blow_win_ScreenReader.DrawAndGetblow_win_BoardFromScreen_VeryQuick()
        Dim blow_win_Board2 As blow_win_Board = blow_win_ScreenReader.DrawAndGetblow_win_BoardFromScreen_VeryQuick()
        Return OscillationDetector.GetDifferenceInBoards(blow_win_Board1, blow_win_Board2) <= 3
    End Function

    Function GetMoves() As List(Of blow_win_Move)
        LOG("Reading from screen")
        Dim blow_win_Board As blow_win_Board = blow_win_ScreenReader.DrawAndGetblow_win_BoardFromScreen()
        DrawBoardOnForm(blow_win_Board)
        If BottomRowOfblow_win_BoardIsNotDirt(blow_win_Board) Then
            LOG("'All clear' detected - sleeping")
            Thread.Sleep(1000) 'Wait a second for 'all clear' message and board to rise up
            Return New List(Of blow_win_Move)
        Else
            Dim moves As List(Of blow_win_Move) = blow_win_Board.FindMoves()
            Me.Text = blow_win_Board.GetUniqueTileCount & " unique tiles - moves " & moves.Count &
                             IIf(blow_win_Board.IsValidBoard, "", " - Board looks invalid")
            If (OscillationDetector.LogMovesAndReturnTrueIfOscillationDetected(moves)) Then
                LOG("Oscillation detected - using fallback")
                Return GetFallbackMovesFromBoard(blow_win_Board)
            ElseIf moves.Count = 0 Then 'Either I failed to identify some tiles or only a hypercube is on the board.
                LOG("0 moves found - using fallback")
                Return GetFallbackMovesFromBoard(blow_win_Board)
            End If
            Return moves
        End If
    End Function

    Private Function GetFallbackMovesFromBoard(blow_win_Board As blow_win_Board) As List(Of blow_win_Move)
        Dim possibleMoves = blow_win_Board.GenerateAndTestListOfAllPossibleMoves
        Dim RandomDirection As ArrowDirection = possibleMoves(Int(Rnd() * possibleMoves.Count)).Direction
        Return possibleMoves.Where(Function(m) m.Direction = RandomDirection).ToList
    End Function

    Private Function BottomRowOfblow_win_BoardIsNotDirt(blow_win_Board As blow_win_Board) As Boolean
        For x = 0 To TileCount - 1
            If Not ColorBucket.IsKnownColor(blow_win_Board.GetTile(x, TileCount - 1).TileCode) Then
                Return False
            End If
        Next
        Return True
    End Function

    Sub DrawBoardOnForm(blow_win_Board As blow_win_Board)
        PictureBox2.Image = blow_win_ScreenReader.LatestBitmap
        For x = 0 To TileCount - 1
            For y = 0 To TileCount - 1
                Dim blow_win_Tile As blow_win_Tile = blow_win_Board.GetTile(x, y)
                dgvBoard.Rows(y).Cells(x).Style.BackColor = Color.FromArgb(blow_win_Tile.TileCode)
                dgvBoard.Rows(y).Cells(x).Value = blow_win_Tile.NormalisedTileCode
            Next
        Next
    End Sub

    Sub PerformMovesUsingMouseIfCapsLock(blow_win_Moves As List(Of blow_win_Move))
        If GetKeyState(VK_CAPSLOCK) = 1 Then
            moveCounter += 1
            LOG("Performing " & blow_win_Moves.Count & " moves")
            For Each blow_win_Move In blow_win_Moves
                PerformMoveUsingMouse(blow_win_Move)
            Next
        End If
    End Sub


    Sub PerformMoveUsingMouse(move As blow_win_Move)
        Dim basePosition = blow_win_ScreenReader.GetTopLeftOfblow_win_Board_DiamondMine()
        Cursor.Position = New Point(basePosition.X + (TileSize * move.X), basePosition.Y + (TileSize * move.Y))
        Call apimouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        Select Case move.Direction
            Case ArrowDirection.Down
                Cursor.Position = New Point(Cursor.Position.X, Cursor.Position.Y - TileSize)
            Case ArrowDirection.Up
                Cursor.Position = New Point(Cursor.Position.X, Cursor.Position.Y + TileSize)
            Case ArrowDirection.Left
                Cursor.Position = New Point(Cursor.Position.X - TileSize, Cursor.Position.Y)
            Case ArrowDirection.Right
                Cursor.Position = New Point(Cursor.Position.X + TileSize, Cursor.Position.Y)
        End Select
        Thread.Sleep(5)
        Call apimouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
    End Sub

    Private Sub TryClickOnPlayAgainButtonIfCapsLock()
        If GetKeyState(VK_CAPSLOCK) = 1 Then
            TryClickOnPlayAgainButton()
        End If
    End Sub

    Private Sub TryClickOnPlayAgainButton()
        Dim oldPosition = Cursor.Position
        Dim b As Rect = blow_win_ScreenReader.Getblow_win_WindowRect()
        Cursor.Position = New Point(b.Left + (b.Right - b.Left) / 2, b.Top + 445)
        Call apimouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        Call apimouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
        Cursor.Position = oldPosition
    End Sub

    Sub LOG(str As String)
        lblStatus.Text = str.ToUpper
        Console.WriteLine(Now & " - " & str)
    End Sub
End Class
