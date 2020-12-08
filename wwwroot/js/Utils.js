var NumberSepr = ",";

function ReplaceAll(str, replaceWhat, replaceTo) {
    var re = new RegExp(replaceWhat, 'g');
    return str.replace(re, replaceTo);
}

function thisselect(obj) {
    // Fuck Chrome
    setTimeout(function () { obj.select(); }, 12);
}

function evobj(e) {
    if (!e) var e = window.event;
    return e;
}

function ekeyCode(e) {
    return e.keyCode ? e.keyCode : (e.which ? e.which : 0);
}

function isKey(e, kc) {
    if (!e) var e = window.event;
    if (ekeyCode(e) == kc)
        return true;
    else
        return false;
}

function isEnterOnly(e) {
    if (!e) var e = window.event;
    if (ekeyCode(e) == 13 && !e.ctrlKey && !e.shiftKey)
        return true;
    else
        return false;
}

function g_tag(id) {
    return document.getElementById(id);
}

function elmText(x) {
    return x.innerText || x.textContent;
}

function _formval2NCR(str) {
    // special encoding
    if (str == '') return '';
    var res = "";
    for (var i = 0; i < str.length; i++) {
        res += str.charCodeAt(i) + ',';
    }
    return res.substr(0, res.length - 1);
}

function ajPage(url, func, params, _towait) {
    // _towait = true: blocking call
    var http = null;
    try { http = new ActiveXObject("Microsoft.XMLHTTP"); }
    catch (ex1) { http = new XMLHttpRequest(); }

    http.open("POST", url, _towait ? false : true);
    http.onreadystatechange = function () {
        if (http.readyState != 4) return;
        else if (http.status == 0 || http.status == 12029) { alert('No network connection'); return; }
        else if (http.status == 404) { alert(url + ' is not found on the Server'); return; }
        else if (http.status != 200) { if (confirm(url + ' failed (status=' + http.status + ')!\nClick OK to view details!')) { document.write(http.responseText); document.close(); } return; }
        else if (http.responseText.substring(0, 12) == '<!--ERROR-->') { document.write(http.responseText); return; }
        func(http);
        http = null;
    }

    if (params) {
        http.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        http.setRequestHeader("Content-length", params.length);
        http.send(params);
    }
    else
        http.send(null);
}

function ajFrmSubmit(url, frm, func, params, _towait) {
    // params appended with form
    if (params == '') params = 'ajfrmsmt_=1'; // so that appends with next '&' below
    for (var i = 0; i < frm.length; i++)
        params += '&' + frm.elements[i].name + '=' + frm.elements[i].value;
    ajPage(url, func, params, _towait);
}

function detectie6() {
    var rv = -1;
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }

    if (rv == -1 || rv > 6)
        return false;
    else
        return true;
}

function ie6fixifrm(b, ifrmId, offsetX) {

    var ifrm;
    if (document.getElementById(ifrmId)) {
        ifrm = document.getElementById(ifrmId);
    }
    else {
        ifrm = document.createElement('iframe');
        ifrm.id = ifrmId;
        document.body.appendChild(ifrm);
    }

    ifrm.frameBorder = 0;
    ifrm.style.position = 'absolute';
    if (b) {
        ifrm.style.left = b.offsetLeft;
        ifrm.style.top = b.offsetTop;
        ifrm.style.width = b.offsetWidth;
        if (offsetX) ifrm.style.width = b.offsetWidth - offsetX;
        ifrm.style.height = b.offsetHeight;
        b.style.zIndex += 1;
    }
    else
        ifrm.style.height = '1px';
}

function ajdia(ref, wi, w, h) {
    t = (document.body.clientHeight - h) / 2;
    if (t < 0) t = 8;
    var wh = window.open(ref, wi, 'toolbar=no,status=no,left=8,top=' + t + ',height=' + h + ',width=' + w + ',menu=no,location=no,scrollbars=yes,resizable=yes');
    wh.focus();
    return wh;
}

function escPage(e) {
    // at some caller, e was checked cross browser. We check against here
    if (!e) var e = window.event;
    var kc = ekeyCode(e);
    if (kc == 27) {
        var h_dnmnu = null;
        var stopE = false;
        if ((h_dnmnu = g_tag('H_dn_mnu')) != null) {
            if (h_dnmnu.style.display != 'none') {
                stopE = true;
                collapsemenu();
            }
        }
        var h_grpdiv = null;
        if ((h_grpdiv = g_tag('H_grpdiv')) != null) {
            if (h_grpdiv.style.display != 'none') {
                stopE = true;
                H_showmgrp();
            }
        }
        if (!stopE) {
            history.back();
        }
    }
    else if ((e.ctrlKey) && kc == 81)
        H_loadsc(e, e.shiftKey);
}

/*
function OnLoadClass() {
    var cls = readCookie("classbody");
    var objBody = document.getElementsByTagName("body");
    objBody.className = cls;
}

function CloseMenu(secure) {
    var cls1 = "skin-blue sidebar-mini";
    var cls2 = "skin-blue sidebar-mini sidebar-collapse";
    var objBody = document.getElementsByTagName("body");
    if (objBody.className == cls1) {
        objBody.className = cls2;
        setCookie("classbody", cls2, secure);
    } else {
        objBody.className = cls1;
        setCookie("classbody", cls1, secure);
    }
}
*/
function setCookie(cookie_name, cookie_value, secure) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + 180);
    // 180 days document.cookie = "tagname = test;secure";document.cookie = cookieName +"=" + cookieValue + ";expires=" + myDate + ";domain=.example.com;path=/";
    document.cookie = cookie_name + "=" + cookie_value + (secure ?"secure;":"") + ";expires=" + exdate.toUTCString() + ";path=/";
}

function readCookie(cookie_name) {
    var cookie_value = "";
    var the_cookie = document.cookie;
    if (the_cookie == null || the_cookie == "") {
        return "";
    }
    else {
        var val1 = the_cookie.split(cookie_name + "=")[1];

        if (val1 != null) {
            var val2 = val1.split(";")[0];
            if (val2 != null) cookie_value = unescape(val2);
        }
    }

    return cookie_value;
}

function elmTop(obj) {
    var toreturn = 0;
    while (obj) {
        toreturn += obj.offsetTop;
        obj = obj.offsetParent;
    }
    return toreturn;
}
function elmLeft(obj) {
    var toreturn = 0;
    while (obj) {
        toreturn += obj.offsetLeft;
        obj = obj.offsetParent;
    }
    return toreturn;
}

var integerExpr = null;
var numericExpr = null;
var numericNegExpr = null;
var vietDateExpr = null;
var vietTimeExpr = null;
var vietDateTimeExpr = null;
var emailExpr = null;
var phoneExpr = null;

function isInteger(d) {
    if (integerExpr == null) integerExpr = new RegExp("^[0-9]+( [0-9]+( [0-9]+( [0-9]+)?)?)?$");  // max 3 spaces - billion
    if (d.match(integerExpr)) return true; else return false;
}

function isNumeric(d) {
    if (numericExpr == null) numericExpr = new RegExp("^[0-9]+(" + NumberSepr + "[0-9]+(" + NumberSepr + "[0-9]+(" + NumberSepr + "[0-9]+)?)?)?([.][0-9]+)?$"); // max 3 spaces - billion
    if (d.match(numericExpr)) return true; else return false;
}

function isNumericNeg(d) {
    if (numericNegExpr == null) numericNegExpr = new RegExp("^(-)?[0-9]+( [0-9]+( [0-9]+( [0-9]+)?)?)?([.][0-9]+)?$");
    if (d.match(numericNegExpr)) return true; else return false;
}

function isVietDate(d) {
    if (vietDateExpr == null) vietDateExpr = new RegExp('^[0-3]?[0-9]/[0-1]?[0-9]/[0-9]{4}$');
    if (d.match(vietDateExpr)) return true; else return false;
}

