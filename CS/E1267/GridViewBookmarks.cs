using System;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.Menu;
using System.Windows.Forms;
using System.Drawing;

namespace DevExpress.XtraGrid.Bookmarks {
    public class GridViewBookmarks : IDisposable {
        const string SetBookmarkStr = "Set bookmark";
        const string GotoBookmarkStr = "Go to bookmark";
        const string BookmarkStr = "Bookmark ";
        const int BookmarkCount = 10;
        GridView view;
        string keyFieldName;
        BookmarkData[] bookMarks = new BookmarkData[BookmarkCount];

        public GridViewBookmarks(GridView view, string keyFieldName) {
            this.view = view;
            this.keyFieldName = keyFieldName;
            View.MouseUp += new System.Windows.Forms.MouseEventHandler(View_MouseUp);
            View.KeyDown += new System.Windows.Forms.KeyEventHandler(View_KeyDown);
            View.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(View_CustomDrawRowIndicator);
        }

        public GridView View { get { return view; } }
        public string KeyFieldName { get { return keyFieldName; } }
        protected BookmarkData[] BookMarks { get { return bookMarks; } }

        protected void SetBookmark(int rowHandle, int bookmarkIndex) {
            while (View.IsGroupRow(rowHandle)) {
                rowHandle = View.GetChildRowHandle(rowHandle, 0);
            }
            if (GetBookmarkIndex(rowHandle) == bookmarkIndex) {
                BookMarks[bookmarkIndex] = null;
            } else {
                if (BookMarks[bookmarkIndex] != null) {
                    View.InvalidateRowIndicator(GetRowHandle(bookmarkIndex));
                }
                BookMarks[bookmarkIndex] = CreateBookmarkData(rowHandle);
            }
            View.InvalidateRowIndicator(rowHandle);
        }
        protected void GotoBookmark(int bookmarkIndex) {
            if (BookMarks[bookmarkIndex] == null) return;
            View.FocusedRowHandle = GetRowHandle(bookmarkIndex);
        }
        protected bool HasBookmark(int rowHandle) {
            return GetBookmarkIndex(rowHandle) > -1;
        }
        protected virtual void DrawBookmark(RowIndicatorCustomDrawEventArgs e) {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(40, Color.DarkBlue))) {
                RectangleF bounds = new RectangleF();
                bounds.X = e.Info.Bounds.X;
                bounds.Y = e.Info.Bounds.Y;
                bounds.Width = e.Info.Bounds.Width;
                bounds.Height = e.Info.Bounds.Height;
                e.Cache.FillRectangle(brush, bounds);
            }
        }
        protected int GetRowHandle(int bookmarkIndex) {
            return View.DataController.FindRowByValue(KeyFieldName, BookMarks[bookmarkIndex].Value);
        }
        protected int GetBookmarkIndex(int rowHandle) {
            if (!View.IsDataRow(rowHandle)) return -1;
            object value = GetKeyValue(rowHandle);
            for (int i = 0; i < BookMarks.Length; i++) {
                if (BookMarks[i] == null) continue;
                if (object.Equals(BookMarks[i].Value, value)) return i;
            }
            return -1;
        }
        protected object GetKeyValue(int rowHandle) {
            return View.GetRowCellValue(rowHandle, KeyFieldName);
        }
        protected DXPopupMenu CreateMenu() {
            DXPopupMenu menu = new DXPopupMenu();
            menu.Items.Add(CreateSetBookmarkMenu());
            menu.Items.Add(CreateGotoBookmarkMenu());
            return menu;
        }
        protected DXSubMenuItem CreateSetBookmarkMenu() {
            DXSubMenuItem menuItem = new DXSubMenuItem(SetBookmarkStr);
            EventHandler eventHandler = new EventHandler(SetBookmarkMenuItem_Click);
            for (int i = 0; i < BookmarkCount; i++) {
                DXMenuCheckItem item = new DXMenuCheckItem(BookmarkStr + i.ToString());
                item.Click += eventHandler;
                item.Tag = i;
                item.Checked = BookMarks[i] != null;
                menuItem.Items.Add(item);
            }
            return menuItem;
        }
        protected DXSubMenuItem CreateGotoBookmarkMenu() {
            DXSubMenuItem menuItem = new DXSubMenuItem(GotoBookmarkStr);
            EventHandler eventHandler = new EventHandler(GotoBookmarkMenuItem_Click);
            for (int i = 0; i < BookmarkCount; i++) {
                if (BookMarks[i] != null) {
                    DXMenuItem item = new DXMenuItem(BookmarkStr + i.ToString(), eventHandler);
                    item.Tag = i;
                    menuItem.Items.Add(item);
                }
            }
            return menuItem;
        }
        protected BookmarkData CreateBookmarkData(int rowHandle) {
            return new BookmarkData(GetKeyValue(rowHandle));
        }
        void View_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e) {
            if (!e.Info.IsRowIndicator) return;
            if (!HasBookmark(e.RowHandle)) return;
            e.Painter.DrawObject(e.Info); // Default drawing
            e.Handled = true;
            DrawBookmark(e);
        }
        void View_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Right) return;
            GridHitInfo hitInfo = View.CalcHitInfo(e.Location);
            if (hitInfo.HitTest != GridHitTest.RowIndicator) return;
            StandardMenuManager.Default.ShowPopupMenu(CreateMenu(), View.GridControl, e.Location);
        }
        void View_KeyDown(object sender, KeyEventArgs e) {
            if (!e.Control) return;
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) return;
            if (e.Shift) {
                SetBookmark(View.FocusedRowHandle, e.KeyCode - Keys.D0);
            } else {
                GotoBookmark(e.KeyCode - Keys.D0);
            }
        }
        void SetBookmarkMenuItem_Click(object sender, EventArgs e) {
            DXMenuItem menuItem = sender as DXMenuItem;
            SetBookmark(View.FocusedRowHandle, (int)menuItem.Tag);
        }
        void GotoBookmarkMenuItem_Click(object sender, EventArgs e) {
            DXMenuItem menuItem = sender as DXMenuItem;
            GotoBookmark((int)menuItem.Tag);
        }

        #region IDisposable Members
        public void Dispose() {
            if (View != null && !View.GridControl.IsDisposed) {
                View.MouseUp -= new System.Windows.Forms.MouseEventHandler(View_MouseUp);
                View.KeyDown -= new System.Windows.Forms.KeyEventHandler(View_KeyDown);
                View.CustomDrawRowIndicator -= new RowIndicatorCustomDrawEventHandler(View_CustomDrawRowIndicator);
                this.view = null;
            }
        }
        #endregion
        protected class BookmarkData {
            object value;
            public BookmarkData(object value) {
                this.value = value;
            }
            public object Value { get { return value; } }
        }
    }
}