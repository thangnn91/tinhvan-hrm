function QueryString(myParam) {
    var urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(myParam);
}

function insertTabRow(tab, valArr) {
    var dm, dc, j, cellc; frm = document.ListForm; var cnt = parseInt(frm.ItemsCnt.value); frm.ItemsCnt.value = cnt + 1;
    dm=document.getElementById(tab).insertRow(1);
    dc=dm.insertCell(-1);
    dc.innerHTML = "<input type='hidden' name='Changed" + cnt + "' id='Changed" + cnt + "' value=1>" +
        "<input type='hidden' name='SysUID" + cnt + "' id='SysUID" + cnt + "' value=" + frm.SysUID.value + ">";
    iCheck = true;
    var a = new Array(); var c = new Array(); var b = new Array(); var e = new Array(); //ColumnData
    a = ColumnName;//.split('^');
    b = ColumnLable;//.split('^');
    c = ColumnType;//.split('^');
    if (valArr) {
        e = valArr.split('^');
    }
    
    for (j = 0; j < a.length; j++) {
        var isTd = false; var a31 = new Array(); var v = ""; var val = "";
        a31 = c[j].split(';');
        if (j >= e.length) {
            if (a31.length > 1) v = a31[1];
            if (v.length > 7) if (v.substring(0, 7) == "REQUEST") v = QueryString(v.substring(7));
            if (val == "" && v != "") val = v;
        } else {
            val = e[j];
        }
        
        if (a31[0] == "-" || a31[0] == "") {
            cellc = "<input type='hidden' name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' value='" + val + "'>" + (a[j] == "STT" ? (cnt+1):"");
            isTd = false;
        }            
        else {
            isTd = true;
            var funcOnchange = "";
            switch (a31[0]) {
                case "HREF":
                    cellc = "<input placeholder=\"" + b[j] + "\" class=\"form-control\" autocomplete=\"off\" name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' size=12 maxlength=30>";
                    break;
                case "DBTypePopup":
                    cellc = "<input class=\"form-control\" autocomplete=\"off\" name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' size=" + a31[3] + " maxlength=" + a31[4] + " value='" + val + "'>";
                    cellc = cellc + "<input type=\"button\" class=\"btn find\" id=\"InputType_DBP" + cnt + "\" name=\"InputType_DBP" + cnt + "\" onclick=\"DoDBType(this.form.elements['InputType" + cnt + "']);\">";
                    break;
                case "Textbox":
                    cellc = "<input placeholder=\"" + b[j] + "\" class=\"form-control\" autocomplete=\"off\" name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' size=" + a31[3] + " maxlength=" + a31[4] + " value='" + val + "'>";
                    break;
                case "Numeric":
                    cellc = "<input placeholder=\"" + b[j] + "\" class=\"form-control\" autocomplete=\"off\" name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' size=12 maxlength=" + a31[4] + " value='" + (a[j].toLowerCase() == "orderby" ? (cnt+1):val) + "' style='text-align:right' onBlur='numericValid(this);this.value=insertSepr(this.value);' onChange='this.value=insertSepr(this.value)'>";
                    break;
                case "Date":
                    var d; 
                    if (val == "@") {
                        d = new Date();
                    } else if (val.substring(0,2) == "@+") {
                        d = new Date();
                        d.setDate(d.getDate() + parseInt(val.substring(2)));
                    } else if (val.substring(0, 2) == "@-") {
                        d = new Date();
                        d.setDate(d.getDate() - parseInt(val.substring(2)));
                    }
                    if (d) {
                        var d1 = d.getDate(); var m1 = d.getMonth() + 1; var y1 = d.getFullYear();
                        val = d1 + '/' + (m1 < 10 ? "0" + m1 : m1) + '/' + y1;
                    } else {
                        val = "";
                    }                    
                    //cellc = "<input autocomplete=\"off\" name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' size=10 maxlength=10 value='" + val + "' style=\"text-align:center;\" onBlur='this.value=datePrompt(this);'><img src='/images/browse.gif' class='imggo' onClick=\"DoCal(document.ListForm.elements['" + a[j] + "" + cnt + "']);\">";
                    cellc = "<div class=\"input-group\">" +
                        "<input type=\"text\" id=\"" + a[j] + "" + cnt + "\" name=\"" + a[j] + "" + cnt + "\" value=\"" + val + "\"  autocomplete=\"off\"  onchange=\"this.value=datePrompt(this)\" class=\"form-control\" placeholder=\"" + b[j] + "\">" +
                        "<span class=\"input-group-addon\" onClick=\"DoCal(document.ListForm.elements['" + a[j] + "" + cnt + "']);', null);\">" +
                        "<i class=\"fa fa-calendar\"></i></span></div>";
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
                    //cellc = "<input autocomplete=\"off\" name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' size=10 maxlength=14 value='" + val + "' style=\"text-align:center;\" onBlur='this.value=datePrompt(this);'><img src='/images/browse.gif' class='imggo' onClick=\"DoCal(document.ListForm.elements['" + a[j] + "" + cnt + "'], null, null, 1);\">";
                    cellc = "<div class=\"input-group\">" +
                        "<input type=\"text\" id=\"" + a[j] + "" + cnt + "\" name=\"" + a[j] + "" + cnt + "\" value=\"" + val + "\"  autocomplete=\"off\"  onchange=\"this.value=datePrompt(this)\" class=\"form-control\" placeholder=\"" + b[j] + "\">" +
                        "<span class=\"input-group-addon\" onClick=\"DoCal(document.ListForm.elements['" + a[j] + "" + cnt + "'], null, null, 1);\">" +
                        "<i class=\"fa fa-calendar\"></i></span></div>";
                    break;
                case "Time":
                    var d = new Date();
                    val = d.getHours() + ':' + d.getMinutes();
                    //cellc = "<input autocomplete=\"off\" name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' size=10 maxlength=6 value='" + val + "' style=\"text-align:center;\" onBlur='timeValid(this);'><img src='/images/browse.gif' class='imggo' onClick=\"DoTime(document.ListForm.elements['" + a[j] + "" + cnt + "'], null, null, null);\">";
                    cellc = "<div class=\"input-group\">" +
                        "<input type=\"text\" id=\"" + a[j] + "" + cnt + "\" name=\"" + a[j] + "" + cnt + "\" value=\"" + val + "\"  autocomplete=\"off\"  onchange=\"this.value=datePrompt(this)\" class=\"form-control\" placeholder=\"" + b[j] + "\">" +
                        "<span class=\"input-group-addon\" onClick=\"DoTime(document.ListForm.elements['" + a[j] + "" + cnt + "'], null, null, null);\">" +
                        "<i class=\"fa fa-calendar\"></i></span></div>";
                    break;
                case "Checkbox":
                    //cellc = "<input type='checkbox' name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' value='1'>";
                    cellc = "<input type=\"checkbox\" id=\"" + a[j] + "" + cnt + "\" name=\"" + a[j] + "" + cnt + "\" value=\"1\" class=\"form-check-input\">" +
                        "<label class=\"form-check-label\" for=\"" + a[j] + "" + cnt + "\"></label>";
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
                    cellc = "<select class=\"select2 form-control\" name='" + a[j] + "" + cnt + "' id='" + a[j] + "" + cnt + "' onchange='" + funcOnchange + "'>" + ColumnData[j] + "</select>";
                    //var inputName = '#' + a[j] + "" + cnt;
                    //setTimeout(function () {
                    //    $(inputName).select2();
                    //}, 300);
                    //$(inputName).select2();
                    break;
            }
        }
        if (isTd)dc = dm.insertCell(-1);
        dc.innerHTML = dc.innerHTML + '' + cellc;
        dc.className = "left";
        $(dc).find('.select2').select2();
    }
    dc = dm.insertCell(-1);
    dc.innerHTML = dc.innerHTML + ' ';
    if (parent) if (parent.iframesize) {
        try {
            parent.iframesizeY();
        }
        catch (ex) {

        }
    }
} 

