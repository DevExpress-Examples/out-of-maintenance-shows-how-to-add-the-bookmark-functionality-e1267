# Shows how to add the bookmark functionality


<p>The following example shows how to implement the bookmark functionality in XtraGridControl.<br />
End-users can use the Ctrl + Shift + 0..9 shortcuts to set a bookmark for the focused record, and the Ctrl + 0..9 shortcuts to go to the appropriate bookmark.<br />
They can also use the bookmark menu for this purpose. It can be raised by right-clicking on the row indicator.<br />
To enable this functionality in your application, you will have to include the GridViewBookmarks.cs(vb) file to it and write the following line of code in your constructor:<br />
new GridViewBookmarks(yourGridView, "yourKeyFieldName");</p>

<br/>


