Imports DevExpress.Data.Browsing
Imports DevExpress.Web
Imports System
Imports System.Collections.Generic
Imports System.Data.Entity
Imports System.IO
Imports System.Linq
Imports System.Web

Public NotInheritable Class ArtsDataProvider

    Private Sub New()
    End Sub

    Private Const ArtsDataContextKey As String = "DXArtsDataContext"
    Public Shared ReadOnly Property DB() As ArtsDBEntities
        Get
            If HttpContext.Current.Items(ArtsDataContextKey) Is Nothing Then
                HttpContext.Current.Items(ArtsDataContextKey) = New ArtsDBEntities()
            End If
            Return DirectCast(HttpContext.Current.Items(ArtsDataContextKey), ArtsDBEntities)
        End Get
    End Property
    Public Shared Function GetArts() As List(Of ArtsFileSystemItem)
        Dim arts As List(Of ArtsFileSystemItem) = DirectCast(HttpContext.Current.Session("Arts"), List(Of ArtsFileSystemItem))
        If arts Is Nothing Then
            arts = ( _
                From art In DB.Arts _
                Select New ArtsFileSystemItem With { _
                    .ArtID = art.ID, _
                    .ParentID = art.ParentID, _
                    .Name = art.Name, _
                    .IsFolder = If(art.IsFolder, False), _
                    .Data = art.Data, _
                    .LastWriteTime = art.LastWriteTime, _
                    .CreationDate = art.CreationDate, _
                    .OwnerUserName = art.OwnerUserName, _
                    .Type = art.Type _
                }).ToList()
            HttpContext.Current.Session("Arts") = arts
        End If
        Return arts
    End Function
    Public Shared Sub InsertArt(ByVal newArt As ArtsFileSystemItem)
        newArt.ArtID = GetNewArtID()
        GetArts().Add(newArt)
    End Sub
    Public Shared Sub DeleteArt(ByVal art As ArtsFileSystemItem)
        If art.IsFolder Then
            Dim childFolders As List(Of ArtsFileSystemItem) = GetArts().FindAll(Function(item) item.IsFolder AndAlso item.ParentID = art.ArtID)
            If childFolders IsNot Nothing Then
                For Each childFolder As ArtsFileSystemItem In childFolders
                    DeleteArt(childFolder)
                Next childFolder
            End If
        End If
        GetArts().Remove(art)
    End Sub
    Public Shared Sub UpdateArt(ByVal art As ArtsFileSystemItem, ByVal update As Action(Of ArtsFileSystemItem))
        update(art)
    End Sub
    Private Shared Function GetNewArtID() As Integer
        Dim arts As IEnumerable(Of ArtsFileSystemItem) = GetArts()
        Return If(arts.Count() > 0, arts.Last().ArtID + 1, 0)
    End Function
End Class
Public Class ArtsFileSystemItem
    Public Property ArtID() As Integer
    Public Property ParentID() As Integer?
    Public Property Name() As String
    Public Property IsFolder() As Boolean
    Public Property Data() As Byte()
    Public Property LastWriteTime() As Date?
    Public Property CreationDate() As Date?
    Public Property OwnerUserName() As String
    Public Property Type() As String

