Imports blow_win_Bot3000

Public Class OscillationDetector
    Private moveListHashcodes(10) As Integer
    Private nextInsertionIndex As Byte = 0

    Function LogMovesAndReturnTrueIfOscillationDetected(moves As List(Of blow_win_Move))
        Dim hashcode As Integer = moves.Select(Function(m) m.GetHashCode).Sum
        Try
            Return hashcode <> 0 AndAlso moveListHashcodes.Where(Function(h) h = hashcode).Count > 2
        Finally
            moveListHashcodes(nextInsertionIndex) = hashcode
            nextInsertionIndex = (nextInsertionIndex + 1) Mod moveListHashcodes.Length
        End Try
    End Function

    Friend Shared Function GetDifferenceInBoards(blow_win_Board1 As blow_win_Board, blow_win_Board2 As blow_win_Board) As Integer
        Dim differences As Integer = 0
        If blow_win_Board1 IsNot Nothing And blow_win_Board2 IsNot Nothing Then
            For x = 0 To TileCount - 1
                For y = 0 To TileCount - 1
                    If blow_win_Board1.GetTile(x, y).TileCode <> blow_win_Board2.GetTile(x, y).TileCode Then
                        differences += 1
                    End If
                Next
            Next
        End If
        Return differences
    End Function
End Class
