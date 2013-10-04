$().ready(function () {
    $('#dialogContent').dialog({
        autoOpen: false,
        modal: true,
        bgiframe: true,
        title: "Confirm Action : ",
        width: 280,
        height: 170
    });
});

function confirmAction(uniqueID) {

    $('#dialogContent').dialog('option', 'buttons',
        {
            "OK": function () { __doPostBack(uniqueID, ''); $(this).dialog("close"); },
            "Cancel": function () { $(this).dialog("close"); }
        });

    $('#dialogContent').dialog('open');

    return false;
}

function confirmAction2(uniqueID) {

    $('#dialogContent').dialog('option', 'buttons',
        {
            "OK": function () { __doPostBack(uniqueID, ''); $(this).dialog("close"); },
            "Cancel": function () { $(this).dialog("close"); }
        });

    $('#dialogContent').dialog('open');

    return false;
}

$('#ShowDiv')
    .hide()  // hide
    .ajaxStart(function () {
        $(this).show();
    })
    .ajaxStop(function () {
        $(this).hide();
    });