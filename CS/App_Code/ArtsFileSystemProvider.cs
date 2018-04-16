using DevExpress.Data.Browsing;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

public static class ArtsDataProvider
{
    const string ArtsDataContextKey = "DXArtsDataContext";
    public static ArtsDBEntities DB
    {
        get
        {
            if (HttpContext.Current.Items[ArtsDataContextKey] == null)
                HttpContext.Current.Items[ArtsDataContextKey] = new ArtsDBEntities();
            return (ArtsDBEntities)HttpContext.Current.Items[ArtsDataContextKey];
        }
    }
    public static List<ArtsFileSystemItem> GetArts()
    {
        List<ArtsFileSystemItem> arts = (List<ArtsFileSystemItem>)HttpContext.Current.Session["Arts"];
        if (arts == null)
        {
            arts = (from art in DB.Arts
                    select new ArtsFileSystemItem
                    {
                        ArtID = art.ID,
                        ParentID = art.ParentID,
                        Name = art.Name,
                        IsFolder = art.IsFolder ?? false,
                        Data = art.Data,
                        LastWriteTime = art.LastWriteTime,
                        CreationDate = art.CreationDate,
                        OwnerUserName = art.OwnerUserName,
                        Type = art.Type,
                    }
                    ).ToList();
            HttpContext.Current.Session["Arts"] = arts;
        }
        return arts;
    }
    public static void InsertArt(ArtsFileSystemItem newArt)
    {
        newArt.ArtID = GetNewArtID();
        GetArts().Add(newArt);
    }
    public static void DeleteArt(ArtsFileSystemItem art)
    {
        if (art.IsFolder)
        {
            List<ArtsFileSystemItem> childFolders = GetArts().FindAll(item => item.IsFolder && item.ParentID == art.ArtID);
            if (childFolders != null)
            {
                foreach (ArtsFileSystemItem childFolder in childFolders)
                {
                    DeleteArt(childFolder);
                }
            }
        }
        GetArts().Remove(art);
    }
    public static void UpdateArt(ArtsFileSystemItem art, Action<ArtsFileSystemItem> update)
    {
        update(art);
    }
    static int GetNewArtID()
    {
        IEnumerable<ArtsFileSystemItem> arts = GetArts();
        return (arts.Count() > 0) ? arts.Last().ArtID + 1 : 0;
    }
}
public class ArtsFileSystemItem
{
    public int ArtID { get; set; }
    public int? ParentID { get; set; }
    public string Name { get; set; }
    public bool IsFolder { get; set; }
    public byte[] Data { get; set; }
    public DateTime? LastWriteTime { get; set; }
    public DateTime? CreationDate { get; set; }
    public string OwnerUserName { get; set; }
    public string Type { get; set; }

}
public class ArtsFileSystemProvider : FileSystemProviderBase
{
    const int ArtsRootItemID = 1;
    string rootFolderDisplayName;
    Dictionary<int, ArtsFileSystemItem> folderCache;
    public ArtsFileSystemProvider(string rootFolder)
        : base(rootFolder)
    {
        RefreshFolderCache();
    }
    public override string RootFolderDisplayName { get { return rootFolderDisplayName; } }
    public Dictionary<int, ArtsFileSystemItem> FolderCache { get { return folderCache; } }
    public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
    {
        ArtsFileSystemItem artFolderItem = FindArtsFolderItem(folder);
        return from artItem in ArtsDataProvider.GetArts()
                where !artItem.IsFolder && artItem.ParentID == artFolderItem.ArtID
                select new FileManagerFile(this, folder, artItem.Name, artItem.ArtID.ToString());
    }
    public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
    {
        ArtsFileSystemItem artFolderItem = FindArtsFolderItem(parentFolder);
        return from artItem in FolderCache.Values
                where artItem.IsFolder && artItem.ParentID == artFolderItem.ArtID
                select new FileManagerFolder(this, parentFolder, artItem.Name, artItem.ArtID.ToString());
    }
    public override bool Exists(FileManagerFile file)
    {
        return FindArtsFileItem(file) != null;
    }
    public override bool Exists(FileManagerFolder folder)
    {
        return FindArtsFolderItem(folder) != null;
    }
    public override Stream ReadFile(FileManagerFile file)
    {
        return new MemoryStream(FindArtsFileItem(file).Data.ToArray());
    }
    public override DateTime GetLastWriteTime(FileManagerFile file)
    {
        var artsFileItem = FindArtsFileItem(file);
        return artsFileItem.LastWriteTime.GetValueOrDefault(DateTime.Now);
    }
    public override long GetLength(FileManagerFile file)
    {
        var artsFileItem = FindArtsFileItem(file);
        return artsFileItem.Data.Length;
    }
    public override void CreateFolder(FileManagerFolder parent, string name)
    {
        ArtsDataProvider.InsertArt(new ArtsFileSystemItem
        {
            IsFolder = true,
            LastWriteTime = DateTime.Now,
            Name = name,
            ParentID = FindArtsFolderItem(parent).ArtID
        });
        RefreshFolderCache();
    }
    public override void RenameFile(FileManagerFile file, string name)
    {
        ArtsDataProvider.UpdateArt(FindArtsFileItem(file), artItem => artItem.Name = name);
    }
    public override void RenameFolder(FileManagerFolder folder, string name)
    {
        ArtsDataProvider.UpdateArt(FindArtsFolderItem(folder), artItem => artItem.Name = name);
        RefreshFolderCache();
    }
    public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder)
    {
        ArtsDataProvider.UpdateArt(FindArtsFileItem(file), artItem => artItem.ParentID = FindArtsFolderItem(newParentFolder).ArtID);
    }
    public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
    {
        ArtsDataProvider.UpdateArt(FindArtsFolderItem(folder), artItem => artItem.ParentID = FindArtsFolderItem(newParentFolder).ArtID);
        RefreshFolderCache();
    }
    public override void UploadFile(FileManagerFolder folder, string fileName, Stream content)
    {
        ArtsDataProvider.InsertArt(new ArtsFileSystemItem
        {
            IsFolder = false,
            LastWriteTime = DateTime.Now,
            Name = fileName,
            ParentID = FindArtsFolderItem(folder).ArtID,
            Data = ReadAllBytes(content)
        });
    }
    public override void DeleteFile(FileManagerFile file)
    {
        ArtsDataProvider.DeleteArt(FindArtsFileItem(file));
    }
    public override void DeleteFolder(FileManagerFolder folder)
    {
        ArtsDataProvider.DeleteArt(FindArtsFolderItem(folder));
        RefreshFolderCache();
    }
    protected ArtsFileSystemItem FindArtsFileItem(FileManagerFile file)
    {
        ArtsFileSystemItem artsFolderItem = FindArtsFolderItem(file.Folder);
        if (artsFolderItem == null)
            return null;
        return ArtsDataProvider.GetArts().FindAll(item => (int)item.ParentID == artsFolderItem.ArtID && !item.IsFolder && item.Name == file.Name).FirstOrDefault();
    }
    protected ArtsFileSystemItem FindArtsFolderItem(FileManagerFolder folder)
    {
        return (from artFolderItem in FolderCache.Values
                where artFolderItem.IsFolder && GetRelativeName(artFolderItem) == folder.RelativeName
                select artFolderItem).FirstOrDefault();
    }
    protected string GetRelativeName(ArtsFileSystemItem artFolderItem)
    {
        if (artFolderItem.ArtID == ArtsRootItemID) return string.Empty;
        if (artFolderItem.ParentID == ArtsRootItemID) return artFolderItem.Name;
        if (!FolderCache.ContainsKey((int)artFolderItem.ParentID)) return null;
        string name = GetRelativeName(FolderCache[(int)artFolderItem.ParentID]);
        return name == null ? null : Path.Combine(name, artFolderItem.Name);
    }
    protected void RefreshFolderCache()
    {
        this.folderCache = ArtsDataProvider.GetArts().FindAll(artItem => artItem.IsFolder).ToDictionary(artItem => artItem.ArtID);
        this.rootFolderDisplayName = (from artFolderItem in FolderCache.Values where artFolderItem.ArtID == ArtsRootItemID select artFolderItem.Name).First();
    }
    protected static byte[] ReadAllBytes(Stream stream)
    {
        byte[] buffer = new byte[16 * 1024];
        int readCount;
        using (MemoryStream ms = new MemoryStream())
        {
            while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, readCount);
            }
            return ms.ToArray();
        }
    }
}