function setSelect(a, a1, a2, a3, b) {
    if (b.type != "select-one") return;
    b.options.length = 0;
    var option = document.createElement("option");
    option.text = "";
    option.value = "";
    option.selected = true;
    b.appendChild(option);
    for (var i = 0; i < a1.length; i++) {
        var a3i = a3[i];
        var j = a3i.indexOf(":");
        if (j > 0) a3i = a3i.substring(0, j);
        if (a == a3i) {
            var option = document.createElement("option");
            option.text = a2[i];
            option.value = a1[i];
            b.appendChild(option);
        }
    }
}

function getChild(a, b, b1, c) {
    var a1 = new Array();
    var a2 = new Array();
    var a3 = new Array();
    a1 = DataVal[c].split('||');
    a2 = DataTxt[c].split('||');
    a3 = DataParent[c].split('||');

    if (b1) if (b1.type == "select-one") b1.options.length = 0;
    setSelect(a, a1, a2, a3, b);
}

function getTwoChild(i, a, b, b1, c) {
    var a1 = new Array();
    var a2 = new Array();
    var a3 = new Array();
    a1 = DataVal[c].split('||');
    a2 = DataTxt[c].split('||');
    a3 = DataParent[c].split('||');
    setSelect(a, a1, a2, a3, b);

    a1 = DataVal[c + i].split('||');
    a2 = DataTxt[c + i].split('||');
    a3 = DataParent[c + i].split('||');
    setSelect(a, a1, a2, a3, b1);
}

