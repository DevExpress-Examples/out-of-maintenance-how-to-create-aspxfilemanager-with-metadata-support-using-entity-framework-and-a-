Imports System
Imports System.Collections.Generic
Imports System.Web.UI.WebControls
Imports System.Linq
Imports System.Data
Imports System.IO
Imports System.Web
Imports System.Web.UI
Imports DevExpress.Web

Public Class MetadataArtsFileSystemProvider
    Inherits ArtsFileSystemProvider


    Private allRecords_Renamed As List(Of ArtsFileSystemItem)
    Private ReadOnly Property AllRecords() As List(Of ArtsFileSystemItem)
        Get
            If allRecords_Renamed Is Nothing Then
                allRecords_Renamed = ArtsDataProvider.GetArts()
            End If
            Return allRecords_Renamed
        End Get
    End Property

    Public Sub New(ByVal rootFolder As String)
        MyBase.New(rootFolder)
    End Sub

    Public Overrides Iterator Function GetFiles(ByVal folder As FileManagerFolder) As IEnumerable(Of FileManagerFile)
        For Each file In MyBase.GetFiles(folder)
            Yield New FileManagerFile(Me, folder, file.FullName, New FileManagerFileProperties With {.Metadata = GetMetadataField(AllRecords, file)})
        Next file
    End Function

    Public Overrides Iterator Function GetFolders(ByVal parentFolder As FileManagerFolder) As IEnumerable(Of FileManagerFolder)
        For Each folder In MyBase.GetFolders(parentFolder)
            Yield New FileManagerFolder(Me, parentFolder, folder.FullName, New FileManagerFolderProperties With {.Metadata = GetMetadataField(AllRecords, folder)})
        Next folder
    End Function

    Private Function GetMetadataField(ByVal items As List(Of ArtsFileSystemItem), ByVal fileManagerItem As FileManagerItem) As Dictionary(Of String, Object)
        Dim result As New Dictionary(Of String, Object)()
        For Each item In items
            If item.ArtID.ToString() = fileManagerItem.Id Then
                result.Add("CreationDate", item.CreationDate.ToString().Trim())
                result.Add("OwnerUserName", item.OwnerUserName.ToString().Trim())
                result.Add("Type", item.Type.ToString().Trim())
                Return result
            End If
        Next item
        Return result
    End Function

End Class