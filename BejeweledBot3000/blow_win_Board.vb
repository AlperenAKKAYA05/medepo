Imports blow_win_Bot3000

Public Class blow_win_Board
    Private ReadOnly tileCount As Integer
    Private squares As blow_win_Tile(,)

    Public Sub New(tileCount As Integer)
        Me.tileCount = tileCount
        ReDim squares(tileCount - 1, tileCount - 1)
    End Sub

    Private Sub New(squares As blow_win_Tile(,))
        Me.New(squares.GetLength(0))
        Array.Copy(squares, Me.squares, squares.Length)
    End Sub

    Friend Sub SetTile(x As Integer, y As Integer, tileCode As Integer)
        squares(x, y) = New blow_win_Tile(tileCode)
    End Sub

    Private Sub SwapTiles(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer)
        Dim tmp As blow_win_Tile = squares(x1, y1)
        squares(x1, y1) = squares(x2, y2)
        squares(x2, y2) = tmp
    End Sub

    Private Function TrySwapTiles(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer) As Boolean
        For Each index In {x1, y1, x2, y2}
            If index < 0 Or index >= tileCount Then
                Return False
            End If
        Next
        SwapTiles(x1, y1, x2, y2)
        Return True
    End Function

    Function FindMoves() As List(Of blow_win_Move)
        Dim possibleMoves As List(Of blow_win_Move) = GenerateAndTestListOfAllPossibleMoves()
        Dim goodMoves = possibleMoves.Where(Function(m) m.Score >= 3).OrderByDescending(Function(m) m.Score).ToList
        Dim workingBoard As New blow_win_Board(squares)
        Dim tilesThatWillBeRemoved As HashSet(Of blow_win_Tile) = workingBoard.GetTilesToRemove
        Dim movesToApply As New List(Of blow_win_Move)
        For Each move In goodMoves
            Dim boardWithCurrentMovedApplied = workingBoard.GetClonedBoardWithMoveApplied(move)
            Dim tilesToRemoveWithCurrentMovedApplied = boardWithCurrentMovedApplied.GetTilesToRemove
            If tilesToRemoveWithCurrentMovedApplied.Count > tilesThatWillBeRemoved.Count Then
                workingBoard = boardWithCurrentMovedApplied
                tilesThatWillBeRemoved = tilesToRemoveWithCurrentMovedApplied
                Dim randomTileCode As Integer = Rnd() * 100000000
                tilesThatWillBeRemoved.ToList.ForEach(Sub(t) t.TileCode = randomTileCode)
                movesToApply.Add(move)
            End If
        Next
        Return movesToApply
    End Function

    Public Function GenerateAndTestListOfAllPossibleMoves() As List(Of blow_win_Move)
        Dim possibleMoves As New List(Of blow_win_Move)
        For x = 0 To tileCount - 1
            For y = 0 To tileCount - 1
                For Each direction In [Enum].GetValues(GetType(ArrowDirection))
                    Dim blow_win_Move As blow_win_Move = New blow_win_Move(x, y, direction)
                    blow_win_Move.TilesToRemoveAsResult = GetClonedBoardWithMoveApplied(blow_win_Move).GetTilesToRemove
                    possibleMoves.Add(blow_win_Move)
                Next
            Next
        Next
        Return possibleMoves
    End Function

    Function IsValidBoard() As Boolean
        Return GetScore() <= 2
    End Function

    Function GetUniqueTileCount() As Integer
        Dim codes As New HashSet(Of Integer)
        For x = 0 To tileCount - 1
            For y = 0 To tileCount - 1
                codes.Add(squares(x, y).TileCode)
            Next
        Next
        Return codes.Count
    End Function

    Sub Normalise()
        Dim nextNorm As Integer = 0
        Dim dic As New Dictionary(Of Integer, Integer)
        For x = 0 To tileCount - 1
            For y = 0 To tileCount - 1
                Dim preNorm As Integer = squares(x, y).TileCode
                If Not dic.ContainsKey(preNorm) Then
                    dic.Add(preNorm, nextNorm)
                    nextNorm += 1
                End If
                squares(x, y).NormalisedTileCode = dic(preNorm)
            Next
        Next
    End Sub

    Friend Function GetTile(x As Integer, y As Integer) As blow_win_Tile
        Return squares(x, y)
    End Function

    Private Function GetClonedBoardWithMoveApplied(move As blow_win_Move) As blow_win_Board
        Dim board As New blow_win_Board(squares)
        board.PerformMoveIfValid(move)
        Return board
    End Function

    Public Function GetScore() As Integer

        Return GetTilesToRemove().Count
    End Function


    Public Function GetTilesToRemove() As HashSet(Of blow_win_Tile)
        Dim tilesToRemove As New HashSet(Of blow_win_Tile)
        For x = 0 To tileCount - 1
            Dim currentMatchingCode As Integer = Nothing
            Dim tilesToRemoveFromThisLine = New HashSet(Of blow_win_Tile)
            For y = 0 To tileCount - 1
                If currentMatchingCode <> squares(x, y).TileCode Then
                    tilesToRemoveFromThisLine.Clear()
                    currentMatchingCode = squares(x, y).TileCode
                End If
                tilesToRemoveFromThisLine.Add(squares(x, y))
                If tilesToRemoveFromThisLine.Count >= 3 Then
                    tilesToRemove.UnionWith(tilesToRemoveFromThisLine)
                End If
            Next
        Next
        For y = 0 To tileCount - 1
            Dim currentMatchingCode As Integer = Nothing
            Dim tilesToRemoveFromThisLine = New HashSet(Of blow_win_Tile)
            For x = 0 To tileCount - 1
                If tilesToRemoveFromThisLine.Count >= 3 Then
                    tilesToRemove.UnionWith(tilesToRemoveFromThisLine)
                End If
                If currentMatchingCode <> squares(x, y).TileCode Then
                    tilesToRemoveFromThisLine.Clear()
                    currentMatchingCode = squares(x, y).TileCode
                End If
                tilesToRemoveFromThisLine.Add(squares(x, y))
                If tilesToRemoveFromThisLine.Count >= 3 Then
                    tilesToRemove.UnionWith(tilesToRemoveFromThisLine)
                End If
            Next
        Next
        Return tilesToRemove
    End Function

    Private Function PerformMoveIfValid(move As blow_win_Move) As Boolean
        Select Case move.Direction
            Case ArrowDirection.Down
                Return TrySwapTiles(move.X, move.Y, move.X, move.Y - 1)
            Case ArrowDirection.Up
                Return TrySwapTiles(move.X, move.Y, move.X, move.Y + 1)
            Case ArrowDirection.Left
                Return TrySwapTiles(move.X, move.Y, move.X - 1, move.Y)
            Case Else 'ArrowDirection.Right
                Return TrySwapTiles(move.X, move.Y, move.X + 1, move.Y)
        End Select
    End Function
End Class