End Class
Public Class ArtsFileSystemProvider
    Inherits FileSystemProviderBase

    Private Const ArtsRootItemID As Integer = 1

    Private rootFolderDisplayName_Renamed As String

    Private folderCache_Renamed As Dictionary(Of Integer, ArtsFileSystemItem)
    Public Sub New(ByVal rootFolder As String)
        MyBase.New(rootFolder)
        RefreshFolderCache()
    End Sub
    Public Overrides ReadOnly Property RootFolderDisplayName() As String
        Get
            Return rootFolderDisplayName_Renamed
        End Get
    End Property
    Public ReadOnly Property FolderCache() As Dictionary(Of Integer, ArtsFileSystemItem)
        Get
            Return folderCache_Renamed
        End Get
    End Property
    Public Overrides Function GetFiles(ByVal folder As FileManagerFolder) As IEnumerable(Of FileManagerFile)
        Dim artFolderItem As ArtsFileSystemItem = FindArtsFolderItem(folder)
        Return From artItem In ArtsDataProvider.GetArts() _
            Where Not artItem.IsFolder AndAlso artItem.ParentID = artFolderItem.ArtID _
            Select New FileManagerFile(Me, folder, artItem.Name, artItem.ArtID.ToString())
    End Function
    Public Overrides Function GetFolders(ByVal parentFolder As FileManagerFolder) As IEnumerable(Of FileManagerFolder)
        Dim artFolderItem As ArtsFileSystemItem = FindArtsFolderItem(parentFolder)
        Return From artItem In FolderCache.Values _
            Where artItem.IsFolder AndAlso artItem.ParentID = artFolderItem.ArtID _
            Select New FileManagerFolder(Me, parentFolder, artItem.Name, artItem.ArtID.ToString())
    End Function
    Public Overrides Function Exists(ByVal file As FileManagerFile) As Boolean
        Return FindArtsFileItem(file) IsNot Nothing
    End Function
    Public Overrides Function Exists(ByVal folder As FileManagerFolder) As Boolean
        Return FindArtsFolderItem(folder) IsNot Nothing
    End Function
    Public Overrides Function ReadFile(ByVal file As FileManagerFile) As Stream
        Return New MemoryStream(FindArtsFileItem(file).Data.ToArray())
    End Function
    Public Overrides Function GetLastWriteTime(ByVal file As FileManagerFile) As Date
        Dim artsFileItem = FindArtsFileItem(file)
        Return artsFileItem.LastWriteTime.GetValueOrDefault(Date.Now)
    End Function
    Public Overrides Function GetLength(ByVal file As FileManagerFile) As Long
        Dim artsFileItem = FindArtsFileItem(file)
        Return artsFileItem.Data.Length
    End Function
    Public Overrides Sub CreateFolder(ByVal parent As FileManagerFolder, ByVal name As String)
        ArtsDataProvider.InsertArt(New ArtsFileSystemItem With { _
            .IsFolder = True, _
            .LastWriteTime = Date.Now, _
            .Name = name, _
            .ParentID = FindArtsFolderItem(parent).ArtID _
        })
        RefreshFolderCache()
    End Sub
    Public Overrides Sub RenameFile(ByVal file As FileManagerFile, ByVal name As String)
        ArtsDataProvider.UpdateArt(FindArtsFileItem(file), Sub(artItem) artItem.Name = name)
    End Sub
    Public Overrides Sub RenameFolder(ByVal folder As FileManagerFolder, ByVal name As String)
        ArtsDataProvider.UpdateArt(FindArtsFolderItem(folder), Sub(artItem) artItem.Name = name)
        RefreshFolderCache()
    End Sub
    Public Overrides Sub MoveFile(ByVal file As FileManagerFile, ByVal newParentFolder As FileManagerFolder)
        ArtsDataProvider.UpdateArt(FindArtsFileItem(file), Sub(artItem) artItem.ParentID = FindArtsFolderItem(newParentFolder).ArtID)
    End Sub
    Public Overrides Sub MoveFolder(ByVal folder As FileManagerFolder, ByVal newParentFolder As FileManagerFolder)
        ArtsDataProvider.UpdateArt(FindArtsFolderItem(folder), Sub(artItem) artItem.ParentID = FindArtsFolderItem(newParentFolder).ArtID)
        RefreshFolderCache()
    End Sub
    Public Overrides Sub UploadFile(ByVal folder As FileManagerFolder, ByVal fileName As String, ByVal content As Stream)
        ArtsDataProvider.InsertArt(New ArtsFileSystemItem With { _
            .IsFolder = False, _
            .LastWriteTime = Date.Now, _
            .Name = fileName, _
            .ParentID = FindArtsFolderItem(folder).ArtID, _
            .Data = ReadAllBytes(content) _
        })
    End Sub
    Public Overrides Sub DeleteFile(ByVal file As FileManagerFile)
        ArtsDataProvider.DeleteArt(FindArtsFileItem(file))
    End Sub
    Public Overrides Sub DeleteFolder(ByVal folder As FileManagerFolder)
        ArtsDataProvider.DeleteArt(FindArtsFolderItem(folder))
        RefreshFolderCache()
    End Sub
    Protected Function FindArtsFileItem(ByVal file As FileManagerFile) As ArtsFileSystemItem
        Dim artsFolderItem As ArtsFileSystemItem = FindArtsFolderItem(file.Folder)
        If artsFolderItem Is Nothing Then
            Return Nothing
        End If
        Return ArtsDataProvider.GetArts().FindAll(Function(item) CInt((item.ParentID)) = artsFolderItem.ArtID AndAlso Not item.IsFolder AndAlso item.Name = file.Name).FirstOrDefault()
    End Function
    Protected Function FindArtsFolderItem(ByVal folder As FileManagerFolder) As ArtsFileSystemItem
        Return ( _
            From artFolderItem In FolderCache.Values _
            Where artFolderItem.IsFolder AndAlso GetRelativeName(artFolderItem) = folder.RelativeName _
            Select artFolderItem).FirstOrDefault()
    End Function
    Protected Function GetRelativeName(ByVal artFolderItem As ArtsFileSystemItem) As String
        If artFolderItem.ArtID = ArtsRootItemID Then
            Return String.Empty
        End If
        If artFolderItem.ParentID.Equals(ArtsRootItemID) Then
            Return artFolderItem.Name
        End If
        If Not FolderCache.ContainsKey(CInt(artFolderItem.ParentID)) Then
            Return Nothing
        End If
        Dim name As String = GetRelativeName(FolderCache(CInt(artFolderItem.ParentID)))
        Return If(name Is Nothing, Nothing, Path.Combine(name, artFolderItem.Name))
    End Function
    Protected Sub RefreshFolderCache()
        Me.folderCache_Renamed = ArtsDataProvider.GetArts().FindAll(Function(artItem) artItem.IsFolder).ToDictionary(Function(artItem) artItem.ArtID)
        Me.rootFolderDisplayName_Renamed = ( _
            From artFolderItem In FolderCache.Values _
            Where artFolderItem.ArtID = ArtsRootItemID _
            Select artFolderItem.Name).First()
    End Sub
    Protected Shared Function ReadAllBytes(ByVal stream As Stream) As Byte()
        Dim buffer((16 * 1024) - 1) As Byte
        Dim readCount As Integer
        Using ms As New MemoryStream()
            readCount = stream.Read(buffer, 0, buffer.Length)
            Do While readCount > 0
                ms.Write(buffer, 0, readCount)
                readCount = stream.Read(buffer, 0, buffer.Length)
            Loop
            Return ms.ToArray()
        End Using
    End Function
End Class
