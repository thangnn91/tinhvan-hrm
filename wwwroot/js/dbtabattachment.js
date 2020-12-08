					var _attCnt=0, _selCnt=0;
					function attachment() {
					_attw=ajdia('../Basetabs/Attachment.aspx?dbtab=' + jsdbtab + '&subfolder=' + document.bosfrm.subfolder.value, '_attachW', 650, 400);
					}
					function addattachedfile(filename, docid, contenttype) {
					var dm, dc;

					dm=document.getElementById('_attdoc').insertRow(-1);
					dm.id='_attid' + _attCnt;
					dc=dm.insertCell(-1);
					var i1=0, i2=0, fileextl;
					while (i2>=0) {
						i1=i2;
						i2=filename.indexOf('.', i2+1);
					}
					fileextl = filename.substring(i1+1).toLowerCase();
					dc.innerHTML = "<a href='../DocRoot/RenderFile.aspx?folder=Basetabs&dbtab=" + jsdbtab + "&subfolder=" + document.bosfrm.subfolder.value + "&filename=" + filename + "&ct=" + contenttype + "' target='_viewImgW'><img src='/images/Icon_" + fileextl.substring(0,3) + ".gif' border=1> " + filename + "</a>";
					
					dc=dm.insertCell(-1);
					dc.innerHTML = "<a href='javascript:removeattrow(" + _attCnt++ + "," + docid + ")'>[Remove]</a>";

					document.bosfrm.attdoc.value += docid + ',';
					attached=true;

					}

					function str_replace_reg(haystack, needle, replacement) {
					var r = new RegExp(needle, 'g');
					return haystack.replace(r, replacement);
					}

					function removeattrow(cnt, docid) {
					document.getElementById('_attid' + cnt).style.display='none';
					var attval = document.bosfrm.attdoc.value;
					if (attval.substring(0, (docid + ',').length) == docid + ',') {
						attval=attval.substring((docid + ',').length);
					}
					else {
						attval = str_replace_reg(attval, ',' + docid + ',', ',');
					}
					document.bosfrm.attdoc.value = attval;
					}

					function removeselrow(cnt, docid) {
					document.getElementById('_selid' + cnt).style.display='none';
					var selval = document.bosfrm.seldoc.value;
					if (selval.substring(0, (docid + ',').length) == docid + ',') {
						selval=selval.substring((docid + ',').length);
					}
					else {
						selval = str_replace_reg(selval, ',' + docid + ',', ',');
					}
					document.bosfrm.seldoc.value = selval;
					}
