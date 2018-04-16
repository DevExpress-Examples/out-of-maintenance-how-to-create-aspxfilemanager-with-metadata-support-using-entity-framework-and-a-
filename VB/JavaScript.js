function showMetadataPopup(items) {
    items.forEach(function (item) {
        var metadata = item.GetMetadata();
        for (var key in metadata) {
            var value = metadata[key];
            switch (key) {
                case "CreationDate":
                    creationDate.SetText(value);
                    break;
                case "OwnerUserName":
                    ownerUserName.SetText(value);
                    break;
                case "Type":
                    type.SetText(value);
                    break;
            }
        }
        popupControl.ShowAtElement(fileManager.GetMainElement());
    });
}

function onCustomCommand(s, e) {
    switch (e.commandName) {
        case "ShowDetails":
            showMetadataPopup(fileManager.GetSelectedItems());
            break;
    }
}

function onToolbarUpdating(s, e) {
    var items = fileManager.GetSelectedItems();
    var enabled = items.length > 0 && e.activeAreaName != "None" && e.activeAreaName != "Folders";
    items.forEach(function (item) {
        if (item.type == "ParentFolder") enabled = false;
        fileManager.GetContextMenuItemByCommandName("ShowDetails").SetEnabled(enabled);
    });
}
