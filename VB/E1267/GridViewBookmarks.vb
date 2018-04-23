Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.Utils.Menu
Imports System.Windows.Forms
Imports System.Drawing

Namespace DevExpress.XtraGrid.Bookmarks
	Public Class GridViewBookmarks
		Implements IDisposable
		Private Const SetBookmarkStr As String = "Set bookmark"
		Private Const GotoBookmarkStr As String = "Go to bookmark"
		Private Const BookmarkStr As String = "Bookmark "
		Private Const BookmarkCount As Integer = 10
		Private view_Renamed As GridView
		Private keyFieldName_Renamed As String
		Private bookMarks_Renamed(BookmarkCount - 1) As BookmarkData

		Public Sub New(ByVal view As GridView, ByVal keyFieldName As String)
			Me.view_Renamed = view
			Me.keyFieldName_Renamed = keyFieldName
			AddHandler Me.View.MouseUp, AddressOf View_MouseUp
			AddHandler Me.View.KeyDown, AddressOf View_KeyDown
			AddHandler Me.View.CustomDrawRowIndicator, AddressOf View_CustomDrawRowIndicator
		End Sub

		Public ReadOnly Property View() As GridView
			Get
				Return view_Renamed
			End Get
		End Property
		Public ReadOnly Property KeyFieldName() As String
			Get
				Return keyFieldName_Renamed
			End Get
		End Property
		Protected ReadOnly Property BookMarks() As BookmarkData()
			Get
				Return bookMarks_Renamed
			End Get
		End Property

		Protected Sub SetBookmark(ByVal rowHandle As Integer, ByVal bookmarkIndex As Integer)
			Do While View.IsGroupRow(rowHandle)
				rowHandle = View.GetChildRowHandle(rowHandle, 0)
			Loop
			If GetBookmarkIndex(rowHandle) = bookmarkIndex Then
				BookMarks(bookmarkIndex) = Nothing
			Else
				If BookMarks(bookmarkIndex) IsNot Nothing Then
					View.InvalidateRowIndicator(GetRowHandle(bookmarkIndex))
				End If
				BookMarks(bookmarkIndex) = CreateBookmarkData(rowHandle)
			End If
			View.InvalidateRowIndicator(rowHandle)
		End Sub
		Protected Sub GotoBookmark(ByVal bookmarkIndex As Integer)
			If BookMarks(bookmarkIndex) Is Nothing Then
				Return
			End If
			View.FocusedRowHandle = GetRowHandle(bookmarkIndex)
		End Sub
		Protected Function HasBookmark(ByVal rowHandle As Integer) As Boolean
			Return GetBookmarkIndex(rowHandle) > -1
		End Function
		Protected Overridable Sub DrawBookmark(ByVal e As RowIndicatorCustomDrawEventArgs)
			Using brush As New SolidBrush(Color.FromArgb(40, Color.DarkBlue))
				Dim bounds As New RectangleF()
				bounds.X = e.Info.Bounds.X
				bounds.Y = e.Info.Bounds.Y
				bounds.Width = e.Info.Bounds.Width
				bounds.Height = e.Info.Bounds.Height
				e.Graphics.FillRectangle(brush, bounds)
			End Using
		End Sub
		Protected Function GetRowHandle(ByVal bookmarkIndex As Integer) As Integer
			Return View.DataController.FindRowByValue(KeyFieldName, BookMarks(bookmarkIndex).Value)
		End Function
		Protected Function GetBookmarkIndex(ByVal rowHandle As Integer) As Integer
			If (Not View.IsDataRow(rowHandle)) Then
				Return -1
			End If
			Dim value As Object = GetKeyValue(rowHandle)
			For i As Integer = 0 To BookMarks.Length - 1
				If BookMarks(i) Is Nothing Then
					Continue For
				End If
				If Object.Equals(BookMarks(i).Value, value) Then
					Return i
				End If
			Next i
			Return -1
		End Function
		Protected Function GetKeyValue(ByVal rowHandle As Integer) As Object
			Return View.GetRowCellValue(rowHandle, KeyFieldName)
		End Function
		Protected Function CreateMenu() As DXPopupMenu
			Dim menu As New DXPopupMenu()
			menu.Items.Add(CreateSetBookmarkMenu())
			menu.Items.Add(CreateGotoBookmarkMenu())
			Return menu
		End Function
		Protected Function CreateSetBookmarkMenu() As DXSubMenuItem
			Dim menuItem As New DXSubMenuItem(SetBookmarkStr)
			Dim eventHandler As New EventHandler(AddressOf SetBookmarkMenuItem_Click)
			For i As Integer = 0 To BookmarkCount - 1
				Dim item As New DXMenuCheckItem(BookmarkStr & i.ToString())
				AddHandler item.Click, eventHandler
				item.Tag = i
				item.Checked = BookMarks(i) IsNot Nothing
				menuItem.Items.Add(item)
			Next i
			Return menuItem
		End Function
		Protected Function CreateGotoBookmarkMenu() As DXSubMenuItem
			Dim menuItem As New DXSubMenuItem(GotoBookmarkStr)
			Dim eventHandler As New EventHandler(AddressOf GotoBookmarkMenuItem_Click)
			For i As Integer = 0 To BookmarkCount - 1
				If BookMarks(i) IsNot Nothing Then
					Dim item As New DXMenuItem(BookmarkStr & i.ToString(), eventHandler)
					item.Tag = i
					menuItem.Items.Add(item)
				End If
			Next i
			Return menuItem
		End Function
		Protected Function CreateBookmarkData(ByVal rowHandle As Integer) As BookmarkData
			Return New BookmarkData(GetKeyValue(rowHandle))
		End Function
		Private Sub View_CustomDrawRowIndicator(ByVal sender As Object, ByVal e As RowIndicatorCustomDrawEventArgs)
			If (Not e.Info.IsRowIndicator) Then
				Return
			End If
			If (Not HasBookmark(e.RowHandle)) Then
				Return
			End If
			e.Painter.DrawObject(e.Info) ' Default drawing
			e.Handled = True
			DrawBookmark(e)
		End Sub
		Private Sub View_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
			If e.Button <> MouseButtons.Right Then
				Return
			End If
			Dim hitInfo As GridHitInfo = View.CalcHitInfo(e.Location)
			If hitInfo.HitTest <> GridHitTest.RowIndicator Then
				Return
			End If
			StandardMenuManager.Default.ShowPopupMenu(CreateMenu(), View.GridControl, e.Location)
		End Sub
		Private Sub View_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
			If (Not e.Control) Then
				Return
			End If
			If e.KeyCode < Keys.D0 OrElse e.KeyCode > Keys.D9 Then
				Return
			End If
			If e.Shift Then
				SetBookmark(View.FocusedRowHandle, e.KeyCode - Keys.D0)
			Else
				GotoBookmark(e.KeyCode - Keys.D0)
			End If
		End Sub
		Private Sub SetBookmarkMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
			Dim menuItem As DXMenuItem = TryCast(sender, DXMenuItem)
			SetBookmark(View.FocusedRowHandle, CInt(Fix(menuItem.Tag)))
		End Sub
		Private Sub GotoBookmarkMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
			Dim menuItem As DXMenuItem = TryCast(sender, DXMenuItem)
			GotoBookmark(CInt(Fix(menuItem.Tag)))
		End Sub

		#Region "IDisposable Members"
		Public Sub Dispose() Implements IDisposable.Dispose
			If View IsNot Nothing AndAlso (Not View.GridControl.IsDisposed) Then
				RemoveHandler View.MouseUp, AddressOf View_MouseUp
				RemoveHandler View.KeyDown, AddressOf View_KeyDown
				RemoveHandler View.CustomDrawRowIndicator, AddressOf View_CustomDrawRowIndicator
				Me.view_Renamed = Nothing
			End If
		End Sub
		#End Region
		Protected Class BookmarkData
			Private value_Renamed As Object
			Public Sub New(ByVal value As Object)
				Me.value_Renamed = value
			End Sub
			Public ReadOnly Property Value() As Object
				Get
					Return value_Renamed
				End Get
			End Property
		End Class
	End Class
End Namespace