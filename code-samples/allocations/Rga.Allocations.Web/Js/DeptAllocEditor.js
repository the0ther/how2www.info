var newAllocId;
var delta;

$(document).ready(function(){
	var userId, jobId, weekNumber;
	userId = parseInt($.query.get('UserId'));
	jobId = parseInt($.query.get('JobId'));
	weekNumber = parseInt($.query.get('WeekNumber'));
	
	$('#userId').val(userId);
	$('#jobId').val(jobId);
	$('#weekNumber').val(weekNumber);
	
	disable($('#MoveToHours'));
	initForm($('#userId').val(),$('#jobId').val(),$('#weekNumber').val());
	var deptId = window.parent.document.getElementById('DeptsDrop').value;
	loadDepts(true,deptId);
	loadEmps(deptId);
	
	var temp = window.parent.$.find('img.close-popup');
	//console.log('temp is: ' + temp.length + ' temp.html: ' + $(temp[0]).html());
	
	//////////////////////////////////
	//event-handlers
	//////////////////////////////////
	$('#Depts').change( function() {
		loadEmps($(this).val());
	});
	
	$('#Emps').change( function () {
		if ($('#Emps').attr('selectedIndex') > 0)
		{
			disable($('#Hours'));
			enable($('#MoveToHours'));
		}
		else
		{
			enable($('#Hours'));
		}		
	});
	
	//keeping initValue in-sync with #Hours to assure logic in #MoveToHours.change() is okay
	$('#Hours').change( function () {
	    if (!isNaN(parseFloat($('#Hours').val())))
	        $('#initValue').val($('#Hours').val());
	});
	
	$('#MoveToHours').change( function () {
		//subtract the new value from the #Hours textbox
		console.log('inside #movetohours.change()');
		if (IsQuarterHourIncrement($(this)))
		{
            //var totHrs = parseFloat($('#Hours').val());
            var totHrs = parseFloat($('#initValue').val());
		    var movedHrs = parseFloat($(this).val());
			if (!isNaN(totHrs) && !isNaN(movedHrs) && (movedHrs <= totHrs))
			{
				var diff = totHrs - movedHrs;
				if (diff >= 0)
					$('#Hours').val(diff);
			}
			else if (movedHrs > totHrs)
			{
				//clear out moved Hrs
				alert("Cannot move more hours than the original number of hours allocated.");
				$('#Hours').val($('#oldValue').val());
				$(this).val('0');
			}
		}
		else
		{
			$('#MoveToHours').val('0');
		}
	});
	
	$('#save').click( function() {
        save();
	});
	
	$('#cancel').click( function() {
		closeWin();
	});
	
	$('#saveAndClose').click( function() {
	    console.log('entering save and close');
	    if (save())
		    closeWin();
	});
});


/////////////////////////////////
// functions
/////////////////////////////////
function initForm(userId, jobId, weekNumber) {
	$.get("Services/InitDeptAllocEditor.aspx?UserId=" + userId + "&JobId=" + jobId + "&WeekNumber=" + weekNumber, function(data){
		var results = eval((data));		
		$('#Hours').val(results[0].hours);
		$('#initValue').val(results[0].hours);
		var name = results[0].firstname + ' ' + results[0].lastname;
		$('#EmpName').html(name);
	});
}

function loadEmps(deptId) {
    $.get("Services/GetEmpsForDept.aspx?DeptId=" + deptId, function(data) {
        var thisUser = $('#userId').val();
        $('#Emps').empty();
        $('#Emps').append('<option value="">Select another Employee</option>');
        var results = eval((data));
        jQuery.each(results, function(i, n) {
            if (n.lastname != '&nbsp;' && n.userid != thisUser)
                $('#Emps').append('<option value="' + n.userid + '">' + n.firstname + ' ' + n.lastname + '</option>');
            else if (n.userid != thisUser)
                $('#Emps').append('<option value="' + n.userid + '">' + n.firstname + '</option>');
        });
    });
}

function UpdatePageBehind(userId, jobId, hrs, moving)
{
    var img;
	//update the span with the new hrs allocated to this user/job
	var sp = window.parent.document.getElementById('val_' + userId + '_' + jobId);
	var prevHrs = 0;
	
	if (!isNaN(parseFloat(sp.innerHTML)))
		prevHrs = parseFloat(sp.innerHTML);

	if (!moving)
	{
		sp.innerHTML = hrs;
		if (hrs == 0)
		{
		    img = window.parent.$('#val_' + userId + '_' + jobId).prev().children();
		    img.css('cursor','default');
		    img.attr('src','Img/c.gif');
		}
	}
		
	//now update the available/total-allocated numbers...
	sp =  window.parent.document.getElementById('avail_' + userId);
	
	var prefix = sp.innerHTML.substring(0,sp.innerHTML.lastIndexOf('/')+1);
	var availHrs = parseFloat(prefix.substring(0,prefix.length-1));
	var prevVal = parseFloat(sp.innerHTML.substring(sp.innerHTML.indexOf('/')+1));
	//assuming adding hours, this # will be positive
	var diff = hrs - prevHrs;
	var total = parseFloat(hrs) + parseFloat(prevHrs);
	
	prevVal = prevVal + diff;
	if (moving)
	{
		window.parent.document.getElementById('val_' + userId + '_' + jobId).innerHTML = total;
		img = window.parent.$('#val_' + userId + '_' + jobId).prev().children();
		if (total == 0)
		{
		    img.css('cursor','default');
		    img.attr('src','Img/c.gif');
        }
	}
	
	//set the text for the available/allocated span...
	var totAllocs = sumAllocHrs(userId);
	sp.innerHTML = prefix + totAllocs;

	//now set over/under coloring...
	if (availHrs > totAllocs)
		sp.className = "under hrs";
	else if (availHrs < totAllocs)
		sp.className = "over hrs";
	else
		sp.className = "hrs";
	
	return total;
}

