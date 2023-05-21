Public Class blow_win_Move
    Public Direction As ArrowDirection
    Public X As Integer
    Public Y As Integer
    Public TilesToRemoveAsResult As HashSet(Of blow_win_Tile)

    Public Sub New(x As Integer, y As Integer, direction As ArrowDirection)
        Me.X = x
        Me.Y = y
        Me.Direction = direction
    End Sub

    Public Function Score() As Integer
        Return TilesToRemoveAsResult.Count
    End Function

    Public Overrides Function ToString() As String
        Return X & "," & Y & "," & Direction.ToString & " = " & Score
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return 31 * X + 17 * Y
    End Function
End Class
