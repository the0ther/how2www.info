function loadDepts(showBlankOption, initSelected)
{
	$.get("Services/GetDepts.aspx", function(data){
		$('#Depts').empty();
		if (showBlankOption)
			$('#Depts').append('<option value="">Select a Department</option>');
		var results = eval((data));
		jQuery.each(results, function(i,n) {
			$('#Depts').append('<option value="' + n.DeptId + '">' + n.DeptName + '</option>');
		});
		if (initSelected > 0)
			$("#Depts option[value='" + initSelected + "']").attr('selected','selected');
	});
}

function IsQuarterHourIncrement(obj)
{
    var val = $.trim($(obj).val());
    var reg = /[^0-9\.]/; //matches anything which is not a digit or a decimal point
    if (reg.test(val) || val == '')
    {
        $(obj).val('0');
    }
    else if ((parseFloat(val) % 1) != 0)
    {
        //actually do the rounding now...
        var vv = parseFloat(val);
        vv = Math.round(vv);
        $(obj).val(vv);        
    }
    return true;
}

function removeFromArray(arr,val)
{
    var retval = new Array();
    for(ii=0;ii<arr.length;ii++)
    {
        //TODO: this will cause errors
        var elem = arr[ii].toString();
        if (elem!=val.toString())
            retval.push(elem);
    }
    return retval;
}

function addToArray(arr,val)
{
    //TODO: this following line sometimes causes problems as well...the first arr[ii]
    var retval = new Array(arr.length);
    var doPush = true;
    for(var ii=0;ii<arr.length;ii++)
    {
        var elem = arr[ii].toString();
        if (elem==val.toString())
            doPush = false;
        else
            retval[ii] = elem;
    }
    if (doPush)
        retval.push(val);
    return retval;
}