Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Bookmarks

Namespace E1267
	Partial Public Class FormMain
		Inherits Form
		Public Sub New()
			InitializeComponent()
			Dim TempGridViewBookmarks As GridViewBookmarks = New GridViewBookmarks(Me.gridView1, "OrderID")
		End Sub

		Private Sub FormMain_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			' TODO: This line of code loads data into the 'nwindDataSet.Orders' table. You can move, or remove it, as needed.
			Me.ordersTableAdapter.Fill(Me.nwindDataSet.Orders)

		End Sub
	End Class
End Namespace