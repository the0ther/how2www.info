<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeptAllocEditor.aspx.cs" Inherits="Allocations.DeptAllocEditor" %>
<html>
<head runat="server">
    <title>Edit Hours</title>
    <style type="text/css">
	body { font-family: Verdana,Arial; font-size: 8pt; font-weight: bold; background-color: #f2f2f2; }
	p { padding: 0; margin: 3px 0 0 0; }
	h2 { font-size: 14px; }
    .btn {
		border:1px solid #CDCDCD;
		padding: 4px 8px 4px 8px;
        display:inline;
        background-color:#E6E6E6;   
        font-family:Verdana,Arial;
		font-size:8pt;
		font-weight: 700;
		cursor: pointer;
    }
    #box { padding-bottom: 10px;}
    #Hours { width: 35px; }
    #MoveToHours { width: 25px; }
    #main-div { width: 265px; height: 225px; background-color: #f2f2f2; }
    select { width: 175px; margin-bottom: 5px; margin-right: 10px;}
    #Hours2 { position: relative; top: -100px; }
    </style>
    <!--[if IE]>
	<style>
	#main-div { width: 255px; height: 175px; background-color: #f2f2f2; }
    </style>
	<![endif]-->
</head>
<body>
    <form id="form1" runat="server">
    <div id="main-div">
		<div id="box">
			<h2>Edit Employee and Hours</h2>
			<p>
				<span id="EmpName">&nbsp;</span>
				<input type="text" id="Hours" maxlength="5" name="Hours"/>
			</p>
			<hr />
			<p>
				Move 
				<input type="text" id="MoveToHours" maxlength="5" />
				hours to (Optional):
			</p>
			<br />
			<select id="Depts"></select><br />
			
			<select id="Emps"></select><br />
			<br />
			
			<div id="cancel" class="btn">Cancel</div>&nbsp;
			<div id="save" class="btn">Save</div>&nbsp;
			<div id="saveAndClose" class="btn">Save + Close</div>
		</div>
	    <input type="hidden" id="jobId"/>
	    <input type="hidden" id="userId" />
	    <input type="hidden" id="weekNumber" />
	    <input type="hidden" id="initValue" value="0" />
    </div>
    </form>    
	<script src="Js/jquery-1.2.6.min.js" type="text/javascript"></script>
	<script src="Js/jquery.query-1.2.3.js" type="text/javascript"></script>
	<script src="Js/firebug.js" type="text/javascript"></script>
    <script src="Js/shared.js" type="text/javascript"></script>
    <script src="Js/DeptAllocEditor.js" type="text/javascript"></script>
</body>
</html>