function DSRet(http) {
    //document.body.style.cursor = 'wait'; // con tro cho
    var retval = http.responseText;
    if (retval == '') return;
    var a = new Array(); a = retval.split('||');
    if (a[0] < 1) {
        alert(a[1]); return;
    }
    // xử lý dữ liệu
    document.body.style.cursor = 'wait'; // con tro cho
    if (retval.substring(0, 10) == '#DSJSADDON') {
        // eval js
        var jssepr = retval.indexOf('<!--JS-->');
        eval(retval.substring(jssepr + 9, retval.length));
        document.body.style.cursor = 'auto'; 
        return;
    }
    else if (retval.substring(0, 11) == '#DSJSAPPEND') {
        // add js
        var jssepr = retval.indexOf('<!--JS-->');
        var newScript = document.createElement('script');
        newScript.text = retval.substring(jssepr + 9, retval.length);
        document.getElementsByTagName('head').item(0).appendChild(newScript);
        document.body.style.cursor = 'auto'; 
        return;
    }
    //convert Json JSON.parse -  xử lý data trực tiếp
    //Duyệt Json Fill to form edit With coloumn name
    document.body.style.cursor = 'auto'; // remove wait
}

function showhelp(C, stage) {
    var helpregion = document.getElementById('helpdisp');
    if (helpregion.style.display == 'none') {
        ajPage('BasetabText.aspx?id=' + C + '&stage=' + stage, showhelpret);
    }
    else {
        helpregion.style.display = 'none';
    }
}

function showhelpret(http) {
    var helpregion = document.getElementById('helpdisp');
    helpregion.style.display = 'inline';
    helpregion.innerHTML = http.responseText;
}

function setVal(val, frm, a3, c) {
    var kt = false; //while (c < DataVal.length && !kt) { if (DataVal[c] == val) kt = true; c++;}
    var a30 = new Array(); a30 = DataVal[(c - 1)].split('||');
    var a31 = new Array(); a31 = DataParent[(c - 1)].split('||');
    var a32 = new Array();
    var a33 = new Array();
    
    var c1 = 0;
    while (c1 < a30.length && !kt) {
        if (a30[c1] == val) kt = true;
        c1++;
    }
    a32 = a3.split('||');
    if (kt) {
        c1--;
        a33 = a31[c1].split(':');
        for (var i = 0; i < a32.length; i++)
            if (frm.elements[a32[i]]) frm.elements[a32[i]].value = a33[i];//if (frm.elements[a32[i]]) 
    } else {
        for (var i = 0; i < a32.length; i++)
            if (frm.elements[a32[i]]) frm.elements[a32[i]].value = "";//if (frm.elements[a32[i]]) 
    }
    /*
    if (kt) {
        c1--;
        a33 = a31[c1].split(':');
        for (var i = 0; i < a32.length; i++) {
            if (frm.elements[a32[i]]) {
                var input = frm.elements[a32[i]];
                if (input.type == "checkbox" || input.type == "radio") {
                    //if (a33[i] == input.value) input.checked = true;
                    input.checked = true;
                } else if (input.type == "select-one") {
                    for (var j = 0; j < input.options.length; j++) {
                        if (input.options[j].value == a33[i]) input.options[j].selected = true;
                    }
                } else {
                    input.value = a33[i];
                }
            }
        }
    }
    else
    {
        for (var i = 0; i < a32.length; i++) {
            if (frm.elements[a32[i]]) {
                var input = frm.elements[a32[i]];
                if (input.type == "checkbox" || input.type == "radio") {
                    //if (a33[i] == input.value) input.checked = true;
                    input.checked = true;
                } else if (input.type == "select-one") {
                    for (var j = 0; j < input.options.length; j++) {
                        if (input.options[j].value == a33[i]) input.options[j].selected = true;
                    }
                } else {
                    input.value = a33[i];
                }
            }
        }
    }
    */
}

