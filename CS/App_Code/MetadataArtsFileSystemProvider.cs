using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using DevExpress.Web;

public class MetadataArtsFileSystemProvider : ArtsFileSystemProvider
{
    private List<ArtsFileSystemItem> allRecords;
    private List<ArtsFileSystemItem> AllRecords {
        get {
            if (allRecords == null)
                allRecords = ArtsDataProvider.GetArts();
            return allRecords;
        }
    }

    public MetadataArtsFileSystemProvider(string rootFolder) : base(rootFolder) { }

    public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
    {
        foreach (var file in base.GetFiles(folder))
            yield return new FileManagerFile(this, folder, file.FullName,
                new FileManagerFileProperties
                {
                    Metadata = GetMetadataField(AllRecords, file),
                });
    }

    public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
    {
        foreach (var folder in base.GetFolders(parentFolder))
            yield return new FileManagerFolder(this, parentFolder, folder.FullName,
                new FileManagerFolderProperties
                {
                    Metadata = GetMetadataField(AllRecords, folder),
                });
    }

    private Dictionary<string, object> GetMetadataField(List<ArtsFileSystemItem> items, FileManagerItem fileManagerItem)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        foreach (var item in items)
        {
            if(item.ArtID.ToString() == fileManagerItem.Id)
            {
                result.Add("CreationDate", item.CreationDate.ToString().Trim());
                result.Add("OwnerUserName", item.OwnerUserName.ToString().Trim());
                result.Add("Type", item.Type.ToString().Trim());
                return result;
            }
        }
        return result;
    }

}