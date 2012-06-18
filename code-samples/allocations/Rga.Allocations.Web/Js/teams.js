//this file handles all ajax for the manage teams panel
var clientid = -1;
var teamid = -1;
var teams = null;

//Frequently used element references
var teamlistDropDown;
var deptlistDropDown;

// constant for MaxRows in Team Lists
var cMaxRowCnt = 8;

$(document).ready(function() {
    teamlistDropDown = $('#Groups');
    deptlistDropDown = $('#Depts');
    disable($('#AddMembers'));
    disable($('#EditGroup'));
    disable($('#DeleteGroup'));
    
    clientid = window.parent.document.getElementById('dropClient').value;
    loadTeams(clientid);

    teamlistDropDown.change(function() {
        //load up a new list of team members based on the id from dropdown
        teamid = teamlistDropDown.attr('value');
        loadTeamMembers(teamid);

        // if the resource ResourcePanel is displayed; we need to refresh the client members; since the resource list is based on the job.
        if (IsResourcePanelDisplayed()) {
            loadClientResources(teamid, clientid, deptlistDropDown.attr('value'));
        }

        ClearGroupFields();
        jQuery.each(teams, function(i, n) {
            if (n.TeamId == teamid) {
                $('#GroupName').text(n.TeamName);
                $('#GroupNote').text(n.Description);
            }
        });

        //if the "Select a Client Group" option is selected, add 8 rows to the table. 
        if ($(this).val() == -1) {
            //fill the table with empty rows
            var tbod = $('#TeamGrid tbody');
            for (ii = 0; ii < 8; ii++) {
                tbod.append(TeamMemberRowHTML(0, 'bleen', '', ''));
            }
            HideResourcesFields();
            $('#ControlsPanel').show();
            $('#NewGroupCtrls').hide();
            //disable all the links
            $('#AddMembers').unbind('click');
            disable($('#AddMembers'));
            $('#EditGroup').unbind('click');
            disable($('#EditGroup'));
            $('#DeleteGroup').unbind('click');
            disable($('#DeleteGroup'));
        } else {
            $('#AddMembers').click(addMembers);
            enable($('#AddMembers'));
            $('#EditGroup').click(editGroup);
            enable($('#EditGroup'));
            $('#DeleteGroup').click(deleteGroup);
            enable($('#DeleteGroup'));
        }
    });

    function addMembers() {
        if (IsTeamSelected(teamid)) {
            //show the add members panel when "Add Members" is clicked
            ShowResourcesFields();
            $('#ControlsPanel').hide();
            //populate the resources panel table
            loadDepts();
            loadClientResources(teamid, clientid, deptlistDropDown.attr('value'));
        }
        else {
            alert('Please select a group first.');
        }
    }

    function editGroup() {
        if (IsTeamSelected(teamid)) {
            $('#NewGroupCtrls').show();
            ShowEditGroupHeader();
            initEditCtrls();
        }
        else {
            alert('Please select a group first');
        }
    }

    function deleteGroup() {
        if (IsTeamSelected(teamid)) {
            if (confirmDeleteGroup()) {
                deleteTeam(teamid);
                loadTeams(clientid);
            }
        }
        else {
            alert('Please select a group first.');
        }
    }

    deptlistDropDown.change(function() {
        loadClientResources(teamid, clientid, deptlistDropDown.attr('value'));
    });

    $('#Done').click(function() {
        HideResourcesFields();
        $('#ControlsPanel').show();
        $('#NewGroupCtrls').hide();
    });

    $('#CreateGroup').click(function() {
        $('#NewGroupCtrls').show();
        initCreateCtrls();
        $('#NewGroupName').focus();
    });

    $('#NewGroupNext').click(function() {
        SaveNewGroup();
    });

    $('#NewGroupCancel').click(function() {
        $('#NewGroupCtrls').hide();
        ShowCreateGroupHeader();
    });

    $('#NewGroupName').keypress(function(e) {
        // if user hit enter in text field
        if (e.which == 13) {
            SaveNewGroup();
        }
    }
	)

    $('#NewGroupNote').keypress(function(e) {
        // if user hit enter in text field
        if (e.which == 13) {
            SaveNewGroup();
        }
    }
	)

});



function TeamMemberRowHTML(teamid,teamMemberId,fullName,shortName)
{

    var tdRemoveCellHTML = '<td class="gridCell tgDelMemberCell" >' +
				'<img alt="Remove this member" title="Remove this member" src="Img/icon_remove.gif" onclick="remove(' + 
				teamMemberId + ','+teamid+')" /></td>';

    //if teamMemberId is not a number we will return an empty row;
    if(isNaN(teamMemberId))
    {
        tdRemoveCellHTML = '<td class="gridCell tgDelMemberCell" >&nbsp;</td>';
    }

    if (typeof(fullName) == "undefined") fullName = "&nbsp;";
    if (typeof(shortName) == "undefined") shortName = "&nbsp;";

				 
    return '<tr rowid="' + teamMemberId + '">' + tdRemoveCellHTML + '<td class="gridCell tgNameCell" >' + fullName + 
				'</td><td class="gridCell tgDeptCell" >' + shortName + 
				'</td></tr>';
}

