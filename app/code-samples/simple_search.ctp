<?
/**
 *
 * @package		Directory
 * @subpackage	Views.Elements
 * @author      Randall Loffelmacher <randall@loffelmacher.com>
 */
?>
<fieldset>
<?
echo $form->create(false, array('action'=>'search'));
echo $form->input('query',array('label'=>'<b>Search</b>'));
$options=array('name'=>'By Name','place'=>'By Place');
$attributes=array('legend'=>false,'value'=>'name',);
echo $form->end();
?>
</fieldset>

<?
echo $html->script('jquery-ui-custom.min.js');
?>
<script>
// see: http://jqueryui.com/demos/autocomplete/#remote-jsonp
jQuery(function(){
		var cache = {};

		var btn = $("<button id=\"search_btn\">&nbsp;</button>")
			.attr("tabIndex", -1)
			.attr("title", "Search")
			.insertAfter('#query')
			.button({
				icons: {
					primary: "ui-icon-search"
				},
				text: false
			}).removeClass("ui-corner-all")
			.addClass("ui-corner-right ui-button-icon");
				
		var hidden = $("<input type=\"hidden\" id=\"q\" />")
			.insertAfter('#search_btn');
				
		$('#query').autocomplete({
			source: function(request, response) {
				if ( request.term in cache ) {
					response( cache[ request.term ] );
					return;
				}

				$.ajax({
					url: "/members/member_list",
					dataType: "json",
					data: {
						q:	request.term,
						t:  new Date().getTime()  //this is here to bust caches
					},
					success: function(data) {
						cache[ request.term ] = data;
						response($.map(data, function(item) {
							return {
								value: item.value,
								label: item.label
							}
						}))
					},
					error: function(err) {
						//console.log('an error occurred' + err);
					},

				})
			},
			delay: 600,
			minLength: 2,
			select: function(event, ui) {
				$('#q').val(ui.item.value);
				$('#query').val(ui.item.label);
				window.location = '/members/view/' + ui.item.value;
			},
			focus: function(event, ui) {
				$('#query').val(ui.item.label);
				$('#q').val(ui.item.value);
			},
			open: function(event, ui) {
				$(this).removeClass("ui-corner-all").addClass("ui-corner-top");
			},
			close: function(event, ui) {
				$(this).removeClass("ui-corner-top").addClass("ui-corner-all");
			},
		});

		$('#query').watermark('name or location');
});
</script>