function isVietDateTime(d) {
    if (vietDateTimeExpr == null) vietDateTimeExpr = new RegExp('^[0-3]?[0-9]/[0-1]?[0-9]/[0-9]{4} [0-2]?[0-9]:[0-5]?[0-9](:00)?$');
    if (d.match(vietDateTimeExpr)) return true; else return false;
}

function isTime(d) {
    if (vietTimeExpr == null) vietTimeExpr = new RegExp('^[0-2]?[0-9]:[0-5]?[0-9](:00)?$');
    if (d.match(vietTimeExpr)) return true; else return false;
}

function isEmail(d) {
    if (emailExpr == null) emailExpr = new RegExp("^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
    if (d.match(emailExpr)) return true; else return false;
}

function isPhone(d) {
    if (phoneExpr == null) phoneExpr = new RegExp(/^[(]{0,1}[0-9]{3}[)]{0,1}[-\s\.]{0,1}[0-9]{3}[-\s\.]{0,1}[0-9]{4}$/);
    if (d.match(phoneExpr)) return true; else return false;
}

function createDiv0(b, h, w, ot, ol, id) {
    var a = document.createElement('div');
    var c = a.style;
    c.border = '3px double black';
    c.padding = '4px';
    c.background = '#f4f4f0';
    c.position = 'fixed';//'absolute';
    c.top = (elmTop(b) + ot) + 'px';
    c.left = (elmLeft(b) + ol) + 'px';
    c.width = w;
    c.height = h;
    c.overflow = "auto";
    c.zIndex = 999;
    a.id = id;
    return a;
}

function createDivTop0(h, w, id) {
    var a = document.createElement('div');
    var c = a.style;
    c.border = '3px double black';
    c.padding = '4px';
    c.background = '#f4f4f0';
    c.position = 'fixed';//'absolute';
    c.top = '50px';
    c.right = '0px';
    c.width = w;
    c.height = h;
    c.zIndex = 999;
    c.overflow = "auto";
    a.id = id;
    return a;
}

function createDivWidthHeight(x, y, w, h, elm, removeFunc, escFunc, classn, px) {
    if (document.getElementById(elm) != null) {
        removeFunc();
        return false;
    }
    if (px == null) px = "%";
    var a = document.createElement('div');
    a.className = classn;
    a.style.top = ((y - 35) > 0 ? (y - 35) : y) + px;
    a.style.left = x + px;
    //a.style.position = 'fixed';//'absolute';
    //a.style.overflow = "auto";
    a.style.width = w + px;
    a.style.height = h + px;
    a.style.background = '#f4f4f0';
    a.style.padding = '5px';
    a.zIndex = 999;
    a.id = elm;
    a.style.display = 'none';
    a.onkeydown = escFunc;
    document.body.appendChild(a);
    return true;
}

function createDiv(x, y, elm, removeFunc, escFunc, classn) {
    if (document.getElementById(elm) != null) {
        removeFunc();
        return false;
    }
    var a = document.createElement('div');
    a.className = classn;
    a.style.top = y + 'px';
    a.style.left = x + 'px';
    a.style.position = 'fixed';//'absolute';
    a.style.overflow = "auto";
    a.style.width = "310px";
    a.style.height = "350px";
    a.style.background = '#f4f4f0';
    a.style.padding = '5px';
    a.style.zIndex = 999;
    a.id = elm;
    a.style.display = 'none';
    a.onkeydown = escFunc;
    document.body.appendChild(a);
    return true;
}

var DoCalTarget = null;
var DoCalFollowFunc = null;
var DoCalFollowFunc2 = null;

function DoCal(elm, func, func2, tt) {

    //func: calls uppon selects a date
    //func 2: calls when escape, selects a date, return, destroy box

    DoCalTarget = elm;

    var x = elmLeft(DoCalTarget);
    var y = elmTop(DoCalTarget) + 23;
    //if (x>document.body.clientWidth - 301) x=document.body.clientWidth - 301;
    //var minY = document.body.clientHeight - 199;
    //if (tt==1) minY-= 30;
    //if (y>minY) y=minY;
    //if (y<0) y=1;
    //if (!createDiv(x, y, 'DoCalbox', removeDoCal, escDoCal, 'DoCal')) return;

    if (!createDivWidthHeight(x, y, 320, 300, 'DoCalbox', removeDoCal, escDoCal, 'datepicker datepicker-dropdown dropdown-menu datepicker-orient-left datepicker-orient-bottom', 'px')) return;
    //if (!createDivWidthHeight(0, 0, 100, 100, 'DoCalbox', removeDoCal, escDoCal, 'DoCal')) return;
    ajPage('/Utils/DatePickerFeed?date=' + DoCalTarget.value + '&timetoo=' + tt, datepickerret);
    if (func) DoCalFollowFunc = func;
    if (func2) DoCalFollowFunc2 = func2;
}

function cancel_bubble(e) {
    // chrome has window.event, but uses stopPropogation
    try {
        e.preventDefault();
        e.stopPropogation();
    }
    catch (ex) {
        e.cancelBubble = true;
        e.returnValue = false;
    }

    if (e.keyCode)
        e.keyCode = 0;
    else
        e.which = 0;

    return false;
    //function stopEvent(evt){
    //evt || window.event;
    //if (evt.stopPropagation){
    //	evt.stopPropagation();
    //	evt.preventDefault();
    //}else if(typeof evt.cancelBubble != "undefined"){
    //	evt.cancelBubble = true;
    //	evt.returnValue = false;
    //}
    //return false;
    //}
}

function escDoCal(evt) {
    var e = evobj(evt);
    if (ekeyCode(e) == 27) {
        cancel_bubble(e);
        removeDoCal();
        DoCalTarget.focus();
    }
}

function removeDoCal() {
    if (document.getElementById('DoCalbox_ie6fix'))
        document.body.removeChild(document.getElementById('DoCalbox_ie6fix'));

    if (document.getElementById('DoCalbox'))document.body.removeChild(document.getElementById('DoCalbox'));

    eval(DoCalFollowFunc2);
    DoCalFollowFunc2 = null;
}

function datepickerret(http) {
    var a = document.getElementById('DoCalbox');
    a.style.display = 'block';

    var rethtml = http.responseText;
    var jssepr = rethtml.indexOf('<!--JS-->');
    if (jssepr > 0) {
        a.innerHTML = rethtml.substring(0, jssepr);
        // Append javascript code!
        var newScript = document.createElement('script');
        newScript.text = rethtml.substring(jssepr + 9, rethtml.length);
        a.appendChild(newScript);
    }
    else {
        a.innerHTML = rethtml;
    }

    document.getElementById('datepickerbtn').focus();
    if (detectie6()) {
        ie6fixifrm(a, 'DoCalbox_ie6fix');
    }
}

function feedd(d, tt) {
    ajPage('/Utils/DatePickerFeed?date=' + d + '&timetoo=' + tt, datepickerret);
}

function DoTime(elm, func, func2) {
    DoCalTarget = elm;
    var x = elmLeft(DoCalTarget);
    var y = elmTop(DoCalTarget) + 23;
    //if (x > document.body.clientWidth - 170) x = document.body.clientWidth - 170;
    //var minY = document.body.clientHeight - 350;
    //if (y > minY) y = minY;
    //if (y < 0) y = 1;
    //if (!createDiv(x, y, 'DoCalbox', removeDoCal, escDoCal, 'DoCal')) return;
    if (!createDivWidthHeight(x, y, 350, 300, 'DoCalbox', removeDoCal, escDoCal, 'datepicker datepicker-dropdown dropdown-menu datepicker-orient-left datepicker-orient-bottom', 'px')) return;
    ajPage('/Utils/ClockPicker?time=' + DoCalTarget.value, datepickerret);
    if (func) DoCalFollowFunc = func;
    if (func2) DoCalFollowFunc2 = func2;
}

function getCalTime(e, msg) {
    if (!e) var e = window.event;
    var t = getTargetElement(e);
    var txt = elmText(t);
    if (txt != "") {
        if (!(DoCalTarget.readOnly || DoCalTarget.disabled))
            DoCalTarget.value = txt;
        else
            alert((msg ? msg : 'Value is Read only'));
        eval(DoCalFollowFunc);
        DoCalFollowFunc = null;
    }
    removeDoCal();
}

function insertSepr(d0) {
    var d = '' + d0; // convert to string
    var i = 0;
    var d2 = '';
    var ic = 0;
    var ofs = d.length - 1;
    var decimalpoint = d.indexOf('.');
    if (decimalpoint >= 0) ofs = decimalpoint - 1;
    for (i = ofs; i >= 0; i--) {
        if (d.charAt(i) != NumberSepr) {
            if (ic++ % 3 == 0 && i != ofs && d.charAt(i) != '-') d2 = NumberSepr + d2;
            d2 = d.charAt(i) + d2;
        }
    }

    if (decimalpoint >= 0) {
        for (i = decimalpoint; i < d.length; i++)
            d2 += d.charAt(i);
    }
    return d2;
}

function removeSepr(d0) {
    return ('' + d0).replace(NumberSepr, '');
}

function monthLastDay(m, y) {
    var monthdays = 31;
    switch (m) {
        case 2:
            if (y % 4 == 0)
                monthdays = 29;
            else
                monthdays = 28;
            break;
        case 4:
        case 6:
        case 9:
        case 11:
            monthdays = 30;
            break;
    }
    return monthdays;
}

function jsrnd(num, d) {
    if (d) {
        var p = Math.pow(10, d);
        return Math.round(num * p) / p;
        //return num.toFixed(d); This method sucks big cross browser, and wrong mathematically!
    }
    else
        return Math.round(num);
}

function jsformat(n, d) {
    var r = '' + jsrnd(n, d);

    if (d) { // padding with zeros to have the fixed decimal points.
        var c = r.indexOf('.');
        if (c < 0) {
            r += '.';
            c = r.length;
        }
        for (var i = 0; i <= parseInt(d) + c - r.length; i++) r += '0';
    }

    return insertSepr(r);
}

function queryStr(param) {
    var params = location.search.substring(1).split('&'); // less '?'
    var p, i, i1;
    for (i = 0; i < params.length; i++) {
        i1 = params[i].indexOf('=');
        if (i1 < 0) {
            return '';
            break;
        }
        p = params[i].substring(0, i1);
        if (p == param) {
            return params[i].substring(i1 + 1, params[i].length);
            break;
        }
    }
    return '';
}

function queryStrM(param) {
    // multiple, for example atype=24&atype=24&atype=50&...
    // result in the form of multiple select 24,24,50 ...
    var params = unescape(location.search.substring(1)).split('&'); // less '?'
    var p, i, i1;
    var r = '';
    for (i = 0; i < params.length; i++) {
        i1 = params[i].indexOf('=');
        if (i1 < 0) {
            return '';
            break;
        }
        p = params[i].substring(0, i1);
        if (p == param) {
            r += params[i].substring(i1 + 1, params[i].length) + ',';
        }
    }
    if (r != '') r = r.substring(0, r.length - 1);
    return r;
}

/* *** ACTB *** */
/* Event Functions */

// Add an event to the obj given
// event_name refers to the event trigger, without the "on", like click or mouseover
// func_name refers to the function callback when event is triggered
function addEvent(obj, event_name, func_name) {
    if (obj.attachEvent) {
        obj.attachEvent("on" + event_name, func_name);
    } else if (obj.addEventListener) {
        obj.addEventListener(event_name, func_name, true);
    } else {
        obj["on" + event_name] = func_name;
    }
}

// Removes an event from the object
function removeEvent(obj, event_name, func_name) {
    if (obj.detachEvent) {
        obj.detachEvent("on" + event_name, func_name);
    } else if (obj.removeEventListener) {
        obj.removeEventListener(event_name, func_name, true);
    } else {
        obj["on" + event_name] = null;
    }
}

// Get the obj that starts the event
//function getElement(evt){
//	if (window.event){
//		return window.event.srcElement;
//	}else{
//		return evt.currentTarget;
//	}
//}
// Get the obj that triggers off the event
function getTargetElement(evt) {
    if (window.event) {
        return window.event.srcElement;
    } else {
        return evt.target;
    }
}
// For IE only, stops the obj from being selected
// function stopSelect(obj)

/*    Caret Functions     */

// Get the end position of the caret in the object. Note that the obj needs to be in focus first
function getCaretEnd(obj) {
    if (typeof obj.selectionEnd != "undefined") {
        return obj.selectionEnd;
    } else if (document.selection && document.selection.createRange) {
        var M = document.selection.createRange();
        try {
            var Lp = M.duplicate();
            Lp.moveToElementText(obj);
        } catch (e) {
            var Lp = obj.createTextRange();
        }
        Lp.setEndPoint("EndToEnd", M);
        var rb = Lp.text.length;
        if (rb > obj.value.length) {
            return -1;
        }
        return rb;
    }
}
// Get the start position of the caret in the object
function getCaretStart(obj) {
    try {
        if (typeof obj.selectionStart != "undefined") {
            return obj.selectionStart;
        } else if (document.selection && document.selection.createRange) {
            var M = document.selection.createRange();
            try {
                var Lp = M.duplicate();
                Lp.moveToElementText(obj);
            } catch (e) {
                var Lp = obj.createTextRange();
            }
            Lp.setEndPoint("EndToStart", M);
            var rb = Lp.text.length;
            if (rb > obj.value.length) {
                return -1;
            }
            return rb;
        }
    }
    catch (ex) { return -1 }
}
// sets the caret position to l in the object
function setCaret(obj, l) {
    obj.focus();
    if (obj.setSelectionRange) {
        obj.setSelectionRange(l, l);
    } else if (obj.createTextRange) {
        m = obj.createTextRange();
        m.moveStart('character', l);
        m.collapse();
        m.select();
    }
}
// sets the caret selection from s to e in the object
function setSelection(obj, s, e) {
    obj.focus();
    if (obj.setSelectionRange) {
        obj.setSelectionRange(s, e);
    } else if (obj.createTextRange) {
        m = obj.createTextRange();
        m.moveStart('character', s);
        m.moveEnd('character', e);
        m.select();
    }
}

/*    Escape function   */
String.prototype.addslashes = function () {
    return this.replace(/(["\\\.\|\[\]\^\*\+\?\$\(\)])/g, '\\$1');
}
String.prototype.trim = function () {
    return this.replace(/^\s*(\S*(\s+\S+)*)\s*$/, "$1");
};
/* --- Escape --- */

/* Types Function */

// is a given input a number?
//function isNumber(a) {
//    return typeof a == 'number' && isFinite(a);
//}

/* Object Functions */

//function replaceHTML(obj,text){
//	while(el = obj.childNodes[0]){
//		obj.removeChild(el);
//	};
//	obj.appendChild(document.createTextNode(text));
//}

function actb(obj, cafa, fEnter) {

    /* ---- Public Variables ---- */
    this.actb_timeOut = 4800; // Autocomplete Timeout in ms (-1: autocomplete never time out)
    this.actb_lim = 10;    // Number of elements autocomplete can show (-1: no limit)
    this.actb_firstText = false; // should the auto complete be limited to the beginning of keyword?
    this.actb_mouse = true; // Enable Mouse Support
    this.actb_delimiter = '';  // Delimiter for multiple autocomplete. Set it to empty array for single autocomplete
    this.actb_startcheck = 0; // Show widget only after this number of characters is typed in.
    this.actb_keywords = new Array();
    this.fEnter = fEnter;
    this.actb_keywords = cafa;
    this.invoked = false;

    /* --- Styles --- */
    this.actb_bgColor = '#FAFAFA';
    this.actb_navbgColor = '#CCC';
    this.actb_textColor = 'black';
    this.actb_hTextColor = '#FFFFFF';
    this.actb_hColor = '#3142C5';
    this.actb_hStyle = 'text-decoration:underline;font-weight:bold';

    /* /End of Public Variables ---- */

    /* ---- Private Variables ---- */
    var actb_delimwords = new Array();
    var actb_cdelimword = 0;
    var actb_delimchar = new Array();
    var actb_display = false;
    var actb_pos = 0;
    var actb_total = 0;
    var actb_rangeu = 0;
    var actb_ranged = 0;
    var actb_bool = new Array();
    var actb_pre = 0;
    var actb_toid;
    var actb_tomake = false;
    var actb_getpre = "";
    var actb_mouse_on_list = 1;
    var actb_kwcount = 0;
    var actb_caretmove = false;
    var isie6 = false;
    var actb_parent_resized = false;
    var actb_curr = obj;
    var actb_self = this;

    /* / End of Private Variables---- */

    addEvent(actb_curr, "focus", actb_setup);

    function actb_setup() {
        addEvent(document, "keydown", actb_checkkey);
        addEvent(actb_curr, "blur", actb_clear);
        //addEvent(document,"keypress",actb_keypress);
    }

    function actb_clear(evt) {
        if (!evt) evt = event;
        cancel_bubble(evt);
        removeEvent(document, "keydown", actb_checkkey);
        removeEvent(actb_curr, "blur", actb_clear);
        //removeEvent(document,"keypress",actb_keypress);
        actb_removedisp();
    }

    function actb_parse(n) {
        if (actb_self.invoked) return n;
        if (actb_self.actb_delimiter.length > 0) {
            var t = actb_delimwords[actb_cdelimword].trim().addslashes();
            var plen = actb_delimwords[actb_cdelimword].trim().length;
        } else {
            var t = actb_curr.value.addslashes();
            var plen = actb_curr.value.length;
        }
        var tobuild = '';
        var i;

        if (actb_self.actb_firstText) {
            var re = new RegExp("^" + t, "i");
        } else {
            var re = new RegExp(t, "i");
        }
        var p = n.search(re);
        var tTmp = '';

        for (i = 0; i < p; i++) {
            if (n.substr(i, 1) == ' ') {
                tTmp += '&nbsp;';
            } else {
                tTmp += n.substr(i, 1);
            }
        }
        tobuild = tTmp + "<font style='" + (actb_self.actb_hStyle) + "'>";
        tTmp = '';
        for (i = p; i < plen + p; i++) {
            if (n.substr(i, 1) == ' ') {
                tTmp += '&nbsp;';
            } else {
                tTmp += n.substr(i, 1);
            }
        }
        tobuild += tTmp + "</font>";
        tTmp = '';
        for (i = plen + p; i < n.length; i++) {
            if (n.substr(i, 1) == ' ') {
                tTmp += '&nbsp;';
            } else {
                tTmp += n.substr(i, 1);
            }
        }
        return tobuild + tTmp;
    }

    function removeie6bugifrm() {
        if (document.getElementById('actb_ie6_ifrm'))
            document.body.removeChild(document.getElementById('actb_ie6_ifrm'));
    }

    function ie6bug() {
        if (!isie6) return;
        ie6fixifrm(document.getElementById('tat_table'), 'actb_ie6_ifrm', 0);
    }
    function actb_generate() {
        if (document.getElementById('tat_table')) { actb_display = false; document.body.removeChild(document.getElementById('tat_table')); }
        if (actb_kwcount == 0) {
            actb_display = false;
            return;
        }
        var a = document.createElement('table');
        a.style.position = 'absolute';
        a.className = 'actbbox';

        a.cellSpacing = '1px';
        a.cellPadding = '2px';

        a.style.backgroundColor = actb_self.actb_bgColor;
        a.style.top = eval(elmTop(actb_curr) + actb_curr.offsetHeight) + "px";
        a.style.left = elmLeft(actb_curr) + "px";

        a.id = 'tat_table';
        document.body.appendChild(a);
        var i;
        var first = true;
        var _first = false;
        var j = 1;
        if (actb_self.actb_mouse) {
            a.onmouseout = actb_table_unfocus;
            a.onmouseover = actb_table_focus;
        }
        var counter = 0;
        for (i = 0; i < actb_self.actb_keywords.length; i++) {
            if (actb_bool[i]) {
                _first = false;
                counter++;
                r = a.insertRow(-1);
                if (first && !actb_tomake) {
                    r.style.backgroundColor = actb_self.actb_hColor;
                    first = false;
                    _first = true;
                    actb_pos = counter;
                } else if (actb_pre == i) {
                    r.style.backgroundColor = actb_self.actb_hColor;
                    first = false;
                    _first = true;
                    actb_pos = counter;
                } else {
                    r.style.backgroundColor = actb_self.actb_bgColor;
                }
                r.id = 'tat_tr' + (j);
                c = r.insertCell(-1);
                if (_first == true) {
                    //alert(actb_self.actb_hColor);
                    c.style.color = actb_self.actb_hTextColor;
                } else {
                    c.style.color = actb_self.actb_textColor;
                }
                c.className = 'actbfont';
                c.innerHTML = actb_parse(actb_self.actb_keywords[i]);
                c.id = 'tat_td' + (j);
                c.setAttribute('pos', j);
                if (actb_self.actb_mouse) {
                    c.style.cursor = 'pointer';
                    c.onclick = actb_mouseclick;
                    c.onmouseover = actb_table_highlight;
                }
                j++;
            }
            if (j - 1 == actb_self.actb_lim && j < actb_total) {
                r = a.insertRow(-1);
                r.style.backgroundColor = actb_self.actb_bgColor;
                c = r.insertCell(-1);
                c.style.backgroundColor = actb_self.actb_navbgColor;
                c.align = 'center';
                c.innerHTML = "<img src='/images/DownArrow.gif'>";
                if (actb_self.actb_mouse) {
                    c.style.cursor = 'pointer';
                    c.onclick = actb_mouse_down;
                }
                break;
            }
        }
        actb_rangeu = 1;
        actb_ranged = j - 1;
        actb_display = true;
        if (actb_pos <= 0) actb_pos = 1;
        if (parent) if (!actb_parent_resized) if (parent.iframesizeY) { parent.iframesizeY(); actb_parent_resized = true; }
    }
    function actb_remake() {
        // scroll up/down
        document.body.removeChild(document.getElementById('tat_table'));
        var a = document.createElement('table');
        a.style.position = 'absolute';
        a.className = 'actbbox';

        a.cellSpacing = '1px';
        a.cellPadding = '2px';

        a.style.backgroundColor = actb_self.actb_bgColor;
        a.style.top = eval(elmTop(actb_curr) + actb_curr.offsetHeight) + "px";
        a.style.left = elmLeft(actb_curr) + "px";

        a.id = 'tat_table';
        if (actb_self.actb_mouse) {
            a.onmouseout = actb_table_unfocus;
            a.onmouseover = actb_table_focus;
        }
        document.body.appendChild(a);
        var i;
        var first = true;
        var j = 1;
        if (actb_rangeu > 1) {
            r = a.insertRow(-1);
            r.style.backgroundColor = actb_self.actb_bgColor;
            c = r.insertCell(-1);
            c.style.backgroundColor = actb_self.actb_navbgColor;
            c.align = 'center';
            c.innerHTML = "<img src='/images/UpArrow.gif'>";
            if (actb_self.actb_mouse) {
                c.style.cursor = 'pointer';
                c.onclick = actb_mouse_up;
            }
        }
        for (i = 0; i < actb_self.actb_keywords.length; i++) {
            if (actb_bool[i]) {
                if (j >= actb_rangeu && j <= actb_ranged) {
                    r = a.insertRow(-1);
                    r.style.backgroundColor = actb_self.actb_bgColor;
                    r.id = 'tat_tr' + (j);
                    c = r.insertCell(-1);
                    c.style.color = actb_self.actb_textColor;
                    c.className = 'actbfont';
                    c.innerHTML = actb_parse(actb_self.actb_keywords[i]);
                    c.id = 'tat_td' + (j);
                    c.setAttribute('pos', j);
                    if (actb_self.actb_mouse) {
                        c.style.cursor = 'pointer';
                        c.onclick = actb_mouseclick;
                        c.onmouseover = actb_table_highlight;
                    }
                    j++;
                } else {
                    j++;
                }
            }
            if (j > actb_ranged) break;
        }
        if (j - 1 < actb_total) {
            r = a.insertRow(-1);
            r.style.backgroundColor = actb_self.actb_bgColor;
            c = r.insertCell(-1);
            c.style.backgroundColor = actb_self.actb_navbgColor;
            c.align = 'center';
            c.innerHTML = "<img src='/images/DownArrow.gif'>";
            if (actb_self.actb_mouse) {
                c.style.cursor = 'pointer';
                c.onclick = actb_mouse_down;
            }
        }
    }
    function actb_goup() {
        try {
            if (!actb_display) return;
            if (actb_pos == 1) return;
            document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_textColor;
            document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_bgColor;
            actb_pos--;
            if (actb_pos < actb_rangeu) actb_moveup();
            document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_hTextColor;
            document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_hColor;
            if (actb_toid) clearTimeout(actb_toid);
            actb_setTimeOut();
        }
        catch (ex) { // array might under loading
            actb_quit();
        }
    }
    function actb_godown() {
        if (!actb_display) return;
        if (actb_pos == actb_total) return;
        try {
            document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_textColor;
            document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_bgColor;
            actb_pos++;
            if (actb_pos > actb_ranged) actb_movedown();
            document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_hTextColor;
            document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_hColor;
            if (actb_toid) clearTimeout(actb_toid);
            actb_setTimeOut();
        }
        catch (ex) {
        }
    }
    function actb_movedown() {
        actb_rangeu++;
        actb_ranged++;
        actb_remake();
        ie6bug();
    }
    function actb_moveup() {
        actb_rangeu--;
        actb_ranged--;
        actb_remake();
        ie6bug();
    }

    /* Mouse */
    function actb_mouse_down() {
        document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_textColor;
        document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_bgColor;
        actb_pos++;
        actb_movedown();
        document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_hTextColor;
        document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_hColor;
        actb_curr.focus();
        actb_mouse_on_list = 0;
        if (actb_toid) clearTimeout(actb_toid);
        actb_setTimeOut();
    }
    function actb_mouse_up(evt) {
        if (!evt) evt = event;
        if (evt.stopPropagation) {
            evt.stopPropagation();
        } else {
            evt.cancelBubble = true;
        }
        document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_textColor;
        document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_bgColor;
        actb_pos--;
        actb_moveup();
        document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_hTextColor;
        document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_hColor;
        actb_curr.focus();
        actb_mouse_on_list = 0;
        if (actb_toid) clearTimeout(actb_toid);
        actb_setTimeOut();
    }
    function actb_mouseclick(evt) {
        //if (!evt) evt = event;
        if (!actb_display) return;
        actb_mouse_on_list = 0;
        actb_pos = this.getAttribute('pos');
        actb_penter();
    }
    function actb_table_focus() {
        actb_mouse_on_list = 1;
    }
    function actb_table_unfocus() {
        actb_mouse_on_list = 0;
        if (actb_toid) clearTimeout(actb_toid);
        actb_setTimeOut();
    }
    function actb_table_highlight() {
        try {
            actb_mouse_on_list = 1;
            document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_textColor;
            document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_bgColor;
            actb_pos = this.getAttribute('pos');
            while (actb_pos < actb_rangeu) actb_moveup();
            while (actb_pos > actb_ranged) actb_movedown();
            document.getElementById('tat_td' + actb_pos).style.color = actb_self.actb_hTextColor;
            document.getElementById('tat_tr' + actb_pos).style.backgroundColor = actb_self.actb_hColor;
            if (actb_toid) clearTimeout(actb_toid);
            actb_setTimeOut();
        }
        catch (ex) {
        }
    }
    /* ---- */

    function actb_insertword(a) {
        if (actb_self.actb_delimiter.length > 0) {
            str = '';
            l = 0;
            for (i = 0; i < actb_delimwords.length; i++) {
                if (actb_cdelimword == i) {
                    prespace = postspace = '';
                    gotbreak = false;
                    for (j = 0; j < actb_delimwords[i].length; ++j) {
                        if (actb_delimwords[i].charAt(j) != ' ') {
                            gotbreak = true;
                            break;
                        }
                        prespace += ' ';
                    }
                    for (j = actb_delimwords[i].length - 1; j >= 0; --j) {
                        if (actb_delimwords[i].charAt(j) != ' ') break;
                        postspace += ' ';
                    }
                    str += prespace;
                    str += a;
                    l = str.length;
                    if (gotbreak) str += postspace;
                } else {
                    str += actb_delimwords[i];
                }
                if (i != actb_delimwords.length - 1) {
                    str += actb_delimchar[i];
                }
            }
            actb_curr.value = str;
            setCaret(actb_curr, l);
        } else {
            actb_curr.value = a;
        }
        actb_quit();
    }
    function actb_penter() {
        if (!actb_display) return;
        actb_display = false;

        if (actb_curr.readOnly) { alert('Input is read-only'); actb_quit(); return; }

        var word = '';
        var c = 0;
        for (var i = 0; i <= actb_self.actb_keywords.length; i++) {
            if (actb_bool[i]) c++;
            if (c == actb_pos) {
                word = actb_self.actb_keywords[i]; //fillKeys
                break;
            }
        }
        actb_insertword(word);
        //l = getCaretStart(actb_curr);
        if (actb_self.fEnter != "") {
            eval(actb_self.fEnter);
        }
    }
    function actb_removedisp() {
        if (actb_mouse_on_list == 0) {
            actb_display = 0;
            if (document.getElementById('tat_table'))
                document.body.removeChild(document.getElementById('tat_table'));

            removeie6bugifrm();

            if (actb_toid) clearTimeout(actb_toid);
        }
    }
    function actb_quit() {
        actb_mouse_on_list = 0;
        actb_removedisp();
        actb_self.invoked = false;
    }
    function actb_setTimeOut() {
        if (actb_self.actb_timeOut > 0) actb_toid = setTimeout(function () { actb_quit(); }, actb_self.actb_timeOut);
    }

    //function actb_keypress(e){
    //	if (actb_caretmove) cancel_bubble(evobj(e));
    //	return !actb_caretmove;
    //}
    function actb_checkkey(evt) {
        if (!evt) evt = event;
        if (evt.shiftKey || evt.ctrlKey || evt.keyCode == 37 || evt.keyCode == 39) { return };

        a = evt.keyCode;
        caret_pos_start = getCaretStart(actb_curr);
        actb_caretmove = 0;
        switch (a) {
            case 38:
                actb_goup();
                actb_caretmove = 1;
                return false;
                break;
            case 40:
                if (actb_display) {
                    actb_godown();
                    actb_caretmove = 1;
                    return false;
                } else {
                    setTimeout(function () { actb_tocomplete(a) }, 50);
                }
                break;
            case 13:
            case 9:
                if (actb_display) {
                    actb_caretmove = 1;
                    actb_penter();
                    return false;
                } else {
                    return true;
                }
                break;
            case 27:
                actb_quit();
                break;
            default:
                actb_self.invoked = false; // start typing, clear invoked flag
                setTimeout(function () { actb_tocomplete(a) }, 40);
                break;
        }
    }

    function actb_tocomplete(kc) {
        if (kc == 38 || kc == 13) return;

        var i;
        if (actb_display) {
            var word = 0;
            var c = 0;
            for (var i = 0; i <= actb_self.actb_keywords.length; i++) {
                if (actb_bool[i]) c++;
                if (c == actb_pos) {
                    word = i;
                    break;
                }
            }
            actb_pre = word;
        } else { actb_pre = -1 };
        //if (actb_curr.value == ''){
        //	actb_mouse_on_list = 0;
        //	actb_removedisp();
        //	return;
        //}
        if (actb_self.actb_delimiter.length > 0) {
            caret_pos_start = getCaretStart(actb_curr);
            caret_pos_end = getCaretEnd(actb_curr);

            delim_split = '';
            for (i = 0; i < actb_self.actb_delimiter.length; i++) {
                delim_split += actb_self.actb_delimiter[i];
            }
            delim_split = delim_split.addslashes();
            delim_split_rx = new RegExp("([" + delim_split + "])");
            c = 0;
            actb_delimwords = new Array();
            actb_delimwords[0] = '';
            for (i = 0, j = actb_curr.value.length; i < actb_curr.value.length; i++ , j--) {
                if (actb_curr.value.substr(i, j).search(delim_split_rx) == 0) {
                    ma = actb_curr.value.substr(i, j).match(delim_split_rx);
                    actb_delimchar[c] = ma[1];
                    c++;
                    actb_delimwords[c] = '';
                } else {
                    actb_delimwords[c] += actb_curr.value.charAt(i);
                }
            }

            var l = 0;
            actb_cdelimword = -1;
            for (i = 0; i < actb_delimwords.length; i++) {
                if (caret_pos_end >= l && caret_pos_end <= l + actb_delimwords[i].length) {
                    actb_cdelimword = i;
                }
                l += actb_delimwords[i].length + 1;
            }
            var ot = actb_delimwords[actb_cdelimword].trim();
            var t = actb_delimwords[actb_cdelimword].addslashes().trim();
        } else {
            var ot = actb_curr.value;
            var t = actb_curr.value.addslashes();
        }
        //if (ot.length == 0){
        //	alert("vo day roi");
        //	actb_mouse_on_list = 0;
        //	actb_removedisp();
        //}
        if (ot.length < actb_self.actb_startcheck) return this;
        if (actb_self.actb_firstText) {
            var re = new RegExp("^" + t, "i");
        } else {
            var re = new RegExp(t, "i");
        }

        actb_total = 0;
        actb_tomake = false;
        actb_kwcount = 0;
        for (i = 0; i < actb_self.actb_keywords.length; i++) {
            actb_bool[i] = false;
            // (ot.length == 0)||. removed this
            if (re.test(actb_self.actb_keywords[i]) || actb_self.invoked) {
                actb_total++;
                actb_bool[i] = true;
                actb_kwcount++;
                if (actb_pre == i) actb_tomake = true;
            }
        }

        if (actb_toid) clearTimeout(actb_toid);
        actb_setTimeOut();
        isie6 = detectie6();
        actb_generate();
        ie6bug();
    }

    function actb_thisfocus() {
        actb_curr.focus();
    }

    // Public methods
    this.invoke = actb_tocomplete;
    this.objFocus = actb_thisfocus;
    this.quit = actb_quit;
    return this;
}

function actbinvoke(obj) {
    if (!obj.invoked) {
        obj.invoked = true;
        obj.invoke(0); // any key
        obj.objFocus(); // focus on text box, so that to setup mouse handlers
    }
    else {
        obj.quit();
    }
}

function UIMultipleSelectAll(f, a, item) {
    var fe = f.elements[item];
    var i;
    var maxl = fe.length;
    //if (maxl > 200) { maxl = 200; alert('List is too long!'); }
    if (a.checked) {
        for (i = 0; i < maxl; i++) {
            if (fe[i].disabled == false)fe[i].checked = true;
        }
    }
    else {
        for (i = 0; i < maxl; i++) {
            fe[i].checked = false;
        }
    }
}

function CreateScriptTag(a, js) {
    var newScript = document.createElement('script');
    newScript.text = js;
    a.appendChild(newScript);
}

function ExecJs(a, s) {
    var i = s.indexOf("<!--TagJS-->");
    var j = s.indexOf("<!--/TagJS-->");
    var js = "";
    while (!(i < 1 || j < 1 || i >= j)) {
        js = js + s.substring(i + 12, j);
        var s1 = s.substring(0, i - 1);
        var s2 = s.substring(j, s.length);
        s = s1 + s2;
        //CreateScriptTag(a, js);        
        //eval(js);
        i = s.indexOf("<!--TagJS-->");
        j = s.indexOf("<!--/TagJS-->");
    }

    i = s.indexOf("<!--JS-->"); var r = "";
    if (i > 0) {
        r = s.substring(0, i);
        js = js + s.substring(i + 9, s.length);
        //eval(js);
        //CreateScriptTag(a, js);
    }
    a.innerHTML = a.innerHTML + r;
    if (js != "") CreateScriptTag(a, js); //eval(js);
}

function UIMultiOneParentChoice(f, a, item, j) {
    //var v = new Array(); v = DataVal[j].split('||');
    var p = new Array(); p = DataParent[j].split('||');
    //CntRepeat = 0;
    var i = 0;
    var fe = f.elements[item]; var i; var maxl = fe.length;
    for (i = 0; i < maxl; i++) {
        fe[i].disabled = true;
        document.getElementById("tr_" + i).style.display = "none";
    }
    for (i = 0; i < maxl; i++) {
        console.log("\na: " + a + "; p[" + i + " + 1]: " + p[i + 1] + "; KT: " + (a == p[i + 1]));
        if (a == p[i + 1]) {
            fe[i].disabled = false;
            document.getElementById("tr_" + i).style.display = "block";
            //HiddenRepeatFuncCall(f, fe[i].value, item, v, p, CntRepeat);
        }
    }
}

var CntRepeat = 0;
function HiddenRepeatFuncCall(f, a, item, v, p, CntRepeat) {
    CntRepeat = CntRepeat + 1
    if (CntRepeat > 10) return;
    var fe = f.elements[item]; var i; var maxl = fe.length;
    for (i = 0; i < maxl; i++) {
        console.log("\na: " + a + "; v[" + i + " + 1]: " + v[i + 1] + "; p[" + i + " + 1]: " + p[i + 1] + "; KT: " + (a == v[i + 1] || a == p[i + 1]));
        if (a == v[i + 1] || a == p[i + 1]) {
            fe[i].disabled = false;
            document.getElementById("tr_" + i).style.display = "block";
            HiddenRepeatFuncCall(f, fe[i].value, item, v, p, CntRepeat);
        }
    }
}

function UIMultiParentChoice(f, a, item, j) {
    var v = new Array(); v = DataVal[j].split('||');
    var p = new Array(); p = DataParent[j].split('||');
    CntRepeat = 0;
    var fe = f.elements[item]; var i; var maxl = fe.length;
    for (var i = 0; i < maxl; i++) {
        fe[i].disabled = true;
        document.getElementById("tr_" + i).style.display = "none";
    }
    HiddenRepeatFuncCall(f, a, item, v, p, CntRepeat);
}

function MultiRepeatFuncCall(f, a, item, v, p, CntRepeat) {
    CntRepeat = CntRepeat + 1
    if (CntRepeat > 10) return;
    var fe = f.elements[item]; var i; var maxl = fe.length;
    if (a.checked) {
        for (i = 0; i < maxl; i++) {
            if (a.value == v[i + 1] || a.value == p[i + 1]) {
                fe[i].checked = true;
                MultiRepeatFuncCall(f, fe[i], item, v, p, CntRepeat);
            }
        }
    }
    else {
        for (i = 0; i < maxl; i++) {
            if (a.value == v[i + 1] || a.value == p[i + 1]) {
                fe[i].checked = false;
                MultiRepeatFuncCall(f, fe[i], item, v, p, CntRepeat);
            }
        }
    }
}

function UIMultiSelectParent(f, a, item, j) {
    var v = new Array(); v = DataVal[j].split('||');
    var p = new Array(); p = DataParent[j].split('||');
    CntRepeat = 0;
    MultiRepeatFuncCall(f, a, item, v, p, CntRepeat);   
}

function DoDBType(elm, ListSchema) {
    DoCalTarget = elm;
    var x = elmLeft(DoCalTarget);
    var y = elmTop(DoCalTarget) + 23;
    if (!createDivWidthHeight(x, y, 600, 300, 'DoCalbox', removeDoCal, escDoCal, 'DoDBType')) return;
    ajPage('/Utils/PopupDataTypeChoice?ListSchema=' + ListSchema + '&ElmType=' + DoCalTarget.value, datepickerret);
}

var DoExecStoreTarget = null;
function escDoExecStore(evt) {
    var e = evobj(evt);
    if (ekeyCode(e) == 27) {
        cancel_bubble(e);
        removeDoExecStore();
        DoExecStoreTarget.focus();
    }
}

function removeDoExecStore() {
    if (document.getElementById('DoExecStorebox')) document.body.removeChild(document.getElementById('DoExecStorebox'));
}

function DoExecStore(elm, ListSchema) {
    DoExecStoreTarget = elm;
    var x = elmLeft(DoExecStoreTarget);
    var y = elmTop(DoExecStoreTarget) + 23;
    if (!createDivWidthHeight(x, y, 600, 300, 'DoExecStorebox', removeDoExecStore, escDoExecStore, 'DoExecStore')) return;
    ajPage('/Utils/PopupStoreChoice?ListSchema=' + ListSchema + '&StoreName=' + DoExecStoreTarget.value, ExecStoreret);
}

function ExecStoreret(http) {
    var a = document.getElementById('DoExecStorebox');
    a.style.display = 'block';

    var rethtml = http.responseText;
    var jssepr = rethtml.indexOf('<!--JS-->');
    if (jssepr > 0) {
        a.innerHTML = rethtml.substring(0, jssepr);
        // Append javascript code!
        var newScript = document.createElement('script');
        newScript.text = rethtml.substring(jssepr + 9, rethtml.length);
        a.appendChild(newScript);
    }
    else {
        a.innerHTML = rethtml;
    }
    DoExecStoreTarget.focus();
}

// ------------ JS Popup -------------------
var intervals = [];
function JsPopup(objName, Id, clsName, msgHeader, msg, IsConfirm, Ok, Cancle, cAction, IsButtonClose, IsProcessBar, iTime, Url, IsReload) {//, w, h, ot, ol) {
    var a = document.createElement('div'); var InnerHtml = "";
    a.id = Id;
    a.className = "alert " + clsName;
    a.style.zIndex = 99995;
    a.style.padding = "10px";
    a.style.position = 'fixed';

    a.style.top = '0px';
    //a.style.left = '0%';
    a.style.right = '10px';
    
    //if (IsButtonClose)
    //    InnerHtml = InnerHtml + "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">×</button>";
    //else
        InnerHtml = InnerHtml + "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" onclick=\"JsPopupClose('" + Url + "','" + IsReload + "')\">×</button>";
    InnerHtml = InnerHtml + "<h4>" + msgHeader + "!</h4>" + (msg == "" ? "<div id=\"idContent" + Id + "\" style=\"overflow:auto; margin: 5px;\"></div>":msg);

    //if (IsProcessBar || IsConfirm)InnerHtml = InnerHtml + "<p>&nbsp</p>";
    if (IsConfirm) InnerHtml = InnerHtml + 
        "<table>" + // class=\"formsearch\"
        "<tr><td>" + // class=\"formrow\" // class=\"controls\"
        "<input type=\"button\" class=\"btn select\" value=\"" + Ok + "\" onclick=\"JsPopupClose('" + Url + "','" + IsReload + "');$('.close').click();" + cAction + ";\">" +
        "<input type=\"button\" class=\"btn cancel\" value=\"" + Cancle + "\" onclick=\"JsPopupClose('" + Url + "','" + IsReload + "');$('.close').click();\"></td></tr></table>";

    if (IsProcessBar) {
        InnerHtml = InnerHtml + "<div class=\"progress-sm\"><div class=\"progress-bar\" style=\"width: 100%;\"></div></div>";
        a.innerHTML = InnerHtml;
        var s = document.createElement('script');
        s.text = "var iTime = " + iTime + "; JsAlertTimeOut(iTime, '" + Url + "', '" + IsReload + "');";
        a.appendChild(s);
    }else     
        a.innerHTML = InnerHtml;

    /*
    if (objName != "") {
        var c = document.getElementById(objName);
        if (c) {
            a.style.position = 'absolute';
            //a.style.top = (elmTop(c) - 35) + 'px';
            a.style.left = '-300px';
            a.style.width = w + 'px';
            a.style.height = h + 'px';
            a.style.overflow = "auto";
        }
    }
    */
    return a;
}

function JsPopupInfo(objName, Id, msgHeader, w, h, ot, ol) {
    var a = JsPopup(objName, Id, 'alert-info', msgHeader, '', false, '', '', '', true, false, 0, '', '0');//, w, h, ot, ol);
    document.body.appendChild(a);
}

function JsConfirm(msgHeader, msg, Ok, Cancle, cAction) {
    var a = JsPopup('', 'JsConfirm', 'alert-info', msgHeader, msg, true, Ok, Cancle, cAction, true, true, 500, '', '0');
    document.body.appendChild(a);
}

function JsErrorWithTimeout(iTime, msgHeader, msg, Url) {
    var IsReload = 0;
    var IsButtonClose = false;
    var a = JsPopup('', 'JsAlert', "alert-error", msgHeader, msg, false, '', '', '', IsButtonClose, true, iTime, Url, IsReload);
    document.body.appendChild(a);
}

function JsAlert(c, msgHeader, msg, Url, IsReload) {    
    if (Url == undefined || Url == null) Url = "";
    if (IsReload == undefined || IsReload == null) IsReload = 0;
    var iTime = 500;
    switch (c) {
        case "alert-warning":
        case "alert-error":
            break;
        case "alert-info":
        case "alert-success":
            //iTime = 200;
            break;
    }
    var IsButtonClose = (Url == "" && IsReload == 0)
    var a = JsPopup('', 'JsAlert', c, msgHeader, msg, false, '', '', '', IsButtonClose, true, iTime, Url, IsReload);
    document.body.appendChild(a);
}

function JsAlertTimeOut(iTime, Url, IsReload) {
    var CntBar = 100;
    ClearInterval();
    var myVar = window.setInterval(myTimer, iTime);
    intervals.push(myVar);
    function myTimer() {
        CntBar = CntBar - 10;
        $('.progress-bar').css('width', CntBar + '%');
        if (CntBar <= 0) {
            CntBar = 100;
            ClearInterval();
            myVar = null;
            $('.close').click();
            if (IsReload == 1)
                window.location.reload();
            else if (Url != "")
                window.location.href = Url;
            else if (IsReload == 2)
                Saving(false);
        };        
    }
}

function JsPopupClose(Url, IsReload) {
    ClearInterval();
    if (IsReload == 1)
        window.location.reload();
    else if (Url != "")
        window.location.href = Url;
    else if (IsReload == 2)
        Saving(false);
    else if (IsReload == 3) {
        window.close();
    } else if (IsReload == 4) {
        var parent = window.opener;
        if (parent == null) parent = dialogArguments;
        parent.focus();
        parent.location.reload();
        window.close();
    }
}

function ClearInterval() {
    for (var i = 0; i < intervals.length; i++) clearInterval(intervals[i]);
    intervals = new Array();
}

function resizePopupPage() {
    var width = document.body.scrollWidth + 100; if (width < window.innerWidth) width = window.innerWidth;
    var height = document.body.scrollHeight + 100; if (height < window.innerHeight) height = window.innerHeight;
    window.resizeTo(width, height);
    window.moveTo(((screen.width - width) / 2), 100);//((screen.height - height) / 2)
}

function insertInputTableRow(frm, tab, lblButton) {
    var dm, dc, j, cellc;
    dm = document.getElementById(tab).insertRow(1);
    dc = dm.insertCell(-1);
    var a = new Array(); var c = new Array(); 
    a = ColumnName.split('||');
    c = ColumnType.split('*');
    for (j = 0; j < a.length; j++) {
        var isTd = false; var val = ""; var a31 = new Array(); a31 = c[j].split(':');
        if (a31.length > 1) val = a31[1];
        if (a31[0] == "-" || a31[0] == "") {
            cellc = "<input type='hidden' name='" + a[j] + "' id='" + a[j] + "' value='" + val + "'>";
            isTd = false;
        } else {
            isTd = true;
            switch (a31[0]) {
                case "Textbox":
                    cellc = "<input name='" + a[j] + "' id='" + a[j] + "' size=" + a31[3] + " maxlength=" + a31[4] + " value='" + val + "'>";
                    break;
                case "Numeric":
                    cellc = "<input name='" + a[j] + "' id='" + a[j] + "' size=12 maxlength=" + a31[4] + " value='" + val + "' style='text-align:right' onBlur='numericValid(this);this.value=insertSepr(this.value);' onChange='this.value=insertSepr(this.value)'>";
                    break;
                case "Date":
                    var d = new Date();
                    if (val == "@") {
                        val = d.getDate() + '/' + d.getMonth() + '/' + d.getFullYear()
                    } else if (val.substring(0, 2) == "@+") {
                        d.setDate(d.getDate() + parseInt(val.Substring(2)));
                        val = d.getDate() + '/' + d.getMonth() + '/' + d.getFullYear()
                    } else if (val.substring(0, 2) == "@-") {
                        d.setDate(d.getDate() - parseInt(val.Substring(2)));
                        val = d.getDate() + '/' + d.getMonth() + '/' + d.getFullYear()
                    }
                    cellc = "<input name='" + a[j] + "' id='" + a[j] + "' size=10 maxlength=10 value='" + val + "' onBlur='this.value=dateprompt(this);'><img src='/images/browse.gif' class='imggo' onClick=\"DoCal(document." + frm + ".elements[" + a[j] + "]);\">";
                    break;
                case "Datetime":
                    var d = new Date();
                    if (val == "@") {
                        val = d.getDate() + '/' + d.getMonth() + '/' + d.getFullYear() + ' ' + d.getHours() + ':' + d.getMinutes()
                    } else if (val.substring(0, 2) == "@+") {
                        d.setDate(d.getDate() + parseInt(val.Substring(2)));
                        val = d.getDate() + '/' + d.getMonth() + '/' + d.getFullYear() + ' ' + d.getHours() + ':' + d.getMinutes()
                    } else if (val.substring(0, 2) == "@-") {
                        d.setDate(d.getDate() - parseInt(val.Substring(2)));
                        val = d.getDate() + '/' + d.getMonth() + '/' + d.getFullYear() + ' ' + d.getHours() + ':' + d.getMinutes()
                    }
                    cellc = "<input name='" + a[j] + "' id='" + a[j] + "' size=10 maxlength=14 value='" + val + "' onBlur='this.value=dateprompt(this);'><img src='/images/browse.gif' class='imggo' onClick=\"DoCal(document." + frm + ".elements[" + a[j] + "], null, null, 1);\">";
                    break;
                case "Time":
                    var d = new Date();
                    val = d.getHours() + ':' + d.getMinutes();
                    cellc = "<input name='" + a[j] + "' id='" + a[j] + "' size=10 maxlength=6 value='" + val + "' onBlur='timeValid(this);'><img src='/images/browse.gif' class='imggo' onClick=\"DoTime(document." + frm + ".elements[" + a[j] + "], null, null, null);\">";
                    break;
                case "Checkbox":
                    cellc = "<input type='checkbox' name='" + a[j] + "' id='" + a[j] + "' value='1'>";
                    break;
                case "Radio":
                case "AutoNumber":
                case "SelectboxText":
                    if (a31.length > 5) funcOnchange = a31[5];
                case "Selectbox":
                    if (a31.length > 6) funcOnchange = a31[6];
                case "ActbText":
                    if (a31.length > 7) funcOnchange = a31[7];
                case "Actb":
                    if (a31.length > 8) funcOnchange = a31[8];
                    cellc = "<select name='" + a[j] + "' id='" + a[j] + "' onchange='" + funcOnchange + "'>" + ColumnData[j] + "</select>";
                    break;
            }
        }
        if (isTd) dc = dm.insertCell(-1);
        dc.innerHTML = dc.innerHTML + '' + cellc;
        dc = dm.insertCell(-1);
        dc.innerHTML = dc.innerHTML + '<input type="button" class="btn delete" id="bntDelete" name="bntDelete" value="' + lblButton + '" onclick="DeleteRow(\'' + tab + '\', this.parentNode.parentNode.rowIndex);">';
    }
}

// Print Pdf
var margins = {
    top: 70,
    bottom: 40,
    left: 30,
    width: 550
};
function footer(doc, pageNumber, totalPages) {
    var str = "Page " + pageNumber + " of " + totalPages;
    doc.setFontSize(10);
    doc.text(str, margins.left, doc.internal.pageSize.height - 20);
};
function header(doc, strHeader) {
    doc.setFontSize(30);
    doc.setTextColor(40);
    doc.setFontStyle('normal');

    if (strHeader == null) strHeader = "Javascript: Convert HTML to Pdf";
    doc.text(strHeader, margins.left + 50, 40);
    doc.setLineCap(2);
    doc.line(3, 70, margins.width + 43, 70); // horizontal line
};
function headerFooterFormatting(doc, totalPages, strHeader) {
    for (var i = totalPages; i >= 1; i--) {
        doc.setPage(i);
        //header
        header(doc, strHeader);
        footer(doc, i, totalPages);
        doc.page++;
    }
};
PrintPdf = function () {
    var pdf = new jsPDF('p', 'pt', 'a4');
    pdf.setFontSize(18);
    pdf.fromHTML(document.getElementById('html-2-pdfwrapper'), //
        margins.left, // x coord
        margins.top,
        {
            // y coord
            width: margins.width// max width of content on PDF
        }, function (dispose) {
            headerFooterFormatting(pdf, pdf.internal.getNumberOfPages());
        },
        margins);
    pdf.save('test.pdf');
};

// tree table
function tt_set_kids(tt, checked) {
    // User has just set/unset a parent. All kids should be updated to match:
    if (!tt) { return }
    tt.children.forEach(function (kid) {
        if (!kid.nn_cb) { return } // has no checkbox, prob not init'd yet
        kid.nn_cb.prop('checked', checked);
        tt_set_kids(kid, checked);
    });
}

function tt_check_parent(tt) {
    // I have just changed a child. See if this makes parent (and 
    // grandparents etc) set/unset/indeterminate:
    if (!tt) { return }
    var ch = false, un = false;
    tt.children.forEach(function (kid) {
        if (kid.nn_cb.is(':indeterminate')) { ch = un = true }
        else if (kid.nn_cb.is(':checked')) { ch = true }
        else { un = true }
    });
    // We should be checked if there are NO unchecked (all checked) kids
    // We should be indeterminate if there are checked+unchecked kids:
    tt.nn_cb.prop('checked', !un).prop('indeterminate', ch && un);
    tt_check_parent(tt.tree[tt.parentId]);
}

function OpenModelSearch(divIframe, Iframe, sUrl) {
    var d = document.getElementById(divIframe);
    var i = document.getElementById(Iframe);
    if (d && i) {
        d.style.display = "block";
        i.style.display = "block";
        i.src = sUrl;
    }
}
function OnLoadModelSearch(d, i) {
    i.style.height = i.contentWindow.document.body.scrollHeight + 'px';
    i.style.width = i.contentWindow.document.body.scrollWidth + 'px';

    var dw = 0, dh = 0, iw = 0, ih = 0; dmt = 0; dml = 0;
    dw = d.offsetWidth;
    dh = d.offsetHeight;
    iw = i.offsetWidth;
    ih = i.offsetHeight;
    dmt = (dw > iw ? (dw - iw) / 2 : 0);
    dml = (dh > ih ? (dh - ih) / 2 : 0);
    i.style.padding = dmt + "px " + dml + "px";
}
function CloseModelSearch(divIframe, Iframe) {
    var d = document.getElementById(divIframe);
    var i = document.getElementById(Iframe);
    if (d && i) {
        d.style.display = "none";
        i.style.display = "none";
    }
}