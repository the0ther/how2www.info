function hideNotesPopup()
{
    var iframe = document.getElementById('notesIFrame');
    var popup = document.getElementById('notesPanel');
    var frameDoc = window.frames.notesIFrame.document;    
    iframe.src = '';
    popup.style.display = 'none';
}

function showpopup(id,weekNum,loginId, userId, clientId)
{
    var popupiframe = document.getElementById("popupiframe");
    var popuppanel = document.getElementById("popupanel");
	popupiframe.src = "UserJobs.aspx?UserId=" + userId + "&ClientId=" + clientId + '&LoginId=' + loginId + '&WeekNumber=' + weekNum;
    popuppanel.style.display = ''; 
}

function hidepopup()
{
    var popupiframe = document.getElementById("popupiframe");
    popupiframe.src = "";
    var popuppanel = document.getElementById("popupanel");
    popuppanel.style.display = 'none'; 
    var rebind = window.parent.document.getElementById('rebind');
    rebind.value = 'true';
    var form = window.parent.document.getElementById('form1');
    form.submit();
}

function showNotesPopup(allocId, enabled)
{
    var iframe = document.getElementById('notesIFrame');
    var popup = document.getElementById('notesPanel');
    iframe.src = 'AllocationNote.aspx?AllocId=' + allocId + '&Enabled=' + enabled;
    centerNote();
    popup.style.display = '';
    return false;
}

function centerNote()
{
    //console.log('entering centerNote()');
    var obj = $('#notesPanel');
    var vWidth = $(window).width();
    var vHeight = $(window).height();
    var vScroll = 0;
    var eWidth = $(obj).width(); //width of the element to be centered
    var eHeight = $(obj).height();
    var x = 0;
    if ($.browser.msie)
    {
        vScroll = window.document.documentElement.scrollTop;
        x = document.documentElement.scrollLeft + (vWidth/2) - (eWidth/2); 
    }
    else
    {
        vScroll = window.scrollY;
        x = document.body.scrollLeft + (vWidth/2) - (eWidth/2); 
    }

    //console.log('scrollLeft: ' + document.documentElement.scrollLeft + ' vWidth: ' + vWidth + ' eWidth: ' + eWidth);
    //console.log('vHeight: ' + vHeight + ' and eHeight: ' + eHeight + ' and vScroll: ' + vScroll);
    var y = ((vHeight/2) - (eHeight/2)) + vScroll;
    //console.log('setting note left,top to: ' + x + ',' + y);
    $(obj).css({
        position: 'absolute',
        left: x,
        top: y
    });
}