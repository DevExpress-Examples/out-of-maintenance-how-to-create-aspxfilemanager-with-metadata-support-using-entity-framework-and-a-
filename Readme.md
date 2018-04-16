# How to create ASPxFileManager with metadata support using Entity Framework and a database as a filestore


<p>This example illustrates how to get metadata from folders and files.</p>
<p>The <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.ASPxFileManager.class">ASPxFileManager</a> control provides access to metadata of files and folders starting with the 17.2 version. It has the <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.FileManagerItemProperties.Metadata.property">FileManagerItemProperties.Metadata</a> property that contains a dictionary with metadata. The Metadata property is used to specify the item's metadata using an appropriate <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.FileManagerFile..ctor.overloads">FileManagerFile.FileManagerFile</a> or <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.FileManagerFolder..ctor.overloads">FileManagerFolder.FileManagerFolder</a> constructor.</p>
<p>The ASPxFileManager has the <a href="https://documentation.devexpress.com/AspNet/DevExpress.Web.Scripts.ASPxClientFileManagerItem.GetMetadata.method">ASPxClientFileManagerItem.GetMetadata</a> method, which is used to get the current File Manager item's metadata on the client side.</p>

<br/>


