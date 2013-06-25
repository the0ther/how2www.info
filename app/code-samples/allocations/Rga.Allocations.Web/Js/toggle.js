var closedToggles = new Array();

$(document).ready(function() {

    var togglesRecord = $('#togglesRecord');
    if (togglesRecord.val().toString() != '')
        closedToggles = togglesRecord.val().split(',');

    restoreToggles();

    //$('img.toggle-all + span').attr('style', 'color: red');

    $('img.toggle-all, img.toggle-all + span').click(function() {
        console.log('entering click handler for toggle-all');
        togglesRecord.val('');
        closedToggles = new Array();
        var $row = $('div.rowalt, tr.rowalt');
        var closedImg = false;
        var img = $(this);
        if ($(img).attr('src') == undefined)
            img = $(this).prev();
            
        if ($(img).attr('src').indexOf('closed') > -1)
            closedImg = true;
        if (!closedImg) {
            $row.hide();
            $(img).attr('src', 'Img/icn_closed.gif');
            //use img.toggle-row to get all the images and set their src
            var $imgs = $('img.toggle-row');
            $.each($imgs, function(k, v) {
                $(v).attr('src', 'Img/icn_closed.gif');
                //add the id from the img's id attr toggleImage_xxxxxx
                togglesRecord.val(togglesRecord.val() + ',' + $(v).attr('id').substring(12));
            });
            togglesRecord.val(togglesRecord.val().substring(1));
            closedToggles = togglesRecord.val().split(',');
        }
        else {
            $row.show();
            $(img).attr('src', 'Img/icn_open.gif');
            var $imgs = $('img.toggle-row');
            $.each($imgs, function(k, v) {
                $(v).attr('src', 'Img/icn_open.gif');
            });
        }
    });

    //team uses span labels, emp view uses a labels, and 
    $('img.toggle-row, span.toggle-row, a.toggle-row').click(function() {
        //console.log('toggling single row...');
        var hide = false;
        var $row = $(this).parents('div.rowmain, tr.main-row');
        var id = $(this).attr('id').substring($(this).attr('id').lastIndexOf('_') + 1);
        console.log('id is: ' + id);
        //check for class signalling project page set sibs accordingly
        var $siblings = null;
        $siblings = $(this).parent().parent().children('.rowalt');
        console.log('sibs.len: ' + $siblings.length);
        if ($siblings.length == 0 && location.href.indexOf('DeptAllocation.aspx') >= 0) {
            //only for dept page
            $siblings = $row.nextAll();
            $.each($siblings, function(k, v) {
                if ($(v).hasClass('main-row')) return false; //needed to prevent closing rows which come further down the page
                if ($(v).is(':visible')) {
                    $(v).hide();
                    hide = true;
                }
                else {
                    $(v).show();
                }
            });
        }
        else {
            //console.log('this was a non-dept page...');
            //all other pages
            $.each($siblings, function(kk, v) {
                if ($(v).is(':visible')) {
                    //console.log('hiding v: ' + v);
                    $(v).hide();
                    hide = true;
                }
                else {
                    //console.log('showing v: ' + v);
                    $(v).show();
                }
            });
        }
        console.log('src is: ' + $(this).attr('src'));
        if (hide) {
            if ($(this).attr('src') != undefined) {
                $(this).attr('src', './Img/icn_closed.gif');
            }
            else if ($(this).prev().attr('src') != undefined) {
                $(this).prev().attr('src', './Img/icn_closed.gif');
            }
            else {
                $(this).prev().prev().attr('src', './Img/icn_closed.gif');
            }
            closedToggles = addToArray(closedToggles, id);
        }
        else {
            if ($(this).attr('src') != undefined) {
                $(this).attr('src', './Img/icn_open.gif');
            }
            else if ($(this).prev().attr('src') != undefined) {
                $(this).prev().attr('src', './Img/icn_open.gif');
            }
            else {
                $(this).prev().prev().attr('src', './Img/icn_open.gif');
            }
            closedToggles = removeFromArray(closedToggles, id);
        }
        togglesRecord.val(closedToggles.join(','));
    });
});

function restoreToggles()
{
    var ids = $('#togglesRecord').val().split(',');
    for (var ii=0;ii<ids.length;ii++)
    {
        var thisId = '#toggleImage_' + ids[ii];
        var $siblings = $(thisId).parent().parent().children('.rowalt');
        if ($siblings.length == 0) 
            $siblings = $(thisId).parent().parent().nextAll();
	    $.each($siblings, function(k, v){
	        //console.log('v has classes: ' + $(v).attr('class'));
		    if ($(v).hasClass('main-row')) return false; //this stops processing of the rows...
		    if($(v).is(':visible'))
			    $(v).hide();
		    else
			    $(v).show();
	    });
        $('#toggleImage_' + ids[ii]).attr('src','./Img/icn_closed.gif');
    }  
}