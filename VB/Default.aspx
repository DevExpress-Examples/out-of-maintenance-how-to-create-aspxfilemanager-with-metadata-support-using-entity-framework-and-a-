<%@ Page Language="vb" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Default" %>
<%@ Register Assembly="DevExpress.Web.v17.2, Version=17.2.16.0, Culture=neutral, PublicKeyToken=B88D1754D700E49A"
    Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>How to create ASPxFileManager with metadata support</title>
    <style>
        .popupContext{
            width: 100%;
            text-align: center;
            border-collapse: separate;
            border-spacing: 0px 5px;
            margin-bottom: 15px;
        }
        .popupContext td {
            padding: 10px 5px;
            border-bottom: 1px solid #c9c9c9;
        }
    </style>
    <script src="JavaScript.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <dx:ASPxFileManager ID="fileManager" ClientInstanceName="fileManager" ProviderType="Custom"
            CustomFileSystemProviderTypeName="MetadataArtsFileSystemProvider" runat="server">
            <ClientSideEvents CustomCommand="onCustomCommand" ToolbarUpdating="onToolbarUpdating" />
            <Settings EnableClientSideItemMetadata="true" />
            <SettingsFileList ShowFolders="true" ShowParentFolder="true"></SettingsFileList>
            <SettingsContextMenu Enabled="true">
                <Items>
                    <dx:FileManagerToolbarCustomButton Text="Show details" CommandName="ShowDetails">
                        <Image IconID="setup_properties_16x16" />
                    </dx:FileManagerToolbarCustomButton>
                </Items>
            </SettingsContextMenu>
        </dx:ASPxFileManager>
        <dx:ASPxPopupControl ID="popupControl" runat="server" ClientInstanceName="popupControl" CloseOnEscape="true"
            PopupHorizontalAlign="Center" PopupVerticalAlign="Middle" Width="300" HeaderText="Details">
            <ContentCollection>
                <dx:PopupControlContentControl>
                   <table class="popupContext">
                       <tr>
                           <td>Creation date:</td>
                           <td><dx:ASPxLabel ClientInstanceName="creationDate" runat="server"></dx:ASPxLabel></td>
                       </tr>
                       <tr>
                           <td>Owner username:</td>
                           <td><dx:ASPxLabel ClientInstanceName="ownerUserName" runat="server"></dx:ASPxLabel></td>
                       </tr>
                       <tr>
                           <td>Type:</td>
                           <td><dx:ASPxLabel ClientInstanceName="type" runat="server"></dx:ASPxLabel></td>
                       </tr>
                   </table>
                </dx:PopupControlContentControl>
            </ContentCollection>
        </dx:ASPxPopupControl>
    </div>
    </form>
</body>
</html>