(function(/*! Brunch !*/) {
  if (!this.require) {
    var modules = {}, cache = {}, require = function(name, root) {
      var module = cache[name], path = expand(root, name), fn;
      if (module) {
        return module;
      } else if (fn = modules[path] || modules[path = expand(path, './index')]) {
        module = {id: name, exports: {}};
        try {
          cache[name] = module.exports;
          fn(module.exports, function(name) {
            return require(name, dirname(path));
          }, module);
          return cache[name] = module.exports;
        } catch (err) {
          delete cache[name];
          throw err;
        }
      } else {
        throw 'module \'' + name + '\' not found';
      }
    }, expand = function(root, name) {
      var results = [], parts, part;
      if (/^\.\.?(\/|$)/.test(name)) {
        parts = [root, name].join('/').split('/');
      } else {
        parts = name.split('/');
      }
      for (var i = 0, length = parts.length; i < length; i++) {
        part = parts[i];
        if (part == '..') {
          results.pop();
        } else if (part != '.' && part != '') {
          results.push(part);
        }
      }
      return results.join('/');
    }, dirname = function(path) {
      return path.split('/').slice(0, -1).join('/');
    };
    this.require = function(name) {
      return require(name, '');
    };
    this.require.brunch = true;
    this.require.define = function(bundle) {
      for (var key in bundle)
        modules[key] = bundle[key];
    };
  }
}).call(this);(this.require.define({
  "helpers": function(exports, require, module) {
    (function() {

  exports.BrunchApplication = (function() {

    function BrunchApplication() {
      var _this = this;
      $(function() {
        _this.initialize(_this);
        return Backbone.history.start();
      });
    }

    BrunchApplication.prototype.initialize = function() {
      return null;
    };

    return BrunchApplication;

  })();

}).call(this);

  }
}));
(this.require.define({
  "initialize": function(exports, require, module) {
    (function() {
  var BrunchApplication, HomeView, MainRouter,
    __hasProp = Object.prototype.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor; child.__super__ = parent.prototype; return child; };

  BrunchApplication = require('helpers').BrunchApplication;

  MainRouter = require('routers/main_router').MainRouter;

  HomeView = require('views/home_view').HomeView;

  exports.Application = (function(_super) {

    __extends(Application, _super);

    function Application() {
      Application.__super__.constructor.apply(this, arguments);
    }

    Application.prototype.initialize = function() {
      this.router = new MainRouter;
      return this.homeView = new HomeView;
    };

    return Application;

  })(BrunchApplication);

  window.app = new exports.Application;

}).call(this);

  }
}));
(this.require.define({
  "views/home_view": function(exports, require, module) {
    (function() {
  var __hasProp = Object.prototype.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor; child.__super__ = parent.prototype; return child; };

  exports.HomeView = (function(_super) {

    __extends(HomeView, _super);

    function HomeView() {
      HomeView.__super__.constructor.apply(this, arguments);
    }

    HomeView.prototype.id = 'home-view';

    HomeView.prototype.render = function() {
      $(this.el).html(require('./templates/home'));
      return this;
    };

    return HomeView;

  })(Backbone.View);

}).call(this);

  }
}));
(this.require.define({
  "routers/main_router": function(exports, require, module) {
    (function() {
  var __hasProp = Object.prototype.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor; child.__super__ = parent.prototype; return child; };

  exports.MainRouter = (function(_super) {

    __extends(MainRouter, _super);

    function MainRouter() {
      MainRouter.__super__.constructor.apply(this, arguments);
    }

    MainRouter.prototype.routes = {
      '': 'home'
    };

    MainRouter.prototype.home = function() {
      return $('body').html(app.homeView.render().el);
    };

    return MainRouter;

  })(Backbone.Router);

}).call(this);

  }
}));
(this.require.define({
  "views/templates/home": function(exports, require, module) {
    module.exports = function (__obj) {
  if (!__obj) __obj = {};
  var __out = [], __capture = function(callback) {
    var out = __out, result;
    __out = [];
    callback.call(this);
    result = __out.join('');
    __out = out;
    return __safe(result);
  }, __sanitize = function(value) {
    if (value && value.ecoSafe) {
      return value;
    } else if (typeof value !== 'undefined' && value != null) {
      return __escape(value);
    } else {
      return '';
    }
  }, __safe, __objSafe = __obj.safe, __escape = __obj.escape;
  __safe = __obj.safe = function(value) {
    if (value && value.ecoSafe) {
      return value;
    } else {
      if (!(typeof value !== 'undefined' && value != null)) value = '';
      var result = new String(value);
      result.ecoSafe = true;
      return result;
    }
  };
  if (!__escape) {
    __escape = __obj.escape = function(value) {
      return ('' + value)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
    };
  }
  (function() {
    (function() {
    
      __out.push('\n<div id="fb-root"></div>\n<script>(function(d, s, id) {\n        var js, fjs = d.getElementsByTagName(s)[0];\n        if (d.getElementById(id)) return;\n        js = d.createElement(s); js.id = id;\n        js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=227268744011500";\n        fjs.parentNode.insertBefore(js, fjs);\n}(document, \'script\', \'facebook-jssdk\'));</script>\n<header>\n<div class="container">\n  <div class="row">\n    <div class="eightcol">\n      <h1>How 2 WWW</h1>\n    </div>\n    <div class="fourcol last">\n      <!--\n      <nav>\n      <ul>\n      <li>About</li>\n      <li>Contact</li>\n      </ul>\n      </nav>\n      -->\n    </div>\n  </div>\n</div>\n</header>\n\n<div class="container">\n  <div class="row">\n    <div class="threecol">\n      <h3>Pepto Bismol Feast for All</h3>\n      <a href="/images/feastforall.jpg" class="thumb" title="Facebook Thanksgiving charitable campaign for Proctor & Gamble\'s Pepto Bismol product.">\n        <img src="/images/feastforall.jpg">\n      </a>\n    </div>\n    <div class="threecol">\n      <h3>Magic Brownie Adventure</h3>\n      <a href="/images/magicbrownie.jpg" class="thumb" title="For the newest Cheech & Chong movie. Uses parallax scrolling effect and lots of video."><img src="/images/magicbrownie.jpg"></a>\n    </div>\n    <div class="threecol">\n      <h3>Yoplait Save Lids Save Lives</h3>\n      <a href="/images/slsl.jpg" class="thumb" title="Users can submit codes printed on Yoplait yogurt lids to donate money to the fight against breast cancer."><img src="/images/slsl.jpg"></a>\n    </div>\n    <div class="threecol last">\n      <h3>Fiber One</h3>\n      <a href="/images/fiberone.jpg" class="thumb" title="A fresh look for a site which sorely needed it. Ties into the General Mills recipe database."><img src="/images/fiberone.jpg"></a>\n    </div>\n  </div>\n  \n   <div class="row">\n     <div class="threecol">\n       <h3>Sanofi Faces of Influenza</h3>\n       <a href="/images/foi-es.jpg" class="thumb" title="Spanish Language site using a i10n backend written in PHP">\n         <img src="/images/foi-es.jpg">\n       </a>\n     </div>\n     <div class="threecol">\n       <h3>Sanofi Fluzone Interdermal</h3>\n       <a href="/images/fluzone.jpg" class="thumb" title="Used popcorn.js to build a custom video player for iPad. Shows clickable hotspots which open supplemental information."><img src="/images/fluzone.jpg"></a>\n     </div>\n     <div class="threecol">\n       <h3>Hubworld</h3>\n       <a href="/images/hubworld.jpg" class="thumb" title="Brings together a ton of content for a new kid\'s TV channel. Includes a meta-game where kids can earn points and trophies, utilized Bunchball gaming platform."><img src="/images/hubworld.jpg"></a>\n     </div>\n     <div class="threecol last">\n       <h3>SDC</h3>\n       <a href="/images/sdcdir.jpg" class="thumb" title="A directory of SDC members, with UI for members, subscribers, and admin. Built with CakePHP, payments handled through Paypal."><img src="/images/sdcdir.jpg"></a>\n     </div>\n         </div>\n </div>\n \n <div class="container">\n \t<div class="row">\n     <div class="twelvecol last">\n       <footer>\n \t\t\t\t<!--\n \t\t\t\tNOT SURE WHY, BUT SUBSCRIBE BUTTON JUST SHOWS A BIG BLANK BOX\n \t\t\t\t<div class="fb-subscribe" data-href="https://www.facebook.com/randy.loffelmacher" data-show-faces="true" data-width="450"></div>\n \t\t\t\t-->\n \t\t\t\t\n \t\t\t\t<div class="fb-like" data-href="http://loffelmacher.com" data-send="true" data-width="450" data-show-faces="true" data-color-scheme="dark"></div>\n \t\t\t\t\n \t\t\t\t\n \t\t\t\t<div>\n \t\t\t\t\t<a rel="license" href="http://creativecommons.org/licenses/by-nc-nd/3.0/"><img alt="Creative Commons License" style="border-width:0" src="http://i.creativecommons.org/l/by-nc-nd/3.0/80x15.png" /></a>\n \t\t\t\t\t<span xmlns:dct="http://purl.org/dc/terms/" href="http://purl.org/dc/dcmitype/InteractiveResource" property="dct:title" rel="dct:type">Loffelmacher.com</span> \n \t\t\t\t\tby <a xmlns:cc="http://creativecommons.org/ns#" href="http://loffelmacher.com" property="cc:attributionName" rel="cc:attributionURL">Randall Loffelmacher</a> \n \t\t\t\t\tis licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-nc-nd/3.0/">Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License</a>.\n \t\t\t\t</div>\n       </footer>\n     </div>\n   </div>\n </div>');
    
    }).call(this);
    
  }).call(__obj);
  __obj.safe = __objSafe, __obj.escape = __escape;
  return __out.join('');
}
  }
}));
