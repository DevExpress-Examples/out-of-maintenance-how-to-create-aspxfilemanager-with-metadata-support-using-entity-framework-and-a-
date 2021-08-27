<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128563972/17.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T575127)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [ArtsFileSystemProvider.cs](./CS/App_Code/ArtsFileSystemProvider.cs) (VB: [ArtsFileSystemProvider.vb](./VB/App_Code/ArtsFileSystemProvider.vb))
* [MetadataArtsFileSystemProvider.cs](./CS/App_Code/MetadataArtsFileSystemProvider.cs) (VB: [MetadataArtsFileSystemProvider.vb](./VB/App_Code/MetadataArtsFileSystemProvider.vb))
* [Model.Context.cs](./CS/App_Code/Model.Context.cs) (VB: [Model.Context.vb](./VB/App_Code/Model.Context.vb))
* [Model.cs](./CS/App_Code/Model.cs) (VB: [Model.vb](./VB/App_Code/Model.vb))
* **[Default.aspx](./CS/Default.aspx) (VB: [Default.aspx](./VB/Default.aspx))**
* [Default.aspx.cs](./CS/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/Default.aspx.vb))
* [JavaScript.js](./CS/JavaScript.js) (VB: [JavaScript.js](./VB/JavaScript.js))
<!-- default file list end -->
# How to create ASPxFileManager with metadata support using Entity Framework and a database as a filestore
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/t575127/)**
<!-- run online end -->


<p>This example illustrates how to get metadata from folders and files.</p>
<p>The <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.ASPxFileManager.class">ASPxFileManager</a> control provides access to metadata of files and folders starting with the 17.2 version. It has the <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.FileManagerItemProperties.Metadata.property">FileManagerItemProperties.Metadata</a> property that contains a dictionary with metadata. The Metadata property is used to specify the item's metadata using an appropriate <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.FileManagerFile..ctor.overloads">FileManagerFile.FileManagerFile</a> or <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.FileManagerFolder..ctor.overloads">FileManagerFolder.FileManagerFolder</a> constructor.</p>
<p>The ASPxFileManager has the <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.Scripts.ASPxClientFileManagerItem.GetMetadata.method">ASPxClientFileManagerItem.GetMetadata</a> method, which is used to get the current File Manager item's metadata on the client side.</p>

<br/>


