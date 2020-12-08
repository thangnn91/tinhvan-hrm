
function searchboxfixie6bug() {
	// once
	if (detectie6()) {
		ie6fixifrm(document.getElementById('_browseprd'), 'browsebox_ie6_ifrm');
	}
}

var browseselpos = 0; // remember up-down key focused item
var browseinput = null; // browse currently for this input. actb2 sets this obj

function actb2(surl, defid, index, inputname, retfunc, w, h, v, formvar) {
	// defid = [I] of table BaseTabGrp
	// index = the order of inputDefs's column. These two use to retrieve inputDefs for surl
	// use2: index = syntaxstring

	var fe = document.bosfrm.elements;
	
	var formvals = '';
	if (formvar) {
		if (formvar=='@TRXELM') {
			// extract params from elm
			var i1=inputname.indexOf('_');
			var elmvar = inputname.substring(1, i1);
			// send full TRX form params to handler page
			formvals = '&' + srcdstwaterclass(elmvar);
		}
		else { 
			var fvars = formvar.split('||');
			var fval;
			for (var i=0;i<fvars.length;i++) {
				if(fvars[i].charAt(0)!='&') { // if '&var=' then direct param, no need to get from <form>
					if((fval=fe[fvars[i]]))
						formvals += '&' + fvars[i] + '=' + escape(fval.value);
				}
				else {
					formvals += fvars[i];
				}
			}
		}
		// sends formvar also
		formvals += '&formvar=' + escape(formvar);
	}
	
	ajPage((surl=='' ? '../Basetabs/ajSearchFrm.aspx' : surl), actb2ret, 'defid=' + defid + '&index=' + index + '&retfunc=' + retfunc + '&inputname=' + inputname + '&str=' + _formval2NCR(v) + formvals);
	//window.open((surl=='' ? '../Basetabs/ajSearchFrm.aspx' : (surl.indexOf('?')>0 ? surl : surl)) + '?defid=' + defid + '&index=' + index + '&retfunc=' + retfunc + '&inputname=' + inputname + '&str=' + _formval2NCR(v) + formvals);

	browseinput = fe[inputname]; // saved obj for later use

	if (w==-1) return; // subsequece call

	if (w==0) w=400;
	if (h==0) h=400;
	var a = fe[inputname];
	var x = elmLeft(a);
	var y = elmTop(a)+23;
	if (x>document.body.clientWidth - w) x=document.body.clientWidth - w;
	if (y>document.body.clientHeight - h) y=document.body.clientHeight - h;
	if (x<0) x=1;
	if (y<0) y=1;

	var b = document.getElementById('_browseprd');
	b.style.left=x;
	b.style.top=y;
	b.style.width=w;
	b.style.height=h;

	var c = document.getElementById('_prdcontent');
	c.style.width=w-9;
	c.style.height=h-33;
	//document.body.style.cursor='wait';
}
function quit_actb2() {
	if (detectie6()) {
		if (document.getElementById('browsebox_ie6_ifrm'))
			document.body.removeChild(document.getElementById('browsebox_ie6_ifrm'));
	}
	document.getElementById('_browseprd').style.display='none';
}

function actb2ret(http) {

	var boxcontent = document.getElementById('_prdcontent');
	document.getElementById('_browseprd').style.display='block';
	
	//// Somewhat get resized all ready!
	//if (parent) if(parent.iframesize) {
	//	try {
	//		parent.iframesize('');
	//	}
	//	catch (ex) {
	//		parent.document.getElementById('subw').style.height=parseInt(boxcontent.style.height) + 40;
	//	}
	//}

	var rethtml = http.responseText;
	var rpartsepr = rethtml.indexOf('<!--R-->');
	if (rpartsepr>0) {
		document.getElementById('_browseprdhdr').innerHTML=rethtml.substring(0, rpartsepr);
		var content2 = rethtml.substring(rpartsepr+8, rethtml.length);
		var jssepr = content2.indexOf('<!--JS-->');
		if (jssepr>0) {
			boxcontent.innerHTML=content2.substring(0, jssepr);
			// Append javascript code!
			var newScript = document.createElement('script');
			newScript.text = content2.substring(jssepr+9, content2.length);
			boxcontent.appendChild(newScript);
		}
		else {
			boxcontent.innerHTML=content2;
		}
		if (document.getElementById('_browseprdfocus')) document.getElementById('_browseprdfocus').focus();
	}
	else {
		boxcontent.innerHTML=rethtml;
	}

	browseselpos = 0;
	//document.body.style.cursor='default';
	var hrefx=document.getElementById('bhref0');
	if (hrefx !=null) {
		document.getElementById('btrid0').style.background='#3399ff';
		hrefx.focus();
	}
	else if (document.getElementById('searchstr')!=null)
		document.getElementById('searchstr').focus();

	searchboxfixie6bug(); // after size is determined
}

