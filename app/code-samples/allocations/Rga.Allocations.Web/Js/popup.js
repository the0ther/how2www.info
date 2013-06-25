function Popup(options){
	// default parameters passed if omitted when called
	options = $.extend({
		pWidth: 365, pHeight: 320, //default width and height of popup
		iWidth: 355, iHeight: 295, //default width and height of iframe
		reload: true, visible: true, multiClose: false
	}, options);

	var self = this;

	$.each(options, function(k, v){
		self[k] = v;
	})

	this.init = function(){
	    //console.log('is target null or undefined in init(): ' + (this.target==null));
	    
	    if (this.target == null)
	    {
	        this.target = $('<div class="popup" id="popup"><div class="popup-content"><img src="img/icon_close.gif" class="close-popup" /></div></div>'); //reference to HTML popup
	        this.iframe = $('<iframe src="' + this.src + '" frameborder="0"></iframe>'); //reference to iframe
    	    
		    self.iframe.css({
			    width: self.iWidth, height: self.iHeight
		    });

		    self.target.css({
			    width: self.pWidth, height: self.pHeight
		    });

		    self.target.find('div.popup-content').append(self.iframe);
		    self.target.appendTo($('body'));

            if (!this.multiClose)
            {
		        self.target.find('img.close-popup').click(function(){
		            console.log('in the close-popup handler...');
			        self.close();
		        });
            }
            self.target.center();
        }
        else
        {
            this.iframe.remove();
            this.iframe = $('<iframe src="' + this.src + '" frameborder="0"></iframe>'); //reference to iframe
		    self.iframe.css({
			    width: self.iWidth, height: self.iHeight
		    });
            self.target.find('div.popup-content').append(self.iframe);
            self.target.center();
            self.target.show();
        }
	}

	this.open = function(){
	    if ($('body div#block').length < 1)
		    $('body').append('<div id="block"></div>');
		
		$('#block').css({
			width: $('body').width(),
			height: $('body').height()
		});
		if (self.visible)
			self.target.visible();
		self.target.show();
		self.target.center();
	}

	this.close = function(){
		self.target.remove();
		$('div#popup iframe').attr('src','');
		
		$('div#block').hide();		
		$('div#popup').hide();
		if (self.reload)
		{
			var rebind = window.parent.document.getElementById('rebind');
			rebind.value = 'true';
			var form = window.parent.document.getElementById('form1');
			form.submit();
		}
	}

	this.init();
}

$.fn.center = function(){
    //console.log('entering center in the popup code');
	var vWidth = $(window).width();
	var vHeight = $(window).height();
	var vScroll = 0;
	//console.log('finding width & height');
	var eWidth = $(this).width(); //width of the element to be centered
	var eHeight = $(this).height();
	//console.log('width & height found...width: ' + eWidth + ' height: ' + eHeight);
	var x = 0;
	if ($.browser.msie)
	{
		vScroll = document.documentElement.scrollTop;
		x = document.documentElement.scrollLeft + (vWidth/2) - (eWidth/2);
    }
	else
	{
		vScroll = window.scrollY;
		//console.log('found FF. scrollLeft: ' + window.scrollX + ' vWidth: ' + vWidth + ' eWidth: ' + eWidth);
		x = window.scrollX + (vWidth/2) - (eWidth/2);
	}	

	//var x = document.body.scrollLeft + (vWidth/2) - (eWidth/2); //used to use document.documentElement.scrollLeft
	//console.log('scrollLeft: ' + document.documentElement.scrollLeft + ' vWidth: ' + vWidth + ' eWidth: ' + eWidth);
	var y = ((vHeight/2) - (eHeight/2)) + vScroll;
    //console.log('setting left,top to: ' + x + ',' + y);
    
	$(this).css({
		position: 'absolute',
		left: x,
		top: y
	});
}

$.fn.visible = function(){
	var vWidth = $(window).width();
	var vHeight = $(window).height();
	var vScroll = 0;
	if ($.browser.msie)
		vScroll = document.body.scrollTop;
	else
		vScroll = window.scrollY;
		
	var hScroll = 0;
	if ($.browser.msie)
		hScroll = document.body.scrollLeft;
	else
		hScroll = window.scrollY;
	if (hScroll == undefined)
		hScroll = 0;
	var eWidth = $(this).width();
	var eHeight = $(this).height();

	var x = hScroll + (vWidth/2) - (eWidth/2);
	var y = ((vHeight/2) - (eHeight/2)) + vScroll;

	$(this).css({
		position: 'absolute',
		left: x,
		top: y
	});
}


function Popup2(options){
	// default parameters passed if omitted when called
	options = $.extend({
		pWidth: 365, pHeight: 320, //default width and height of popup
		iWidth: 355, iHeight: 295, //default width and height of iframe
		teamPage: false
	}, options);

	var self = this;

	$.each(options, function(k, v){
		self[k] = v;
	})

	this.target = $('<div class="popup2 popup" id="popup2"><div class="popup-content2"><img src="img/icon_close.gif" class="close-popup" /></div></div>'); //reference to HTML popup
	this.iframe = $('<iframe src="' + this.src + '" frameborder="0"></iframe>'); //reference to iframe

	this.init = function(){
		self.iframe.css({
			width: self.iWidth, height: self.iHeight
		});
		self.target.css({
			width: self.pWidth, height: self.pHeight
		});
		self.target.find('div.popup-content2').append(self.iframe);
		self.target.appendTo($('body'));
		self.target.find('img.close-popup').click(function(){
			self.close();
		});
	}

	this.open = function(pos){
		$('body').append('<div id="block2"></div>');
		$('#block').css({
			width: $('body').width(),
			height: $('body').height()
		});
		if (pos == 'visible')
			self.target.visible();
		else
			self.target.center();
		self.target.show();
	}

	this.close = function(){
		$('#block2').remove();
		self.target.remove();
	}

	this.init();
}