function CalulatorInputForm(frm, col, formulas) {
    var a = new Array(); a = col.split('||');
    formulas = formulas.replace(a[0] + "=", "document." + frm.name + ".elements['" + a[0] + "'].value=");
    for (var i = 1; i < a.length; i++) {
        formulas = formulas.replace(a[i], "ParseDouble(document." + frm.name + ".elements['" + a[i] + "'].value)");
    }
    return eval(formulas);
}

function ParseDouble(s) {
    s = ReplaceAll(s, NumberSepr, '');
    if (s == "") { s = '0'; }
    return parseFloat(s);
}

function HiddenTableGroup(i, j, hidden) {
    var txt = "editgrp_" + i + "_" + j;
    var IdTable = document.getElementById(txt);
    IdTable.style.display = (hidden?"none":"block");
}

function HiddenTableGrp(i, j) {
    var txt = "editgrp_" + i + "_" + j;
    var Spantxt = "SpanId_" + i + "_" + j;
    var IdTable = document.getElementById(txt);
    var IdSpan = document.getElementById(Spantxt);
    if (IdTable.style.display == "block") {
        IdTable.style.display = "none";
        IdSpan.innerHTML = "Hiển thị";
    } else {
        IdTable.style.display = "block";
        IdSpan.innerHTML = "Ẩn";
    }
}

function HiddenTableGrpOneVal(i) {
    var txt = "editgrp_" + i;
    var Spantxt = "SpanId_" + i;
    var IdTable = document.getElementById(txt);
    var IdSpan = document.getElementById(Spantxt);
    if (IdTable.style.display == "block") {
        IdTable.style.display = "none";
        IdSpan.innerHTML = "Hiển thị";
    } else {
        IdTable.style.display = "block";
        IdSpan.innerHTML = "Ẩn";
    }
}

// Thang NV - Load khoa dao tao
function LoadCourses(f) {
    // f==this.form; s==StoreName-Server; p==ParamStore-Server; url==''->Index; a==`ColumnNameIn`; b==`ColumnNameOut`
    var s, p, url, a, ta, b, tb; // khai báo param tĩnh
    url = "/Utils";
    s = "SP_CMS__TR_ChangePlan"; // Store name
    p = "OrgID|0|1|8|0||Year|8|1|4|0||IS_Irregularly|16|1|2|0||TR_PlanID|12|3|8000|"; // Param Execute (Param input)

    a = "OrgID||Year||IS_Irregularly";
    ta = "";

    b = "TR_PlanID"; // ParamOut
    tb = "";//ParamOutType  

    GetURLParam(f, s, p, url, a, ta, b, tb);
}

// Lampt
function ValadateContractDateSelectBox(a,b,c) {
    ValidateContractDate(b);
}

function ValidateContractDate(f) {
    // f==this.form; s==StoreName-Server; p==ParamStore-Server; url==''->Index; a==`ColumnNameIn`; b==`ColumnNameOut`
    var s, p, url, a, ta, b, tb; // khai báo param tĩnh
    s = "SP_CMS__HU_Contract_ValidateContractDate";
    p = "ContractTypeID|0|1|8|0||EffectDate|4|1|8|01/01/1900||ExpireDate|22|3|50| ";
    ta = "::::*Date:01/01/1900:::";
    url = "/Utils";
    a = "ContractTypeID||EffectDate";
    b = "ExpireDate";
    tb = "";//Date||Date
    GetURLParam(f, s, p, url, a, ta, b, tb);
}

function GetURLParamChangeDateToMathCLT(f) {// f==this.form; s==StoreName-Server; p==ParamStore-Server; url==''->Index; a==`ColumnNameIn`; b==`ColumnNameOut`
    var s, p, url, a, ta, b, tb; // khai báo param tĩnh
    s = "SP_CMS__HU_Employee_MathMoneyInsChangeCLT"; // tên store
    p = "Creator|0|1|8|0||EmployeeID|0|1|8|0||CLTFromDate|4|1|8|01/01/1900||CLTToDate|4|1|8|01/01/1900||OldSalary|6|1|8|0||NewSalary|6|1|8|0||CLTXH|6|3|8|0||CLTYT|6|3|8|0||CLTTN|6|3|8|0";// param thực thi store
    ta = "::::*::::*Date::::*Date::::";// format
    url = "/Utils"; // đường dẫn thực thi store
    a = "OldSalary||NewSalary||EmployeeID||CLTFromDate||CLTToDate";// Tên param store input
    b = "CLTXH||CLTYT||CLTTN";// Ten param out put
    tb = "Numeric||Numeric||Numeric||Numeric";//Date||Date
    GetURLParam(f, s, p, url, a, ta, b, tb);
}