function actb2fill0(inputname, dcode, dname) {
	var obj;
	if ((obj=document.bosfrm.elements[inputname]).readOnly)
		alert('Value is Read only!');
	else
		document.bosfrm.elements[inputname].value = dcode + ' - ' + dname;
}

function actb2fill1(inputname, dcode, dname) {
	actb2fill0(inputname, dcode, dname);
}

function actb2fill(inputname, retfunc, dcode, dname) {
	actb2fill0(inputname, dcode, dname);
	quit_actb2();
	eval(retfunc);
}

function focuscaller() {
	browseinput.focus();
}

function escbrowsebox(e) {
if (!e) var e=window.event;
if (ekeyCode(e)==27) {cancel_bubble(e); quit_actb2();browseinput.focus();}
}

function selhrefkeypress(e) {
if (!e) var e=window.event;
var k = ekeyCode(e);

var d=0;
if (k==40) 
	d = 1; 
else if(k==38)
	d = -1;

if (d!=0) {
	cancel_bubble(e);
	var hrefx=document.getElementById('bhref' + (browseselpos+d))
	if (hrefx !=null ) {
		document.getElementById('btrid' + (browseselpos+d)).style.background='#33ccff';
		document.getElementById('btrid' + browseselpos).style.background='';
		hrefx.focus();
		browseselpos += d;
	}
}
}

function inputBuilderRet(inputname, val, retfunc) {
	document.bosfrm.elements[inputname].value=val;
	quit_actb2();
	eval(retfunc);
}

// multiple dropdownbox helpers
function uiddmh(frm, input, force) {
    var b = document.getElementById(input + '_div');
    var fe = frm; //document.bosfrm.elements;
    if (b.style.display != 'none' && !force) {
        uiddmhclose(input);
        return;
    }

    var a = fe[input];
    b.style.top = elmTop(a) + 20;
    b.style.left = elmLeft(a);
    b.style.display = 'block';
    document.getElementById(input + 'img').src = '/images/Icon_SelectDone.gif';

    // Need to check 'checked' boxes!
    var input_ = input + '_';
    var selected = ',' + a.value + ',';

    try {
        if (fe[input_][1]) {
            for (var i = 0; i < fe[input_].length; i++) {
                if (selected.indexOf(',' + fe[input_][i].value + ',') >= 0)
                    fe[input_][i].checked = true;
                else
                    fe[input_][i].checked = false;
            }
        }
        else {
            if (selected.indexOf(',' + fe[input_].value + ',') >= 0)
                fe[input_].checked = true;
            else
                fe[input_].checked = false;
        }
    }
    catch (ex) {
        // no content
    }
    if (detectie6()) ie6fixifrm(b, input + 'ie6fix');
}

function uiddmhsel(input) {
    var val = '';
    var f = document.bosfrm;
    var input_ = input + '_';
    try {
        if (f.elements[input_][1]) {
            // content multiple line
            for (var i = 0; i < f.elements[input_].length; i++) {
                if (f.elements[input_][i].checked) val += f.elements[input_][i].value + ',';
            }
        }
        else {
            // content one line
            if (f.elements[input_].checked) val = f.elements[input_].value + ',';
        }
    }
    catch (ex) {
        // no content
    }

    document.bosfrm.elements[input].value = val.substring(0, val.length - 1);
}

function uiddmhclose(input) {

    document.getElementById(input + '_div').style.display = 'none';
    document.getElementById(input + 'img').src = '/images/Icon_Select.gif';

    if (detectie6()) document.body.removeChild(document.getElementById(input + 'ie6fix'));
}