function sumAllocHrs(userId)
{
	var total = 0;
	var thisVal = 0;
	
	var spans = window.parent.$("span[id^='val_" + userId + "']");
	$.each(spans, function(i,n) {
		thisVal = parseFloat(spans[i].innerHTML);
		if (!isNaN(thisVal))
			total += thisVal;
	});
	return total;
}

function Allocate(newVal, userId, jobId, weekNumber, imgStr)
{
	//update DB
	var url = "Services/UpdateAllocation.aspx?allocationid=-1&mins=" + newVal * 60 + "&jobid=" + jobId + "&employeeid=" + userId + "&week=" + weekNumber + "&doNotAssign=1";
    $.get(url, function(data, img){
        newAllocId = eval((data))[0].AllocId;
        var noteLen = eval((data))[0].NoteLength;
        if (imgStr != null && imgStr != '')
        {
            var img = window.parent.$(imgStr).prev().children();
            img.css('cursor','pointer');
            if (img.attr('src') != 'Img/icon_note_on.gif')
		        img.attr('src','Img/icon_note_off.gif');
		    if (noteLen > 0)
		        img.attr('src','Img/icon_note_on.gif');
		    //set the id as well...
		    img.attr('id','allocNote_' + newAllocId + '_true');
		    img.show();
		    if (newVal == 0)
		    {
		        img.css('cursor','default');
		        img.attr('src','Img/c.gif');
            }
        }
    });	
    return true;
}

function closeWin()
{   
    //this should look just like the $('a#back').click() block in teams.js
    //TODO: see if this still should match. i don't think so, they work pretty differently.
    //      this should match what you find in deptallocation.aspx.
    window.parent.$('div#block').hide();		
    window.parent.$('div#popup').hide();
}

function save()
{
    console.log('entering save()');
    var retval = false;
    var imgStr = '';
	var hrs = $('#Hours').val();
	if (!isNaN(parseFloat(hrs)))
		hrs = parseFloat(hrs);
		
	//do allocation for #Hours if not disabled...
	var movingHours = false;
	var arr = $('#Hours[disabled]');
	if (arr.length > 0)
		movingHours = true;
		
	if (movingHours)
	{
	    console.log('moving hours...');
		if (IsQuarterHourIncrement($('#Hours')))
		{
		    //hrs = $('#Hours').val();
			if (IsQuarterHourIncrement($('#MoveToHours')))
			{
                var hrsMoving = $('#MoveToHours').val();
				//update first user...
				imgStr = '#val_' + $('#userId').val() + '_' + $('#jobId').val();
				retval = Allocate(hrs, $('#userId').val(), $('#jobId').val(), $('#weekNumber').val(), imgStr);
				//console.log('in save...moving hours...first save succeeded: ' + retval);
				UpdatePageBehind($('#userId').val(), $('#jobId').val(), hrs);	
				$('#initValue').val(hrs);

				//allocate hours for user in dropdown
				var movedToUser = $('#Emps').val();
				var totNewHrs = UpdatePageBehind(movedToUser, $('#jobId').val(), hrsMoving, true);
                imgStr = '#val_' + movedToUser + '_' + $('#jobId').val();
				retval = Allocate(totNewHrs, movedToUser, $('#jobId').val(), $('#weekNumber').val(), imgStr);
                //console.log('in save...moving hours...second save succeeded: ' + retval);
                
				//reset form elements
				$('#Emps').attr('selectedIndex','0');
				$('#MoveToHours').val('0');
				disable($('#MoveToHours'));
				enable($('#Hours'));
			}
			else
			{
			    console.log('MoveToHours failed is numeric');
			    retval = true; //recent change might cause trouble
				$('#MoveToHours').val('0');
			}
		}
		else
		{
			$('#Hours').val($('#initValue').val());
		}
	}
	else {
		//we're not moving hours, doing a single allocation
		if (IsQuarterHourIncrement($('#Hours')))
		{
		    hrs = $('#Hours').val();
			UpdatePageBehind($('#userId').val(), $('#jobId').val(), hrs, false);
            imgStr = '#val_' + $('#userId').val() + '_' + $('#jobId').val();
			retval = Allocate(hrs, $('#userId').val(), $('#jobId').val(), $('#weekNumber').val(), imgStr);
			//console.log('in save()...not moving just allocating...save succeeded: ' + retval);
			$('#initValue').val(hrs);
		}
		else
		{
			$('#Hours').val($('#initValue').val());
		}
	}
	return retval;
}

function disable(thing)
{
    thing.attr('disabled','disabled');
    if (thing.attr('type') != 'radio')
        thing.css('background-color', '#f2f2f2');
}

function enable(thing)
{
    thing.removeAttr('disabled');
    thing.css('background-color', '#ffffff');
}