function GetURLParamChangeDateToMathRep(f) {// f==this.form; s==StoreName-Server; p==ParamStore-Server; url==''->Index; a==`ColumnNameIn`; b==`ColumnNameOut`
    var s, p, url, a, ta, b, tb; // khai báo param tĩnh
    s = "SP_CMS__HU_Employee_MathMoneyInsChangeREP";
    p = "Creator|0|1|8|0||EmployeeID|0|1|8|0||REPFromDate|4|1|8|01/01/1900||REPToMDate|4|1|8|01/01/1900||OldSalary|6|1|8|0||NewSalary|6|1|8|0||REPXH|6|3|8|0||REPYT|6|3|8|0||REPTN|6|3|8|0";
    ta = "::::*::::*Date::::*Date::::";
    url = "/Utils";
    a = "OldSalary||NewSalary||EmployeeID||REPFromDate||REPToMDate";
    b = "REPXH||REPYT||REPTN";
    tb = "Numeric||Numeric||Numeric||Numeric";//Date||Date
    GetURLParam(f, s, p, url, a, ta, b, tb);
}

// Huydt
function setValAndCalculateSelectbox(val, frm, col, formulas, a3, c) {
    var kt = false; //while (c < DataVal.length && !kt) { if (DataVal[c] == val) kt = true; c++;}
    var a30 = new Array(); a30 = DataVal[(c - 1)].split('||');
    var a31 = new Array(); a31 = DataParent[(c - 1)].split('||');
    var a32 = new Array();
    var a33 = new Array();

    var c1 = 0;
    while (c1 < a30.length && !kt) {
        if (a30[c1] == val) kt = true;
        c1++;
    }
    a32 = a3.split('||');
    if (kt) {
        c1--;
        a33 = a31[c1].split(':');
        for (var i = 0; i < a32.length; i++)
            if (frm.elements[a32[i]]) {
                frm.elements[a32[i]].value = a33[i];//if (frm.elements[a32[i]]) 
                frm.elements[a32[i]].onchange();
            } 
    } else {
        for (var i = 0; i < a32.length; i++)
            if (frm.elements[a32[i]]) frm.elements[a32[i]].value = "";//if (frm.elements[a32[i]]) 
    }
    /*
    if (kt) {
        c1--;
        a33 = a31[c1].split(':');
        for (var i = 0; i < a32.length; i++) {
            if (frm.elements[a32[i]]) {
                var input = frm.elements[a32[i]];
                if (input.type == "checkbox" || input.type == "radio") {
                    //if (a33[i] == input.value) input.checked = true;
                    input.checked = true;
                } else if (input.type == "select-one") {
                    for (var j = 0; j < input.options.length; j++) {
                        if (input.options[j].value == a33[i]) input.options[j].selected = true;
                    }
                } else {
                    input.value = a33[i];
                }
            }
        }
    }
    else
    {
        for (var i = 0; i < a32.length; i++) {
            if (frm.elements[a32[i]]) {
                var input = frm.elements[a32[i]];
                if (input.type == "checkbox" || input.type == "radio") {
                    //if (a33[i] == input.value) input.checked = true;
                    input.checked = true;
                } else if (input.type == "select-one") {
                    for (var j = 0; j < input.options.length; j++) {
                        if (input.options[j].value == a33[i]) input.options[j].selected = true;
                    }
                } else {
                    input.value = a33[i];
                }
            }
        }
    }
    */
    CalulatorInputForm(frm, col, formulas);
}

function setValAndCalculateDiffDate(a, b, c) {
    var result = round(DateDiffByTime('m', a.value, b.value) / 60, 1);
    c.value = result;
}

function round(value, precision) {
    var multiplier = Math.pow(10, precision || 0);
    return Math.round(value * multiplier) / multiplier;
}

function setValSymbol(a, c) {
    //c.value = c.value + a.value;
    insertAtCursor(c, a.value);
    a.selectedIndex = -1;
}

