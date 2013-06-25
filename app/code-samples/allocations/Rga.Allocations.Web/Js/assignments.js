//this file handles all ajax for the manage assignments panel
//test for hours already billed to job, if so then show a "deactivate" icon instead

var clientid = -1;
var jobid = -1;
var teams = null;
//could not get this to init with the value in 1st option so it's hardcoded for now.
var deptid = 8;
var minRows = 8;

$(document).ready(function(){
	jobid = window.parent.document.getElementById('JobsDrop').value;
	//jobid = 4021;
	loadDepts(false);
	loadJobResources(jobid,deptid);
	loadAssignedEmps(jobid);
	$('#GroupName').text(window.parent.document.getElementById('JobsDrop')[window.parent.document.getElementById('JobsDrop').selectedIndex].text);
	
	$('#Depts').change(function(){
		deptid = $(this).attr('value');
		loadJobResources(jobid, deptid);
	});
});


//these methods load data by calling services in the site's Services folder
function loadAssignedEmps(jobid)
{
	$.get("Services/Assignments.aspx?JobId=" + jobid, function(data){
		//now load up rows in the table TeamGrid
		var tbod = $('#TeamGrid tbody');
		tbod.empty();
		var results = eval((data));
		jQuery.each(results, function(i,n) {
			if (n.timebilled == 0)
			{
				tbod.append('<tr rowid="' + n.id + '"><td class="gridCell tgDelMemberCell">' +
					'<img src="Img/icon_remove.gif" onclick="remove(' + 
					n.id + ',' + jobid + ')" alt="Unassign" title="Unassign" /></td>' + 
					'<td class="gridCell tgDeptCell">' + n.fullname + 
					'</td><td class="gridCell tgDeptCell">' + n.deptname + 
					'</td></tr>');
			}
			else
			{
				tbod.append('<tr rowid="' + n.id + '"><td class="gridCell tgDelMemberCell">' +
					'<img src="Img/icon_readd.gif" onclick="add(' + 
					n.id + ',' + jobid + ')" alt="Reassign" title="Reassign" /></td>' + 
					'<td class="gridCell tgDeptCell">' + n.fullname + 
					'</td><td class="gridCell tgDeptCell">' + n.deptname + 
					'</td></tr>');
			}
		});
		checkRowCount('#TeamGrid');
	});
}

function loadJobResources(jobid, deptid)
{
	$.get("Services/GetJobResources.aspx?JobId=" + jobid + "&DeptId=" + deptid, function(data){
		var rowsAdded = 0;
		//now load up rows in the table TeamGrid
		var tbod = $('#ResourceGrid tbody');
		tbod.empty();
		var results = eval((data));
		jQuery.each(results, function(i,n) {
			tbod.append('<tr rowid="' + n.id + '"><td class="gridCell tgClientNameCell">' 
				+ n.fullname + '</td><td class="gridCell tgClientTitleCell">' + n.title + 
				'</td><td class="gridCell tgClientAddCell"><a href="javascript:;" onclick="add(' + 
				n.id + ',' + jobid + ')">Add</a></td></tr>');
			rowsAdded++;
		});
		checkRowCount('#ResourceGrid');
	});
}

//these methods handle user actions
function remove(userid, jobid)
{
	var id = -1;
	//if this is NOT reassign remove that row from the table
	$.get("Services/UpdateAssignment.aspx?UserId=" + userid + "&JobId=" + jobid + "&action=unassign", function(data){
		//make this read the return value of update and act accordingly
		var timeBilled = eval(data);
		if (timeBilled == 99)
		{
			var p = new Popup2({
				pWidth: 350, pHeight: 150, iWidth: 335, iHeight: 125, src: 'ResolveAllocations.html?userid=' + userid + '&jobid=' + jobid,
				teamPage: false
			});
			p.open();
		}
		else if (timeBilled == 0)
		{
			$('#TeamGrid tbody tr[rowid=' + userid + ']').remove();
		}
		else
		{
			$('#TeamGrid tbody tr[rowid=' + userid + '] td:first').html(
					'<img src="Img/icon_readd.gif" onclick="add(' + 
						userid + ',' + jobid + ')" alt="Reassign" title="Reassign" />');
		}
	});
	//refresh the resources grid
	loadJobResources(jobid,$('#Depts').attr('value'));
}

function add(userid, jobid)
{
	if ($('#TeamGrid tbody tr[rowid=' + userid + ']').attr('alt') == 'Reassign')
	{
		$('#TeamGrid tbody tr[rowid=' + userid + ' td img]').html(
				'<img src="Img/icon_remove.gif" onclick="remove(' + 
					userid + ',' + jobid + ')" alt="Assign" title="Assign" />');
	}
	else
	{
		//now remove that row from the table...
		$('#ResourceGrid tbody tr[rowid=' + userid + ']').remove();
	}
	$.get("Services/UpdateAssignment.aspx?UserId=" + userid + "&JobId=" + jobid + "&action=assign", function(data){
		;
	});
	//and load the team members list again so newly added shows
	loadAssignedEmps(jobid);
}

function checkRowCount(tbl) {
	var rowCount = $(tbl + ' tbody tr').length;
	if (rowCount < minRows)
	{
		if (tbl == '#TeamGrid')
		{
			for (ii=0;ii<(minRows-rowCount);ii++)
			{
				tbod.append('<tr rowid="-1"><td class="gridCell tgDelMemberCell">&nbsp;</td>' + 
					'<td class="gridCell tgDeptCell">&nbsp</td><td class="gridCell tgDeptCell">&nbsp;</td></tr>');
			}
		}
		else
		{
			for (ii=0;ii<(minRows-rowCount);ii++)
			{
				$(tbl + ' tbody').append('<tr rowid="-1"><td class="gridCell tgClientNameCell">' +
					'&nbsp;</td><td class="gridCell tgClientTitleCell">&nbsp;' +
					'</td><td class="gridCell tgClientAddCell"></td></tr>');
			}
		}
	}
}