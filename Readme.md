<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128632601/13.1.4%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E1267)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [Form1.cs](./CS/E1267/Form1.cs) (VB: [Form1.vb](./VB/E1267/Form1.vb))
* [GridViewBookmarks.cs](./CS/E1267/GridViewBookmarks.cs) (VB: [GridViewBookmarks.vb](./VB/E1267/GridViewBookmarks.vb))
* [Program.cs](./CS/E1267/Program.cs) (VB: [Program.vb](./VB/E1267/Program.vb))
<!-- default file list end -->
# Shows how to add the bookmark functionality


<p>The following example shows how to implement the bookmark functionality in XtraGridControl.<br />
End-users can use the Ctrl + Shift + 0..9 shortcuts to set a bookmark for the focused record, and the Ctrl + 0..9 shortcuts to go to the appropriate bookmark.<br />
They can also use the bookmark menu for this purpose. It can be raised by right-clicking on the row indicator.<br />
To enable this functionality in your application, you will have to include the GridViewBookmarks.cs(vb) file to it and write the following line of code in your constructor:<br />
new GridViewBookmarks(yourGridView, "yourKeyFieldName");</p>

<br/>