function GetURLParamNew(f) {// f==this.form; s==StoreName-Server; p==ParamStore-Server; url==''->Index; a==`ColumnNameIn`; b==`ColumnNameOut`
    var s, p, url, a, ta, b, tb; // khai báo param tĩnh

    GetURLParam(f, s, p, url, a, b);
}

function GetURLParam(f, s, p, url, a, ta, b, tb) {
    // f==this.form; s==StoreName-Server; p==ParamStore-Server; url==''->Index;
    //a == `ColumnNameIn`; ta == `TypeColumn`; b == `ColumnNameOut`; tb == `TypeColumn`;
    if (url == "") url = "/Utils"; // Controller Index
    var a1 = a.split("||");
    var params = "FormName=" + f.name + "&StoreName=" + s + "&StoreParam=" + encodeURI(p) + "&ParamOut=" + encodeURI(b) + "&ParamTpyeOut=" + encodeURI(tb) + "&ParamTpyeIn=" + encodeURI(ta);
    for (var i = 0; i < a1.length; i++) {
        params = params + "&" + a1[i] + "=" + f.elements[a1[i]].value;
    }
    ajPage(url, ExecGetURLParam, params);
}

function ExecGetURLParam(http) {
    var retval = http.responseText;
    document.body.style.cursor = 'wait'; // con tro cho
    if (retval.substring(0, 10) == '#DSJSADDON') {
        // eval js
        var jssepr = retval.indexOf('<!--JS-->');
        eval(retval.substring(jssepr + 9, retval.length));
        document.body.style.cursor = 'auto';
        return;
    }
    else if (retval.substring(0, 11) == '#DSJSAPPEND') {
        // add js
        var jssepr = retval.indexOf('<!--JS-->');
        var newScript = document.createElement('script');
        newScript.text = retval.substring(jssepr + 9, retval.length);
        document.getElementsByTagName('head').item(0).appendChild(newScript);
        document.body.style.cursor = 'auto';
        return;
    }
    else {
        var a = new Array(); 
        a = retval.split("$");
        var f = document.forms[a[0]]; // FormName request từ client lên server trả về
        var b = a[1]; // ParamOut request từ client lên server trả về
        var c = a[2]; // ParamOutValue server trả về
        var b1 = b.split("||");
        var c1 = c.split("||");
        for (var i = 0; i < b1.length; i++) {
            var input = f.elements[b1[i]];
            if (input.type == "checkbox" || input.type == "radio") {
                if (c1[i] == input.value) input.checked = true;
            } else if (input.type == "select-one") {
                for (var j = 0; j < input.options.length; j++) {
                    if (input.options[j].value == c1[i]) input.options[j].selected = true;
                }
            } else {
                input.value = c1[i];
            }
        }
        document.body.style.cursor = 'auto';
    }
}

// Outsoure -- Nguồn https://stackoverflow.com/questions/7763327/how-to-calculate-date-difference-in-javascript
var dateDiff = {
    inSeconds: function (d1, d2) {
        var t2 = d2.getTime();
        var t1 = d1.getTime();
        return parseInt((t2 - t1) / 1000);
    },
    inMinutes: function (d1, d2) {
        var t2 = d2.getTime();
        var t1 = d1.getTime();
        return parseInt((t2 - t1) / (60 * 1000));
    },
    inHours: function (d1, d2) {
        var t2 = d2.getTime();
        var t1 = d1.getTime();
        return parseInt((t2 - t1) / (3600 * 1000));
    },
    inDays: function (d1, d2) {
        var t2 = d2.getTime();
        var t1 = d1.getTime();
        return parseInt((t2 - t1) / (24 * 3600 * 1000));
    },
    inWeeks: function (d1, d2) {
        var t2 = d2.getTime();
        var t1 = d1.getTime();
        return parseInt((t2 - t1) / (24 * 3600 * 1000 * 7));
    },
    inMonths: function (d1, d2) {
        var d1Y = d1.getFullYear();
        var d2Y = d2.getFullYear();
        var d1M = d1.getMonth();
        var d2M = d2.getMonth();
        return (d2M + 12 * d2Y) - (d1M + 12 * d1Y);
    },
    inYears: function (d1, d2) {
        return d2.getFullYear() - d1.getFullYear();
    }
}