//these methods load data by calling services in the site's Services folder
function loadTeamMembers(teamid)
{
    var tbod = $('#TeamGrid tbody');
    tbod.empty();
    if (teamid != -1) {
        $.get("Services/GetClientTeamMembers.aspx?TeamId=" + teamid, function(data) {
            //now load up rows in the table TeamGrid

            var results = eval((data));
            var rowCnt = 0;

            jQuery.each(results, function(i, n) {
                tbod.append(TeamMemberRowHTML(teamid, n.TeamMemberId, n.FullName, n.ShortName));
                rowCnt++;
            });
            //This section of code adds the empty padding rows
            if (rowCnt < cMaxRowCnt) {
                var y = cMaxRowCnt - rowCnt;
                for (var x = 0; x < y; x++) tbod.append(TeamMemberRowHTML());
            }
        });
    }
}


function ClientRowHTML(teamId,empId,fullName,title)
{
    var html ='';
    
    var tdAddCellHTML = '<td class="gridCell tgClientAddCell" >' +
    				'<a href="javascript:;" onclick="add(' + 
				empId + ','+teamId+')">Add</a></td>';

    //if teamMemberId; we will return an empty row;
    if(isNaN(empId))
    {
        tdAddCellHTML = '<td class="gridCell tgClientAddCell" >&nbsp;</td>';
    }

    if (typeof(fullName) == "undefined") fullName = "&nbsp;";
    if (typeof(title) == "undefined") title = "&nbsp;";

				 
    html = '<tr rowid="' + empId + '"><td class="gridCell tgClientNameCell" >' + fullName + 
				'</td><td class="gridCell tgClientTitleCell" >' + title + 
				'</td>'+tdAddCellHTML+'</tr>';
	return html;
}

function loadClientResources(_teamid, _clientid, _deptid)
{
        var uri = "Services/GetClientResources.aspx?TeamId=" + _teamid + "&DeptId=" + _deptid + "&ClientId=" + _clientid;
	$.get(uri, function(data){
		//now load up rows in the table TeamGrid
		var tbod = $('#ResourcesGrid tbody');
        var rowCnt = 0;
		tbod.empty();
		var results = eval((data));
		jQuery.each(results, function(i,n) {

		    tbod.append(ClientRowHTML(_teamid,n.EmpId,n.FullName,n.Title));
		    rowCnt++;
		});
		
		if(rowCnt < cMaxRowCnt)
	    {
	        var y = cMaxRowCnt - rowCnt;
    	    for (var x=0;x<y;x++) tbod.append(ClientRowHTML());
        }

	});
}

function loadDepts()
{
	$.get("Services/GetDepts.aspx", function(data){
		deptlistDropDown.empty();
		var results = eval((data));
		jQuery.each(results, function(i,n) {
			deptlistDropDown.append('<option id="department'+n.DeptId+'" value="' + n.DeptId + '">' + n.DeptName + '</option>');
		});
	});
}

function loadTeams(clientid, initVal)
{
    //console.log('entering loadTeams');
	$.get("Services/GetClientTeams.aspx?ClientId=" + clientid, function(data){
		teamlistDropDown.empty();
		teamlistDropDown.append('<option id="client'+clientid+'" value="-1">Select a Client Group</option>');
		teams = eval((data));
		jQuery.each(teams, function(i,n) {
		    if (initVal)
            {
                if (n.TeamId == initVal)
                    teamlistDropDown.append('<option id= "team'+n.TeamId+'" value="' + n.TeamId + '" selected="selected">' + n.TeamName + '</option>');
                else
                    teamlistDropDown.append('<option id= "team'+n.TeamId+'" value="' + n.TeamId + '">' + n.TeamName + '</option>');
            }
		    else
		    {
    			teamlistDropDown.append('<option id= "team'+n.TeamId+'" value="' + n.TeamId + '">' + n.TeamName + '</option>');
    		}
		});
	});
	//console.log('done loading teams dropdown...options count is: ' + $('#Groups').html());
}

//these methods handle user actions
function remove(_id, _teamid)
{

	$.get("Services/UpdateClientTeam.aspx?TeamMemberId=" + _id + "&action=remove", function(data){
    	loadTeamMembers(_teamid);
	    loadClientResources(teamid,clientid,deptlistDropDown.attr('value'));
	});
	//now refresh the grids
}

function add(userid, teamid)
{
    if(isNaN(teamid))
    {
        alert('Please select a group first before adding a resource.');
        return false;
    }

	$.get("Services/UpdateClientTeam.aspx?UserId=" + userid + "&TeamId=" + teamid + "&action=add", function(data){
	    //now remove that row from the table...
	    loadClientResources(teamid,clientid,deptlistDropDown.attr('value'));
	    loadTeamMembers(teamid);
	});
}

function deleteTeam(teamid)
{
	$.get("Services/UpdateClientTeam.aspx?TeamId=" + teamid + "&action=delete", function(data){
		;
	});
	//refresh the teams dropdown
	loadTeams(clientid);
	ClearGroupFields();
	ResetTeamGrid();
}