function insertAtCursor(myField, myValue) {
    //IE support
    if (document.selection) {
        myField.focus();
        sel = document.selection.createRange();
        sel.text = myValue;
    }
    // Microsoft Edge
    else if (window.navigator.userAgent.indexOf("Edge") > -1) {
        var startPos = myField.selectionStart;
        var endPos = myField.selectionEnd;

        myField.value = myField.value.substring(0, startPos) + myValue
            + myField.value.substring(endPos, myField.value.length);

        var pos = startPos + myValue.length;
        myField.focus();
        myField.setSelectionRange(pos, pos);
        myField.selectionStart = myField.value.length;
    }
    //MOZILLA and others
    else if (myField.selectionStart || myField.selectionStart == '0') {
        var startPos = myField.selectionStart;
        var endPos = myField.selectionEnd;
        myField.value = myField.value.substring(0, startPos)
            + myValue
            + myField.value.substring(endPos, myField.value.length);
        myField.selectionStart = myField.value.length;
    } else {
        myField.value += myValue;
        myField.selectionStart = myField.value.length;
    }
}

function DateDiffByTime(s, d1, d2) {
    var d = new Date();
    var a1 = new Date(d.getMonth() + "/" + d.getDay() + "/" + d.getFullYear() + " " + d1 + ":00.000");
    var a2 = new Date(d.getMonth() + "/" + d.getDay() + "/" + d.getFullYear() + " " + d2 + ":00.000");
    
    switch (s) {
        case "H": return dateDiff.inHours(a1, a2); break;
        case "m": return dateDiff.inMinutes(a1, a2); break;
        case "s": return dateDiff.inSeconds(a1, a2); break;
        default: return dateDiff.inDays(a1, a2); break;
    }
}

function DateDiff(s, d1, d2) {
    var a = new Array(); 
    a = d1.split("/"); var a1 = new Date(a[1] + "/" + a[0] + "/" + a[2]);
    a = d2.split("/"); var a2 = new Date(a[1] + "/" + a[0] + "/" + a[2]);
    switch (s) {
        case "Y": return dateDiff.inYears(a1, a2); break;
        case "M": return dateDiff.inMonths(a1, a2); break;
        case "W": return dateDiff.inWeeks(a1, a2); break;
        default: return dateDiff.inDays(a1, a2); break;
    }
}

function DeleteRow(TblId, rowIndex) {
    //r.parentNode.parentNode.rowIndex;
    document.getElementById(TblId).deleteRow(rowIndex);
}

function DeleteAllRow(TblId) {
    document.getElementById(TblId).innerHTML = "";
}

function SetInnerHTML(TblId, HTML) {
    document.getElementById(TblId).innerHTML = HTML;
}

// Chiennt - tinh tien ky luat (Tính số tiền phạt); viết thêm tính tiền bồi thường
function CalFine(f, a, b, c) {
    // a. Tổng tiền
    // b. Chi tiết
    if (c == null) c = 1;
    var t = 0; var cnt = 0;
    try {
        t = ParseDouble(f.elements[a].value);
        if (f.elements[b].length)
            cnt = f.elements[b].length;
        else
            cnt = 1;
    }
    catch (ex) {
        t = 0; cnt = 0;
    }

    if (t < 1 || cnt < 1) return;

    if (c == 1) { // chia đều - tính từ tổng -> Chi tiết
        if (cnt > 1) {
            var v = Math.round(t / cnt);
            for (var i = 0; i < cnt; i++) {
                f.elements[b][i].value = insertSepr(v);
            }
        } else f.elements[b].value = insertSepr(t);
    } else { // tính từ chi tiết -> tổng
        var t1 = 0; 
        if (cnt > 1) {
            for (var i = 0; i < cnt; i++) {
                t1 = t1 + ParseDouble(f.elements[b][i].value);
            }
        } else t1 = ParseDouble(f.elements[b].value);
        if (t1 > 0 && t != t1) f.elements[a].value = insertSepr(t1);
    }
}

function SelectTree(a, c, chk) {
    //var a = $(b).parent();
    if (chk == null) chk = false;
    if (!chk) {
        var i = 0; var kt = false;
        while (i < a.options.length && !kt) {
            if (a.options[i].value == c) kt = true;
            i = i + 1;
        }
        if (!kt) return;
        chk = (a.options[i - 1].selected == true);
    }    
    for (var i = 0; i < a.options.length; i++) {
        if (a.options[i].getAttribute("data-pup") == c) {
            a.options[i].selected = chk;
            SelectTree(a, a.options[i].value, chk);
        }
    }
}