function ClearGroupFields()
{
	$('#GroupName').text('');
	$('#GroupNote').text('');
}

function ShowResourcesFields()
{
    $('#ResourcesDropDownCell').show();
    $('#ResourcesPanel').show();
    $('#Done').show();
}

function HideResourcesFields()
{
    $('#ResourcesDropDownCell').hide();
    $('#ResourcesPanel').hide();
    $('#Done').hide();
}

//returns true if new team created, false otherwise
function createTeam()
{
	var name = escape($('#NewGroupName').attr('value'));
	var desc = escape($('#NewGroupNote').attr('value'));

	if (desc == 'undefined') 
		desc = '';
	if (name != 'undefined')
	{
		$.get("Services/UpdateClientTeam.aspx?TeamName=" + name + "&TeamDesc=" + desc + "&ClientId=" + clientid + "&action=create", function(data){
			teamid = data;
			loadTeams(clientid, teamid);
			loadDepts();
			loadClientResources(teamid, clientid, deptlistDropDown.attr('value'));
			$('#TeamGrid tbody').empty();
			//set the group name and note labels
			$('#GroupName').text($('#NewGroupName').attr('value'));
			$('#GroupNote').text($('#NewGroupNote').attr('value'));
			ResetTeamGrid();
			//console.log('calling SelectTeam from createTeam...');
			//SelectTeam(teamid);
		});
		return true;
	}
	else
	{
		return false;
	}
}

function editTeam()
{
	var name = escape($('#NewGroupName').attr('value'));
	var desc = escape($('#NewGroupNote').attr('value'));
	
	if (desc == 'undefined')
		desc = '';
	if (name != 'undefined')
	{
	    var _teamid = teamid;
		$.get("Services/UpdateClientTeam.aspx?TeamName=" + name + "&TeamDesc=" + desc  + "&TeamId=" + teamid + "&action=edit", function(data){
			;
		});
		loadTeams(clientid, _teamid);
		//update the labels
		$('#GroupName').text($('#NewGroupName').attr('value'));
		$('#GroupNote').text($('#NewGroupNote').attr('value'));
		//console.log('calling SelectTeam from editTeam()');
        //SelectTeam(_teamid);
		return true;
	}
	else
	{
		return false;
	}
}

function confirmDeleteGroup()
{
    return confirm('Are you sure you want to delete the group \"' + $('#GroupName').text() + '\"?');
}

function initCreateCtrls()
{
	$('#NewGroupName').attr('value','');
	$('#NewGroupNote').attr('value','');
	$('#NewGroupNext').attr('action','create');
	$('#NewGroupNext').attr('value', 'Next');
	$('#NewGroupNext').text('Add Group');
}

function initEditCtrls()
{
	//setup the name & desc textboxes	
	$('#NewGroupName').attr('value',$('#Groups option:selected').text());
	//change the button to say Edit instead of w/e it usually says
	$('#NewGroupNext').attr('action','edit');
	$('#NewGroupNext').attr('value', 'Save');
	$('#NewGroupNext').text('Save Changes');
	//now get the description for this team
	$.get("Services/GetClientTeams.aspx?ClientId=" + clientid + "&TeamId=" + teamid, function(data){
		var results = eval((data));
		$('#NewGroupNote').val(results[0].Description);
	});

}


function IsResourcePanelDisplayed()
{
    return $('#ResourcesPanel').css ('display') != 'none';
}

function IsTeamSelected(teamid)
{
    return $('#Groups').val() != -1;
}


function ResetTeamGrid()
{
	var tbod = $('#TeamGrid tbody');
	tbod.empty();
    for (var x=0;x < cMaxRowCnt;x++) tbod.append(TeamMemberRowHTML());
}


function SaveNewGroup()
{
//create the group
		var action = $('#NewGroupNext').attr('action');
		if (action == 'create')
		{
			if (createTeam())
			{
				//show the resources panel
				$('#NewGroupCtrls').hide();
				$('#ControlsPanel').hide();
				teamlistDropDown.attr('value',$('#GroupName'));
    			ShowResourcesFields();
    			//loadTeams(clientid);
    			//console.log('calling selectteam from SaveNewGroup...');
                //SelectTeam(teamid);
    			ResetTeamGrid();
			}
			else
			{
			    alert("Please enter a group name.");
			}
		}
		else if (action == 'edit')
		{
			if (editTeam())
			{
				$('#NewGroupCtrls').hide();			
			}
			else
			{
			    alert("Please enter a group name.");
			}
		}
        ShowCreateGroupHeader();
}

function ShowEditGroupHeader()
{
    var egh = $('#editGroupHeader');
    $('#CreateGroup').hide();
    egh.text('Editing Existing Group:' + $('#GroupName').text());
	egh.show();
}

function ShowCreateGroupHeader()
{
    $('#editGroupHeader').hide();
	$('#CreateGroup').show();
}

function disable(thing) {
    thing.attr('disabled', 'disabled');
    thing.css('color', '#c5c5c5');
}

function enable(thing) {
    thing.removeAttr('disabled');
    thing.css('color', '#000000');
}