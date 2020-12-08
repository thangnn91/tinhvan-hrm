using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Utils {
    public static class UIFormElements
    {
        public static void UIFillterFormRpt(ref StringBuilder bx, ref StringBuilder jsdbcols, ref bool IsFilterForm, ref string json,
            HRSContext context, ToolDAO DataDAO,
            string[] a1, string[] a2, string[] a3, string[] a4, string CharSplit, bool IsSearch = false)
        {
            //string onclick = "";
            bool IsOpenTag = false; string clsFullwidth = "";
            string[] DataVal = new string[a1.Length];
            string[] DataTxt = new string[a1.Length];
            string[] DataParent = new string[a1.Length];
            int ArrLength = a1.Length;
            if (ArrLength > a2.Length) ArrLength = a2.Length;
            if (ArrLength > a3.Length) ArrLength = a3.Length;
            StringBuilder b = new StringBuilder(); bool IsOpenBeginTag = false;
            //b.Append("<div class=\"formrow mg-top-minus-10\">");
            for (var i = 0; i < a1.Length; i++)
            {
                string[] a11 = a1[i].Split(new string[] { "|" }, StringSplitOptions.None);
                if (i >= ArrLength)
                {
                    if (IsSearch) json = json + ",{\"ParamName\":\"" + a11[0] + "\", \"ParamType\":\"" + a11[1] + "\", \"ParamInOut\":\"" + a11[2] + "\", \"ParamLength\":\"" + a11[3] + "\", \"InputValue\":\"" + a11[4] + "\"}";
                }
                else
                {
                    DataVal[i] = ""; DataTxt[i] = ""; DataParent[i] = "";
                    string[] a31 = a3[i].Split(new string[] { CharSplit }, StringSplitOptions.None); //";"
                    string v = context.GetRequestVal(a11[0]);
                    // fix PageSize
                    if (v == "" && a11[0] == "PageSize") v = context.GetSession("PageSizeBaseTab");
                    if (v == "") v = Tools.ParseValue(context, a31[1], true);

                    string val = v;
                    if (IsSearch)
                    {
                        json = json + ",{\"ParamName\":\"" + a11[0] + "\", \"ParamType\":\"" + a11[1] + "\", \"ParamInOut\":\"" + a11[2] + "\", \"ParamLength\":\"" + a11[3] + "\", \"InputValue\":\"" + val + "\"}";
                    }
                    if (a31[0] == "Hidden" || a11[0] == "Page") // Ẩn phân trang  || a11[0] == "PageSize" || a11[0] == "Rowcount"
                    {
                        b.Append(UIDef.UIHidden(a11[0], val));
                    }
                    else
                    {
                        bool ColSpan = false;
                        if (i < a4.Length)
                        {
                            ColSpan = (Tools.CIntNull(a4[i]) == 1);
                        }
                        clsFullwidth = "fullwidth";
                        if (ColSpan)
                            clsFullwidth = "";
                        else
                        {
                            if (i + 1 < a4.Length) if (Tools.CIntNull(a4[i + 1]) == 1) clsFullwidth = "";
                        }

                        if (!ColSpan)
                        {
                            //b.Append(Environment.NewLine + "<tr class=\"formrow\"><td align=right><label class=\"name\">" + context.GetLanguageLable(a2[i]) + ": </label><td class=\"controls\">");
                            b.Append(Environment.NewLine + (IsOpenTag ? "</div>" : "") + "<div class=\"col-s\"><div class=\"rowitem " + clsFullwidth + "\"><label class=\"lb\">" + context.GetLanguageLable(a2[i]) + ": </label><div class=\"value\">");
                            IsOpenTag = true;
                        }
                        else
                        {
                            //b.Append(Environment.NewLine + "<td align=right><label class=\"name\">" + context.GetLanguageLable(a2[i]) + ": </label><td class=\"controls\">");
                            b.Append(Environment.NewLine + "<div class=\"rowitem " + clsFullwidth + "\"><label class=\"lb\">" + context.GetLanguageLable(a2[i]) + ": </label><div class=\"value\">");
                        }
                        IsOpenBeginTag = true;
                        string ReqJson = ""; string OnChangePageSize = (a11[0] == "PageSize" ? " this.form.submit(); " : "");
                        switch (a31[0])
                        {
                            case "Textbox":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                b.Append(UITextbox(context.GetLanguageLable(a2[i]), a11, 0, val, a31, ""));
                                break;
                            case "Numeric":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                b.Append(UINumeric(context, context.GetLanguageLable(a2[i]), a11, a2, 0, val, a31));
                                break;
                            case "Date":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                b.Append(UIDate(context.GetLanguageLable(a2[i]), a11, 0, val, "FilterForm"));
                                break;
                            case "Datetime":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                b.Append(UIDateTime(context.GetLanguageLable(a2[i]), a11, 0, val, "FilterForm"));
                                break;
                            case "Checkbox":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                b.Append(UIDef.UICheckbox(a11[0], a31[3], a31[4], val, UIDef.FocusAfter(a11[0])));
                                break;
                            case "Radio":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                b.Append(UIFormElements.UIRadio(context, a1, i, val, a31));
                                break;
                            case "Actb":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                ReqJson = context.InputDataSetParam(a31[6]);
                                b.Append(UIDef.UIActbId(a11[0], ref DataDAO, a31[5], ReqJson, a31[7], val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a11[0]), "FilterForm"));
                                break;
                            case "ActbText":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                b.Append(UIDef.UIActbStrId(a11[0], context.ReplaceStringLangValue(a31[5]), a31[6], val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a11[0]), "FilterForm"));
                                break;
                            case "Treebox":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                ReqJson = context.InputDataSetParam(a31[4]);
                                b.Append(UIFormElements.UITreeview(a11[0], ref DataDAO, a31[3], ReqJson, a31[5], val, a31, i));
                                //b.Append(UIDef.UITreeview(context.GetLanguageLable(a2[i]), DataDAO, context, a11[0], a31[3], ReqJson, a31[5], a31[6], val, int.Parse(a31[7]), int.Parse(a31[8]), false, false, true, a31[9], a31[10]));
                                break;
                            case "DivMutilCheckbox":
                                ////if (a11[0] != "PageSize") IsFilterForm = true;
                                ////ReqJson = context.InputDataSetParam(a31[4]);
                                //////r1.Append(UIDef.UIMultipleSelect(a11[0], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a11[0]), ""));
                                ////onclick = ""; if (a31.Length > 8) onclick = " onclick=\"" + a31[8] + ",'" + a11[0] + "'," + i + ");\"";
                                ////b.Append(UIDef.UIDD(a11[0], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a11[0]), onclick, "", int.Parse(a31[6]), int.Parse(a31[7]), ref DataVal[i], ref DataParent[i], "document.FilterForm"));
                                ////break;
                            case "Selectbox":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                ReqJson = context.InputDataSetParam(a31[4]);
                                UIDef.OptionStringVal(ref DataDAO, a31[3], ReqJson, a31[5], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                                b.Append(UIFormElements.UISelectStr(a11[0], DataVal[i], DataTxt[i], a31, ref val, true, "", i));
                                ////if (a31.Length > 6)
                                ////    b.Append(UIDef.UISelectStr(a11[0], DataVal[i], DataTxt[i], ref val, true, "", a31[6] + "," + (i + 1) + ");" + UIDef.NextFocus(a11[0]), "document.FilterForm"));
                                ////else
                                ////    b.Append(UIDef.UISelectStr(a11[0], DataVal[i], DataTxt[i], ref val, true, "", UIDef.NextFocus(a11[0]), "document.FilterForm"));
                                //////b.Append(UIDef.UISelect(a11[0], ref DataDAO, a31[3], ReqJson, a31[5], val, true, "", UIDef.CNextFocus(a11[0])));
                                break;
                            case "SelectboxText":
                                if (a11[0] != "PageSize") IsFilterForm = true;
                                b.Append(UIFormElements.UISelectStr(a11[0], a31[3], context.ReplaceStringLangValue(a31[4]), a31, ref val, true, "", i, 6));
                                ////b.Append(UIDef.UISelectStr(a11[0], context.ReplaceStringLangValue(a31[3]), a31[4], ref val, true, "", OnChangePageSize + UIDef.NextFocus(a11[0]), "document.FilterForm"));
                                break;
                            
                        }
                        //if (!ColSpan)
                        //b.Append(Environment.NewLine + "<tr class=\"formrow\"><td align=right><label class=\"name\">" + context.GetLanguageLable(a2[i]) + ": </label><td class=\"controls\">");
                        //    b.Append(Environment.NewLine + "</div></div></div>");
                        //else
                        //b.Append(Environment.NewLine + "<td align=right><label class=\"name\">" + context.GetLanguageLable(a2[i]) + ": </label><td class=\"controls\">");
                        b.Append(Environment.NewLine + "</div></div>");
                    }
                    jsdbcols.Append("jsdbcols[" + i + "] = '" + a11[0] + "';" +
                        Environment.NewLine + "DataVal[" + i + "] = '" + DataVal[i] + "';" +
                        Environment.NewLine + "DataTxt[" + i + "] = '" + DataTxt[i] + "';" +
                        Environment.NewLine + "DataParent[" + i + "] = '" + DataParent[i] + "';");
                }
            }
            //b.Append("</div>");//</div> 
            if (IsOpenBeginTag) b.Append("</div>");

            if (IsFilterForm) bx.Append("<div class=\"formrow mg-top-minus-10\">");
            bx.Append(b.ToString());
            if (IsFilterForm) bx.Append("</div>");
        }
        public static void UIFillterForm(ref StringBuilder bx, ref StringBuilder jsdbcols, ref string json,
            HRSContext context, ToolDAO DataDAO,
            string[] a1, string[] a2, string[] a3, string[] a4, string[] lblButton, string[] StoreName)
        {
            //string onclick = "";
            bool IsOpenTag = false;
            bool IsFilterForm = false;//string clsFullwidth = ""; 
            string[] DataVal = new string[a1.Length];
            string[] DataTxt = new string[a1.Length];
            string[] DataParent = new string[a1.Length];
            int ArrLength = a1.Length;
            if (ArrLength > a2.Length) ArrLength = a2.Length;
            if (ArrLength > a3.Length) ArrLength = a3.Length;
            StringBuilder b = new StringBuilder(); bool IsOpenBeginTag = false;
            StringBuilder bKeyword = new StringBuilder(); 

            // form ẩn ko có trường Keyword
            for (int i = 0; i < a1.Length; i++)
            {
                string[] a11 = a1[i].Split(new string[] { "|" }, StringSplitOptions.None);
                string[] a31 = a3[i].Split(new string[] { ";" }, StringSplitOptions.None);
                string val = context.GetRequestVal(a11[0]);
                if (a31[0].ToLower() == "hidden" || a11[0].ToLower() == "page") // Ẩn phân trang  || a11[0] == "PageSize" || a11[0] == "Rowcount"
                {
                    b.Append(UIDef.UIHidden(a11[0], val));
                }
                else
                {
                    bool ColSpan = false;
                    if (i < a4.Length)
                    {
                        ColSpan = (Tools.CIntNull(a4[i]) == 1);
                    }

                    b.Append(Environment.NewLine + (IsOpenTag ? "</div>" : "") +
                        "<div class=\"form-group row\">" +
                        Environment.NewLine + "<label class=\"col-form-label active\">" + context.GetLanguageLable(a2[i]) + ": </label>");
                    IsOpenTag = true;
                    IsOpenBeginTag = true;
                    string ReqJson = ""; string OnChangePageSize = (a11[0] == "PageSize" ? " this.form.submit(); " : "");
                    switch (a31[0])
                    {
                        case "Textbox":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            b.Append(UITextbox(context.GetLanguageLable(a2[i]), a11, 0, val, a31, ""));
                            break;
                        case "Numeric":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            b.Append(UINumeric(context, context.GetLanguageLable(a2[i]), a11, a2, 0, val, a31));
                            break;
                        case "Date":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            b.Append(UIDate(context.GetLanguageLable(a2[i]), a11, 0, val, "FilterForm"));
                            break;
                        case "Datetime":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            b.Append(UIDateTime(context.GetLanguageLable(a2[i]), a11, 0, val, "FilterForm"));
                            break;
                        case "Checkbox":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            b.Append(UIDef.UICheckbox(a11[0], a31[3], a31[4], val, UIDef.FocusAfter(a11[0])));
                            break;
                        case "Radio":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            b.Append(UIFormElements.UIRadio(context, a1, i, val, a31));
                            break;
                        case "Actb":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            ReqJson = context.InputDataSetParam(a31[6]);
                            b.Append(UIDef.UIActbId(a11[0], ref DataDAO, a31[5], ReqJson, a31[7], val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a11[0]), "FilterForm"));
                            break;
                        case "ActbText":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            b.Append(UIDef.UIActbStrId(a11[0], context.ReplaceStringLangValue(a31[5]), a31[6], val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a11[0]), "FilterForm"));
                            break;
                        case "Treebox":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            ReqJson = context.InputDataSetParam(a31[4]);
                            b.Append(UIFormElements.UITreeview(a11[0], ref DataDAO, a31[3], ReqJson, a31[5], val, a31, i));
                            //b.Append(UIDef.UITreeview(context.GetLanguageLable(a2[i]), DataDAO, context, a11[0], a31[3], ReqJson, a31[5], a31[6], val, int.Parse(a31[7]), int.Parse(a31[8]), false, false, true, a31[9], a31[10]));
                            break;
                        case "DivMutilCheckbox":
                            ////////if (a11[0] != "PageSize") IsFilterForm = true;
                            ////ReqJson = context.InputDataSetParam(a31[4]);
                            ////onclick = ""; if (a31.Length > 8) onclick = " onclick=\"" + a31[8] + ",'" + a11[0] + "'," + i + ");\"";
                            ////b.Append(UIDef.UIDD(a11[0], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a11[0]), onclick, "", int.Parse(a31[6]), int.Parse(a31[7]), ref DataVal[i], ref DataParent[i], "document.FilterForm"));
                            ////break;
                        case "Selectbox":
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            ReqJson = context.InputDataSetParam(a31[4]);
                            UIDef.OptionStringVal(ref DataDAO, a31[3], ReqJson, a31[5], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                            b.Append(UIFormElements.UISelectStr(a11[0], DataVal[i], DataTxt[i], a31, ref val, true, "", i));
                            ////if (a31.Length > 6)
                            ////    b.Append(UIDef.UISelectStr(a11[0], DataVal[i], DataTxt[i], ref val, true, "", a31[6] + "," + (i + 1) + ");" + UIDef.NextFocus(a11[0]), "document.FilterForm"));
                            ////else
                            ////    b.Append(UIDef.UISelectStr(a11[0], DataVal[i], DataTxt[i], ref val, true, "", UIDef.NextFocus(a11[0]), "document.FilterForm"));
                            break;
                        case "SelectboxText":
                            b.Append(UIFormElements.UISelectStr(a11[0], a31[3], context.ReplaceStringLangValue(a31[4]), a31, ref val, true, "", i, 6));
                            if (a11[0] != "PageSize") IsFilterForm = true;
                            ////b.Append(UIDef.UISelectStr(a11[0], context.ReplaceStringLangValue(a31[3]), a31[4], ref val, true, "", OnChangePageSize + UIDef.NextFocus(a11[0]), "document.FilterForm"));
                            break;
                        
                    }
                }

                jsdbcols.Append("jsdbcols[" + i + "] = '" + a11[0] + "';" +
                    Environment.NewLine + "DataVal[" + i + "] = '" + DataVal[i] + "';" +
                    Environment.NewLine + "DataTxt[" + i + "] = '" + DataTxt[i] + "';" +
                    Environment.NewLine + "DataParent[" + i + "] = '" + DataParent[i] + "';");
            }

            if (IsOpenBeginTag) b.Append("</div>");

            bx.Append(Environment.NewLine + "<div class=\"row inline-input\">" +
                Environment.NewLine + "<div class=\"col-sm-6\">");
            if (IsFilterForm && !(StoreName.Length > 1))
            {
                bx.Append(Environment.NewLine + "<div class=\"form-group row\">");
                if (StoreName.Length > 1)
                {
                    for (int i = 1; i < StoreName.Length; i++)
                    {
                        string lblBnt = "DataAggregation";
                        try { lblBnt = lblButton[i]; } catch { lblBnt = "DataAggregation"; }
                        bx.Append(UIDef.UIButton("bntAction" + i, context.GetLanguageLable(lblBnt),
                            "var a=this.form.elements['Page'];if(a)a.value=1;" +
                            "var b=this.form.elements['PageSize'];if(b)b.value=" + context.GetSession("PageSizeReport") + ";" +
                            "btnSearch('" + i + "');", " class=\"btn btn-primary waves-effect waves-light\""));
                    }
                }
                bx.Append(Environment.NewLine + UIDef.UIButton("bntSearch", context.GetLanguageLable("Search"), "var a=this.form.elements['Page'];if(a)a.value=1;btnSearch('');", "class=\"btn btn-primary waves-effect waves-light\"") +
                    Environment.NewLine + UIDef.UIButton("bntSearchReset", context.GetLanguageLable("Reset"), "btnReset(this.form);", " class=\"btn btn-outline-grey waves-effect waves-light\"") +
                    Environment.NewLine + "</div>");
            }
            
            bx.Append(b.ToString() +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "</div>");

            bKeyword = null;
        }
        public static void UIFillterForm(ref StringBuilder bx, ref StringBuilder jsdbcols, ref string json,
            HRSContext context, ToolDAO DataDAO,
            string[] a1, string[] a2, string[] a3, string[] a4, string CharSplit, bool IsSearch = false)
        {
            //string onclick = "";
            bool IsOpenTag = false; //string clsFullwidth = ""; 
            string[] DataVal = new string[a1.Length];
            string[] DataTxt = new string[a1.Length];
            string[] DataParent = new string[a1.Length];
            int ArrLength = a1.Length;
            if (ArrLength > a2.Length) ArrLength = a2.Length;
            if (ArrLength > a3.Length) ArrLength = a3.Length;
            StringBuilder b = new StringBuilder(); bool IsOpenBeginTag = false;
            StringBuilder bKeyword = new StringBuilder(); bool IsKeyword = false;

            // form ẩn ko có trường Keyword
            for (var i = 0; i < a1.Length; i++)
            {
                string[] a11 = a1[i].Split(new string[] { "|" }, StringSplitOptions.None);
                if (i >= ArrLength)
                {
                    if (IsSearch) json = json + ",{\"ParamName\":\"" + a11[0] + "\", \"ParamType\":\"" + a11[1] + "\", \"ParamInOut\":\"" + a11[2] + "\", \"ParamLength\":\"" + a11[3] + "\", \"InputValue\":\"" + a11[4] + "\"}";
                }
                else 
                {
                    DataVal[i] = ""; DataTxt[i] = ""; DataParent[i] = "";
                    string[] a31 = a3[i].Split(new string[] { CharSplit }, StringSplitOptions.None); //";"
                    string v = context.GetRequestVal(a11[0]);
                    // fix PageSize
                    if (v == "" && a11[0] == "PageSize") v = context.GetSession("PageSizeBaseTab");
                    if (v == "") v = Tools.ParseValue(context, a31[1], true);

                    string val = v;
                    if (IsSearch)
                    {
                        json = json + ",{\"ParamName\":\"" + a11[0] + "\", \"ParamType\":\"" + a11[1] + "\", \"ParamInOut\":\"" + a11[2] + "\", \"ParamLength\":\"" + a11[3] + "\", \"InputValue\":\"" + val + "\"}";
                    }
                    if (a11[0].ToLower() != "keyword")
                    {
                        if (a31[0].ToLower() == "hidden" || a11[0].ToLower() == "page") // Ẩn phân trang  || a11[0] == "PageSize" || a11[0] == "Rowcount"
                        {
                            b.Append(UIDef.UIHidden(a11[0], val));
                        }
                        else
                        {
                            bool ColSpan = false;
                            if (i < a4.Length)
                            {
                                ColSpan = (Tools.CIntNull(a4[i]) == 1);
                            }
                            ////clsFullwidth = "fullwidth";
                            ////if (ColSpan)
                            ////    clsFullwidth = "";
                            ////else
                            ////{
                            ////    if (i + 1 < a4.Length) if (Tools.CIntNull(a4[i + 1]) == 1) clsFullwidth = "";
                            ////}

                            ////if (!ColSpan)
                            ////{
                            ////    b.Append(Environment.NewLine + (IsOpenTag ? "</div>" : "") + 
                            ////        "<div class=\"form-group row\">" +
                            ////        Environment.NewLine + "<label class=\"col-form-label active\">" + context.GetLanguageLable(a2[i]) + ": </label>");
                            ////    IsOpenTag = true;
                            ////}
                            ////else
                            ////{
                            ////    b.Append(Environment.NewLine + "<div class=\"rowitem " + clsFullwidth + "\"><label class=\"lb\">" + context.GetLanguageLable(a2[i]) + ": </label><div class=\"value\">");
                            ////}
                            b.Append(Environment.NewLine + (IsOpenTag ? "</div>" : "") +
                                "<div class=\"form-group row\">" +
                                Environment.NewLine + "<label class=\"col-form-label active\">" + context.GetLanguageLable(a2[i]) + ": </label>");
                            IsOpenTag = true;
                            IsOpenBeginTag = true;
                            string ReqJson = ""; string OnChangePageSize = (a11[0] == "PageSize" ? " this.form.submit(); " : "");
                            switch (a31[0])
                            {
                                case "Textbox":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    b.Append(UITextbox(context.GetLanguageLable(a2[i]), a11, 0, val, a31, ""));
                                    break;
                                case "Numeric":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    b.Append(UINumeric(context, context.GetLanguageLable(a2[i]), a11, a2, 0, val, a31));
                                    break;
                                case "Date":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    b.Append(UIDate(context.GetLanguageLable(a2[i]), a11, 0, val, "FilterForm"));
                                    break;
                                case "Datetime":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    b.Append(UIDateTime(context.GetLanguageLable(a2[i]), a11, 0, val, "FilterForm"));
                                    break;
                                case "Checkbox":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    b.Append(UIDef.UICheckbox(a11[0], a31[3], a31[4], val, UIDef.FocusAfter(a11[0])));
                                    break;
                                case "Radio":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    b.Append(UIFormElements.UIRadio(context, a1, i, val, a31));
                                    break;
                                case "Actb":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    ReqJson = context.InputDataSetParam(a31[6]);
                                    b.Append(UIDef.UIActbId(a11[0], ref DataDAO, a31[5], ReqJson, a31[7], val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a11[0]), "FilterForm"));
                                    break;
                                case "ActbText":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    b.Append(UIDef.UIActbStrId(a11[0], context.ReplaceStringLangValue(a31[5]), a31[6], val, " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a11[0]), "FilterForm"));
                                    break;
                                case "Treebox":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    ReqJson = context.InputDataSetParam(a31[4]);
                                    b.Append(UIFormElements.UITreeview(a11[0], ref DataDAO, a31[3], ReqJson, a31[5], val, a31, i));
                                    //b.Append(UIDef.UITreeview(context.GetLanguageLable(a2[i]), DataDAO, context, a11[0], a31[3], ReqJson, a31[5], a31[6], val, int.Parse(a31[7]), int.Parse(a31[8]), false, false, true, a31[9], a31[10]));
                                    break;
                                case "DivMutilCheckbox":
                                    ////////if (a11[0] != "PageSize") IsFilterForm = true;
                                    ////ReqJson = context.InputDataSetParam(a31[4]);
                                    ////onclick = ""; if (a31.Length > 8) onclick = " onclick=\"" + a31[8] + ",'" + a11[0] + "'," + i + ");\"";
                                    ////b.Append(UIDef.UIDD(a11[0], context, ref DataDAO, a31[3], ReqJson, a31[5], val, UIDef.FocusAfter(a11[0]), onclick, "", int.Parse(a31[6]), int.Parse(a31[7]), ref DataVal[i], ref DataParent[i], "document.FilterForm"));
                                    ////break;
                                case "Selectbox":
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    ReqJson = context.InputDataSetParam(a31[4]);
                                    UIDef.OptionStringVal(ref DataDAO, a31[3], ReqJson, a31[5], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                                    b.Append(UIFormElements.UISelectStr(a11[0], DataVal[i], DataTxt[i], a31, ref val, true, "", i));
                                    ////if (a31.Length > 6)
                                    ////    b.Append(UIDef.UISelectStr(a11[0], DataVal[i], DataTxt[i], ref val, true, "", a31[6] + "," + (i + 1) + ");" + UIDef.NextFocus(a11[0]), "document.FilterForm"));
                                    ////else
                                    ////    b.Append(UIDef.UISelectStr(a11[0], DataVal[i], DataTxt[i], ref val, true, "", UIDef.NextFocus(a11[0]), "document.FilterForm"));
                                    break;
                                case "SelectboxText":
                                    b.Append(UIFormElements.UISelectStr(a11[0], a31[3], context.ReplaceStringLangValue(a31[4]), a31, ref val, true, "", i, 6));
                                    ////if (a11[0] != "PageSize") IsFilterForm = true;
                                    ////b.Append(UIDef.UISelectStr(a11[0], context.ReplaceStringLangValue(a31[3]), a31[4], ref val, true, "", OnChangePageSize + UIDef.NextFocus(a11[0]), "document.FilterForm"));
                                    break;
                                
                            }
                        }
                    }
                    else // Param Keyword
                    {
                        IsKeyword = true;
                        bKeyword.Append("<a href=\"#\" class=\"search-group-addon\">" +
                        Environment.NewLine + "<i class=\"fa fa-search active\" aria-hidden=\"true\"></i>" +
                        Environment.NewLine + "</a>" +
                        Environment.NewLine + UITextbox(context.GetLanguageLable(a2[i]), a11, 0, val, a31, " onkeyPress=\"if(isEnterOnly(event)){var a=this.form.elements['Page'];if(a)a.value=1;btnSearch('');}\" ") + 
                        Environment.NewLine + "<a href=\"#\" class=\"search-group-addon angle\" data-toggle=\"dropdown\">" +
                        Environment.NewLine + "<i class=\"fa fa-angle-down\" aria-hidden=\"true\"></i>" +
                        Environment.NewLine + "</a>");
                    }
                    
                    jsdbcols.Append("jsdbcols[" + i + "] = '" + a11[0] + "';" +
                        Environment.NewLine + "DataVal[" + i + "] = '" + DataVal[i] + "';" +
                        Environment.NewLine + "DataTxt[" + i + "] = '" + DataTxt[i] + "';" +
                        Environment.NewLine + "DataParent[" + i + "] = '" + DataParent[i] + "';");
                }
            }

            if (IsOpenBeginTag) b.Append("</div>");

            bx.Append(Environment.NewLine + "<div class=\"row inline-input\">" +
                Environment.NewLine + "<div class=\"col-sm-6\">" +
                Environment.NewLine + "<div class=\"form-group row\">");
            if (IsKeyword)
            {
                bx.Append("<div class=\"search-dropdown\">" +
                    Environment.NewLine + "<div class=\"input-group dropdown form-inline\">");
                bx.Append(bKeyword.ToString());
            }
            bx.Append("<div class=\"dropdown-menu dropdown-primary\">" +
                Environment.NewLine + "<div class=\"row inline-input\">" +
                Environment.NewLine + "<div class=\"col-sm-12\">");
            bx.Append("<div class=\"row inline-input search-btn-dropdown\">" +
                Environment.NewLine + UIDef.UIButton("bntSearch", context.GetLanguageLable("Search"), "var a=this.form.elements['Page'];if(a)a.value=1;btnSearch('');", "class=\"btn btn-primary waves-effect waves-light\"") +
                Environment.NewLine + UIDef.UIButton("bntSearchReset", context.GetLanguageLable("Reset"), "btnReset(this.form);", " class=\"btn btn-outline-grey waves-effect waves-light\"") +
                Environment.NewLine + "</div>");
            bx.Append(b.ToString());
            bx.Append("</div></div></div>");
            if (IsKeyword) bx.Append("</div></div>");
            bx.Append("</div></div></div>");

            bKeyword = null;
        }
        public static string UISelectStr(string InputName, string ValStr, string TextStr, string[] a31, ref string InputCheck, bool NotFoundDisp, string Param, int i, int Cnt = 7)
        {
            if (a31.Length > Cnt)
                return UIDef.UISelectStr(InputName, ValStr, TextStr, ref InputCheck, NotFoundDisp, Param, (a31[(Cnt - 1)] != "" ? a31[(Cnt - 1)].Replace("`", "'") + "," + (i + 1) + ");" : "") + UIDef.NextFocus(InputName), (a31[Cnt] == "1"));
            else if (a31.Length > (Cnt-1))
                return UIDef.UISelectStr(InputName, ValStr, TextStr, ref InputCheck, NotFoundDisp, Param, (a31[(Cnt - 1)] != "" ? a31[(Cnt - 1)].Replace("`", "'") + "," + (i + 1) + ");" : "") + UIDef.NextFocus(InputName), "document.ActionForm");
            else
                return UIDef.UISelectStr(InputName, ValStr, TextStr, ref InputCheck, NotFoundDisp, Param, UIDef.NextFocus(InputName), "document.ActionForm");
        }
        public static string UITreeview(string InputName, ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, string InputValue, string[] a31, int i, int Cnt = 7)
        {
            if (a31.Length > Cnt)
                return UIDef.UITreeview(InputName, ref toolDAO, ProcName, ProcParam, ColumnName, InputValue, (a31[(Cnt - 1)] != "" ? a31[(Cnt - 1)].Replace("`", "'") + "," + (i + 1) + ");" : "") + UIDef.NextFocus(InputName), (a31[Cnt] == "1"));
            else if (a31.Length > (Cnt - 1))
                return UIDef.UITreeview(InputName, ref toolDAO, ProcName, ProcParam, ColumnName, InputValue, (a31[(Cnt - 1)] != "" ? a31[(Cnt - 1)].Replace("`", "'") + "," + (i + 1) + ");" : "") + UIDef.NextFocus(InputName));
            else
                return UIDef.UITreeview(InputName, ref toolDAO, ProcName, ProcParam, ColumnName, InputValue, UIDef.NextFocus(InputName));
        }
        public static string UIRadio(HRSContext context, string[] a1, int i, string val, string[] a31, string ClassReadonly = "")
        {
            string r = "";
            if (a31.Length > 5)
                r = UIDef.UIRadio(a1[i], context.ReplaceStringLangValue(a31[3]), a31[4], val, ClassReadonly + UIDef.FocusAfter(a1[i]), a31[5]);
            else
                r = UIDef.UIRadio(a1[i], context.ReplaceStringLangValue(a31[3]), a31[4], val, ClassReadonly + UIDef.FocusAfter(a1[i]));
            return r;
        }
        public static string UITextbox (string placeholder, string[] a1, int i, string val, string[] a31, string ClassReadonly = "") // UIFormElements.UITextbox
        {
            string r = "";
            if (a31.Length > 5)
                r = UIDef.UITextbox(placeholder, a1[i], val, ClassReadonly + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\" ", UIDef.FocusAfter(a1[i]), a31[5]);
            else
                r = UIDef.UITextbox(placeholder, a1[i], val, ClassReadonly + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", UIDef.FocusAfter(a1[i]));
            return r;
        }
        public static string UINumeric(HRSContext context, string placeholder, string[] a1, string[] a2, int i, string val, string[] a31, string ClassReadonly = "") // UIFormElements.UINumeric
        {
            string r = ""; int min; int max;
            try
            {
                min = int.Parse(a31[6]); max = int.Parse(a31[7]);
            }
            catch
            {
                min = 0; max = 0;
            }
            if (a31.Length > 5)
                r = UIDef.UINumeric(placeholder, a1[i], val, ClassReadonly + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", "onkeypress =\"if(ParseDouble(this.value)<" + min + ")JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Column") + " [" + context.GetLanguageLable(a2[i]) + "] " + context.GetLanguageLable("MIN") + ": " + min + "', '', '0'); else if(ParseDouble(this.value)>" + max + ")JsAlert('alert-error', '" + context.GetLanguageLable("Alert-Error") + "', '" + context.GetLanguageLable("Column") + " [" + context.GetLanguageLable(a2[i]) + "] " + context.GetLanguageLable("MAX") + ": " + max + "', '', '0');  else cnextfocus(event, this.form, '" + a1[i] + "');\"", a31[5]);
            else
                r = UIDef.UINumeric(placeholder, a1[i], val, ClassReadonly + " autocomplete=\"off\" size=\"" + a31[3] + "\" maxlength=\"" + a31[4] + "\"", UIDef.FocusAfter(a1[i]), "");
            return r;
        }
        public static string UIDate(string placeholder, string[] a1, int i, string val, string frm = "FilterForm") // UIFormElements.UIDate
        {
            string r = "";
            DateTime dNow = DateTime.Now; int iDay = 0;
            if (val == "@")
            {
                val = string.Format("{0:dd/MM/yyyy}", dNow);
            }
            else if (val.Length > 2 && val.Substring(0, 1) == "@")
            {
                if (val.Substring(0, 2) == "@+") iDay = int.Parse(Tools.Right(val, 2, false));
                if (val.Substring(0, 2) == "@-") iDay = 0 - int.Parse(Tools.Right(val, 2, false));
                val = string.Format("{0:dd/MM/yyyy}", dNow.AddDays(iDay));
            }
            r = UIDef.UIDate(placeholder, a1[i], val, " autocomplete=\"off\" ", UIDef.CNextFocusAction(a1[i]), frm);
            return r;
        }
        public static string UIDateTime(string placeholder, string[] a1, int i, string val, string frm = "FilterForm") // UIFormElements.UIDateTime
        {
            string r = "";
            DateTime dNow1 = DateTime.Now; int iDay1 = 0;
            if (val == "@")
            {
                val = string.Format("{0:dd/MM/yyyy HH:mm}", dNow1);
            }
            else if (val.Length > 2 && val.Substring(0, 1) == "@")
            {
                if (val.Substring(0, 2) == "@+") iDay1 = int.Parse(Tools.Right(val, 2, false));
                if (val.Substring(0, 2) == "@-") iDay1 = 0 - int.Parse(Tools.Right(val, 2, false));
                val = string.Format("{0:dd/MM/yyyy HH:mm}", dNow1.AddDays(iDay1));
            }
            r = UIDef.UIDateTime(placeholder, a1[i], val, " autocomplete=\"off\" ", UIDef.CNextFocusAction(a1[i]), frm);
            return r;
        }
        public static string UISearchBox(string placeholder, string[] a1, int j, string val, string[]a3, string[] a31, string ClassReadonly, 
            ref StringBuilder jsdbcols, HRSContext context, dynamic EditData, bool jsTag = true, string frm = "bosfrm", bool IsInsert = true, bool IsUpdate = true, bool IsDelete = true)
        {
            StringBuilder r1 = new StringBuilder(); string r = ""; string v = ""; //if (ClassReadonly != "") ClassReadonly = "disabled";
            r1.Append(UIDef.UIHidden(a1[j], val));
            try
            {
                if (a1[j].Substring(a1[j].Length - 2) == "ID")
                    v = a1[j].Substring(0, a1[j].Length - 2);
                else
                    v = a1[j];
                v = v + "Name";
                v = Tools.GetDataJson(EditData.EditForm.Items[0], v);
            }
            catch { v = ""; }
            jsdbcols.Append("jsdbcols[" + j + "] = 'txt_" + a1[j] + "';");

            r1.Append(Environment.NewLine + "<div class=\"input-group\">");
            r1.Append(UIDef.UITextbox(placeholder, "txt_" + a1[j], v, " autocomplete=\"off\" class=\"form-control\" onkeyPress=\"if(isEnterOnly(event)) " + ClassReadonly + " ", ""));
            r1.Append(Environment.NewLine + "<span class=\"input-group-addon\" data-toggle=\"modal\">" +
                Environment.NewLine + "<a id=\"bnt_" + a1[j] + "\" href=\"javascript:" + ClassReadonly + "><i class=\"fa fa-window-restore\" aria-hidden=\"true\"></i></a>" +
                Environment.NewLine + "</span>");
            r1.Append(Environment.NewLine + "</div>");            
            //r1.Append(UIDef.UIButton("Bnt" + a1[j], "", "Search_" + a1[j] + "();", " class=\"iconsearch\" "));//<br>

            string UrlSearch; string[] ImageColumn; string jsImage = ""; string jsColAdd = "";
            string[] aParam = null; if (a31.Length > 5) aParam = a31[5].Split(new string[] { "||" }, StringSplitOptions.None); string sParam = "";
            string[] aParamType = null; if (a31.Length > 7) aParamType = a31[7].Split(new string[] { "*" }, StringSplitOptions.None);
            for (int i = 0; i < aParam.Length; i++)
            {
                string[] a11 = aParam[i].Split(new string[] { "|" }, StringSplitOptions.None);
                //a11[0] != "Page" && a11[0] != "PageSize" && a11[0] != "Rowcount" && a11[0] != "Keyword" && aParamType[i].IndexOf("SESSION") < 0
                if (i < aParamType.Length)
                if (aParamType[i].IndexOf("REQUEST") > 0)
                {
                    sParam = sParam + "'&" + a11[0] + "=' + (f.elements['" + a11[0] + "']?(f.elements['" + a11[0] + "'].value?f.elements['" + a11[0] + "'].value:''):'')";
                }
            }
            if (a31.Length > 11)
            {
                jsColAdd = a31[11];
            }
            if (a31.Length > 12)
                UrlSearch = a31[12];
            else
                UrlSearch = "/Utils/Search";
            if (a31.Length > 13)
            {
                ImageColumn = a31[13].Split(new string[] { "||" }, StringSplitOptions.None);
                if (ImageColumn.Length > 2)
                {
                    jsImage = ImageColumn[2];
                    r1.Append("<img id=\"Img" + a1[j] + "\" name=\"Img" + a1[j] + "\" src=\"/Media/RenderFile?TabIndex=" + ImageColumn[0] + "&ImageColumnName=" + ImageColumn[2] + "&NoCrop=\" width=" + ImageColumn[1] + ">");
                }
                else
                {
                    jsImage = "Img";
                    r1.Append("<img id=\"Img" + a1[j] + "\" name=\"Img" + a1[j] + "\" src=\"/Media/RenderFile?TabIndex=" + ImageColumn[0] + "&ImageColumnName=Img&NoCrop=\" width=" + ImageColumn[1] + ">");
                }
            }

            if (IsInsert || IsUpdate || IsDelete)
            {
                string iframe = context.GetRequestVal("iframe");
                string[] ParamArr = a31[6].Split(new string[] { "||" }, StringSplitOptions.None);//' + f.elements['" + a1[j] + "'].value + ' Tools.UrlEncode
                r1.Append(Environment.NewLine + (jsTag? "<script language=\"javascript\">": "<!--TagJS-->") +
                   Environment.NewLine + "function Search_" + a1[j] + "() {");
                r1.Append(Environment.NewLine + "var f=document." + frm + ";" +
                    Environment.NewLine + "var sUrl='" + UrlSearch + "?SearchIndex=" + (Compress.Zip(a3[j])) +
                   "&InputName=" + a1[j] + /*"&ColID=" + ParamArr[0] + "&" + ParamArr[0] + "=-1*/"&jsColAdd=" + (Compress.Zip(jsColAdd)) +
                   "&jsImage=" + jsImage + "' " + (sParam != "" ? "+ " + sParam + " +" : " + ") + " '&Keyword=' + f.elements['txt_" + a1[j] + "'].value;" +
                   Environment.NewLine + "window.open(sUrl,'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=10, left=20, width=900, height=600'); " +
                   Environment.NewLine + "//OpenModelSearch('saving" + iframe + "', 'saveTranFrm" + iframe + "', sUrl);");
                ////r1.Append(Environment.NewLine + "var f=document." + frm + ";" +
                ////   Environment.NewLine + "_attw = window.open('" + UrlSearch + "?SearchIndex=" + (Compress.Zip(a3[j])) +
                ////   "&InputName=" + a1[j] + /*"&ColID=" + ParamArr[0] + "&" + ParamArr[0] + "=-1*/"&jsColAdd=" + (Compress.Zip(jsColAdd)) +
                ////   "&jsImage=" + jsImage + "' " + (sParam != ""? "+ " + sParam + " +":" + ") + " '&Keyword=' + f.elements['txt_" + a1[j] + "'].value, " +
                ////   "'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=10, left=20, width=900, height=600');");
                r1.Append(Environment.NewLine + "} " +
                   Environment.NewLine + "function SearchResult_" + a1[j] + "(id, name" + (jsImage != "" ? "," + jsImage : "") + ", jsColAdd, jsColAddVal) {" +
                   Environment.NewLine + "/*alert(jsColAdd);alert(jsColAddVal);*/" +
                   Environment.NewLine + "CloseModelSearch('saving" + iframe + "', 'saveTranFrm" + iframe + "');" +
                   Environment.NewLine + "var f=document." + frm + ";" +
                   Environment.NewLine + "f.elements['" + a1[j] + "'].value=id;" +
                   Environment.NewLine + "f.elements['txt_" + a1[j] + "'].value=name;" +
                   Environment.NewLine + "var a = new Array(); " +
                   Environment.NewLine + "var b = new Array(); " +
                   Environment.NewLine + "a = jsColAdd.split('||'); " +
                   Environment.NewLine + "b = jsColAddVal.split('||');" +
                   Environment.NewLine + "for(var i = 0; i<a.length; i++){" +
                   Environment.NewLine + "if(f.elements[a[i]]) {" +
                   Environment.NewLine + "var input = f.elements[a[i]];" +
                   Environment.NewLine + "if (input.type == \"checkbox\" || input.type == \"radio\") {" +
                   Environment.NewLine + "//input.checked = true;" +
                   Environment.NewLine + "if (b[i] == input.value) input.checked = true;" +
                   Environment.NewLine + "} else if (input.type == \"select-one\") {" +
                   Environment.NewLine + "for (var j = 0; j < input.options.length; j++) {" +
                   Environment.NewLine + "if (input.options[j].value == b[i]) input.options[j].selected = true;" +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "} else {" +
                   Environment.NewLine + "input.value = b[i];" +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "nextfocus(document." + frm + ",'txt_" + a1[j] + "');" +
                   Environment.NewLine + "}" + (jsTag ? "</s" + "cript>" : "<!--/TagJS-->"));
            }
            r = r1.ToString(); r1 = null;
            return r;
        }
        public static string UISearchBoxMulti(string placeholder, string[] a1, int j, string val, string[] a3, string[] a31, string ClassReadonly,
            ref StringBuilder jsdbcols, HRSContext context, dynamic EditData, bool jsTag = true, string frm = "bosfrm", bool IsInsert = true, bool IsUpdate = true, bool IsDelete = true)
        {
            StringBuilder r1 = new StringBuilder(); string r = ""; string sFuncName = ""; string sFunc = "";
            string v = ""; string jsColAdd = ""; string[] Col; string[] sParamType; string TableHtml; string UrlSearch; 
            //r1.Append(UIDef.UIHidden(a1[j], val));
            if (a31.Length > 11)
            {
                jsColAdd = a31[11];
            }
            if (a31.Length > 12)
            {
                if (a31[12] != "")
                    UrlSearch = a31[12];
                else
                    UrlSearch = "/Utils/SearchMulti";
            }
            else
                UrlSearch = "/Utils/SearchMulti";

            if (a31.Length > 13)
            {
                if (a31[13] != "") sFuncName = a31[13];
            }

            if (a31.Length > 14)
            {
                if (a31[14] != "") sFunc = a31[14];
            }

            if (sFuncName == "" || sFunc == "")
            {
                sFuncName = ""; sFunc = "";
            }

            Col = a31[9].Split(new string[] { "||" }, StringSplitOptions.None);
            sParamType = a31[10].Split(new string[] { "*" }, StringSplitOptions.None);
            TableHtml = "<thead class=\"thead-light\"><tr>";
            for (int i = 0; i < Col.Length; i++) {
                string[] ColType = sParamType[i].Split(new string[] { ";" }, StringSplitOptions.None);
                if (ColType[0] != "-") TableHtml = TableHtml + "<th>" + context.GetLanguageLable(Col[i]);
            } 
            TableHtml = TableHtml + "<th></tr></thead><tbody><tr>";
            if (val != "")
            {
                for (int i = 0; i < Col.Length; i++)
                {
                    string[] ColType = sParamType[i].Split(new string[] { ";" }, StringSplitOptions.None);
                    try { v = Tools.GetDataJson(EditData.EditForm.Items[0], Col[i]); } catch { v = ""; }
                    if (ColType[0] != "-") TableHtml = TableHtml + "<td>" + v;
                }
                TableHtml = TableHtml + "<td>" + /*UIDef.UIHidden(a1[j], val)*/UIDef.UICheckbox(a1[j], "", val, val, " disabled ") +
                    UIDef.UIHrefButton(context.GetLanguageLable("Delete"), (ClassReadonly != ""? "#": "DeleteRow('tbl_" + a1[j] + "Info', this.parentNode.parentNode.rowIndex);"), "fa fa-trash-o do");
                    //UIDef.UIButton("DeleteRow_" + a1[j], context.GetLanguageLable("Delete"), "DeleteRow('tbl_" + a1[j] + "Info', this.parentNode.parentNode.rowIndex);", ClassReadonly + " class=\"btn delete\"");
            }
            jsdbcols.Append("jsdbcols[" + j + "] = 'txt_" + a1[j] + "';");
            try
            {
                if (a1[j].Substring(a1[j].Length - 2) == "ID")
                    v = a1[j].Substring(0, a1[j].Length - 2);
                else
                    v = a1[j];
                v = v + "Name";
                v = Tools.GetDataJson(EditData.EditForm.Items[0], v);
            }
            catch { v = ""; }
            r1.Append(Environment.NewLine + "<div class=\"input-group\">" +
                UIDef.UITextbox(placeholder, "txt_" + a1[j], v, " autocomplete=\"off\" class=\"form-control\" onkeyPress=\"if(isEnterOnly(event)) " + ClassReadonly + " ", ""));
            if (IsInsert || IsUpdate || IsDelete) r1.Append(Environment.NewLine + "<span class=\"input-group-addon\" data-toggle=\"modal\">" +
                 Environment.NewLine + "<a id=\"bnt_" + a1[j] + "\" href=\"javascript:" + ClassReadonly + "><i class=\"fa fa-window-restore\" aria-hidden=\"true\"></i></a>" +
                 Environment.NewLine + "</span>" +
                Environment.NewLine + "</div>");

            if (sFuncName != "" && sFunc != "") r1.Append(UIDef.UIButton("Act" + a1[j], context.GetLanguageLable(sFuncName), sFunc, ClassReadonly + " class=\"btn inport\"") + "");//<br>
            r1.Append(Environment.NewLine + "<div id=\"box_" + a1[j] + "Info\">" +
                Environment.NewLine + "<table width=100% class=\"table treetinhvan table-hover use-icon\" id=\"tbl_" + a1[j] + "Info\">" + TableHtml);
            r1.Append(Environment.NewLine + "</tbody>" +
                Environment.NewLine + "</table>" +
                Environment.NewLine + "</div>");

            if (IsInsert || IsUpdate || IsDelete)
            {
                string[] ParamArr = a31[6].Split(new string[] { "||" }, StringSplitOptions.None);//' + f.elements['" + a1[j] + "'].value + ' Tools.UrlEncode
                r1.Append(Environment.NewLine + (jsTag ? "<script language=\"javascript\">" : "<!--TagJS-->") +
                   Environment.NewLine + "function Search_" + a1[j] + "() {var f=document." + frm + ";" +
                   Environment.NewLine + "_attw = window.open('" + UrlSearch + "?SearchIndex=" + (Compress.Zip(a3[j])) +
                   "&InputName=" + a1[j] + /*"&ColID=" + ParamArr[0] + "&" + ParamArr[0] + "=-1*/"&jsColAdd=" + (Compress.Zip(jsColAdd)) +
                   "&Keyword=' + (f.elements['txt_" + a1[j] + "'].value), " +
                   "'_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=10, left=20, width=900, height=600');" +
                   Environment.NewLine + "} " +
                   Environment.NewLine + "function SearchResult_" + a1[j] + "(TrHTML) {" +
                   Environment.NewLine + "DeleteAllRow(\"tbl_" + a1[j] + "Info\");" +
                   Environment.NewLine + "SetInnerHTML(\"tbl_" + a1[j] + "Info\", TrHTML);" +
                   Environment.NewLine + "nextfocus(document." + frm + ", 'txt_" + a1[j] + "'); " +
                   Environment.NewLine + "}" + (jsTag ? "</s" + "cript>" : "<!--/TagJS-->"));
            }
            r = r1.ToString(); r1 = null;
            return r;
        }
        public static string UISearchBoxCount(string placeholder, string[] a1, int j, string val, string[] a3, string[] a31, string ClassReadonly,
            ref StringBuilder jsdbcols, HRSContext context, dynamic EditData, bool jsTag = true, string frm = "bosfrm", bool IsInsert = true, bool IsUpdate = true, bool IsDelete = true)
        {
            StringBuilder r1 = new StringBuilder(); string r = "";

            r1.Append(Environment.NewLine + "<div class=\"input-group\">");
            r1.Append(UIDef.UITextbox(placeholder, a1[j], val, " autocomplete=\"off\" class=\"form-control\" onkeyPress=\"if(isEnterOnly(event)) " + ClassReadonly + "  ", ""));
            if (IsInsert || IsUpdate || IsDelete) r1.Append(Environment.NewLine + "<span class=\"input-group-addon\" data-toggle=\"modal\">" +
                 Environment.NewLine + "<a id=\"bnt_" + a1[j] + "\" href=\"javascript:" + ClassReadonly + "><i class=\"fa fa-window-restore\" aria-hidden=\"true\"></i></a>" +
                 Environment.NewLine + "</span>");
            r1.Append(Environment.NewLine + "</div>");
            //r1.Append("&nbsp;" + UIDef.UIButton("Bnt" + a1[j], context.GetLanguageLable("Search"), "Search_" + a1[j] + "();", " class=\"btn find\"") + "");//<br>
            string UrlSearch; string jsColAdd = "";
            string[] aParam = null; if (a31.Length > 5) aParam = a31[5].Split(new string[] { "||" }, StringSplitOptions.None); string sParam = "";
            string[] aParamType = null; if (a31.Length > 7) aParamType = a31[7].Split(new string[] { "*" }, StringSplitOptions.None);
            for (int i = 0; i < aParam.Length; i++)
            {
                string[] a11 = aParam[i].Split(new string[] { "|" }, StringSplitOptions.None);
                if (i < aParamType.Length)
                    if (aParamType[i].IndexOf("REQUEST") > 0)
                    {
                        sParam = sParam + "'&" + a11[0] + "=' + (f.elements['" + a11[0] + "']?(f.elements['" + a11[0] + "'].value?f.elements['" + a11[0] + "'].value:''):'')";
                    }
            }
            if (a31.Length > 11)
            {
                jsColAdd = a31[11];
            }
            if (a31.Length > 12)
                UrlSearch = a31[12];
            else
                UrlSearch = "/Utils/SearchCountEmp";

            if (IsInsert || IsUpdate || IsDelete)
            {
                string[] ParamArr = a31[6].Split(new string[] { "||" }, StringSplitOptions.None);//' + f.elements['" + a1[j] + "'].value + ' Tools.UrlEncode
                r1.Append(Environment.NewLine + (jsTag ? "<script language=\"javascript\">" : "<!--TagJS-->") +
                   Environment.NewLine + "function Search_" + a1[j] + "() {var f=document." + frm + ";" +
                   Environment.NewLine + "_attw = window.open('" + UrlSearch + "?SearchIndex=" + (Compress.Zip(a3[j])) +
                   "&InputName=" + a1[j] + "&jsColAdd=" + (Compress.Zip(jsColAdd)) + (sParam != "" ? "+ " + sParam + "'" : "'") +
                   ", '_attachmentW', 'toolbar=no,menubar=no,location=no,status=no,scrollbars=yes, resizable=yes, top=10, left=20, width=900, height=600');" +
                   Environment.NewLine + "} " +
                   Environment.NewLine + "function SearchResult_" + a1[j] + "(Cnt, jsColAdd, jsColAddVal) {" +
                   Environment.NewLine + "var f=document." + frm + ";" +
                   Environment.NewLine + "f.elements['" + a1[j] + "'].value=Cnt;" +
                   Environment.NewLine + "var a = new Array(); " +
                   Environment.NewLine + "var b = new Array(); " +
                   Environment.NewLine + "a = jsColAdd.split('||'); " +
                   Environment.NewLine + "b = jsColAddVal.split('||');" +
                   Environment.NewLine + "for(var i = 0; i<a.length; i++){" +
                   Environment.NewLine + "if(f.elements[a[i]]) {" +
                   Environment.NewLine + "var input = f.elements[a[i]];" +
                   Environment.NewLine + "if (input.type == \"checkbox\" || input.type == \"radio\") {" +
                   Environment.NewLine + "if (b[i] == input.value) input.checked = true;" +
                   Environment.NewLine + "} else if (input.type == \"select-one\") {" +
                   Environment.NewLine + "for (var j = 0; j < input.options.length; j++) {" +
                   Environment.NewLine + "if (input.options[j].value == b[i]) input.options[j].selected = true;" +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "} else {" +
                   Environment.NewLine + "input.value = b[i];" +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "}" +
                   Environment.NewLine + "nextfocus(document." + frm + ", 'txt_" + a1[j] + "'); " +
                   Environment.NewLine + "}" + (jsTag ? "</s" + "cript>" : "<!--/TagJS-->"));
            }
            r = r1.ToString(); r1 = null;
            return r;
        }
        
        public static string UIBoxTable(string[] a1, int j, string[] a31, string ClassReadonly,
            ToolDAO DataDAO, HRSContext context, dynamic EditData, string frm = "bosfrm")
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            /*
            InputName;Default;Req;InputNameRows;HeaderRows;TypeRows
            HeaderRows: a||b||c||d
            TypeRows: a*b*c*d
            InputNameRows:a||b||c||d 
            */
            a31 = "tblBoxTest;;1;aInput||bInput||cInput||dInput;Text||Combobox||Nummeric||Date;Textbox::0:40:100*Selectbox:-1:0:SP_CMS__HU_Title_GetAll::ID,Name*Textbox:1:0:10:10*SelectboxText:10:0:10||20||50||100:10||20||50||100".Split(new string[] { ";" }, StringSplitOptions.None);
            string[] a = a31[3].Split(new string[] { "||" }, StringSplitOptions.None); int aCnt = a.Length;
            string[] b = a31[4].Split(new string[] { "||" }, StringSplitOptions.None);
            string[] c = a31[5].Split(new string[] { "*" }, StringSplitOptions.None);
            // chiều data rows
            string val; try { val = Tools.GetDataJson(EditData.EditForm.Items[0], a[0]); } catch { val = ""; }
            string[] v = val.Split(new string[] { "," }, StringSplitOptions.None); int vCnt = v.Length;
            string[,] d = new string[vCnt, aCnt];

            string[] DataVal; DataVal = new string[aCnt];
            string[] DataTxt; DataTxt = new string[aCnt];
            string[] DataParent; DataParent = new string[aCnt]; string ReqJson;
            r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                Environment.NewLine + "var " + a1[j] + "ColumnName = new Array();" +
                Environment.NewLine + "var " + a1[j] + "ColumnLable = new Array();" +
                Environment.NewLine + "var " + a1[j] + "ColumnRequire = new Array();" +
                Environment.NewLine + "var " + a1[j] + "ColumnData = new Array();" +
                Environment.NewLine + "var " + a1[j] + "DataParent = new Array();" +
                Environment.NewLine + "var " + a1[j] + "DataTxt = new Array();" +
                Environment.NewLine + "var " + a1[j] + "DataVal = new Array();</script>");
            r1.Append("<table class=\"table table-hover table-border flex\" id=\"" + a1[j] + "\">");
            r1.Append("<tr>");
            r1.Append("<td>" + UIDef.UIButton("bntAddnew", context.GetLanguageLable("AddNew"), "insertInputTableRow('" + frm + "', '" + a1[j] + "', '" + context.GetLanguageLable("Delete") + "');", " class=\"btn add\" " + ClassReadonly));
            r1.Append("<thead class=\"thead-light\"><tr>");
            for (int i = 0; i < aCnt; i++)
            {
                try { val = Tools.GetDataJson(EditData.EditForm.Items[0], a[0]); } catch { val = ""; }
                v = val.Split(new string[] { "," }, StringSplitOptions.None);
                for (int l = 0; l < vCnt; l++) d[l, i] = v[l];

                string mandantoryLabel = "";  string s = "";
                DataVal[i] = ""; DataTxt[i] = ""; DataParent[i] = "";
                string[] c1 = c[i].Split(new string[] { ":" }, StringSplitOptions.None);

                r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                    Environment.NewLine + "" + a1[j] + "ColumnRequire[" + i + "] = '_';" +
                    Environment.NewLine + "" + a1[j] + "ColumnName[" + i + "] = '" + a[i] + "';" +
                    Environment.NewLine + "" + a1[j] + "ColumnLable[" + i + "] = '" + context.GetLanguageLable(b[i]) + "';</script>");
                if (c1.Length > 2)
                {
                    if (c1[2] == "1")
                    {
                        r1.Append(Environment.NewLine + "<script language=\"javascript\">" +
                            Environment.NewLine + "ColumnRequire[" + i + "] = '" + a[i] + "';</script>");
                        mandantoryLabel = " <font color='red'>*</font>";//(*)
                    }
                }
                if (c1[0] != "-" && c1[0] != "Hidden") r1.Append("<th>" + context.GetLanguageLable(b[i]) + mandantoryLabel + "</th>");
                switch (c1[0])
                {
                    case "Selectbox":
                        ReqJson = context.InputDataSetParam(c1[4]);
                        UIDef.OptionStringVal(ref DataDAO, c1[3], ReqJson, c1[5], ref DataVal[i], ref DataTxt[i], ref DataParent[i]);
                        s = UIDef.SelectStrOption(DataVal[i], DataTxt[i], ref val, true);
                        break;
                    case "SelectboxText":
                        s = UIDef.SelectStrOption(c1[3], c1[4], ref val, true);
                        break;
                }
                r1.Append(Environment.NewLine + "<script language=\"javascript\">");
                r1.Append(Environment.NewLine + "" + a1[j] + "ColumnData[" + i + "] = '" + s + "'");
                r1.Append(Environment.NewLine + "" + a1[j] + "DataParent[" + i + "] = '" + DataParent[i] + "'");
                r1.Append(Environment.NewLine + "" + a1[j] + "DataTxt[" + i + "] = '" + DataTxt[i] + "'");
                r1.Append(Environment.NewLine + "" + a1[j] + "DataVal[" + i + "] = '" + DataVal[i] + "'");
                r1.Append(Environment.NewLine + "</script>");
            }
            r1.Append("</thead>");

            //for (int i = 0; i < aCnt; i++)
            //{
            //    r1.Append("<th>" + a[i]);
            //}
            for (int i = 0; i < vCnt; i++)
            {
                r1.Append("<tr>");
                for (int l = 0; l < aCnt; l++)
                {
                    r1.Append("<td>");
                    string[] c1 = c[l].Split(new string[] { ":" }, StringSplitOptions.None);
                    val = d[i, l];
                    if (a31[0] == "-")
                        r1.Append(UIDef.UIHidden(a[l], val));
                    else
                    {
                        switch (c1[0])
                        {
                            case "Textbox":
                                if (c1.Length > 5)
                                    r1.Append(UIDef.UITextbox(context.GetLanguageLable(b[i]), a[l], val, " " + ClassReadonly + " size=\"" + c1[3] + "\" maxlength=\"" + c1[4] + "\"", "", c1[5]));
                                else
                                    r1.Append(UIDef.UITextbox(context.GetLanguageLable(b[i]), a[l], val, " " + ClassReadonly + " size=\"" + c1[3] + "\" maxlength=\"" + c1[4] + "\"", ""));
                                break;
                            case "Numeric":
                                if (c1.Length > 5)
                                    r1.Append(UIDef.UINumeric(context.GetLanguageLable(b[i]), a[l], val, " " + ClassReadonly + " size=\"" + c1[3] + "\" maxlength=\"" + c1[4] + "\"", "", c1[5]));
                                else
                                    r1.Append(UIDef.UINumeric(context.GetLanguageLable(b[i]), a[l], val, " " + ClassReadonly + " size=\"" + c1[3] + "\" maxlength=\"" + c1[4] + "\"", "", ""));
                                break;
                            case "Date":
                                DateTime dNow = DateTime.Now; int iDay = 0;
                                if (val == "@")
                                {
                                    val = string.Format("{0:dd/MM/yyyy}", dNow);
                                }
                                else if (Tools.Left(val, 1) == "@")//(val.Length > 2 && val.Substring(0, 1) == "@") //
                                {
                                    if (Tools.Left(val, 2) == "@+") iDay = int.Parse(Tools.Right(val, 2, false));
                                    if (Tools.Left(val, 2) == "@-") iDay = 0 - int.Parse(Tools.Right(val, 2, false));
                                    val = string.Format("{0:dd/MM/yyyy}", dNow.AddDays(iDay));
                                }
                                r1.Append(UIDef.UIDate(context.GetLanguageLable(b[i]), a[l], val, " " + ClassReadonly, "", frm));
                                break;
                            case "Datetime":
                                DateTime dNow1 = DateTime.Now; int iDay1 = 0;
                                if (val == "@")
                                {
                                    val = string.Format("{0:dd/MM/yyyy HH:mm}", dNow1);
                                }
                                else if (Tools.Left(val, 1) == "@")//(val.Length > 2 && val.Substring(0, 1) == "@")
                                {
                                    if (Tools.Left(val, 2) == "@+") iDay1 = int.Parse(Tools.Right(val, 2, false));
                                    if (Tools.Left(val, 2) == "@-") iDay1 = 0 - int.Parse(Tools.Right(val, 2, false));
                                    val = string.Format("{0:dd/MM/yyyy HH:mm}", dNow1.AddDays(iDay1));
                                }
                                r1.Append(UIDef.UIDateTime(context.GetLanguageLable(b[i]), a[l], val, " " + ClassReadonly, "", frm));
                                break;
                            case "Time":
                                r1.Append(UIDef.UITime(context.GetLanguageLable(b[i]), a[l], val, " " + ClassReadonly, "", frm));
                                break;
                            case "Selectbox":
                                ReqJson = context.InputDataSetParam(c1[4]);
                                r1.Append(UIDef.UISelectStr(a[l], DataVal[l], DataTxt[l], ref val, true, ClassReadonly, "", "document." + frm));
                                break;
                            case "SelectboxText":
                                r1.Append(UIDef.UISelectStr(a[l], c1[3], c1[4], ref val, true, ClassReadonly, "", "document." + frm));
                                break;
                        }
                    }
                }
                r1.Append("<td>" + UIDef.UIButton("bntDelete", context.GetLanguageLable("Delete"), "DeleteRow('" + a1[j] + "',this.parentNode.parentNode.rowIndex);", " class=\"btn delete\" " + ClassReadonly));
            }
            
            r1.Append("</table>");
            r = r1.ToString(); r1 = null;
            return r;
        }
    }
    public static class UIDef {
        #region Textbox
        public const string TextLengthDefault = "12";
        public const string DateLengthDefault = "10";
        public const int Rows = 500;
        public const int RowMaxSelect = 10000;
        
        public static string FocusAfter(string col)
        {
            return " onkeyPress=\"cnextfocus(event, this.form, '" + col + "');\"";
        }
        public static string NextFocus(string col)
        {
            return " nextfocus(this.form, '" + col + "');";
        }
        public static string NextFocus(string col, string FrmName)
        {
            return " nextfocus(document." + FrmName + ", \\'" + col + "\\');";
        }
        public static string CNextFocus(string col)
        {
            return " cnextfocus(event, this.form, '" + col + "');";
        }
        public static string CNextFocusAction(string col)
        {
            return " cnextfocus(event, this.form, \\'" + col + "\\');";
        }
        public static string UITextarea(string placeholder, string InputName, string InputValue, string Param, string NextFocus = "", string Info = "", bool IsEditor = false)
        {
            if (Param == "") Param = "  cols=\"45\" rows=\"3\"";
            if (IsEditor)
            {
                return Environment.NewLine + "<scr" + "ipt src=\"/js/ckeditor_basic.js\"></scr" + "ipt>" +
                    Environment.NewLine + "<textarea id=\"" + InputName + "\" name=\"" + InputName + "\" " + Param + NextFocus + ">" +
                InputValue + "</textarea>" +
                    Environment.NewLine + "<scri" + "pt>" +
                    Environment.NewLine + "CKEDITOR.replace( '" + InputName + "' );" +
                    Environment.NewLine + "</scr" + "ipt>";
            }
            else 
                return Environment.NewLine + "<textarea placeholder=\"" + placeholder + "\" class=\"form-control\" id=\"" + InputName + "\" name=\"" + InputName + "\" " + Param + NextFocus + ">" +
                    InputValue + "</textarea>";
        }
        public static string UIHidden (string InputName, string InputValue) {
            return Environment.NewLine + "<input type=\"hidden\" id=\"" + InputName + "\" name=\"" + InputName + "\" value=\"" + InputValue+"\">";
        }
        public static string UITextbox(string placeholder, string InputName, string InputValue, string Param, string NextFocus = "", string OnChange = "") {
            if (Param == "") Param = " size = 43 ";// + Param.Replace(" size =", "").Replace(" size=", "").Replace("size =", "").Replace("size=", "");

            if (Tools.Right(InputName, 6).ToUpper() == "MOBILE" || Tools.Right(InputName, 5).ToUpper() == "PHONE" || 
                Tools.Right(InputName, 3).ToUpper() == "TEL" || Tools.Right(InputName, 3).ToUpper() == "FAX")
            {
                Param = " onBlur='phoneValid(this);' size = " + TextLengthDefault + " maxlength = 30 " + Param;
                OnChange = (OnChange == null || OnChange == ""? "": OnChange + ";") + "";
            }
            else if (Tools.Right(InputName, 5).ToUpper() == "EMAIL")
            {
                Param = " onBlur='emailValid(this);' size = " + TextLengthDefault + " maxlength = 30 " + Param;
                OnChange = (OnChange == null || OnChange == "" ? "" : OnChange + ";") + "";
            }
            else if (Tools.Right(InputName, 4).ToUpper() == "CODE" || Tools.Right(InputName, 2).ToUpper() == "NO" ||
                InputName == "FirstName" || InputName == "LastName")
                Param = " size = " + TextLengthDefault + " maxlength = 30 " + Param;
            else if (Tools.Left(InputName, 4) == "txt_")
                Param = " size = 31 maxlength = 30 " + Param;

            return Environment.NewLine + "<input placeholder=\"" + placeholder + "\" class=\"form-control\" type=\"textbox\" id=\"" + InputName + "\" name=\"" + InputName + "\" value=\"" + InputValue + "\" " + Param + NextFocus + (OnChange != null && OnChange != "" ? " onchange=\"" + OnChange + "\"" : "") + ">";
        }
        public static string UIPassword(string InputName, string Param, string NextFocus = "")
        {
            return Environment.NewLine + "<input class=\"form-control\" type=\"password\" id=\"" + InputName + "\" name=\"" + InputName + "\" " + Param + NextFocus + ">";
        }
        public static string UINumeric(string placeholder, string InputName, string InputValue, string Param, string NextFocus, string OnChange) {
            return Environment.NewLine + "<input class=\"form-control\" placeholder=\"" + placeholder + "\" type=\"textbox\" size=" + TextLengthDefault + " style=\"text-align:right;\" id=\"" + InputName + "\" name=\"" + InputName + "\" value=\"" + Tools.insertSepr(InputValue) + "\" " + Param +
                " onBlur='numericValid(this);' onchange=\"this.value=insertSepr(this.value);" + (OnChange != null && OnChange != "" ? OnChange + ";" : "") + "\">";
        }
        public static string UIDate(string placeholder, string InputName, string InputValue, string Param, string NextFocus, string FrmName = "bosfrm") {
            ////return Environment.NewLine + "<input type=\"textbox\" id=\"" + InputName + "\" name=\"" + InputName +
            ////    "\" value=\"" + InputValue + "\" " + Param +
            ////    " size=" + DateLengthDefault + " maxlength=10 style=\"text-align:center;\" onchange=\"this.value=datePrompt(this)\">" +
            ////    //"<label class=\"date\" onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\"></label>";
            ////    "<img src =\"/images/icon_date.png\" class=\"imggo\" " +
            ////    "onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\">";
            return Environment.NewLine + "<div class=\"input-group\">" +
                Environment.NewLine + "<input type=\"text\" id=\"" + InputName + "\" name=\"" + InputName + "\" value=\"" + InputValue + "\" " + 
                Param + " onchange=\"this.value=datePrompt(this)\" " + "class=\"form-control\" placeholder=\"" + placeholder + "\">" +
                Environment.NewLine + "<span class=\"input-group-addon\" onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\">" +
                "<i class=\"fa fa-calendar\"></i></span></div>";
        }
        public static string UIDate(string placeholder, string InputName, string InputValue, string Param, string OnChange, string NextFocus, string FrmName = "bosfrm") {
            if (OnChange != "") OnChange = OnChange.Replace("FrmName", FrmName);
            ////return Environment.NewLine + "<input type=\"textbox\" id=\"" + InputName + "\" name=\"" + InputName +
            ////    "\" value=\"" + InputValue + "\" " + Param +
            ////    " size=" + DateLengthDefault + " maxlength=10 style=\"text-align:center;\" onchange=\"this.value=datePrompt(this);" + OnChange + ";\">" +
            ////    //"<label class=\"date\" onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\"></label>";
            ////    "<img src =\"/images/icon_date.png\" class=\"imggo\" " +
            ////    "onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + OnChange + ";" + NextFocus + "', null);\">";

            return Environment.NewLine + "<div class=\"input-group\">" +
                Environment.NewLine + "<input type=\"text\" id=\"" + InputName + "\" name=\"" + InputName + "\" value=\"" + InputValue + "\" " + Param + " onchange=\"this.value=datePrompt(this)\" class=\"form-control\" placeholder=\"" + placeholder + "\">" +
                Environment.NewLine + "<span class=\"input-group-addon\" onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + OnChange + ";" + NextFocus + "', null);\"><i class=\"fa fa-calendar\"></i></span>" +
                "</div>";
        }
        public static string UIDateTime(string placeholder, string InputName, string InputValue, string Param, string NextFocus, string FrmName = "bosfrm") {
            return Environment.NewLine + "<div class=\"input-group\">" +
                Environment.NewLine + "<input type=\"text\" id=\"" + InputName + "\" name=\"" + InputName + "\" " +
                "value=\"" + InputValue + "\" " + Param + " onchange=\"this.value=datePrompt(this)\" " +
                "class=\"form-control\" placeholder=\"" + placeholder + "\">" +
                Environment.NewLine + "<span class=\"input-group-addon\" onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\">" +
                "<i class=\"fa fa-calendar\"></i></span>" +
                "</div>";
            ////return Environment.NewLine + "<input type=\"textbox\" id=\"" + InputName + "\" name=\"" + InputName +
            ////    "\" value=\"" + InputValue + "\" " + Param +
            ////    " size=" + DateLengthDefault + " maxlength=16 style=\"text-align:center;\" onchange=\"this.value=datePrompt(this)\">" +
            ////    //"<label class=\"date\" onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\"></label>";
            ////    "<img src =\"/images/icon_date.png\" class=\"imggo\" " +
            ////    "onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', 1);\">";
        }
        public static string UIDateTime(string placeholder, string InputName, string InputValue, string Param, string OnChange, string NextFocus, string FrmName = "bosfrm") {
            if (OnChange != "") OnChange = OnChange.Replace("FrmName", FrmName);
            return Environment.NewLine + "<div class=\"input-group\">" +
                Environment.NewLine + "<input type=\"text\" id=\"" + InputName + "\" name=\"" + InputName + "\" " +
                "value=\"" + InputValue + "\" " + Param + " onchange=\"this.value=datePrompt(this);" + OnChange + ";\" " +
                "class=\"form-control\" placeholder=\"" + placeholder + "\">" +
                Environment.NewLine + "<span class=\"input-group-addon\" " +
                "onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + OnChange + ";" + NextFocus + "', 1);\">" +
                "<i class=\"fa fa-calendar\"></i></span>" +
                "</div>";
            ////return Environment.NewLine + "<input type=\"textbox\" id=\"" + InputName + "\" name=\"" + InputName +
            ////    "\" value=\"" + InputValue + "\" " + Param +
            ////    " size=" + DateLengthDefault + " maxlength=16 style=\"text-align:center;\" onchange=\"this.value=datePrompt(this);" + OnChange + ";\">" +
            //////"<label class=\"date\" onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\"></label>";
            ////"<img src =\"/images/icon_date.png\" class=\"imggo\" " +
            ////"onClick=\"DoCal(document." + FrmName + ".elements['" + InputName + "'], null, '" + OnChange + ";" + NextFocus + "', 1);\">";
        }
        public static string UITime(string placeholder, string InputName, string InputValue, string Param, string NextFocus, string FrmName = "bosfrm") {
            return Environment.NewLine + "<div class=\"input-group\">" +
                Environment.NewLine + "<input type=\"text\" id=\"" + InputName + "\" name=\"" + InputName + "\" " +
                "value=\"" + InputValue + "\" " + Param + " onchange=\"timeValid(this);\" " +
                "class=\"form-control\" placeholder=\"" + placeholder + "\">" +
                Environment.NewLine + "<span class=\"input-group-addon\" " +
                "onClick=\"DoTime(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\">" +
                "<i class=\"fa fa-calendar\"></i></span>" +
                "</div>";
            ////return Environment.NewLine + "<input type=\"textbox\" id=\"" + InputName + "\" name=\"" + InputName +
            ////    "\" value=\"" + InputValue + "\" " + Param +
            ////    " size=" + DateLengthDefault + " maxlength=5 style=\"text-align:center;\" onchange=\"timeValid(this);\">" +
            //////"<label class=\"time\" onClick=\"DoTime(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\"></label>";
            ////"<img src =\"/images/icon_time.png\" class=\"imggo\" " +
            ////"onClick=\"DoTime(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\">";
        }
        public static string UITime(string placeholder, string InputName, string InputValue, string Param, string OnChange, string NextFocus, string FrmName = "bosfrm") {
            return Environment.NewLine + "<div class=\"input-group\">" +
                Environment.NewLine + "<input type=\"text\" id=\"" + InputName + "\" name=\"" + InputName + "\" " +
                "value=\"" + InputValue + "\" " + Param + " onchange=\"timeValid(this);" + OnChange + ";\" " +
                "class=\"form-control\" placeholder=\"" + placeholder + "\">" +
                Environment.NewLine + "<span class=\"input-group-addon\" " +
                "onClick=\"DoTime(document." + FrmName + ".elements['" + InputName + "'], null, '" + OnChange + ";" + NextFocus + "');\">" +
                "<i class=\"fa fa-calendar\"></i></span>" +
                "</div>";
            ////return Environment.NewLine + "<input type=\"textbox\" id=\"" + InputName + "\" name=\"" + InputName +
            ////    "\" value=\"" + InputValue + "\" " + Param +
            ////    " size=" + DateLengthDefault + " maxlength=5 style=\"text-align:center;\"  onchange=\"timeValid(this);" + OnChange + ";\">" +
            ////// "<label class=\"time\" onClick=\"DoTime(document." + FrmName + ".elements['" + InputName + "'], null, '" + NextFocus + "', null);\"></label>";
            ////"<img src =\"/images/icon_time.png\" class=\"imggo\" " +
            ////    "onClick=\"DoTime(document." + FrmName + ".elements['" + InputName + "'], null, '" + OnChange + ";" + NextFocus + "');" + OnChange + ";\">";
        }
        #endregion

        #region UICheckbox || UIRadiobox || UIButton
        public static string UIMultipleSelect (string InputName, HRSContext context, ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, 
            ref string InputCheck, string NextFocus, string OnClick, ref string ValStr, ref string ParentStr, string Param = "")
        {
            StringBuilder r = new StringBuilder(); string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d = null;string[] a = ColumnName.Split(new string[] { "," }, StringSplitOptions.None);
            if (ProcParam != "") d = JObject.Parse(ProcParam);
            toolDAO.ExecuteStore("UIMultipleSelect", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse(json);

            //NextFocus = NextFocus + (OnClick != "" ? "onclick=\"" + OnClick + "\";" : ""); 
            //UIMultipleSelectAll(this.form,this,'" + InputName + "');" + OnClick + "

            string inputCheck = "";

            //if (d.UIMultipleSelect.Items.Count > 1) r.Append(Environment.NewLine + "<table width=100% class=\"table-custom\"><tr><td><label><input type=\"checkbox\" name=\"" + InputName + "_HA\" value=\"Y\" "+
            //    "onClick=\"UIMultipleSelectAll(this.form,this,'" + InputName + "');\"><div style=\"margin:5px 0 0; float:right;text-align:left; width:315px\">" + context.GetLanguageLable("CHECKALL") + "</div></label>");
            if (d.UIMultipleSelect.Items.Count > 1)
                r.Append(Environment.NewLine + "<div class=\"divMutiselect\" id=\"tr_\">" +
                    "<input type=\"checkbox\" id=\"" + InputName + "_0\" name=\"" + InputName + "_HA\" value=\"Y\" " +
                    "onClick=\"UIMultipleSelectAll(this.form,this,'" + InputName + "');\"> <label for=\"" + InputName + "_0\">" +
                    context.GetLanguageLable("CHECKALL") + "</label></div>");
            ParentStr = "";
            for (int i = 0; i<d.UIMultipleSelect.Items.Count; i++)
            {
                string a_0 = Tools.GetDataJson(d.UIMultipleSelect.Items[i], a[0]); string a_1 = Tools.GetDataJson(d.UIMultipleSelect.Items[i], a[1]); string a_2 = "";
                try { a_2 = Tools.GetDataJson(d.UIMultipleSelect.Items[i], a[2]); } catch { a_2 = ""; }
                //r.Append(Environment.NewLine + "<tr id=\"tr_" + i + "\"><td><input type=\"checkbox\" name=\"" + InputName + "\" " + Param + (a.Length > 2 ? OnClick : "") + " value=\"" + a_0 + "\"");
                r.Append(Environment.NewLine + "<div class=\"divMutiselect\" id=\"tr_" + i + "\">" +
                    "<input id=\"" + InputName + "_" + (i + 1) + "\" type=\"checkbox\" name=\"" + InputName + "\" " + Param + (a.Length > 2 ? OnClick : "") + " value=\"" + a_0 + "\"");
                if (("," + InputCheck + ",").IndexOf("," + a_0 + ",") > -1)
                {
                    inputCheck = inputCheck + ", " + a_1.Replace("&nbsp;", "");
                    r.Append(" checked");
                }
                
                r.Append("><label for=\"" + InputName + "_" + (i + 1) + "\">" + a_1 + "</label></div>");

                ValStr = ValStr + "||" + a_0;
                ParentStr = ParentStr + "||" + a_2;
            }
            if (inputCheck != "") Tools.RemoveFisrtChar(inputCheck, 2);
            InputCheck = inputCheck;
            json = r.ToString();// + "</table>";
            r = null;
            return json;
        }
        public static string UIDD(string InputName, HRSContext context, ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, string InputCheck, 
            string NextFocus, string OnClick, string Param, int Height, int Width, ref string ValStr, ref string ParentStr, string frm = "document.bosfrm")
        {
            int _width = 43; int w = _width;
            int _widthPx = 357; int wpx = _widthPx;
            if (Width > 0)
            {
                w = Width;
                wpx = (Width * _widthPx / _width);
            }
            string rUI = UIMultipleSelect(InputName, context, ref toolDAO, ProcName, ProcParam, ColumnName, ref InputCheck, NextFocus, OnClick, ref ValStr, ref ParentStr, Param);
            return Environment.NewLine + "<input name=\"txt_" + InputName + "\" size=\"" + w.ToString() + "\" value=\"" + InputCheck + "\" " + Param + " readonly>" +
                Environment.NewLine + "<img src=\"/images/Icon_Select.gif\" Class=\"imggo\" id=\"" + InputName + "img\" onclick=\"uiddmh(" + frm + ",'" + InputName + "');\">" +
                Environment.NewLine + "<div id=\"" + InputName + "_div\" style=\"background:white;border:1px solid #ccc;display:none;height:" + Height.ToString() + "px;width:" + _widthPx.ToString() + "px;overflow:auto\">" + //" + Width + "
                rUI + "</div>";
            //position:absolute;
        }
        public static string UICheckbox(string InputName, string InputLable, string InputValue, string InputCheck, string NextFocus, bool IsRadio = false, int i = 0) {

            if (IsRadio)
                return Environment.NewLine + "<div class=\"form-check\"><input type=\"radio\" id=\"" + InputName + i.ToString() + "\" name=\"" + InputName + "\" value=\"" + InputValue + "\" " + (InputValue == InputCheck ? " checked" : "") + " " + NextFocus + " class=\"form-check-input\">" +
                    Environment.NewLine + "<label class=\"form-check-label\" for=\"" + InputName + i.ToString() + "\">" + InputLable + "</label></div>";
            else
                return Environment.NewLine + "<div class=\"form-check\"><input type=\"checkbox\" id=\"" + InputName + i.ToString() + "\" name=\"" + InputName + "\" value=\"" + InputValue + "\" " + (InputValue == InputCheck ? " checked" : "") + " " + NextFocus + " class=\"form-check-input\">" +
                    Environment.NewLine + "<label class=\"form-check-label\" for=\"" + InputName + i.ToString() + "\">" + InputLable + "</label></div>";
        }
        public static string UIHrefButton(string InputLable, string Action,             
            string ClassNameTagI = "fa fa-plus xanhla", string TabIndex = "", string Id = "",
            string ClassNameBnt = "btn btn-outline-grey btn-sm my-0 waves-effect waves-light")
        {
            return Environment.NewLine + "<a " +
               (Id!=""?"id=\"" + Id + "\"":"") + " " + 
               (TabIndex != "" ? "tabindex=\"" + TabIndex + "\"" : "") + 
               " href=\"javascript: " + Action + "\" class=\"" + ClassNameBnt + "\">" +
               Environment.NewLine + (ClassNameTagI != ""?"<i class=\"" + ClassNameTagI + "\" aria-hidden=\"true\"></i> ":"") + InputLable +
               Environment.NewLine + " </a>";
        }
        public static string UIButton(string InputName, string InputLable, string Action, string ClassName = " class=\"btn-custom\"") {
            return "<input type=\"button\" " + ClassName + " id=\"" + InputName + "\" name=\"" + InputName +
                "\" value=\"" + InputLable + "\" onClick=\"" + Action + "\">";
        }
        public static string UIButton(string InputName, string InputLable, bool IsSubmit = true, string ClassName = " class=\"btn-custom\"") {
            return Environment.NewLine + "<input id=\"" + InputName + "\" " + ClassName + " name=\"" + InputName +
                "\" value=\"" + InputLable + "\" type=\"" + (IsSubmit ? "submit":"reset") + "\">";
        }
        public static string UIRadio(string InputName, string LblStr, string ValStr, string InputCheck, string NextFocus, string ColorStr = "") {
            string outBuff = ""; string[] Val; string[] Lbl; string[] Color; int i = 0; int j = 0;
            Lbl = LblStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Val = ValStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Color = ColorStr.Split(new string[] { "||" }, StringSplitOptions.None);
            j = (Val.Length <= Lbl.Length? Val.Length: Lbl.Length);
            for (i = 0; i < j; i++) {
                bool ktColor = false;
                if (i < ColorStr.Length) if (Color[i] != "") ktColor = true;
                ////if (ktColor)
                ////    outBuff = outBuff + Environment.NewLine + "<label class=\"name\" style=\"background-color: " + Color[i] + ";\"><input type=\"radio\" id=\"" + InputName + "\" name=\"" + InputName +
                ////"\" value=\"" + Val[i] + "\" " + (Val[i] == InputCheck ? " checked" : "") + " " + NextFocus + ">" + Lbl[i] + "</label> ";
                ////else
                ////    outBuff = outBuff + Environment.NewLine + "<label class=\"name\"><input type=\"radio\" id=\"" + InputName + "\" name=\"" + InputName +
                ////    "\" value=\"" + Val[i] + "\" " + (Val[i] == InputCheck ? " checked" : "") + " " + NextFocus + ">" + Lbl[i] + "</label> ";
                if (ktColor)
                    outBuff = outBuff +
                        Environment.NewLine + "<div class=\"form-check form-check-inline\">" +
                        Environment.NewLine + "<input type=\"radio\" class=\"form-check-input\" id=\"" + InputName + i + "\" name=\"" + InputName + "\" value=\"" + Val[i] + "\" " + (Val[i] == InputCheck ? " checked" : "") + " " + NextFocus + ">" +
                        Environment.NewLine + "<label class=\"form-check-label\" for=\"" + InputName + i + "\" style=\"background-color: " + Color[i] + ";\">" + Lbl[i] + "</label>" +
                        Environment.NewLine + "</div>";
                else
                    outBuff = outBuff +
                        Environment.NewLine + "<div class=\"form-check form-check-inline\">" +
                        Environment.NewLine + "<input type=\"radio\" class=\"form-check-input\" id=\"" + InputName + i + "\" name=\"" + InputName + "\" value=\"" + Val[i] + "\" " + (Val[i] == InputCheck ? " checked" : "") + " " + NextFocus + ">" +
                        Environment.NewLine + "<label class=\"form-check-label\" for=\"" + InputName + i + "\">" + Lbl[i] + "</label>" +
                        Environment.NewLine + "</div>";
            }
            return outBuff;
        }
        #endregion

        #region UIactb        
        public static string UIActbId(string InputName, ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, string InputValue, string Param, string OnChange, string FrmName = "bosfrm")
        {
            StringBuilder r = new StringBuilder(); string r1 = ""; string InputValue_txt = InputValue;
            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = ""; dynamic d = null;
            string[] a = ColumnName.Split(new string[] { "," }, StringSplitOptions.None);
            if (ProcParam != "") d = JObject.Parse(ProcParam);
            toolDAO.ExecuteStore("Actb", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse(json);
            r.Append(Environment.NewLine + "<input type=\"hidden\" name=\"" + InputName + "\" id=\"" + InputName + "\" value=\"" + InputValue + "\">");
            r.Append(Environment.NewLine + "<s" + "cript language=" + "\"javascript\">");
            r.Append(Environment.NewLine + "var " + InputName + "_text = new Array();");
            r.Append(Environment.NewLine + "var " + InputName + "_id = new Array();");

            for (int i = 0; i < d.Actb.Items.Count; i++)
            {
                r.Append(Environment.NewLine + "" + InputName + "_text[" + i + "]=\"" + Tools.GetDataJson(d.Actb.Items[i], a[1]).Replace("&nbsp;", "") + "\";" +
                    Environment.NewLine + "" + InputName + "_id[" + i + "]=\"" + Tools.GetDataJson(d.Actb.Items[i], a[0]) + "\";");
                if (Tools.GetDataJson(d.Actb.Items[i], a[0]) == InputValue) InputValue_txt = Tools.GetDataJson(d.Actb.Items[i], a[1]);
            }
            r.Append(Environment.NewLine + "</script>" +
                    Environment.NewLine + "<input name=\"" + InputName + "_txt\" " + Param + " value=\"" + InputValue_txt + "\" autocomplete=\"off\">" +
                    Environment.NewLine + "<img src=\"/images/Icon_Select.gif\" class=\"imggo\" onClick=\"actbinvoke(_" + InputName + "actb);\">" +
                    Environment.NewLine + "<s" + "cript language=" + "\"javascript\">");
            r.Append(Environment.NewLine + "var _" + InputName + "actb = new actb(document." + FrmName + "." + InputName + "_txt, " + InputName + "_text, \"document." + FrmName + "." + InputName + ".value=" + InputName + "_id[i];" + OnChange + "\");" +
                    Environment.NewLine + "</script>");            
            
            r1 = r.ToString();
            r = null;
            return r1;
        }
        private static string UIActbStrIdGen(string InputName, string[] Val, string[] Lbl, string InputValue, string Param, string OnChange, string FrmName = "bosfrm")
        {
            FrmName = FrmName.Replace("document.", "");
            StringBuilder r = new StringBuilder(); string r1 = ""; int i = 0; int j = 0; string InputValue_txt = InputValue;
            j = (Val.Length <= Lbl.Length ? Val.Length : Lbl.Length);
            r.Append(Environment.NewLine + "<input type=\"hidden\" name=\"" + InputName + "\" id=\"" + InputName + "\" value=\"" + InputValue + "\">");
            r.Append(Environment.NewLine + "<s" + "cript language=" + "\"javascript\">");
            r.Append(Environment.NewLine + "var " + InputName + "_text = new Array();");
            r.Append(Environment.NewLine + "var " + InputName + "_id = new Array();");

            for (i = 0; i < j; i++)
            {
                r.Append(Environment.NewLine + "" + InputName + "_text[" + i + "]=\"" + Lbl[i].Replace("&nbsp;", "") + "\";" +
                    Environment.NewLine + "" + InputName + "_id[" + i + "]=\"" + Val[i] + "\";");
                if (Val[i] == InputValue) InputValue_txt = Lbl[i].Replace("&nbsp;", "");
            }
            r.Append(Environment.NewLine + "</script>" +
                    Environment.NewLine + "<input name=\"" + InputName + "_txt\" " + Param + " value=\"" + InputValue_txt + "\" autocomplete=\"off\">" +
                    Environment.NewLine + "<img src=\"/images/Icon_Select.gif\" class=\"imggo\" onClick=\"actbinvoke(_" + InputName + "actb);\">" +
                    Environment.NewLine + "<s" + "cript language=" + "\"javascript\">");
            r.Append(Environment.NewLine + "var _" + InputName + "actb = new actb(document." + FrmName + "." + InputName + "_txt, " + InputName + "_text, \"document." + FrmName + "." + InputName + ".value=" + InputName + "_id[i];" + OnChange + "\");" +
                    Environment.NewLine + "</script>");
            r1 = r.ToString();
            return r1;
        }
        public static string UIActbStrId(string InputName, string ValStr, string TextStr, string InputValue, string Param, string OnChange, string FrmName = "bosfrm")
        {
            StringBuilder r = new StringBuilder(); string r1 = ""; string InputValue_txt = InputValue;
            string[] Val; string[] Lbl;
            Lbl = TextStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Val = ValStr.Split(new string[] { "||" }, StringSplitOptions.None);
            r.Append(UIActbStrIdGen(InputName, Val, Lbl, InputValue, Param, OnChange, FrmName));

            r1 = r.ToString();
            r = null;
            return r1;
        }
        public static string UIActb(string InputName, ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, string InputValue, string Param, string OnChange, string FrmName = "bosfrm")
        {
            StringBuilder r = new StringBuilder(); string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d = null; string[] a = ColumnName.Split(new string[] { "," }, StringSplitOptions.None);
            if (ProcParam != "") d = JObject.Parse(ProcParam);
            toolDAO.ExecuteStore("Actb", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse(json);
            r.Append(Environment.NewLine + "<input name=\"" + InputName + "\" " + Param + " value=\"" + InputValue + "\" autocomplete=\"off\">");
            r.Append(Environment.NewLine + "<img src=\"/images/Icon_Select.gif\" class=\"imggo\" onClick=\"actbinvoke(_" + InputName + "actb);\">");
            r.Append(Environment.NewLine + "<s" + "cript language=" + "\"javascript\">var " + InputName + "_text = new Array(\"\"");
            for (int i = 0; i<d.Actb.Items.Count; i++)
            {
                r.Append(", \"" + Tools.GetDataJson(d.Actb.Items[i], a[0]) + Tools.C_ActbSepr + Tools.GetDataJson(d.Actb.Items[i], a[1]) + "\""); //Tools.C_ActbSepr
            }
            r.Append(");var _" + InputName + "actb = new actb(document." + FrmName + "." + InputName + ", " + InputName + "_text, \"" + OnChange + "\");</script>");
            json = r.ToString();
            r = null;
            return json;
        }
        public static string UIActbStr(string InputName, string ValStr, string TextStr, string InputValue, string Param, string OnChange, string FrmName = "bosfrm")
        {
            StringBuilder r = new StringBuilder(); string r1 = ""; string[] Val; string[] Lbl; 
            Lbl = TextStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Val = ValStr.Split(new string[] { "||" }, StringSplitOptions.None);
            r.Append(UIActbStrGen(InputName, Val, Lbl, InputValue, Param, OnChange, FrmName));
            r1 = r.ToString();
            return r1;
        }
        private static string UIActbStrGen(string InputName, string[] Val, string[] Lbl, string InputValue, string Param, string OnChange, string FrmName = "bosfrm")
        {
            FrmName = FrmName.Replace("document.", "");
            StringBuilder r = new StringBuilder(); string r1 = ""; int i = 0; int j = 0;
            j = (Val.Length <= Lbl.Length ? Val.Length : Lbl.Length);
            r.Append(Environment.NewLine + "<input name=\"" + InputName + "\" " + Param + " width=43 value=\"" + InputValue + "\" autocomplete=\"off\">");
            r.Append(Environment.NewLine + "<img src=\"/images/Icon_Select.gif\" class=\"imggo\" onClick=\"actbinvoke(_" + InputName + "actb);\">");
            r.Append(Environment.NewLine + "<s" + "cript language=" + "\"javascript\">var " + InputName + "_text = new Array(\"\"");
            for (i = 0; i < j; i++)
            {
                r.Append(", \"" + Val[i] + Tools.C_ActbSepr + Lbl[i] + "\""); //Tools.C_ActbSepr
            }
            r.Append(");var _" + InputName + "actb = new actb(document." + FrmName + "." + InputName + ", " + InputName + "_text, \"" + OnChange + "\");</script>");
            r1 = r.ToString();
            return r1;
        }
        #endregion

        #region UISelect || UITree
        /*
        public static string UILanguage(string l, string urlBack = "/Home/Index")
        {
            StringBuilder r1 = new StringBuilder(); string r = "";
            r1.Append(Environment.NewLine + "<form name=\"Languagefrm\" method=\"POST\" action=\"/Utils/Language\">");
            r1.Append(Environment.NewLine + UISelectStr("language", "vi||en", "Tiếng Việt||English", ref l, true, "", "this.form.submit();"));
            r1.Append(Environment.NewLine + UIHidden("UrlBack", urlBack));
            r1.Append(Environment.NewLine + "</form>");
            r = r1.ToString();
            r1 = null;
            return r;
        }
        */
        private static string SelectOption(ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, string InputValue, bool NotFoundDisp)
        {
            StringBuilder r = new StringBuilder(); string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d = null; bool Found = false; string r1 = ""; string[] a = ColumnName.Split(new string[] { "," }, StringSplitOptions.None);
            if (ProcParam != "") d = JObject.Parse(ProcParam);
            toolDAO.ExecuteStore("Select", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse(json);
            for (int i = 0; i < d.Select.Items.Count; i++)
            {
                r.Append("<option value=\"" + Tools.GetDataJson(d.Select.Items[i], a[0]) + "\"");
                if (Tools.GetDataJson(d.Select.Items[i], a[0]) == InputValue)
                {
                    Found = true;
                    r.Append(" selected");
                }
                r.Append(">" + Tools.GetDataJson(d.Select.Items[i], a[1]) + "</option>");
            }
            if (NotFoundDisp && !Found && InputValue != "")
            {
                r.Append("<option value=\"" + InputValue + "\" selected></option>");//[" + InputValue + "]
            }
            r1 = r.ToString(); r = null;
            return r1;
        }
        public static string SelectStrOption(string ValStr, string TextStr, ref string InputCheck, bool NotFoundDisp)
        {
            string outBuff = ""; string[] Val; string[] Lbl; int i = 0; int j = 0; bool Found = false;
            Lbl = TextStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Val = ValStr.Split(new string[] { "||" }, StringSplitOptions.None);
            j = (Val.Length <= Lbl.Length ? Val.Length : Lbl.Length);

            for (i = 0; i < j; i++)
            {
                outBuff = outBuff + "<option value=\"" + Val[i] + "\"";
                if (Val[i] == InputCheck)
                {
                    Found = true;
                    outBuff = outBuff + " selected";
                    InputCheck = InputCheck + " - " + Lbl[i];
                }
                outBuff = outBuff + ">" + Lbl[i] + "</option>";

            }
            if (NotFoundDisp && !Found && InputCheck != "")
            {
                outBuff = outBuff + "<option value=\"" + InputCheck + "\" selected></option>";//[" + InputCheck + "]
            }
            return outBuff;
        }
        public static string SelectStrOption (ref bool IsSelect, string InputName, string OnClick, string ValStr, string TextStr, ref string InputCheck, bool NotFoundDisp)
        {
            string outBuff = ""; string[] Val; string[] Lbl; int i = 0; int j = 0; bool Found = false;
            Lbl = TextStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Val = ValStr.Split(new string[] { "||" }, StringSplitOptions.None);
            j = (Val.Length <= Lbl.Length ? Val.Length : Lbl.Length);
            if (j > RowMaxSelect) // Div radio
            {
                IsSelect = false;
                outBuff = outBuff + Environment.NewLine + "<table width=100% class=\"table table-hover table-border flex\">";
                for (i = 0; i < j; i++)
                {
                    outBuff = outBuff + Environment.NewLine + "<tr><td><input type=\"radio\" name=\"" + InputName + "\" " + (OnClick != "" ? "onclick=\"if(this.form.txt_" + InputName + ")this.form.txt_" + InputName + ".value='" + Lbl[i] + "';" + OnClick + "\"" : "") + 
                        " value=\"" + Val[i] + "\"";
                    if (Val[i] == InputCheck)
                    {
                        InputCheck = Lbl[i];
                        outBuff = outBuff + " checked";
                    }
                    outBuff = outBuff + "><div style=\"margin:5px 0 0; float:right;text-align:left; width:315px\">" + Lbl[i] + "</div>";
                }
                outBuff = outBuff + "</table>";
            }
            else // Select box
            {
                IsSelect = true;
                for (i = 0; i < j; i++)
                {
                    outBuff = outBuff + "<option value=\"" + Val[i] + "\"";
                    if (Val[i] == InputCheck)
                    {
                        Found = true;
                        outBuff = outBuff + " selected";
                        InputCheck = InputCheck + " - " + Lbl[i];
                    }
                    outBuff = outBuff + ">" + Lbl[i] + "</option>";

                }
                if (NotFoundDisp && !Found && InputCheck != "")
                {
                    outBuff = outBuff + "<option value=\"" + InputCheck + "\" selected></option>";//[" + InputCheck + "]
                }
            }
            
            return outBuff;
        }
        public static string SelectStrOption(ref bool IsSelect, string InputName, string OnClick, string ValStr, string TextStr, ref string InputCheck, bool NotFoundDisp, string FrmName)
        {
            string outBuff = ""; string[] Val; string[] Lbl; int i = 0; int j = 0; bool Found = false;
            Lbl = TextStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Val = ValStr.Split(new string[] { "||" }, StringSplitOptions.None);
            j = (Val.Length <= Lbl.Length ? Val.Length : Lbl.Length);
            if (j > RowMaxSelect) // Div radio
            {
                IsSelect = false;
                outBuff = outBuff + UIActbStrIdGen(InputName, Val, Lbl, InputCheck, " size=43 ", "", FrmName);
            }
            else // Select box
            {
                IsSelect = true;
                for (i = 0; i < j; i++)
                {
                    outBuff = outBuff + "<option value=\"" + Val[i] + "\"";
                    if (Val[i] == InputCheck)
                    {
                        Found = true;
                        outBuff = outBuff + " selected";
                        InputCheck = InputCheck + " - " + Lbl[i];
                    }
                    outBuff = outBuff + ">" + Lbl[i] + "</option>";

                }
                if (NotFoundDisp && !Found && InputCheck != "")
                {
                    outBuff = outBuff + "<option value=\"" + InputCheck + "\" selected></option>";//[" + InputCheck + "]
                }
            }

            return outBuff;
        }
        public static string UISelectStr(string InputName, string ValStr, string TextStr, ref string InputCheck, bool NotFoundDisp, string Param, string OnChange, string frm = "document.bosfrm") {
            bool IsSelect = true;
            string outBuff = SelectStrOption (ref IsSelect, InputName, OnChange, ValStr, TextStr, ref InputCheck, NotFoundDisp, frm);
            if (IsSelect)
                return Environment.NewLine + "<select class=\"select2 form-control\" id=\"" + InputName + "\" name=\"" + InputName +
                "\" " + Param + (OnChange != "" ? " onChange=\"" + OnChange + "\"" : "") + ">" + outBuff + "</select>";
            else
                return outBuff;
                //return Environment.NewLine + "<input name=\"txt_" + InputName + "\" size=\"43\" value=\"" + InputCheck + "\" " + Param + " readonly>" +
                //Environment.NewLine + "<img src=\"/images/Icon_Select.gif\" Class=\"imggo\" id=\"" + InputName + "img\" onclick=\"uiddmh(" + frm + ",'" + InputName + "');\">" +
                //Environment.NewLine + "<div id=\"" + InputName + "_div\" style=\"background:white;border:1px solid #ccc;display:none;height:200px;width:357px;overflow:auto\">" + //" + Width + "
                //outBuff + "</div>";
        }
        public static string UISelectStr(string InputName, string ValStr, string TextStr, ref string InputCheck, bool NotFoundDisp, string Param, string OnChange, bool IsMultiple)
        {
            return Environment.NewLine + "<select " + (IsMultiple ? "multiple=\"multiple\"" : "") + " class=\"select2 form-control\" id=\"" + InputName + "\" name=\"" + InputName +
                "\" " + Param + (OnChange != "" ? " onChange=\"" + OnChange + "\"" : "") + ">" + SelectStrOption(ValStr, TextStr, ref InputCheck, NotFoundDisp) + "</select>";
        }
        public static void OptionStringVal(ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, 
            ref string ValStr, ref string TextStr, ref string ParentStr)
        {
            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d = null; string[] a = ColumnName.Split(new string[] { "," }, StringSplitOptions.None);
            if (ProcParam != "") d = JObject.Parse(ProcParam);
            toolDAO.ExecuteStore("Select", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse(json);
            ValStr = ""; TextStr = ""; ParentStr = "";
            for (int i = 0; i < d.Select.Items.Count; i++)
            {
                ValStr = ValStr + "||" + Tools.GetDataJson(d.Select.Items[i], a[0]);
                TextStr = TextStr + "||" + Tools.GetDataJson(d.Select.Items[i], a[1]);
                if (a.Length > 2)
                    ParentStr = ParentStr + "||" + Tools.GetDataJson(d.Select.Items[i], a[2]);
                else
                    ParentStr = ParentStr + "||";
            }
            if (ValStr.Length > 2)
            {
                ValStr = Tools.RemoveFisrtChar(ValStr, 2);
                TextStr = Tools.RemoveFisrtChar(TextStr, 2);
                ParentStr = Tools.RemoveFisrtChar(ParentStr, 2);
            }
        }
        public static string UISelect (string InputName, ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, string InputValue, bool NotFoundDisp, string Param, string OnChange, bool IsMultiple = false) {
            string r1 = "";
            r1 = Environment.NewLine + "<select " + (IsMultiple ? "multiple=\"multiple\"" : "") + " class=\"select2 form-control tree-search\" id=\"" + InputName + "\" name=\"" + InputName +
                "\" " + Param + (OnChange != "" ? " onChange=\"" + OnChange + "\"" : "") + ">" + SelectOption(ref toolDAO, ProcName, ProcParam, ColumnName, InputValue, NotFoundDisp) + "</select>";
            return r1;
        }
        public static string UITreeview(string InputName, ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName, string InputValue, string OnChange, bool IsMultiple = false)
        {
            StringBuilder r = new StringBuilder(); string r1 = "";
            r.Append(Environment.NewLine + "<select " + (IsMultiple ? "multiple=\"multiple\"" : "") + " " +
                "class=\"form-control tree-search\" " +
                "id=\"" + InputName + "\" name=\"" + InputName +
                "\" " + (OnChange != "" ? " onChange=\"" + OnChange + "\"" : "") + ">");

            dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            string[] a = ColumnName.Split(new string[] { "," }, StringSplitOptions.None);
            if (ProcParam != "") d = JObject.Parse(ProcParam);
            toolDAO.ExecuteStore("UITree", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse(json);
            for (int i = 0; i < d.UITree.Items.Count; i++)
            {
                string a_0 = Tools.GetDataJson(d.UITree.Items[i], a[0]);
                string a_1 = Tools.GetDataJson(d.UITree.Items[i], a[1]);
                string a_OrderBy = Tools.GetDataJson(d.UITree.Items[i], "OrderBy");
                string a_Parent = Tools.GetDataJson(d.UITree.Items[i], "ParentID");
                int a_Cnt = Tools.GetDataJson(d.UITree.Items[i], "Cnt", "int");
                string[] nodeLevel = a_OrderBy.Split(new string[] { "-" }, StringSplitOptions.None);
                int NodeLevel = nodeLevel.Length;
                r.Append("<option " +
                    "data-id=\"l" + nodeLevel[nodeLevel.Length - 1] + "\" " + 
                    //"onclick=\"" + OnChange + "\" " +
                    "data-pup=\"" + a_Parent + "\" " +
                    "value=\"" + Tools.GetDataJson(d.UITree.Items[i], a[0]) + "\" " +
                    "class=\"showme opened " + // Open
                    "l" + NodeLevel.ToString() + " " + // Level
                    "l" + a_OrderBy.Replace("-", " l") + " " + // Class parent
                    (a_Cnt>0? "non-leaf" :"") + "\""); // Point and End Class
                if (("," + Tools.GetDataJson(d.UITree.Items[i], a[0]) + ",").IndexOf("," + InputValue + ",") > -1) r.Append(" selected");
                r.Append(">" + Tools.GetDataJson(d.UITree.Items[i], a[1]) + "</option>");
            }

            r.Append(Environment.NewLine + "</select>" +
                Environment.NewLine + "<script>" +
                "$(\"#" + InputName + "\").select2ToTree();" +
                "</script>");
            // return data
            r1 = r.ToString(); r = null;
            return r1;
        }
        #endregion

        #region UIAttach
        public static string UIAttachImage(string InputName, string ImgDefault, string InputLable = "File-size: Maxlenght")
        {
            string r = ""; StringBuilder r1 = new StringBuilder();
            r1.Append(Environment.NewLine + "<img id=\"Img" + InputName + "\" name=\"Img" + InputName + "\" src=\"" + ImgDefault + "\" height=50>");
            r1.Append(Environment.NewLine + "<input type=\"file\" id=\"" + InputName + "\" name=\"" + InputName +
                "\" onchange=\"readURL(this);\" accept=\"image/png,image/jpeg,image/gif\"> " + InputLable);
            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            r1.Append(Environment.NewLine + "function readURL(input) {");
            r1.Append(Environment.NewLine + "if (input.files && input.files[0]) {");
            r1.Append(Environment.NewLine + "var reader = new FileReader();");
            r1.Append(Environment.NewLine + "reader.onload = function (e) {");
            r1.Append(Environment.NewLine + "var element = document.getElementById('Img" + InputName + "');element.src = e.target.result;element.height = 50;");
            r1.Append(Environment.NewLine + "};reader.readAsDataURL(input.files[0]);}}");
            r1.Append(Environment.NewLine + "</script>");

            r = r1.ToString();
            return r;
        }
        public static string UIAttachFiles(string InputName, string InputLable = "File-size: Maxlenght")
        {
            string r = ""; StringBuilder r1 = new StringBuilder();
            r1.Append(Environment.NewLine + "<input type=\"file\" id=\"" + InputName + "\" name=\"" + InputName +
               "\" onchange=\"readURL(this);\" accept=\"image/png,image/jpeg,image/gif\"> " + InputLable);
            r1.Append(Environment.NewLine + "<script language=\"javascript\">");
            r1.Append(Environment.NewLine + "function readURL(input) {return;}");
            r1.Append(Environment.NewLine + "</script>");
            r = r1.ToString();
            return r;
        }
        #endregion

        #region SetWorksheetValue
        public static void SetCellFormula(ref OfficeOpenXml.ExcelWorksheet worksheet, int i, string Formula, bool IsValue = true,
            bool IsFontBold = false, string CellsColor = "",
            int iCol = 0, string colName = "", string TabIndex = "", int l2 = 0,
            bool IsReadOnly = true)
        {
            worksheet.Column(i).Style.Locked = true;

            for (int j = 4; j < Rows; j++)
            {
                if (IsFontBold || CellsColor != "")
                {
                    worksheet.Cells[j, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (IsFontBold) worksheet.Cells[j, i].Style.Font.Bold = true;
                    if (CellsColor == "Yellow")
                        worksheet.Cells[j, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    else if (CellsColor == "Red")
                        worksheet.Cells[j, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                }
                if (IsValue)
                {
                    worksheet.Cells[j, i].Value = Formula;
                    if (!IsReadOnly)
                    {
                        worksheet.ProtectedRanges.Add("editable", new OfficeOpenXml.ExcelAddress(worksheet.Cells[4, i, Rows, i].Address));
                        worksheet.Cells[4, i, Rows, i].Style.Locked = false;
                    }
                }
                else
                {
                    if (Formula == "VLOOKUP")
                        worksheet.Cells[j, i].Formula = "=VLOOKUP(" + worksheet.Cells[j, iCol].Address + "," + TabIndex + "_" + colName + "!$C:$ZZ, " + (l2 - 2) + ", FALSE)";
                    else if (Formula == "VLOOKUP1")
                        worksheet.Cells[j, i].Formula = "=VLOOKUP(" + worksheet.Cells[j, iCol].Address + "," + TabIndex + "_" + colName + "!$B:$C, 2, FALSE)";
                    else
                        worksheet.Cells[j, i].Formula = Formula;
                }
            }
        }
        public static void SetCellVall(ref OfficeOpenXml.ExcelWorksheet worksheet, XlsxForm[] xlsx, int i, ref int j, string[] a2, string TabIndex, bool IsReadOnly = true)
        {
            int l = 0; int l1 = -1; int l2 = -1; bool kt = false;
            while (!kt && l < xlsx.Length)
            {
                if (xlsx[l] != null)
                {
                    l1 = xlsx[l].GetColPos(a2[i]);
                    if (l1 > -1)
                    {
                        l2 = xlsx[l].GetColParamPos(a2[i]);
                    }
                    if (l1 > -1 && l2 > -1) kt = true;
                }
                l++;
            }
            if (kt)
            {
                l--;
                //worksheet.Cells[4, (i + j)].Formula = "=VLOOKUP(" + worksheet.Cells[4, xlsx[l].iCol].Address + "," + TabIndex + "_" + xlsx[l].colName + "!$C:$ZZ, " + (l2-2) + ", FALSE)";
                SetCellFormula(ref worksheet, (i + j), "VLOOKUP", false, false, "", xlsx[l].iCol, xlsx[l].colName, TabIndex, l2);
                //SetCellFormula(ref OfficeOpenXml.ExcelWorksheet worksheet, int i, string Formula, bool IsValue = true,
                //  bool IsFontBold = false, string CellsColor = "",
                //  int iCol = 0, string colName = "", string TabIndex = "", int l2 = 0)
            }
            if (!IsReadOnly && !kt)
            {
                worksheet.ProtectedRanges.Add("editable", new OfficeOpenXml.ExcelAddress(worksheet.Cells[4, (i + j), Rows, (i + j)].Address));
                worksheet.Cells[4, (i + j), Rows, (i + j)].Style.Locked = false;
            }
        }
        public static void SetCellVall(ref HRSContext context, ref OfficeOpenXml.ExcelWorksheet worksheet, XlsxFormSelect[] xlsx, int i, ref int j, string[] a2, string TabIndex, ref bool IsSetValidation)
        {
            int l = 0;
            while (IsSetValidation && l < xlsx.Length)
            {
                if (xlsx[l] != null)
                {
                    if (xlsx[l].colNameChild == a2[i] + "_" + i) IsSetValidation = false;
                }
                l++;
            }
            if (!IsSetValidation)//" + Rows
            {
                l--;
                SetValidation(ref context, ref worksheet, worksheet.Cells[4, (xlsx[l].iColChild + 1), Rows, (xlsx[l].iColChild + 1)].Address, "=OFFSET(" + TabIndex + "_" + a2[i] + "!$A$1,MATCH(" + worksheet.Cells[4, xlsx[l].iCol].Address + "," + OffSet(TabIndex + "_" + a2[i], "A", "1") + ",0)-1,1,COUNTIF(" + OffSet(TabIndex + "_" + a2[i], "A", "1") + "," + worksheet.Cells[4, xlsx[l].iCol].Address + "),1)");
                //worksheet.Cells[4, xlsx[l].iColChild].Formula = "=OFFSET(" + TabIndex + "_" + a2[i] + ",MATCH(" + worksheet.Cells[4, xlsx[l].iCol].Address + "," + OffSet(TabIndex + "_" + a2[i], "A", "1") + ",0)-1,1,COUNTIF(" + OffSet(TabIndex + "_" + a2[i], "A", "1") + "," + worksheet.Cells[4, xlsx[l].iCol].Address + "),1)"; 
                // " = VLOOKUP(" + worksheet.Cells[4, xlsx[l].iCol].Address + "," + TabIndex + "_" + xlsx[l].colName + "!$C:$ZZ, " + xlsx[l].iColChild + ", FALSE)";
            }
        }
        public static void SetCellVal(ref OfficeOpenXml.ExcelWorksheet worksheet, ref OfficeOpenXml.ExcelPackage package,
            ref ToolDAO toolDAO, int i, ref int j, string TabIndex, string[] a2, bool IsRequire, HRSContext context,
            string a31_4, string a31_3, string a31_5, string a31_7, string a31_11, ref XlsxForm[] xlsx)
        {
            xlsx[i] = new XlsxForm();
            xlsx[i].colList = a31_11.Split(new string[] { "||" }, StringSplitOptions.None);
            xlsx[i].colParamList = a31_5.Split(new string[] { "||" }, StringSplitOptions.None);
            xlsx[i].colName = a2[i];
            xlsx[i].iCol = (i + j);
            worksheet.Column(i + j).Hidden = true;
            OfficeOpenXml.ExcelWorksheet worksheet1;
            try { worksheet1 = package.Workbook.Worksheets.Add(TabIndex + "_" + a2[i]); } catch { worksheet1 = package.Workbook.Worksheets[TabIndex + "_" + a2[i]]; }
            worksheet1.Protection.IsProtected = true;
            worksheet1.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
            string ReqJson = context.InputDataSetParam(a31_4, a31_7);
            SetWorksheetValueSearch(ref worksheet1, context, ref toolDAO, a31_3, ReqJson, a31_5);
            SetCellsDataValidation(ref worksheet, i, ref j, TabIndex, a2, IsRequire, context);
        }
        public static void SetCellVal(ref OfficeOpenXml.ExcelWorksheet worksheet, ref OfficeOpenXml.ExcelPackage package,
            ref ToolDAO toolDAO, int i, ref int j, string TabIndex, string[] a2, bool IsRequire, HRSContext context,
            string a31_4, string a31_3, string a31_5, string a31_6, ref XlsxFormSelect[] xlsxSelect)
        {
            int iChild = -1;
            if (a31_6.Length > 8)
            {
                if (a31_6.Substring(0, 8) == "getChild")
                {
                    string[] a31_6i = a31_6.Split(new string[] { "," }, StringSplitOptions.None);
                    if (a31_6i.Length > 3)
                        iChild = int.Parse(a31_6i[3]);
                    else
                        iChild = (i + 1);
                }
            }
            if (iChild > 0)
            {
                xlsxSelect[i] = new XlsxFormSelect();
                xlsxSelect[i].colNameChild = a2[iChild] + "_" + iChild;
                xlsxSelect[i].colName = a2[i] + "_" + i;
                xlsxSelect[i].iCol = (i + j);
                xlsxSelect[i].iColChild = (iChild + j + 1);
            }
            worksheet.Column(i + j).Hidden = true;
            OfficeOpenXml.ExcelWorksheet worksheet1;
            try { worksheet1 = package.Workbook.Worksheets.Add(TabIndex + "_" + a2[i]); } catch { worksheet1 = package.Workbook.Worksheets[TabIndex + "_" + a2[i]]; }
            worksheet1.Protection.IsProtected = true;
            worksheet1.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
            string ReqJson = context.InputDataSetParam(a31_4);
            SetWorksheetValue(ref worksheet1, context, ref toolDAO, a31_3, ReqJson, a31_5);
            bool IsSetValidation = true;
            SetCellVall(ref context, ref worksheet, xlsxSelect, i, ref j, a2, TabIndex, ref IsSetValidation);
            SetCellsDataValidation(ref worksheet, i, ref j, TabIndex, a2, IsRequire, context, IsSetValidation);
        }
        public static void SetCellVal(ref OfficeOpenXml.ExcelWorksheet worksheet, ref OfficeOpenXml.ExcelPackage package,
            ref ToolDAO toolDAO, int i, ref int j, string TabIndex, string[] a2, bool IsRequire, HRSContext context,
            string a31_4, string a31_3, string a31_5)
        {
            worksheet.Column(i + j).Hidden = true;
            OfficeOpenXml.ExcelWorksheet worksheet1;
            try { worksheet1 = package.Workbook.Worksheets.Add(TabIndex + "_" + a2[i]); } catch { worksheet1 = package.Workbook.Worksheets[TabIndex + "_" + a2[i]]; }
            worksheet1.Protection.IsProtected = true;
            worksheet1.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
            string ReqJson = context.InputDataSetParam(a31_4);
            SetWorksheetValue(ref worksheet1, context, ref toolDAO, a31_3, ReqJson, a31_5);
            SetCellsDataValidation(ref worksheet, i, ref j, TabIndex, a2, IsRequire, context);
        }
        public static void SetCellVal(ref OfficeOpenXml.ExcelWorksheet worksheet, ref OfficeOpenXml.ExcelPackage package,
            string ValStr, string TextStr, int i, ref int j, string TabIndex, string[] a2, bool IsRequire, HRSContext context)
        {
            worksheet.Column(i + j).Hidden = true;
            OfficeOpenXml.ExcelWorksheet worksheet1;
            try { worksheet1 = package.Workbook.Worksheets.Add(TabIndex + "_" + a2[i]); } catch { worksheet1 = package.Workbook.Worksheets[TabIndex + "_" + a2[i]]; }
            worksheet1.Protection.IsProtected = true;
            worksheet1.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
            SetWorksheetValue(ref worksheet1, context, ValStr, TextStr, a2[i]);
            SetCellsDataValidation(ref worksheet, i, ref j, TabIndex, a2, IsRequire, context);
        }
        private static void SetValidation(ref HRSContext _context, ref OfficeOpenXml.ExcelWorksheet worksheet, string CellAddress, string ExcelFormula)
        {
            try
            {
                var validation = worksheet.DataValidations.AddListValidation(CellAddress);
                validation.ShowErrorMessage = true;
                validation.ErrorStyle = OfficeOpenXml.DataValidation.ExcelDataValidationWarningStyle.warning;
                validation.ErrorTitle = _context.GetLanguageLable("InvalidValue");//"An invalid value was entered";//
                validation.Error = _context.GetLanguageLable("EnterValue");//"Select a value from the list";//
                                                                           //validation.Formula.ExcelFormula = "" + TabIndex + "_" + a2[i] + "!$B:$B";// "" + TabIndex + "_" + a2[i] + "!$B$3:$B"
                validation.Formula.ExcelFormula = ExcelFormula; // "=" + OffSet(TabIndex + "_" + a2[i]);
                                                                //"=OFFSET(" + TabIndex + "_" + a2[i] + "!$B$3,0,0,COUNTA(" + TabIndex + "_" + a2[i] + "!$B:$B)-1,1)";// "" + TabIndex + "_" + a2[i] + "!$B$3:$B$" + Rows;
            }
            catch
            {

            }
        }
        private static void SetCellsDataValidation(ref OfficeOpenXml.ExcelWorksheet worksheet, int i, ref int j, string TabIndex, string[] a2, bool IsRequire,
            HRSContext context, bool IsSetValidation = true)
        {
            //worksheet.Cells[4, (i + j)].Formula = "=VLOOKUP(" + worksheet.Cells[4, (i + j + 1)].Address + "," + TabIndex + "_" + a2[i] + "!$B:$C, 2, FALSE)";
            SetCellFormula(ref worksheet, (i + j), "VLOOKUP1", false, false, "", (i + j + 1), a2[i], TabIndex, 2);
            j = j + 1;

            worksheet.ProtectedRanges.Add("editable", new OfficeOpenXml.ExcelAddress(worksheet.Cells[4, (i + j), Rows, (i + j)].Address));
            worksheet.Cells[4, (i + j), Rows, (i + j)].Style.Locked = false;
            worksheet.Cells[1, (i + j)].Style.Font.Bold = true;
            worksheet.Cells[1, (i + j)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            if (IsRequire)
            {
                worksheet.Cells[1, (i + j)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                worksheet.Cells[1, (i + j)].Value = context.GetLanguageLable("Selectbox") + " " + context.GetLanguageLable("IsRequire");
            }
            else
            {
                worksheet.Cells[1, (i + j)].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                worksheet.Cells[1, (i + j)].Value = context.GetLanguageLable("Selectbox");
            }
            //worksheet.Cells[1, (i + j)].AutoFitColumns();
            worksheet.Column((i + j)).AutoFit();

            worksheet.Cells[2, (i + j)].Value = context.GetLanguageLable(a2[i]);// + "Lockup";
            worksheet.Cells[2, (i + j)].Style.Font.Bold = true;
            //worksheet.Cells[2, (i + j)].AutoFitColumns();
            worksheet.Cells[2, (i + j)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[3, (i + j)].Value = a2[i] + "Lockup";
            if (IsSetValidation) SetValidation(ref context, ref worksheet, worksheet.Cells[4, (i + j), Rows, (i + j)].Address, "=" + OffSet(TabIndex + "_" + a2[i]));
            /*
            var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[4, (i + j), Rows, (i + j)].Address);
            validation.ShowErrorMessage = true;
            validation.ErrorStyle = OfficeOpenXml.DataValidation.ExcelDataValidationWarningStyle.warning;
            validation.ErrorTitle = "An invalid value was entered";//_context.GetLanguageLable("InvalidValue")
            validation.Error = "Select a value from the list";//_context.GetLanguageLable("EnterValue")
            //validation.Formula.ExcelFormula = "" + TabIndex + "_" + a2[i] + "!$B:$B";// "" + TabIndex + "_" + a2[i] + "!$B$3:$B"
            validation.Formula.ExcelFormula = "=" + OffSet(TabIndex + "_" + a2[i]);
                //"=OFFSET(" + TabIndex + "_" + a2[i] + "!$B$3,0,0,COUNTA(" + TabIndex + "_" + a2[i] + "!$B:$B)-1,1)";// "" + TabIndex + "_" + a2[i] + "!$B$3:$B$" + Rows; 
            */
        }
        private static string OffSet(string SheetName, string ColName = "B", string StartRow = "3")
        {
            return "OFFSET(" + SheetName + "!$" + ColName + "$" + StartRow + ",0,0,COUNTA(" + SheetName + "!$" + ColName + ":$" + ColName + ")-1,1)";
            // "" + SheetName + "!$" + ColName + "$3:$" + ColName + "$" + Rows;
        }
        private static void SetWorksheetValue(ref OfficeOpenXml.ExcelWorksheet worksheet1, HRSContext context,
            ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName)
        {
            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d = null; string[] a = ColumnName.Split(new string[] { "," }, StringSplitOptions.None);
            if (ProcParam != "") d = JObject.Parse(ProcParam);
            toolDAO.ExecuteStore("Select", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse(json);
            worksheet1.Cells[1, 2].Value = context.GetLanguageLable(a[1]);
            worksheet1.Cells[2, 2].Value = a[1];
            worksheet1.Cells[1, 3].Value = context.GetLanguageLable(a[0]);
            worksheet1.Cells[2, 3].Value = a[0];
            worksheet1.Row(2).Hidden = true;
            worksheet1.Column(1).Hidden = true;
            if (a.Length > 2)
            {
                worksheet1.Cells[1, 1].Value = context.GetLanguageLable(a[2]);
                worksheet1.Cells[2, 1].Value = a[2];
                //worksheet1.Column(1).AutoFit();
                for (int i = 3; i < a.Length; i++)
                {
                    worksheet1.Cells[1, i + 1].Value = context.GetLanguageLable(a[i]);
                    worksheet1.Column(i + 1).AutoFit();
                    worksheet1.Cells[2, i + 1].Value = a[i];
                }
            }
            else
            {
                worksheet1.Cells[1, 1].Value = "NULL";
                worksheet1.Cells[2, 1].Value = "NULL";
            }
            int j = 0;
            for (j = 0; j < d.Select.Items.Count; j++)
            {
                worksheet1.Cells[j + 3, 2].Value = Tools.GetDataJson(d.Select.Items[j], a[1]).Replace("&nbsp;", "  ");
                worksheet1.Cells[j + 3, 3].Value = Tools.GetDataJson(d.Select.Items[j], a[0]).Replace("&nbsp;", "  ");
                //worksheet1.Cells[j + 3, 2].AutoFitColumns();
                //worksheet1.Cells[j + 3, 3].AutoFitColumns();
                if (a.Length > 2)
                {
                    string[] val = Tools.GetDataJson(d.Select.Items[j], a[2]).Replace("&nbsp;", "  ").Split(new string[] { ":" }, StringSplitOptions.None);
                    worksheet1.Cells[j + 3, 1].Value = val[0];
                    for (int i = 3; i < a.Length; i++)
                    {
                        worksheet1.Cells[j + 3, i + 1].Value = Tools.GetDataJson(d.Select.Items[j], a[i]).Replace("&nbsp;", "  ");
                        //worksheet1.Cells[j + 3, i + 1].AutoFitColumns();
                    }
                    if (a.Length > 3)
                    {
                        if (val.Length > 1) worksheet1.Cells[j + 3, a.Length].Value = val[1];
                    }
                    else
                    {
                        if (val.Length > 1) worksheet1.Cells[j + 3, a.Length + 1].Value = val[1];
                    }
                }
                else
                {
                    worksheet1.Cells[j + 3, 1].Value = "NULL";
                }
            }
            //worksheet1.Cells[j + 3, 2].Value = " ";
            //worksheet1.Cells[j + 3, 3].Value = " ";
            //worksheet1.Cells[j + 3, 1].Value = " ";
        }
        private static void SetWorksheetValueSearch(ref OfficeOpenXml.ExcelWorksheet worksheet1, HRSContext context,
            ref ToolDAO toolDAO, string ProcName, string ProcParam, string ColumnName)
        {
            string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = "";
            dynamic d = null; string[] a = ColumnName.Split(new string[] { "||" }, StringSplitOptions.None);
            if (ProcParam != "") d = JObject.Parse(ProcParam);
            toolDAO.ExecuteStore("Select", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
            d = JObject.Parse(json);
            worksheet1.Cells[1, 2].Value = context.GetLanguageLable(a[2]);
            worksheet1.Cells[2, 2].Value = a[2];
            //worksheet1.Cells[1, 2].AutoFitColumns();
            worksheet1.Column(2).AutoFit();
            worksheet1.Cells[1, 3].Value = context.GetLanguageLable(a[1]);
            //worksheet1.Cells[1, 3].AutoFitColumns();
            worksheet1.Column(3).AutoFit();
            worksheet1.Cells[2, 3].Value = a[1];
            worksheet1.Row(2).Hidden = true;
            worksheet1.Column(1).Hidden = true;
            if (a.Length > 3)
            {
                worksheet1.Cells[1, 1].Value = context.GetLanguageLable(a[3]);
                //worksheet1.Cells[1, 1].AutoFitColumns();
                worksheet1.Column(2).AutoFit();
                worksheet1.Column(3).AutoFit();
                worksheet1.Cells[2, 1].Value = a[3];
                for (int i = 4; i < a.Length; i++)
                {
                    worksheet1.Cells[1, i].Value = context.GetLanguageLable(a[i]);
                    //worksheet1.Cells[1, i].AutoFitColumns();
                    worksheet1.Column(i).AutoFit();
                    worksheet1.Cells[2, i].Value = a[i];
                }
            }
            else
            {
                worksheet1.Cells[1, 1].Value = "NULL";
                worksheet1.Cells[2, 1].Value = "NULL";
            }
            int j = 0;
            for (j = 0; j < d.Select.Items.Count; j++)
            {
                worksheet1.Cells[j + 3, 2].Value = Tools.GetDataJson(d.Select.Items[j], a[2]);
                worksheet1.Cells[j + 3, 3].Value = Tools.GetDataJson(d.Select.Items[j], a[1]);
                //worksheet1.Cells[j + 3, 2].AutoFitColumns();
                //worksheet1.Cells[j + 3, 3].AutoFitColumns();
                if (a.Length > 3)
                {
                    worksheet1.Cells[j + 3, 1].Value = Tools.GetDataJson(d.Select.Items[j], a[3]);
                    for (int i = 4; i < a.Length; i++)
                    {
                        worksheet1.Cells[j + 3, i].Value = Tools.GetDataJson(d.Select.Items[j], a[i]);
                        //worksheet1.Cells[j + 3, i].AutoFitColumns();                        
                    }
                }
                else
                {
                    worksheet1.Cells[j + 3, 1].Value = "NULL";
                }
            }
            //worksheet1.Cells[j + 3, 2].Value = " ";
            //worksheet1.Cells[j + 3, 3].Value = " ";
            //worksheet1.Cells[j + 3, 1].Value = " ";
        }
        private static void SetWorksheetValue(ref OfficeOpenXml.ExcelWorksheet worksheet1, HRSContext context,
            string ValStr, string TextStr, string col)
        {
            string[] Val; string[] Lbl; int i = 0; int j = 0;
            Lbl = TextStr.Split(new string[] { "||" }, StringSplitOptions.None);
            Val = ValStr.Split(new string[] { "||" }, StringSplitOptions.None);
            j = (Val.Length <= Lbl.Length ? Val.Length : Lbl.Length);
            worksheet1.Cells[1, 3].Value = context.GetLanguageLable(col) + "ID";
            worksheet1.Cells[2, 3].Value = col;
            worksheet1.Cells[1, 2].Value = context.GetLanguageLable(col) + "Name";
            worksheet1.Cells[2, 2].Value = col + "Name";
            worksheet1.Row(2).Hidden = true;
            worksheet1.Column(1).Hidden = true;
            worksheet1.Column(2).AutoFit();
            worksheet1.Column(3).AutoFit();
            for (i = 0; i < j; i++)
            {
                worksheet1.Cells[i + 3, 3].Value = Val[i];
                worksheet1.Cells[i + 3, 2].Value = Lbl[i];
                //worksheet1.Cells[i + 3, 3].AutoFitColumns();
                //worksheet1.Cells[i + 3, 2].AutoFitColumns();
            }
            //worksheet1.Cells[j + 3, 2].Value = " ";
            //worksheet1.Cells[j + 3, 1].Value = " ";
        }
        #endregion

        #region UI Header || Footer || Menu - Private
        private static string UINotifications(ref ToolDAO toolDAO, HRSContext context)
        {
            StringBuilder builder = new StringBuilder();
            // <!-- Notifications: style can be found in dropdown.less -->
            builder.Append(Environment.NewLine + "<li class=\"noti\" data-rel=\"noti\">");
            builder.Append(Environment.NewLine + "<a><span class=\"num oranges\">16</span></a>");
            builder.Append(Environment.NewLine + "<ul class=\"dropdown-menu\"  id=\"noti\">");
            builder.Append(Environment.NewLine + "<li class=\"headermenu\">" + context.GetLanguageLable("You have 10 notifications") + "</li>");
            builder.Append(Environment.NewLine + "<li>" +
                Environment.NewLine + "<ul class=\"menu\">" +
                Environment.NewLine + "<li>" +
                Environment.NewLine + "<a href=\"#\">" +
                Environment.NewLine + "<i class=\"fa fa-users text-aqua\"></i> 5 new members joined today" +
                Environment.NewLine + "</a>" +
                Environment.NewLine + "</li>" +
                Environment.NewLine + "<li>" +
                Environment.NewLine + "<a href=\"#\">" +
                Environment.NewLine + "<i class=\"fa fa-users text-aqua\"></i> Very long description here that may not fit into the page and may cause design problems" +
                Environment.NewLine + "</a>" +
                Environment.NewLine + "</li>" +
                Environment.NewLine + "</ul>" +
                Environment.NewLine + "</li>");
            builder.Append(Environment.NewLine + "<li class=\"footer\"><a href=\"#\">" + context.GetLanguageLable("View all") + "</a></li>");
            builder.Append(Environment.NewLine + "</ul>");
            builder.Append(Environment.NewLine + "</li>");
            string r = builder.ToString(); builder = null;
            return r;
        }
        ////private static string UINotifications_NoUse(ref ToolDAO toolDAO, HRSContext context)
        ////{
        ////    StringBuilder builder = new StringBuilder();
        ////    // <!-- Notifications: style can be found in dropdown.less -->
        ////    builder.Append(Environment.NewLine + "<li class=\"dropdown notifications-menu\">");
        ////    builder.Append(Environment.NewLine + "<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">");
        ////    builder.Append(Environment.NewLine + "<i class=\"fa fa-bell-o\"></i><span class=\"label bg-red\">10</span></a>");
        ////    builder.Append(Environment.NewLine + "<ul class=\"dropdown-menu\">");
        ////    builder.Append(Environment.NewLine + "<li class=\"header\">" + context.GetLanguageLable("You have 10 notifications") + "</li>");
        ////    builder.Append(Environment.NewLine + "<li>" +
        ////        Environment.NewLine + "<ul class=\"menu\">" +
        ////        Environment.NewLine + "<li>" +
        ////        Environment.NewLine + "<a href=\"#\">" +
        ////        Environment.NewLine + "<i class=\"fa fa-users text-aqua\"></i> 5 new members joined today" +
        ////        Environment.NewLine + "</a>" +
        ////        Environment.NewLine + "</li>" +
        ////        Environment.NewLine + "<li>" +
        ////        Environment.NewLine + "<a href=\"#\">" +
        ////        Environment.NewLine + "<i class=\"fa fa-users text-aqua\"></i> 5 new members joined today" +
        ////        Environment.NewLine + "</a>" +
        ////        Environment.NewLine + "</li>" +
        ////        Environment.NewLine + "</ul>" +
        ////        Environment.NewLine + "</li>");
        ////    builder.Append(Environment.NewLine + "<li class=\"footer\"><a href=\"#\">" + context.GetLanguageLable("View all") + "</a></li>");
        ////    builder.Append(Environment.NewLine + "</ul>");
        ////    builder.Append(Environment.NewLine + "</li>");
        ////    string r = builder.ToString(); builder = null;
        ////    return r;
        ////}
        private static string UIMessages(ref ToolDAO toolDAO, HRSContext context)
        {
            StringBuilder builder = new StringBuilder();
            // <!-- Notifications: style can be found in dropdown.less -->
            builder.Append(Environment.NewLine + "<li class=\"email\" data-rel=\"email\">");
            builder.Append(Environment.NewLine + "<a><span class=\"num green\">28</span></a>");
            builder.Append(Environment.NewLine + "<ul class=\"dropdown-menu\" id=\"email\">");
            builder.Append(Environment.NewLine + "<li class=\"headermenu\">" + context.GetLanguageLable("You have 4 messages") + "</li>");
            builder.Append(Environment.NewLine + "<li>" +
                Environment.NewLine + "<ul class=\"menu\">" +
                Environment.NewLine + "<li>" +
                Environment.NewLine + "<a href=\"#\">" +
                Environment.NewLine + "<i class=\"fa fa-users text-aqua\"></i> 5 new members joined today" +
                Environment.NewLine + "</a>" +
                Environment.NewLine + "</li>" +
                Environment.NewLine + "<li>" +
                Environment.NewLine + "<a href=\"#\">" +
                Environment.NewLine + "<i class=\"fa fa-users text-aqua\"></i> Very long description here that may not fit into the page and may cause design problems" +
                Environment.NewLine + "</a>" +
                Environment.NewLine + "</li>" +
                Environment.NewLine + "</ul>" +
                Environment.NewLine + "</li>");
            builder.Append(Environment.NewLine + "<li class=\"footer\"><a href=\"#\">" + context.GetLanguageLable("View all") + "</a></li>");
            builder.Append(Environment.NewLine + "</ul>");
            builder.Append(Environment.NewLine + "</a>");
            builder.Append(Environment.NewLine + "</li>");
            string r = builder.ToString(); builder = null;
            return r;
        }
        ////private static string UIMessages_NoUse(ref ToolDAO toolDAO, HRSContext context)
        ////{
        ////    StringBuilder builder = new StringBuilder();
        ////    // <!-- Notifications: style can be found in dropdown.less -->
        ////    builder.Append(Environment.NewLine + "<li class=\"dropdown messages-menu\">");
        ////    builder.Append(Environment.NewLine + "<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">");
        ////    builder.Append(Environment.NewLine + "<i class=\"fa fa-envelope-o\"></i><span class=\"label label-success\">4</span></a>");
        ////    builder.Append(Environment.NewLine + "<ul class=\"dropdown-menu\">");
        ////    builder.Append(Environment.NewLine + "<li class=\"header\">" + context.GetLanguageLable("You have 4 messages") + "</li>");
        ////    builder.Append(Environment.NewLine + "<li>" +
        ////        Environment.NewLine + "<ul class=\"menu\">" +
        ////        Environment.NewLine + "<li>" +
        ////        Environment.NewLine + "<a href=\"#\">" +
        ////        Environment.NewLine + "<div class=\"pull-left\"><img src=\"/images/user-avatar.png\" class=\"img-circle\" alt=\"User Image\"></div>" +
        ////        Environment.NewLine + "<h4>Support Team<small><i class=\"fa fa-clock-o\"></i> 5 mins</small></h4>" +
        ////        Environment.NewLine + "</a>" +
        ////        Environment.NewLine + "</li>" +
        ////        Environment.NewLine + "<li>" +
        ////        Environment.NewLine + "<a href=\"#\">" +
        ////        Environment.NewLine + "<div class=\"pull-left\"><img src=\"/images/user-avatar.png\" class=\"img-circle\" alt=\"User Image\"></div>" +
        ////        Environment.NewLine + "<h4>Support Team<small><i class=\"fa fa-clock-o\"></i> 5 mins</small></h4>" +
        ////        Environment.NewLine + "</a>" +
        ////        Environment.NewLine + "</li>" +
        ////        Environment.NewLine + "</ul>" +
        ////        Environment.NewLine + "</li>");
        ////    builder.Append(Environment.NewLine + "<li class=\"footer\"><a href=\"#\">" + context.GetLanguageLable("View all") + "</a></li>");
        ////    builder.Append(Environment.NewLine + "</ul>");
        ////    builder.Append(Environment.NewLine + "</a>");
        ////    builder.Append(Environment.NewLine + "</li>");
        ////    string r = builder.ToString(); builder = null;
        ////    return r;
        ////}
        private static string UILanguage(HRSContext context)
        {
            string l = (context.GetSession("language") == "" ? context.LanguageDefault : context.GetSession("language"));
            string lclass = "/images/icon_vn.png";
            string lName = "Việt Nam";
            string urlBack = context.GetUrlBack();
            dynamic d;
            context._cache.Get("cate-language", out d);
            string ui = "";
            for (int i = 0; i < d.CateLanguage.Items.Count; i++)
            {
                if (l == Tools.GetDataJson(d.CateLanguage.Items[i], "ID"))
                {
                    lclass = Tools.GetDataJson(d.CateLanguage.Items[i], "Img");
                    lName = Tools.GetDataJson(d.CateLanguage.Items[i], "NAME");
                    ui = ui + Environment.NewLine + "<a class=\"dropdown-item\" href=\"#\"><img height=24 src=\"" + Tools.GetDataJson(d.CateLanguage.Items[i], "Img") + "\">" + Tools.GetDataJson(d.CateLanguage.Items[i], "Name") + "</a>";
                }
                else
                {
                    ui = ui + Environment.NewLine + "<a class=\"dropdown-item\" href=\"javascript:document.Languagefrm.language.value='" + Tools.GetDataJson(d.CateLanguage.Items[i], "ID") + "';document.Languagefrm.submit();\"><img height=24 src=\"" + Tools.GetDataJson(d.CateLanguage.Items[i], "Img") + "\">" + Tools.GetDataJson(d.CateLanguage.Items[i], "Name") + "</a>";
                }
            }
            StringBuilder builder = new StringBuilder(); string r = "";
            builder.Append(Environment.NewLine + "<li class=\"nav-item dropdown\">");
            builder.Append(Environment.NewLine + "<a class=\"nav-link dropdown-toggle ngonngu\" href=\"#\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">" +
                Environment.NewLine + "<img src=\"" + lclass + "\" class=\"img-fluid z-depth-1 rounded-circle img35\" alt=\"" + lName + "\">" +
                Environment.NewLine + " <span class=\"an-mobile\">" + lName + "</span>" +
                Environment.NewLine + "</a>" +
                Environment.NewLine + "<div class=\"dropdown-menu dropdown-menu-right\">" + ui);
            builder.Append(Environment.NewLine + "</div>" +
                Environment.NewLine + "<form name=\"Languagefrm\" method=\"POST\" action=\"/Utils/Language\">" +
                Environment.NewLine + UIHidden("UrlBack", urlBack) +
                Environment.NewLine + UIHidden("language", l) +
                Environment.NewLine + "</form>" +
                Environment.NewLine + "</li>");

            r = builder.ToString();
            builder = null;
            return r;
        }
        ////private static string UIFormLanguage(HRSContext context)
        ////{
        ////    string urlBack = context.GetUrlBack();
        ////    string l = (context.GetSession("language") == "" ? context.LanguageDefault : context.GetSession("language"));
        ////    StringBuilder builder = new StringBuilder(); string r = "";

        ////    builder.Append(Environment.NewLine + "<li class=\"language-s\" data-rel=\"language-s\"><form name=\"Languagefrm\" method=\"POST\" action=\"/Utils/Language\">");
        ////    builder.Append(Environment.NewLine + UIHidden("UrlBack", urlBack));
        ////    builder.Append(Environment.NewLine + UIHidden("language", l) +
        ////        Environment.NewLine + "</form></li>");

        ////    r = builder.ToString();
        ////    builder = null;
        ////    return r;
        ////}
        ////private static string UILanguage(HRSContext context)
        ////{
        ////    string l = (context.GetSession("language") == "" ? context.LanguageDefault : context.GetSession("language"));
        ////    string lclass = "vie";// = (l == "vi" ? "vie" : "eng")
        ////    //string lCate = context.GetSession("cate-language");
        ////    dynamic d;// = JObject.Parse(lCate);
        ////    context._cache.Get("cate-language", out d);
        ////    string ui = "";
        ////    for (int i = 0; i < d.CateLanguage.Items.Count; i++)
        ////    {
        ////        if (l == Tools.GetDataJson(d.CateLanguage.Items[i], "ID"))
        ////        {
        ////            lclass = Tools.GetDataJson(d.CateLanguage.Items[i], "ClassName");
        ////            ui = ui + "<li class=\"active\"><a href=\"#\" class=\"" + lclass + "\">" + Tools.GetDataJson(d.CateLanguage.Items[i], "Name") + "</a></li>";
        ////        }
        ////        else
        ////        {
        ////            ui = ui + "<li><a href=\"javascript:document.Languagefrm.language.value='" + Tools.GetDataJson(d.CateLanguage.Items[i], "ID") + "';document.Languagefrm.submit();\" class=\"" + Tools.GetDataJson(d.CateLanguage.Items[i], "ClassName") + "\">" + Tools.GetDataJson(d.CateLanguage.Items[i], "Name") + "</a></li>";
        ////        }
        ////    }
        ////    StringBuilder builder = new StringBuilder(); string r = "";
        ////    builder.Append(Environment.NewLine + "<li class=\"language-" + lclass + "\" data-rel=\"language\">");
        ////    builder.Append(Environment.NewLine + "<ul class=\"dropdown-menu\" id=\"language\"><li><ul class=\"menu\">" +
        ////            Environment.NewLine + ui +
        ////            Environment.NewLine + "</ul></li></ul>" +
        ////            Environment.NewLine + "</li>");


        ////    r = builder.ToString();
        ////    builder = null;
        ////    return r;
        ////}
        ////private static string UILanguage_NoUse(HRSContext context)
        ////{
        ////    string urlBack = context.GetUrlBack();
        ////    string l = (context.GetSession("language") == "" ? context.LanguageDefault : context.GetSession("language"));
        ////    string lclass = "vie";// = (l == "vi" ? "vie" : "eng")
        ////    string lCate = context.GetSession("cate-language");
        ////    dynamic d = JObject.Parse(lCate);
        ////    string ui = "";
        ////    for (int i = 0; i < d.CateLanguage.Items.Count; i++)
        ////    {
        ////        if (l == Tools.GetDataJson(d.CateLanguage.Items[i], "ID"))
        ////        {
        ////            lclass = Tools.GetDataJson(d.CateLanguage.Items[i], "ClassName");
        ////            ui = ui + "<li class=\"header\"><a href=\"#\" class=\"dropdown-item active\">" + Tools.GetDataJson(d.CateLanguage.Items[i], "Name") + " <img src=\"" + Tools.GetDataJson(d.CateLanguage.Items[i], "Img") + "\"></a></li>";
        ////        }
        ////        else
        ////        {
        ////            ui = ui + "<li class=\"header\"><a href=\"javascript:document.Languagefrm.language.value='" + Tools.GetDataJson(d.CateLanguage.Items[i], "ID") + "';document.Languagefrm.submit();\" class=\"dropdown-item\">" + Tools.GetDataJson(d.CateLanguage.Items[i], "Name") + " <img src=\"" + Tools.GetDataJson(d.CateLanguage.Items[i], "Img") + "\"></a></li>";
        ////        }
        ////    }
        ////    StringBuilder builder = new StringBuilder(); string r = "";
        ////    builder.Append(Environment.NewLine + "<li class=\"dropdown langue-menu\">");
        ////    builder.Append(Environment.NewLine + "<a href=\"#\" class=\"dropdown-toggle " + lclass + "\" data-toggle=\"dropdown\" aria-expanded=\"false\"></a>");
        ////    builder.Append(Environment.NewLine + "<form name=\"Languagefrm\" method=\"POST\" action=\"/Utils/Language\">");
        ////    builder.Append(Environment.NewLine + UIHidden("UrlBack", urlBack));
        ////    builder.Append(Environment.NewLine + UIHidden("language", l));
        ////    builder.Append(Environment.NewLine + "</form>" +
        ////            Environment.NewLine + "<ul class=\"dropdown-menu\">" +
        ////            Environment.NewLine + ui +
        ////            Environment.NewLine + "</ul>" +
        ////            Environment.NewLine + "</li>");


        ////    r = builder.ToString();
        ////    builder = null;
        ////    return r;
        ////}
        private static string UIAccount(ref ToolDAO toolDAO, HRSContext context)
        {
            StringBuilder builder = new StringBuilder(); DateTime localDate = DateTime.Now;
            string UserName = context.GetSession("UserName");
            string ImageID = context.GetSession("ImageID");
            string CompanyCode = context.GetSession("CompanyCode");
            builder.Append(Environment.NewLine + "<li class=\"nav-item dropdown\">" +
                Environment.NewLine + "<a class=\"nav-link dropdown-toggle\" href=\"#\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">" +
                Environment.NewLine + "<img src=\"/Media/RenderFile?ImageId=" + ImageID + "&NoCrop=\" class=\"img-fluid z-depth-1 rounded-circle img35\" alt=\"" + UserName + "\">" +
                Environment.NewLine + "<span class=\"an-mobile\">" + UserName + "</span>" +
                Environment.NewLine + "</a>" +
                Environment.NewLine + "<div class=\"dropdown-menu dropdown-menu-right\">" +
                Environment.NewLine + "<a class=\"dropdown-item\" href=\"#\">" + CompanyCode + "</a>" +
                Environment.NewLine + "<a class=\"dropdown-item\" href=\"#\">" + localDate.ToString("dd/MM/yyyy HH:mm") + "</a>" +
                Environment.NewLine + "<a class=\"dropdown-item\" href=\"#\">" + context.GetLanguageLable("Contract") + "</a>" +
                Environment.NewLine + "<a class=\"dropdown-item\" href=\"#\">" + context.GetLanguageLable("Profile") + "</a>" +
                Environment.NewLine + "<a class=\"dropdown-item\" href=\"/Home/Logout\">" + context.GetLanguageLable("Logout") + "</a>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "</li>");

            string r = builder.ToString(); builder = null;
            return r;
        }
        ////private static string UIAccount(ref ToolDAO toolDAO, HRSContext context)
        ////{
        ////    StringBuilder builder = new StringBuilder(); DateTime localDate = DateTime.Now;
        ////    string UserName = context.GetSession("UserName");
        ////    string ImageID = context.GetSession("ImageID");
        ////    string CompanyCode = context.GetSession("CompanyCode");
        ////    // <!-- Notifications: style can be found in dropdown.less -->
        ////    builder.Append(Environment.NewLine + "<li class=\"userinfo\" data-rel=\"userinfo\">");
        ////    builder.Append(Environment.NewLine + "<div class=\"infouser\">" +
        ////        "<div class=\"avt\"><img src=\"/Media/RenderFile?ImageId=" + ImageID + "&NoCrop=\"></div>" +
        ////        "<div class=\"username\"><div class=\"bold\">" + UserName + "</div><div>" + CompanyCode + "</div></div></div>");
        ////    builder.Append(Environment.NewLine + "<ul class=\"dropdown-menu\" id=\"userinfo\">");
        ////    builder.Append(Environment.NewLine + "<li>" +
        ////        Environment.NewLine + "<ul class=\"menu align-right\">" +
        ////        Environment.NewLine + "<li><a>" + localDate.ToString("dd/MM/yyyy HH:mm:ss") + "</a></li>" +
        ////        Environment.NewLine + "<li><a>" + context.GetLanguageLable("Contract") + "</a></li>" +
        ////        Environment.NewLine + "<li><a>" + context.GetLanguageLable("Profile") + "</a></li>" +
        ////        Environment.NewLine + "<li><a href=\"/Home/Logout\">" + context.GetLanguageLable("Logout") + "</a></li>");
        ////    builder.Append(Environment.NewLine + "</ul>");
        ////    builder.Append(Environment.NewLine + "</li>");
        ////    string r = builder.ToString(); builder = null;
        ////    return r;
        ////}
        ////private static string UIAccount_NoUse(ref ToolDAO toolDAO, HRSContext context)
        ////{
        ////    StringBuilder builder = new StringBuilder(); DateTime localDate = DateTime.Now;
        ////    string UserName = context.GetSession("UserName"); string ImageID = context.GetSession("ImageID");
        ////    // <!-- Notifications: style can be found in dropdown.less -->
        ////    builder.Append(Environment.NewLine + "<li class=\"dropdown user user-menu\">");
        ////    builder.Append(Environment.NewLine + "<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">");
        ////    builder.Append(Environment.NewLine + "<img src=\"/Media/RenderFile?ImageId=" + ImageID + "&NoCrop=\" class=\"user-image\" alt=\"User Image\"><strong class=\"hidden-xs\">" + UserName + "</strong></a>");
        ////    builder.Append(Environment.NewLine + "<ul class=\"dropdown-menu\">");
        ////    builder.Append(Environment.NewLine + "<li class=\"user-header\">" +
        ////        Environment.NewLine + "<img src=\"/Media/RenderFile?ImageId=" + ImageID + "&NoCrop=\" class=\"img-circle\" alt=\"User Image\">" +
        ////        Environment.NewLine + "<p>" +
        ////        Environment.NewLine + "" + UserName + "<small>" + localDate.ToString("dd/MM/yyyy HH:mm:ss") + "</small>" +
        ////        Environment.NewLine + "</p>" +
        ////        Environment.NewLine + "</li>");
        ////    builder.Append(Environment.NewLine + "<li class=\"footer\">" +
        ////        Environment.NewLine + "<div class=\"pull-left\"><a href=\"#\" class=\"btn btn-default btn-flat\">" + context.GetLanguageLable("Profile") + "</a></div>" +
        ////        Environment.NewLine + "<div class=\"pull-right\"><a href=\"/Home/Logout\" class=\"btn btn-default btn-flat\">" + context.GetLanguageLable("Logout") + "</a></div>" +
        ////        Environment.NewLine + "</li>");
        ////    builder.Append(Environment.NewLine + "</ul>");
        ////    builder.Append(Environment.NewLine + "</a>");
        ////    builder.Append(Environment.NewLine + "</li>");
        ////    string r = builder.ToString(); builder = null;
        ////    return r;
        ////}
        private static string SetUrl(string Url, string Id, int Cnt = 0)
        {
            if (Cnt > 0) Url = "#";
            if (!(Url.IndexOf("#") > -1))
            {
                if (Url.IndexOf("?") > -1)
                    Url = Url + "&MenuID=" + Id;
                else
                    Url = Url + "?MenuID=" + Id;
            }

            return Url;
        }
        private static bool IsShowMenu(HRSContext context, dynamic d)
        {
            bool kt = (Tools.GetDataJson(d, "IsDisplay") == "1" && Tools.GetDataJson(d, "Grp", "int") == context.Application);
            return kt;
        }
        public static string UIMenuSubForm(HRSContext context, ToolDAO toolDAO, bool MenuOn = true, string MenuID = "0")
        {
            StringBuilder b = new StringBuilder();
            b.Append(Environment.NewLine + "<div class=\"menulevel3\"><ul>");
            string UserID = context.GetSession("UserID");
            //string jsonMenu = context.GetSession("Menu_" + UserID);
            dynamic d; //context._cache.Get("Menu_" + UserID, out d);// = JObject.Parse(jsonMenu);
            GetMenuData(UserID, toolDAO, context, out d);
            for (var i = 0; i < d.Menu.Items.Count; i++)
            {
                if (IsShowMenu(context, d.Menu.Items[i]) && MenuID == Tools.GetDataJson(d.Menu.Items[i], "ParentID"))
                {
                    string urlMenu = SetUrl(Tools.GetDataJson(d.Menu.Items[i], "Url"), Tools.GetDataJson(d.Menu.Items[i], "MenuID"));
                    string urlName = Tools.GetDataJson(d.Menu.Items[i], "MenuName");
                    string urlLogoName = Tools.GetDataJson(d.Menu.Items[i], "ActionName");
                    urlLogoName = (urlLogoName == "" ? "contract" : urlLogoName);

                    b.Append(Environment.NewLine + "<li>");
                    b.Append(Environment.NewLine + "<a href=\"" + urlMenu + "\">");
                    b.Append(Environment.NewLine + "<div class=\"item\">");
                    b.Append(Environment.NewLine + "<img src=\"/images/" + urlLogoName + ".png\">");
                    b.Append(Environment.NewLine + "<img class=\"active\" src=\"/images/" + urlLogoName + "_active.png\">");
                    b.Append(Environment.NewLine + "</div>");
                    b.Append(Environment.NewLine + "<span class=\"text\">" + urlName + "</span>");
                    b.Append(Environment.NewLine + "</a>");
                    b.Append(Environment.NewLine + "</li>");
                }
            }
            b.Append(Environment.NewLine + "</ul></div>");

            string r = b.ToString(); b = null;
            return r;
        }
        private static string UIMenuSub(HRSContext context, bool MenuOn = true, string MenuID = "0", bool IsMenuSubPage = false)
        {
            StringBuilder b = new StringBuilder(); 
            if (MenuOn && MenuID != "0")
            {
                string MenuSubID = "0"; string MenuParentID = "0"; int i = 0; bool IsFound = false;
                string UserID = context.GetSession("UserID");
                //string jsonMenu = context.GetSession("Menu_" + UserID);
                dynamic d; context._cache.Get("Menu_" + UserID + "_" + context.GetSession("language"), out d);// = JObject.Parse(jsonMenu);
                if (IsMenuSubPage)
                {
                    MenuSubID = MenuID; IsFound = false; i = 0;
                    while (i < d.Menu.Items.Count && !IsFound)
                    {
                        if (MenuSubID == Tools.GetDataJson(d.Menu.Items[i], "MenuID"))//Tools.GetDataJson(d.Menu.Items[i], "MenuID")
                        {
                            MenuParentID = Tools.GetDataJson(d.Menu.Items[i], "ParentID"); //Tools.GetDataJson(d.Menu.Items[i], "ParentID")
                            IsFound = true;
                        }
                        i = i + 1;
                    }
                    if (MenuParentID == "0")
                    {
                        MenuParentID = MenuID;
                    }                    
                }
                else
                {
                    IsFound = false; i = 0;
                    while (i < d.Menu.Items.Count && !IsFound)
                    {
                        if (MenuID == Tools.GetDataJson(d.Menu.Items[i], "MenuID")) //Tools.GetDataJson(d.Menu.Items[i], "MenuID")
                        {
                            MenuSubID = Tools.GetDataJson(d.Menu.Items[i], "ParentID"); // Tools.GetDataJson(d.Menu.Items[i], "ParentID");
                            IsFound = true;
                        }
                        i = i + 1;
                    }
                }                
                if (MenuParentID == "0")
                {
                    IsFound = false; i = 0;
                    while (i < d.Menu.Items.Count && !IsFound)
                    {
                        if (MenuSubID == Tools.GetDataJson(d.Menu.Items[i], "MenuID"))
                        {
                            MenuParentID = Tools.GetDataJson(d.Menu.Items[i], "ParentID"); 
                            IsFound = true;
                        }
                        i = i + 1;
                    }
                }
                if (MenuParentID == "0") MenuParentID = MenuSubID;
                b.Append(Environment.NewLine + "<div class=\"fastmenu\"><ul>");
                for (i = 0; i < d.Menu.Items.Count; i++)
                {
                    if (IsShowMenu(context, d.Menu.Items[i]) && MenuParentID == Tools.GetDataJson(d.Menu.Items[i], "ParentID") && int.Parse(MenuParentID) > 0)
                    {
                        int Cnt = int.Parse(Tools.GetDataJson(d.Menu.Items[i], "Cnt"));
                        if (Cnt > 0)
                        {
                            b.Append(Environment.NewLine + "<li" + (MenuSubID == Tools.GetDataJson(d.Menu.Items[i], "MenuID") ? " class=\"active\"" : "") + ">");
                            b.Append(Environment.NewLine + "<a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "Url"), Tools.GetDataJson(d.Menu.Items[i], "MenuID")) + "\">");
                            b.Append(Environment.NewLine + Tools.GetDataJson(d.Menu.Items[i], "MenuName"));
                            b.Append(Environment.NewLine + "</a>");
                            b.Append(Environment.NewLine + "</li>");
                        }
                    }
                }
                b.Append(Environment.NewLine + "</ul></div>");
            }
            string r = b.ToString(); b = null;
            return r;
        }
        public static string UIMenuRelated(HRSContext context, ToolDAO toolDAO, bool MenuOn = true, string MenuID = "0")
        {
            StringBuilder b = new StringBuilder();
            string MenuIDRelated = "";
            if (MenuOn)
            {
                string UserID = context.GetSession("UserID");
                dynamic d = null;
                int i = -1;
                bool kt = false;

                // Get cache Related
                if (MenuID == "0" || MenuID == "") MenuID = context.GetRequestVal("MenuID");
                if (MenuID == "") MenuID = "0";
                kt = context._cache.Get("MenuRelated_" + MenuID + "_" + UserID + "_" + context.GetSession("language"), out MenuIDRelated);
                if (kt) return MenuIDRelated;

                // Get cache Menu
                GetMenuData(UserID, toolDAO, context, out d);
                kt = false;
                if (!kt) { }
                while (!kt && i < d.Menu.Items.Count - 1)
                {
                    i = i + 1;
                    if (Tools.GetDataJson(d.Menu.Items[i], "MenuID") == MenuID)
                    {
                        kt = true;
                        MenuIDRelated = Tools.GetDataJson(d.Menu.Items[i], "FunctionIDRelated");
                    }
                }
                if (MenuIDRelated != "")
                {
                    MenuIDRelated = "," + MenuIDRelated + ",";
                    for (int j = 0; j < d.Menu.Items.Count; j++)
                    {
                        if (MenuIDRelated.IndexOf("," + Tools.GetDataJson(d.Menu.Items[j], "MenuID") + ", ") > -1)
                        {
                            b.Append(Environment.NewLine + "<a " +
                                "href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[j], "Url"), Tools.GetDataJson(d.Menu.Items[j], "MenuID")) + "\" " +
                                "class=\"btn btn-outline-grey btn-sm my-0 waves-effect waves-light\">" + Tools.GetDataJson(d.Menu.Items[j], "MenuName") + "</a>");
                        }
                    }
                    MenuIDRelated = b.ToString();
                    if (MenuIDRelated != "") context._cache.Set("MenuRelated_" + MenuID + "_" + UserID + "_" + context.GetSession("language"), MenuIDRelated, context._cache.CacheByMinute * 5);
                }
            }
            return MenuIDRelated;
        }
        ////private static string UIMenuRelated(HRSContext context, bool MenuOn = true, string MenuID = "0")
        ////{
        ////    StringBuilder b = new StringBuilder();
        ////    if (MenuOn)
        ////    {
        ////        string UserID = context.GetSession("UserID"); dynamic d = null;
        ////        if (MenuID == "0" || MenuID == "") MenuID = context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
        ////        string MenuIDRelated = "";
        ////        string MenuList = context.GetSession("Menu_" + UserID);
        ////        string jsonMenuSearch = context.GetSession("jsonMenuSearch_" + UserID);
        ////        b.Append(Environment.NewLine + "<ul class=\"menu-horizontal\">");
        ////        if (MenuList != "" || MenuID == "")
        ////        {
        ////            d = JObject.Parse(MenuList);
        ////            int i = -1; bool kt = false;
        ////            while (!kt && i < d.Menu.Items.Count - 1)
        ////            {
        ////                i = i + 1;
        ////                if (Tools.GetDataJson(d.Menu.Items[i], "MenuID") == MenuID) { kt = true; MenuIDRelated = Tools.GetDataJson(d.Menu.Items[i], "FunctionIDRelated"); }
        ////            }
        ////            if (MenuIDRelated != "")
        ////            {
        ////                MenuIDRelated = "," + MenuIDRelated + ",";                        
        ////                for (int j = 0; j < d.Menu.Items.Count; j++)
        ////                {
        ////                    if (MenuIDRelated.IndexOf("," + Tools.GetDataJson(d.Menu.Items[j], "MenuID") + ", ") > -1)
        ////                    {
        ////                        b.Append(Environment.NewLine + "<li " + (Tools.GetDataJson(d.Menu.Items[j], "MenuID") == MenuID ? "class=\"active\"" : "") + ">");
        ////                        b.Append(Environment.NewLine + "<a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[j], "Url"), Tools.GetDataJson(d.Menu.Items[j], "MenuID")) + "\">");
        ////                        b.Append(Environment.NewLine + "<span>" + Tools.GetDataJson(d.Menu.Items[j], "MenuName") + "</span>");
        ////                        b.Append(Environment.NewLine + "</a>");
        ////                    }
        ////                }                        
        ////            }
        ////        }
        ////        // search menu box
        ////        string MenuSearchCache = context.GetSession("MenuSearchCache");
        ////        StringBuilder bx = new StringBuilder();
        ////        if (MenuSearchCache != "")
        ////        {
        ////            //HTTP_CODE.WriteLogAction("MenuSearchCache: " + MenuSearchCache, context);
        ////            bx.Append(MenuSearchCache);
        ////        }                    
        ////        else
        ////        {
        ////            //HTTP_CODE.WriteLogAction("MenuSearchCache: 1", context);
        ////            d = JObject.Parse(jsonMenuSearch);
        ////            bx.Append(Environment.NewLine + "<li class=\"search\">" +
        ////                Environment.NewLine + "<input id=\"actb_SearchTxt\" size=30 name=\"actb_SearchTxt\" autocomplete=\"off\" class=\"searchtext\" type=\"text\" placeholder=\"" + context.GetLanguageLable("SearchMenu") + "\">" +
        ////                Environment.NewLine + "</li>");
        ////            bx.Append(Environment.NewLine + "<script language='javascript'>" +
        ////                Environment.NewLine + "var actb_SearchTxt_text = new Array();" +
        ////                Environment.NewLine + "var actb_SearchTxt_Url = new Array();");
        ////            for (int i = 0; i < d.MenuSearch.Count; i++)
        ////            {
        ////                bx.Append(Environment.NewLine + "actb_SearchTxt_text[" + i + "]=\"" + d.MenuSearch[i].MenuName.ToString() + "\";" +
        ////                    Environment.NewLine + "actb_SearchTxt_Url[" + i + "]=\"" + d.MenuSearch[i].Url.ToString() + "\";");
        ////            }
        ////            bx.Append(Environment.NewLine + "var _SearchTxtactb = new actb(document.getElementById('actb_SearchTxt'), actb_SearchTxt_text, \"window.location.href=actb_SearchTxt_Url[i]\");");
        ////            bx.Append(Environment.NewLine + "</script>");
        ////            bx.Append(Environment.NewLine + "</ul>");
        ////            context.SetSession("MenuSearchCache", bx.ToString());
        ////        }
        ////        b.Append(bx.ToString());
        ////    }
        ////    return b.ToString();
        ////}
        private static string UIMenuPath(ref ToolDAO toolDAO, HRSContext context, bool MenuOn = true, string MenuID = "0")
        {
            StringBuilder b = new StringBuilder();
            b.Append(Environment.NewLine + "<style>" +
                Environment.NewLine + "/* actb */" +
                Environment.NewLine + ".actbbox {" +
                Environment.NewLine + "    border: 1px solid #CCC;" +
                Environment.NewLine + "    padding: 2px 5px;" +
                Environment.NewLine + "    z-index:100;" +
                Environment.NewLine + "}" +
                Environment.NewLine + ".actbfont {" +
                Environment.NewLine + "    //font-size: 12px;" +
                Environment.NewLine + "    padding: 5px 10px;" +
                Environment.NewLine + "}" +
                Environment.NewLine + "</style>");
            if (MenuOn)
            {
                dynamic d = null;
                string UserID = context.GetSession("UserID");
                string jsonMenuSearch;
                context._cache.Get("jsonMenuSearch_" + UserID + "_" + context.GetSession("language"), out jsonMenuSearch);

                if (MenuID == "0" || MenuID == "") MenuID = context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
                GetMenuData(UserID, toolDAO, context, out d);

                bool IsFound = false; int i = 0;
                b.Append(Environment.NewLine + "<ol class=\"breadcrumb\">");
                while (i < d.Menu.Items.Count && !IsFound)
                {
                    if (Tools.GetDataJson(d.Menu.Items[i], "MenuID") == MenuID) IsFound = true;
                    i = i + 1;
                }
                if (IsFound)
                {
                    i = i - 1;
                    if (Tools.GetDataJson(d.Menu.Items[i], "NameP1") != "")
                    {
                        b.Append(Environment.NewLine + "<li class=\"breadcrumb-item\"><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "UrlP1"), Tools.GetDataJson(d.Menu.Items[i], "IDP1")) + "\">" + Tools.GetDataJson(d.Menu.Items[i], "NameP1") + "</a></li>");
                    }
                    if (Tools.GetDataJson(d.Menu.Items[i], "NameP2") != "")
                    {
                        b.Append(Environment.NewLine + "<li class=\"breadcrumb-item\"><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "UrlP2"), Tools.GetDataJson(d.Menu.Items[i], "IDP2")) + "\">" + Tools.GetDataJson(d.Menu.Items[i], "NameP2") + "</a></li>");
                    }
                    if (Tools.GetDataJson(d.Menu.Items[i], "NameP3") != "")
                    {
                        b.Append(Environment.NewLine + "<li class=\"breadcrumb-item\"><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "UrlP3"), Tools.GetDataJson(d.Menu.Items[i], "IDP3")) + "\">" + Tools.GetDataJson(d.Menu.Items[i], "NameP3") + "</a></li>");
                    }
                    b.Append(Environment.NewLine + "<li class=\"breadcrumb-item active\" aria-current=\"page\"><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "Url"), Tools.GetDataJson(d.Menu.Items[i], "MenuID")) + "\">" + Tools.GetDataJson(d.Menu.Items[i], "MenuName") + "</a></li>");
                }
                else b.Append(Environment.NewLine + "<li class=\"breadcrumb-item active\" aria-current=\"page\"><a href=\"/Home\">" + context.GetLanguageLable("lHome") + "</a></li>");
                b.Append(Environment.NewLine + "</ol>");



                // search menu box
                string MenuSearchCache;
                context._cache.Get("MenuSearchCache_" + UserID + "_" + context.GetSession("language"), out MenuSearchCache);// = context.GetSession("MenuSearchCache");
                StringBuilder bx = new StringBuilder();
                if (MenuSearchCache != "")
                {
                    bx.Append(MenuSearchCache);
                }
                else
                {
                    d = JObject.Parse(jsonMenuSearch);
                    bx.Append(Environment.NewLine + "<div class=\"col-sm-3 top-search\">" +
                        Environment.NewLine + "<div class=\"form-group row\">" +
                        Environment.NewLine + "<div class=\"dropdown search-dropdown\">" +
                        Environment.NewLine + "<div class=\"input-group form-inline\">" +
                        Environment.NewLine + "<i class=\"fa fa-search active\" aria-hidden=\"true\"></i>" +
                        Environment.NewLine + "<input id=\"actb_SearchTxt\" name=\"actb_SearchTxt\" class=\"form-control\" type=\"text\" placeholder=\"" + context.GetLanguageLable("SearchMenu") + "\" aria-label=\"Search\">" +
                        Environment.NewLine + "</div></div></div></div>");
                    bx.Append(Environment.NewLine + "<script language='javascript'>" +
                        Environment.NewLine + "var actb_SearchTxt_text = new Array();" +
                        Environment.NewLine + "var actb_SearchTxt_Url = new Array();");
                    for (i = 0; i < d.MenuSearch.Count; i++)
                    {
                        bx.Append(Environment.NewLine + "actb_SearchTxt_text[" + i + "]=\"" + d.MenuSearch[i].MenuName.ToString() + "\";" +
                            Environment.NewLine + "actb_SearchTxt_Url[" + i + "]=\"" + d.MenuSearch[i].Url.ToString() + "\";");
                    }
                    bx.Append(Environment.NewLine + "var _SearchTxtactb = new actb(document.getElementById('actb_SearchTxt'), actb_SearchTxt_text, \"window.location.href=actb_SearchTxt_Url[i]\");");
                    bx.Append(Environment.NewLine + "</script>");
                    context._cache.Set("MenuSearchCache_" + UserID + "_" + context.GetSession("language"), bx.ToString(), context._cache.CacheByMinute * 5);
                }
                b.Append(bx.ToString());
            }
            string r = b.ToString(); b = null;
            return r;
        }
        ////private static string UIMenuPath(ref ToolDAO toolDAO, HRSContext context, bool MenuOn = true, string MenuID = "0")
        ////{
        ////    StringBuilder b = new StringBuilder();
        ////    if (MenuOn)
        ////    {
        ////        //string json = ""; //string parameterOutput = ""; int errorCode = 0; string errorString = "";
        ////        dynamic d = null; 
        ////        string UserID = context.GetSession("UserID");
        ////        string jsonMenuSearch;// = context.GetSession("jsonMenuSearch_" + UserID);
        ////        context._cache.Get("jsonMenuSearch_" + UserID + "_" + context.GetSession("language"), out jsonMenuSearch);

        ////        if (MenuID == "0" || MenuID == "") MenuID = context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
        ////        //json = context.GetSession("MenuPath_" + MenuID);
        ////        //json = context.GetSession("Menu_" + UserID);
        ////        //context._cache.Get("Menu_" + UserID, out d);
        ////        GetMenuData(UserID, toolDAO, context, out d);
        ////        //if (json == "")
        ////        //{
        ////        //d = JObject.Parse("{\"parameterInput\":[{\"ParamName\":\"MenuID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + MenuID + "\"}]}");
        ////        //toolDAO.ExecuteStore("MenuPath", "SP_CMS__Functions_GetPathNew", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
        ////        //    context.SetSession("MenuPath_" + MenuID, json);
        ////        //}
        ////        //d = JObject.Parse(json);

        ////        bool IsFound = false; int i = 0;
        ////        b.Append(Environment.NewLine + "<div class=\"breakcrumbs\"><ul class=\"breadcrumb\">");
        ////        while (i < d.Menu.Items.Count && !IsFound)
        ////        {
        ////            if (Tools.GetDataJson(d.Menu.Items[i], "MenuID") == MenuID) IsFound = true;
        ////            i = i + 1;
        ////        }
        ////        if (IsFound)
        ////        {
        ////            i = i - 1;
        ////            //b.Append(Environment.NewLine + "<li class=\"" + Tools.GetDataJson(d.Menu.Items[i], "ClassP0") + "\"><a href=\"" + Tools.GetDataJson(d.Menu.Items[i], "UrlP4") + "\">" + Tools.GetDataJson(d.Menu.Items[i], "NameP0") + "</a></li>");
        ////            if (Tools.GetDataJson(d.Menu.Items[i], "NameP1") != "")
        ////            {
        ////                b.Append(Environment.NewLine + "<li class=\"" + Tools.GetDataJson(d.Menu.Items[i], "ClassP1") + "\"><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "UrlP1"), Tools.GetDataJson(d.Menu.Items[i], "IDP1")) + "\">" + Tools.GetDataJson(d.Menu.Items[i], "NameP1") + "</a></li>");
        ////            }
        ////            if (Tools.GetDataJson(d.Menu.Items[i], "NameP2") != "")
        ////            {
        ////                b.Append(Environment.NewLine + "<li class=\"" + Tools.GetDataJson(d.Menu.Items[i], "ClassP2") + "\"><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "UrlP2"), Tools.GetDataJson(d.Menu.Items[i], "IDP2")) + "\">" + Tools.GetDataJson(d.Menu.Items[i], "NameP2") + "</a></li>");
        ////            }
        ////            if (Tools.GetDataJson(d.Menu.Items[i], "NameP3") != "")
        ////            {
        ////                b.Append(Environment.NewLine + "<li class=\"" + Tools.GetDataJson(d.Menu.Items[i], "ClassP3") + "\"><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "UrlP3"), Tools.GetDataJson(d.Menu.Items[i], "IDP3")) + "\">" + Tools.GetDataJson(d.Menu.Items[i], "NameP3") + "</a></li>");
        ////            }
        ////            b.Append(Environment.NewLine + "<li class=\"" + Tools.GetDataJson(d.Menu.Items[i], "ClassName") + "\"><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "Url"), Tools.GetDataJson(d.Menu.Items[i], "MenuID")) + "\">" + Tools.GetDataJson(d.Menu.Items[i], "MenuName") + "</a></li>");
        ////        }
        ////        else b.Append(Environment.NewLine + "<li class=\"first active\"><a href=\"/Home\">" + context.GetLanguageLable("lHome") + "</a></li>");

        ////        b.Append(Environment.NewLine + "</ul>");

        ////        // search menu box
        ////        string MenuSearchCache; context._cache.Get("MenuSearchCache_" + UserID + "_" + context.GetSession("language"), out MenuSearchCache);// = context.GetSession("MenuSearchCache");
        ////        StringBuilder bx = new StringBuilder();
        ////        if (MenuSearchCache != "")
        ////        {
        ////            //HTTP_CODE.WriteLogAction("MenuSearchCache: " + MenuSearchCache, context);
        ////            bx.Append(MenuSearchCache);
        ////        }
        ////        else
        ////        {
        ////            d = JObject.Parse(jsonMenuSearch);
        ////            bx.Append(Environment.NewLine + "<input id=\"actb_SearchTxt\" name=\"actb_SearchTxt\" autocomplete=\"off\" class=\"search\" type=\"text\" placeholder=\"" + context.GetLanguageLable("SearchMenu") + "\">");
        ////            bx.Append(Environment.NewLine + "<script language='javascript'>" +
        ////                Environment.NewLine + "var actb_SearchTxt_text = new Array();" +
        ////                Environment.NewLine + "var actb_SearchTxt_Url = new Array();");
        ////            for (i = 0; i < d.MenuSearch.Count; i++)
        ////            {
        ////                bx.Append(Environment.NewLine + "actb_SearchTxt_text[" + i + "]=\"" + d.MenuSearch[i].MenuName.ToString() + "\";" +
        ////                    Environment.NewLine + "actb_SearchTxt_Url[" + i + "]=\"" + d.MenuSearch[i].Url.ToString() + "\";");
        ////            }
        ////            bx.Append(Environment.NewLine + "var _SearchTxtactb = new actb(document.getElementById('actb_SearchTxt'), actb_SearchTxt_text, \"window.location.href=actb_SearchTxt_Url[i]\");");
        ////            bx.Append(Environment.NewLine + "</script>");

        ////            bx.Append("</div>");
        ////            //context.SetSession("MenuSearchCache", bx.ToString());
        ////            context._cache.Set("MenuSearchCache_" + UserID + "_" + context.GetSession("language"), bx.ToString(), context._cache.CacheByMinute * 5);
        ////        }
        ////        b.Append(bx.ToString());
        ////    }
        ////    string r = b.ToString(); b = null;
        ////    return r;
        ////}
        ////private static string UIMenuPath_NoUse(ref ToolDAO toolDAO, HRSContext context, bool MenuOn = true, string MenuID = "0")
        ////{
        ////    StringBuilder b = new StringBuilder();
        ////    if (MenuOn)
        ////    {
        ////        string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = ""; dynamic d = null;
        ////        if (MenuID == "0" || MenuID == "") MenuID = context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";
        ////        json = context.GetSession("MenuPath_" + MenuID);
        ////        if (json == "")
        ////        {
        ////            d = JObject.Parse("{\"parameterInput\":[{\"ParamName\":\"MenuID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + MenuID + "\"}]}");
        ////            toolDAO.ExecuteStore("MenuPath", "SP_CMS__Functions_GetPath", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
        ////            context.SetSession("MenuPath_" + MenuID, json);
        ////        }
        ////        d = JObject.Parse(json);
        ////        b.Append(Environment.NewLine + "<ul class=\"breadcrumb\">");
        ////        if (d.MenuPath.Items.Count > 0)
        ////        {
        ////            //b.Append(Environment.NewLine + "<li><a href=\"/Home\">" + Tools.GetDataJson(d.MenuPath.Items[0], "NameP0") + "</a></li>");
        ////            if (Tools.GetDataJson(d.MenuPath.Items[0], "NameP1") != "")
        ////            {
        ////                b.Append(Environment.NewLine + "<li><a href=\"" + Tools.GetDataJson(d.MenuPath.Items[0], "UrlP1") + "\">" + Tools.GetDataJson(d.MenuPath.Items[0], "NameP1") + "</a></li>");
        ////            }
        ////            if (Tools.GetDataJson(d.MenuPath.Items[0], "NameP2") != "")
        ////            {
        ////                b.Append(Environment.NewLine + "<li><a href=\"" + Tools.GetDataJson(d.MenuPath.Items[0], "UrlP2") + "\">" + Tools.GetDataJson(d.MenuPath.Items[0], "NameP2") + "</a></li>");
        ////            }
        ////            if (Tools.GetDataJson(d.MenuPath.Items[0], "NameP3") != "")
        ////            {
        ////                b.Append(Environment.NewLine + "<li><a href=\"" + Tools.GetDataJson(d.MenuPath.Items[0], "UrlP3") + "\">" + Tools.GetDataJson(d.MenuPath.Items[0], "NameP3") + "</a></li>");
        ////            }
        ////            b.Append(Environment.NewLine + "<li class=\"active\"><a href=\"" + Tools.GetDataJson(d.MenuPath.Items[0], "Url") + "\">" + Tools.GetDataJson(d.MenuPath.Items[0], "Name") + "</a></li>");
        ////        }
        ////        else b.Append(Environment.NewLine + "<li><a href=\"/Home\">" + context.GetLanguageLable("lHome") + "</a></li>");

        ////        b.Append(Environment.NewLine + "</ul>");
        ////    }
        ////    string r = b.ToString(); b = null;
        ////    return r;
        ////}
        #endregion

        #region UI Header || Footer || Menu || Treeview
        /*private static string Treeview(int i, dynamic d, bool IsCheckbox = false, string InputName = "", string val = "")
        {
            StringBuilder b = new StringBuilder(); string r = "";
            long ParentID = long.Parse(Tools.GetDataJson(d.Items[i], "ID"));
            int Cnt = int.Parse(Tools.GetDataJson(d.Items[i], "Cnt"));
            if (IsCheckbox)
            {
                val = "," + val + ",";
                if (val.IndexOf("," + Tools.GetDataJson(d.Items[i], "ID") + ",") > -1) val = Tools.GetDataJson(d.Items[i], "ID");
                b.Append("<li>" + UIDef.UICheckbox(InputName, Tools.GetDataJson(d.Items[i], "Name"), Tools.GetDataJson(d.Items[i], "ID"), val, "")); // Onclick check Parent; Check Child
            }
            else
            {
                b.Append("<li>" + (Cnt > 0 ? "<i class=\"indicator glyphicon glyphicon-plus-sign\"></i>": "<i class=\"indicator glyphicon glyphicon-minus-sign\"></i>") + " <a href=\"#" + Tools.GetDataJson(d.Items[i], "ID") + "\">" + Tools.GetDataJson(d.Items[i], "Name") + "</a>");
            }
            if (Cnt > 0)
            {
                b.Append("<ul>");
                for (int j = 0; j < d.Items.Count; j++)
                {
                    if (ParentID == long.Parse(Tools.GetDataJson(d.Items[j], "Parent")))
                        b.Append(Treeview(j, d, IsCheckbox, InputName, val));
                }
                b.Append("</ul>");
            }
            b.Append("</li>");
            r = b.ToString(); b = null;
            return r;
        }
        public static string UITreeview(ToolDAO toolDAO, HRSContext context, bool TreeviewOn = true, bool IsCheckbox = false, string InputName = "", string val = "")
        {
            StringBuilder b = new StringBuilder(); string r = "";
            if (TreeviewOn)
            {
                string UserID = context.GetSession("UserID");
                string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = ""; dynamic d = null;
                json = context.GetSession("Treeview_" + UserID);
                //if (json == "")
                //{
                d = JObject.Parse("{\"parameterInput\":[{\"ParamName\":\"Creator\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + UserID + "\"}]}");
                toolDAO.ExecuteStore("Treeview", "SP_CMS__HU_Organization_ListTreeByUsers", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                context.SetSession("Treeview_" + UserID, json);
                //}
                d = JObject.Parse(json);
                b.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/treeview.css\">");
                b.Append("<div id=\"OrgID_div\" style=\"background: white; border: 1px solid rgb(204, 204, 204); display: block; height: 100%; width: 100%; overflow: auto;\"><ul id=\"tree1\" class=\"treeOrg\">");
                for (int i = 0; i < d.Treeview.Items.Count; i++)
                {
                    if (long.Parse(Tools.GetDataJson(d.Treeview.Items[i], "Parent")) == 0 || long.Parse(Tools.GetDataJson(d.Treeview.Items[i], "Parent")) == -1)
                        b.Append(Treeview(i, d.Treeview, IsCheckbox, InputName, val));
                }
                b.Append("</ul></div>");
                b.Append(Environment.NewLine + "<script src=\"/js/treeview.js\"></script>");
            }
            r = b.ToString(); b = null;            
            return r;
        }*/
        public static string UIBodyTagClose(string iframe = "", string txtClose = "x")
        {
            //string iframe = context.GetRequestVal("iframe");
            return Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/cuttom.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/mdb.min.js\"></script>" +
                Environment.NewLine + "<div class=\"modal fade\" id=\"modalVM\" role=\"dialog\" aria-labelledby=\"myModalLabel\" aria-hidden=\"true\">" +
                Environment.NewLine + "<div class=\"modal-dialog modal-fluid \" role=\"document\">" +
                Environment.NewLine + "<!--Content-->" +
                Environment.NewLine + "<div class=\"modal-content\">" +
                Environment.NewLine + "<!-- mb-0 p-0 -->" +
                Environment.NewLine + "<div class=\"modal-body\">" +
                Environment.NewLine + "<div class=\"embed-responsive embed-responsive-16by9\">" +
                Environment.NewLine + "<iframe id=\"ModelPopup" + iframe + "\" name=\"ModelPopup" + iframe + "\" class=\"embed-responsive-item\" src=\"\" allowfullscreen></iframe>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "<!--Footer-->" +
                Environment.NewLine + "<div class=\"modal-footer justify-content-center flex-column flex-md-row\">" +
                Environment.NewLine + "<button type=\"button\" class=\"btn btn-outline-primary btn-rounded btn-md ml-4\" data-dismiss=\"modal\">" + txtClose + "</button>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "<!--/.Content-->" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "</div>" +
                Environment.NewLine + "</body>";
        }
        public static string UIBodyTagOpen(string secure = "", bool IsPageLogin = true)
        {
            return Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/select2totree.js\"></script>" +
                "<body onload=\"resizePopupPage();\" onkeyDown=\"actionKeypress(event)\">";
                //(IsPageLogin? "<body class=\"hold-transition login-page\" onload=\"resizePopupPage();\">" : 
                //"<body class=\"hold-transition skin-blue sidebar-mini" + secure + "\" onkeyDown=\"actionKeypress(event)\" onLoad='if (parent) if(parent.iframesize) parent.iframesize();'>");
        }
        public static string UIChartReport(HRSContext context, string PageTitle)
        {
            StringBuilder builder = new StringBuilder(); string r = "";
            //builder.Append("<!DOCTYPE html>");
            //builder.Append("<html>");
            // builder.Append("<head>");
            builder.Append("<meta charset=\"utf-8\">");
            builder.Append(Environment.NewLine + "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
            builder.Append(Environment.NewLine + "<title>" + PageTitle + "</title>");
            builder.Append(Environment.NewLine + "<meta content=\"width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no\" name=\"viewport\">");
            // Css
            builder.Append(Environment.NewLine + "<link href=\"/css/jquery-ui.min.css\" rel=\"stylesheet\" type=\"text/css\"/>  " +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/orgchart.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/bootstrap.min.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/font-awesome.min.css\">");

            // js
            builder.Append(Environment.NewLine + "<script src=\"/js/loader.js\"></script>" +
                Environment.NewLine + "<script src=\"/js/jspdf.min.js\"></script>" +
                Environment.NewLine + "<script src=\"/js/jquery.min.js\"></script>" +
                Environment.NewLine + "<script src=\"/js/jquery-ui.min.js\"></script> " +
                Environment.NewLine + "<script src=\"/js/bootstrap.min.js\"></script>" +
                Environment.NewLine + "<script src=\"/js/ajSearch.js\"></script>" +
                Environment.NewLine + "<script src=\"/js/Utils.js\"></script>" +
                
                Environment.NewLine + "<script language=\"javascript\">" + Tools.DatePrompt(context) +
                Environment.NewLine + "</script>");

            r = builder.ToString(); builder = null;
            return r;
        }
        ////public static string UIHeaderLoginPage(string PageTitle)
        ////{
        ////    StringBuilder builder = new StringBuilder(); string r = "";
        ////    builder.Append("<meta charset=\"utf-8\">");
        ////    builder.Append(Environment.NewLine + "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
        ////    builder.Append(Environment.NewLine + "<title>" + PageTitle + "</title>");
        ////    builder.Append(Environment.NewLine + "<meta content=\"width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no\" name=\"viewport\">");
        ////    builder.Append(Environment.NewLine + "<script src=\"/js/jquery.min.js\"></script>");
        ////    builder.Append(Environment.NewLine + "<script src=\"/js/jquery-ui.min.js\"></script> ");
        ////    builder.Append(Environment.NewLine + "<script src=\"/js/bootstrap.min.js\"></script>");
        ////    builder.Append(Environment.NewLine + "<script src=\"/js/main.js?v=20190421\"></script>");
        ////    builder.Append(Environment.NewLine + "<script src=\"/js/Utils.js\"></script>");
        ////    builder.Append(Environment.NewLine + "<script src=\"/js/dbtab.js\"></script>");
        ////    builder.Append(Environment.NewLine + "<script src=\"/js/jquery.treetable.js\"></script>");
        ////    builder.Append(Environment.NewLine + "<script src=\"/js/jquery.contextMenu.min.js\"></script>");

        ////    builder.Append(Environment.NewLine + "<link href=\"/css/jquery-ui.css\" rel=\"stylesheet\" type=\"text/css\"/>  ");
        ////    builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/style.css\">");
        ////    builder.Append(Environment.NewLine + "<link href=\"/css/jquery.treetable.css\" rel=\"stylesheet\" type=\"text/css\"/>  ");
        ////    builder.Append(Environment.NewLine + "<link href=\"/css/jquery.treetable.default.css\" rel=\"stylesheet\" type=\"text/css\"/>  ");
        ////    builder.Append(Environment.NewLine + "<link href=\"/css/jquery.screen.css\" rel=\"stylesheet\" type=\"text/css\"/>  ");
        ////    r = builder.ToString(); builder = null;
        ////    return r;
        ////}
        public static string UIHeaderLoginPage(string PageTitle)
        {
            StringBuilder builder = new StringBuilder(); string r = "";
            builder.Append("<meta charset=\"utf-8\">");
            builder.Append(Environment.NewLine + "<meta http-equiv=\"x-ua-compatible\" content=\"ie=edge\">");
            builder.Append(Environment.NewLine + "<title>" + PageTitle + "</title>");
            builder.Append(Environment.NewLine + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");

            builder.Append(Environment.NewLine + "<link href=\"/css/jquery-ui.min.css\" rel=\"stylesheet\" type=\"text/css\"/>  ");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/bootstrap.min.css\">");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/jquery.mCustomScrollbar.css\">");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/jquery.treetable.css\">");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/jquery.treetable.theme.default.css\">");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/mdb.min.css\">");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/bootstrap-datepicker.css\">");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/addons/datatables.min.css\">");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/addons/fixedColumns.dataTables.min.css\">");
            builder.Append(Environment.NewLine + "<link href=\"/css/addons/select2.css\" rel=\"stylesheet\" type=\"text/css\"/>  ");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/style.css\">");
            builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" href=\"/tvc/style.css\">");

            builder.Append(Environment.NewLine + "<style>");
            builder.Append(Environment.NewLine + "/*.edititemcol {border-bottom:1px solid #222;text-align:right;}*/");
            builder.Append(Environment.NewLine + ".trelmtab {border-top:1px solid #ccc;border-collapse:collapse}");
            builder.Append(Environment.NewLine + "</style>");

            builder.Append(
                Environment.NewLine + "<script type=\"text/javascript\" type=\"text/javascript\" src=\"/js/jquery-30.4.1.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery-ui.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.ui.touch-punch.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.treetable.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.mCustomScrollbar.js\"></script>" +

                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/popper.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/bootstrap.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/bootstrap-datepicker.min.js\"></script>" +

                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/datatables.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/dataTables.fixedColumns.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/select2.full.js\"></script>" +                

                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/ajSearch.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/dbtab.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/dbtabattachment.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/Utils.js\"></script>" +

                Environment.NewLine + "<script language=\"javascript\">var ActionKeypress = 0;");

            builder.Append(Environment.NewLine + "function actionKeypress(evt) {" +
                    Environment.NewLine + "    $.widget.bridge('uibutton', $.ui.button);" +
                    Environment.NewLine + "    var e = evobj(evt);" +
                    Environment.NewLine + "	if (e.ctrlKey && ekeyCode(e)==83) {" +
                    Environment.NewLine + "		if (ActionKeypress==1){ // Basetab edit" +
                    Environment.NewLine + "			onUpdateRow (document.ListForm);" +
                    Environment.NewLine + "		} else if (ActionKeypress==2){ // Form edit" +
                    Environment.NewLine + "			mySubmit (document.bosfrm);" +
                    Environment.NewLine + "		}		cancel_bubble(e);" +
                    Environment.NewLine + "	}	else {" +
                    Environment.NewLine + "		escPage(e);	}}" +
                    Environment.NewLine + "");
            builder.Append(Environment.NewLine + "</script>");

            r = builder.ToString(); builder = null;
            return r;
        }
        public static string UIHeaderPage(string PageTitle, bool IsHtml = true)
        {
            StringBuilder builder = new StringBuilder(); string r = "";
            //builder.Append("<!DOCTYPE html>");
            //builder.Append("<html>");
            // builder.Append("<head>");
            builder.Append("<meta charset=\"utf-8\">" +
                Environment.NewLine + "<meta http-equiv=\"x-ua-compatible\" content=\"ie=edge\">" +
                Environment.NewLine + "<title>" + PageTitle + "</title>" +
                Environment.NewLine + "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");

            if (IsHtml) builder.Append(Environment.NewLine + "<link href=\"/css/jquery-ui.min.css\" rel=\"stylesheet\" type=\"text/css\"/>  " +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/bootstrap.min.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/jquery.mCustomScrollbar.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/jquery.treetable.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/jquery.treetable.theme.default.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/mdb.min.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/bootstrap-datepicker.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/addons/datatables.min.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/addons/fixedColumns.dataTables.min.css\">" +
                Environment.NewLine + "<link href=\"/css/addons/select2.css\" rel=\"stylesheet\" type=\"text/css\"/>  " +
                
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/style.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/style-mobile.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" href=\"/tvc/style.css\">");// +
                //Environment.NewLine + "<link rel=\"stylesheet\" href=\"/css/orgchart.css\">"

            builder.Append(Environment.NewLine + "<style>" +
                Environment.NewLine + "/*.edititemcol {border-bottom:1px solid #222;text-align:right;}*/" +
                Environment.NewLine + ".trelmtab {border-top:1px solid #ccc;border-collapse:collapse}" +
                Environment.NewLine + "</style>");

            if (IsHtml) builder.Append(                
                Environment.NewLine + "<script type=\"text/javascript\" type=\"text/javascript\" src=\"/js/jquery-30.4.1.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery-ui.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.ui.touch-punch.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.treetable.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.mCustomScrollbar.js\"></script>" +

                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/popper.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/bootstrap.min.js\"></script>" +                
                //Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/bootstrap-datepicker.min.js\"></script>" +

                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/datatables.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/dataTables.fixedColumns.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/select2.full.js\"></script>" +

                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/loader.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jspdf.min.js\"></script>" +

                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/ajSearch.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/dbtab.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/dbtabattachment.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/Utils.js\"></script>" +

                Environment.NewLine + "<script language=\"javascript\">var ActionKeypress = 0;");

            if (IsHtml) builder.Append(Environment.NewLine + "function actionKeypress(evt) {" +
                    Environment.NewLine + "    $.widget.bridge('uibutton', $.ui.button);" +
                    Environment.NewLine + "    var e = evobj(evt);" +
                    Environment.NewLine + "	if (e.ctrlKey && ekeyCode(e)==83) {" +
                    Environment.NewLine + "		if (ActionKeypress==1){ // Basetab edit" +
                    Environment.NewLine + "			onUpdateRow (document.ListForm);" +
                    Environment.NewLine + "		} else if (ActionKeypress==2){ // Form edit" +
                    Environment.NewLine + "			mySubmit (document.bosfrm);" +
                    Environment.NewLine + "		}		cancel_bubble(e);" +
                    Environment.NewLine + "	}	else {" +
                    Environment.NewLine + "		escPage(e);	}}" +
                    Environment.NewLine + "" +
                    Environment.NewLine + "</script>");

            //builder.Append("</head>");
            r = builder.ToString(); builder = null;
            return r;
        }
        private static string UIIframeSaving(HRSContext context)
        {
            string iframe = context.GetRequestVal("iframe");
            return "<div id='saving" + iframe + "' style=\"display:none; position: fixed;top: 0%;left: 0%;width: 100%;height: 100%;background:#FFF;-moz-opacity: 0.30;opacity: .30;filter: alpha(opacity=30);z-index: 1000;-webkit-animation: animateBackground 1s;animation: animateBackground 1s;\">" +
               Environment.NewLine + "<iframe name='saveTranFrm" + iframe + "' id='saveTranFrm" + iframe + "' style='display:none'></iframe></div>";
        }
        public static string UIMenu(ref ToolDAO toolDAO, HRSContext context, bool MenuOn = true, string MenuID = "0")
        {
            DateTime localDate = DateTime.Now; StringBuilder b = new StringBuilder();
            string jsonMenuSearch = ""; dynamic d = null; 
            string UserID = context.GetSession("UserID");
            string UserName = context.GetSession("UserName");
            string ImageID = context.GetSession("ImageID");
            if (MenuID == "0" || MenuID == "") MenuID = context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";

            string MenuCache; bool IsCached = context._cache.Get("MenuCache_" + UserID + "_" + context.GetSession("language"), out MenuCache);
            if (IsCached)
            {
                //b.Append(UIIframeSaving(context));
                if (MenuOn) b.Append(MenuCache);
            }
            else
            {
                //string iframe = context.GetRequestVal("iframe");
                //b.Append(UIIframeSaving(context));
                StringBuilder bx = new StringBuilder();
                if (MenuOn)
                {
                    GetMenuData(UserID, toolDAO, context, out d);
                    bx.Append(Environment.NewLine + "<ul class=\"custom-scrollbar\">" +
                        Environment.NewLine + "<li>" +
                        Environment.NewLine + "<ul class=\"collapsible collapsible-accordion\">");
                    // Menu cấp 1
                    for (var i = 0; i < d.Menu.Items.Count; i++)
                    {
                        // Menu search
                        if (Tools.GetDataJson(d.Menu.Items[i], "IsSearch") == "1")
                            jsonMenuSearch = jsonMenuSearch + ",{\"MenuName\":\"" + Tools.GetDataJson(d.Menu.Items[i], "MenuName") + "\"," +
                                "\"Url\":\"" + Tools.GetDataJson(d.Menu.Items[i], "Url") + "\"}";
                        if (IsShowMenu(context, d.Menu.Items[i]) && 
                            (Tools.GetDataJson(d.Menu.Items[i], "ParentID", "bigint") == 0 || Tools.GetDataJson(d.Menu.Items[i], "ParentID", "bigint") == -1))
                        {
                            int Cnt = Tools.GetDataJson(d.Menu.Items[i], "Cnt", "int");
                            long Parent1 = Tools.GetDataJson(d.Menu.Items[i], "MenuID", "bigint");
                            bx.Append(Environment.NewLine + "<li>");
                            bx.Append(Environment.NewLine + "<a " +
                                "href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "Url"), Tools.GetDataJson(d.Menu.Items[i], "MenuID"), Cnt) + "\" " +
                                "class=\"collapsible-header waves-effect " + (Tools.GetDataJson(d.Menu.Items[i], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\">" + 
                                Tools.GetDataJson(d.Menu.Items[i], "MenuName") + (Cnt > 0? "<i class=\"fa fa-chevron-right rotate-icon arrow-r\"></i>":"") + " </a>");

                            if (Cnt > 0) // Menu con cấp 2
                            {
                                bx.Append(Environment.NewLine + "<div class=\"collapsible-body\">" +
                                    Environment.NewLine + "<ul>");
                                for (var i1 = 0; i1 < d.Menu.Items.Count; i1++)
                                {
                                    if (IsShowMenu(context, d.Menu.Items[i1]) && (Tools.GetDataJson(d.Menu.Items[i1], "ParentID", "bigint") == Parent1))
                                    {
                                        int Cnt1 = Tools.GetDataJson(d.Menu.Items[i1], "Cnt", "int");
                                        long Parent2 = Tools.GetDataJson(d.Menu.Items[i1], "MenuID", "bigint");
                                        bx.Append(Environment.NewLine + "<li>" +
                                            "<a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i1], "Url"), Tools.GetDataJson(d.Menu.Items[i1], "MenuID"), Cnt1) + "\" class=\"waves-effect " + (Tools.GetDataJson(d.Menu.Items[i1], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\">" + Tools.GetDataJson(d.Menu.Items[i1], "MenuName"));
                                        if (Cnt1 > 0)
                                        {
                                            bx.Append(Environment.NewLine + "<i class=\"fa fa-chevron-right rotate-icon arrow-r\"></i>");
                                            bx.Append(Environment.NewLine + "</a>");
                                        }
                                        else
                                        {
                                            bx.Append(Environment.NewLine + "</a>");
                                        }
                                        if (Cnt1 > 0)// Menu con cấp 3
                                        {
                                            bx.Append(Environment.NewLine + "<div>" +
                                                Environment.NewLine + "<div class=\"ul\">" +
                                                Environment.NewLine + "<ul>");
                                            for (var i2 = 0; i2 < d.Menu.Items.Count; i2++)
                                            {
                                                if (IsShowMenu(context, d.Menu.Items[i2]) && (Tools.GetDataJson(d.Menu.Items[i2], "ParentID", "bigint") == Parent2))
                                                {
                                                    bx.Append(Environment.NewLine + "<li><a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i2], "Url"), Tools.GetDataJson(d.Menu.Items[i2], "MenuID")) + "\" class=\"waves-effect " + (Tools.GetDataJson(d.Menu.Items[i2], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\">" + Tools.GetDataJson(d.Menu.Items[i2], "MenuName") + "</a></li>");
                                                }
                                            }
                                            bx.Append(Environment.NewLine + "</ul>" +
                                                Environment.NewLine + "</div>" +
                                                Environment.NewLine + "</div>");
                                        }// End menu con cấp 3
                                        bx.Append(Environment.NewLine + "</li>");
                                    }
                                }
                                bx.Append(Environment.NewLine + "</ul>" +
                                    Environment.NewLine + "</div>");
                            } // End menu con cấp 2
                            bx.Append(Environment.NewLine + "</li>");
                        }
                    }
                    // End cấp 1
                    bx.Append(Environment.NewLine + "</ul>" +
                        Environment.NewLine + "</li>" +
                        Environment.NewLine + "</ul>");

                    if (jsonMenuSearch != "")
                        jsonMenuSearch = "{\"MenuSearch\":[" + Tools.RemoveFisrtChar(jsonMenuSearch) + "]}";
                    else
                        jsonMenuSearch = "{\"MenuSearch\":[]}";
                    context._cache.Set("jsonMenuSearch_" + UserID + "_" + context.GetSession("language"), jsonMenuSearch, context._cache.CacheByMinute * 5);
                    bx.Append(Environment.NewLine + "</ul></div>");
                    context._cache.Set("MenuCache_" + UserID + "_" + context.GetSession("language"), bx.ToString(), context._cache.CacheByMinute * 5);
                    b.Append(bx.ToString()); bx = null;
                }
            }

            string r = b.ToString(); b = null;
            return r;
        }
        public static string UIHeader(ref ToolDAO toolDAO, HRSContext _context, bool MenuOn = true, string MenuID = "0")
        {
            string l = _context.GetSession("language"); if (l == null || l == "") l = _context.LanguageDefault;
            StringBuilder builder = new StringBuilder(); string r;
            string msg = _context.GetRequestVal("Message");
            if (msg != "")
            {
                string[] arr = (_context.GetRequestVal("Message")).Split(new string[] { "^" }, StringSplitOptions.None);
                int SecondNo = 0; string BoxType = "alert-success";
                if (arr.Length < 2) // Length = 1 
                {
                    msg = arr[0];
                    BoxType = "alert-info";
                }
                else if (arr.Length < 3) // Length = 2
                {
                    BoxType = "alert-info";
                    SecondNo = int.Parse(arr[0]);
                    msg = arr[1];
                }
                else // Length = 3
                {
                    SecondNo = int.Parse(arr[1]);
                    BoxType = arr[0];
                    msg = arr[2];
                }
                if (SecondNo > 0) builder.Append(UIPopupMsg(_context, msg, SecondNo, BoxType));
            }

            builder.Append(Environment.NewLine + "<script language=\"javascript\">" + Tools.DatePrompt(_context));
            builder.Append(Environment.NewLine + "</script>");
            builder.Append(UIIframeSaving(_context));
            if (MenuOn)
            {
                builder.Append(Environment.NewLine + "<header>");
                builder.Append(Environment.NewLine + "<!-- Sidebar navigation -->" +
                    Environment.NewLine + "<!--them class wide de hien icon-->" +
                    Environment.NewLine + "<div id=\"slide-out\" class=\"fixed side-nav mdb-sidenav side-nav-light\">");
                builder.Append(UIMenu(ref toolDAO, _context, MenuOn, MenuID));
                builder.Append(Environment.NewLine + "<div class=\"sidenav-bg\"></div>");
                builder.Append(Environment.NewLine + "</div>");

                builder.Append(Environment.NewLine + "<!-- Navbar -->" +
                    Environment.NewLine + "<nav class=\"navbar fixed-top navbar-toggleable-md navbar-expand-lg scrolling-navbar double-nav red  navbar-dark\">");
                builder.Append(Environment.NewLine + "<div class=\"float-left text-right button_side-nav\">" +
                    Environment.NewLine + "<a href=\"#\" data-activates=\"slide-out\" class=\"button-collapse\">" +
                    Environment.NewLine + "<img src=\"/images/bieutrung.png\" class=\"img-fluid flex-center\">" +
                    Environment.NewLine + "</a>" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- Breadcrumb-->" +
                    Environment.NewLine + "<ul class=\"nav navbar-nav nav-flex-icons ml-auto\">");

                builder.Append(Environment.NewLine + "<li class=\"nav-item\">" +
                    Environment.NewLine + "<a class=\"nav-link\"><i class=\"fa fa-envelope\"></i> <span class=\"clearfix d-none d-sm-inline-block\">Contact</span></a>" +
                    Environment.NewLine + "</li>" +
                    Environment.NewLine + "<li class=\"nav-item\"><a class=\"nav-link\"><i class=\"fa fa-bell\"></i></a>" +
                    Environment.NewLine + "</li>");

                builder.Append(UILanguage(_context));

                builder.Append(UIAccount(ref toolDAO, _context));

                builder.Append(Environment.NewLine + "</ul>" +
                    Environment.NewLine + "</nav>");

                ////builder.Append(Environment.NewLine + "<ul class=\"loged\">");
                ////// <!-- Notifications: style can be found in dropdown.less -->
                ////builder.Append(UINotifications(ref toolDAO, _context));
                ////// <!-- Messages: style can be found in dropdown.less-->
                ////builder.Append(UIMessages(ref toolDAO, _context));
                ////// <!-- User Language: style can be found in dropdown.less -->
                ////builder.Append(UILanguage(_context));
                ////// <!-- User Account: style can be found in dropdown.less -->
                ////builder.Append(UIAccount(ref toolDAO, _context));
                ////builder.Append(UIFormLanguage(_context));
                ////builder.Append(Environment.NewLine + "</ul>");
                builder.Append(Environment.NewLine + "</header>");
            }
            r = builder.ToString(); builder = null;
            return r;
        }
        ////public static string UIHeader_NoUse(ref ToolDAO toolDAO, HRSContext _context, bool MenuOn = true)
        ////{
        ////    string l = _context.GetSession("language"); if (l == null || l == "") l = _context.LanguageDefault;
        ////    StringBuilder builder = new StringBuilder(); string r;
        ////    string msg = _context.GetRequestVal("Message");
        ////    if (msg != "")
        ////    {
        ////        string[] arr = (_context.GetRequestVal("Message")).Split(new string[] { "^" }, StringSplitOptions.None);
        ////        int SecondNo = 0; string BoxType = "alert-success";
        ////        if (arr.Length < 2) // Length = 1 
        ////        {
        ////            msg = arr[0];
        ////            BoxType = "alert-info";
        ////        }
        ////        else if (arr.Length < 3) // Length = 2
        ////        {
        ////            BoxType = "alert-info";
        ////            SecondNo = int.Parse(arr[0]);
        ////            msg = arr[1];
        ////        }
        ////        else // Length = 3
        ////        {
        ////            SecondNo = int.Parse(arr[1]);
        ////            BoxType = arr[0];
        ////            msg = arr[2];
        ////        }
        ////        if(SecondNo > 0) builder.Append(UIPopupMsg(_context, msg, SecondNo, BoxType));
        ////    }
            
        ////    builder.Append(Environment.NewLine + "<script language=\"javascript\">" + Tools.DatePrompt(_context));
        ////    builder.Append(Environment.NewLine + "</script>");
        ////    if (MenuOn)
        ////    {
        ////        builder.Append(Environment.NewLine + "<header class=\"main-header\">");
        ////        builder.Append(Environment.NewLine + "<a href=\"/Home\" class=\"logo\">");
        ////        builder.Append(Environment.NewLine + "<span class=\"logo-mini\"><img src=\"/images/logo-small.png\" /></span>");
        ////        builder.Append(Environment.NewLine + "<span class=\"logo-lg\"><img class=\"\" src=\"/images/logo.png\"></span>");
        ////        builder.Append(Environment.NewLine + "</a>");
        ////        builder.Append(Environment.NewLine + "<nav class=\"navbar navbar-static-top\">");
        ////        builder.Append(Environment.NewLine + "<a href=\"#\" class=\"sidebar-toggle\" data-toggle=\"push-menu\" role=\"button\">");//
        ////        builder.Append(Environment.NewLine + "<span class=\"sr-only\">Toggle navigation</span>");
        ////        builder.Append(Environment.NewLine + "</a>");

        ////        builder.Append(Environment.NewLine + "<div class=\"navbar-custom-menu\">");
        ////        builder.Append(Environment.NewLine + "<ul class=\"nav navbar-nav\">");
        ////        // <!-- Notifications: style can be found in dropdown.less -->
        ////        builder.Append(UINotifications(ref toolDAO, _context));
        ////        // <!-- Messages: style can be found in dropdown.less-->
        ////        builder.Append(UIMessages(ref toolDAO, _context));
        ////        // <!-- User Language: style can be found in dropdown.less -->
        ////        builder.Append(UILanguage(_context));
        ////        // <!-- User Account: style can be found in dropdown.less -->
        ////        builder.Append(UIAccount(ref toolDAO, _context));
        ////        builder.Append(UIFormLanguage(_context));
        ////        builder.Append(Environment.NewLine + "</ul>");
        ////        builder.Append(Environment.NewLine + "</div>");

        ////        builder.Append(Environment.NewLine + "</nav>");//<div style=\"float:right\">" + UILanguage(l, _context.GetUrlBack()) + "</div>
        ////        builder.Append(Environment.NewLine + "</header>");
        ////    }
        ////    r = builder.ToString(); builder = null;
        ////    return r;
        ////}
        public static string UIFooter(bool IsPageLogin = true, string ListShortcut = "")
        {
            StringBuilder builder = new StringBuilder(); string r;
            if (!IsPageLogin)
            {
                builder.Append(Environment.NewLine + "<footer class=\"footer full\">" +
                    Environment.NewLine + "<div class=\"cantrai\">" + ListShortcut +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<div class=\"canphai\">Copyright © 2019 Tinhvan Consulting. All rights reserved.</div>" +
                    Environment.NewLine + "</footer>");
            }
            
            r = builder.ToString(); builder = null;
            return r;
        }
        ////public static string UIFooter_NoUse(bool IsPageLogin = true, bool MenuOn = true)
        ////{
        ////    StringBuilder builder = new StringBuilder(); string r;
        ////    if (!IsPageLogin)
        ////    {
        ////        if (MenuOn)
        ////        {
        ////            builder.Append(Environment.NewLine + "<footer class=\"main-footer\">");
        ////            builder.Append(Environment.NewLine + "<strong>Copyright &copy; 2017-2018 <a href=\"http://histaff.vn\">TVC</a>.</strong> All rights reserved.");
        ////            builder.Append(Environment.NewLine + "</footer><div class=\"control-sidebar-bg\"></div>");
        ////        }
        ////        //builder.Append("</body>");
        ////        //builder.Append("</html>");
        ////    }
        ////    r = builder.ToString(); builder = null;
        ////    return r;
        ////}
        private static void GetMenuData(string UserID, ToolDAO toolDAO, HRSContext context, out dynamic d)
        {
            d = null;
            bool IsCached = context._cache.Get("Menu_" + UserID + "_" + context.GetSession("language"), out d);
            if (!IsCached || context.GetSession("IsCached") == "")
            {
                string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = "";
                d = JObject.Parse("{\"parameterInput\":[" +
                                    "{\"ParamName\":\"SessionUserID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + UserID + "\"}," +
                                    "{\"ParamName\":\"MenuID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"0\"}]}");
                toolDAO.ExecuteStore("Menu", "SP_CMS__Functions_GetListByUser", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
                //context.SetSession("Menu_" + UserID, json);
                d = JObject.Parse(json);
                context._cache.Set("Menu_" + UserID + "_" + context.GetSession("language"), d, context._cache.CacheByMinute * 5);
                context.SetSession("IsCached", "1");
            }
        }
        ////public static string UIMenu(ref ToolDAO toolDAO, HRSContext context, bool MenuOn = true, string MenuID = "0")
        ////{
        ////    DateTime localDate = DateTime.Now; StringBuilder b = new StringBuilder();
        ////    string jsonMenuSearch = ""; dynamic d = null; //string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = "";
        ////    string UserID = context.GetSession("UserID"); string UserName = context.GetSession("UserName"); string ImageID = context.GetSession("ImageID");
        ////    if (MenuID == "0" || MenuID == "") MenuID = context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";

        ////    string MenuCache;//= context.GetSession("MenuCache");
        ////    bool IsCached = context._cache.Get("MenuCache_" + UserID + "_" + context.GetSession("language"), out MenuCache);
        ////    if (IsCached)
        ////    {
        ////        //HTTP_CODE.WriteLogAction("MenuCache: " + MenuCache, context);
        ////        string iframe = context.GetRequestVal("iframe");
        ////        b.Append("<div id='saving" + iframe + "' style=\"display:none; position: fixed;top: 0%;left: 0%;width: 100%;height: 100%;background:#FFF;-moz-opacity: 0.30;opacity: .30;filter: alpha(opacity=30);z-index: 1000;-webkit-animation: animateBackground 1s;animation: animateBackground 1s;\">" +
        ////           Environment.NewLine + "<iframe name='saveTranFrm" + iframe + "' id='saveTranFrm" + iframe + "' style='display:none'></iframe></div>");
        ////        if (MenuOn) b.Append(MenuCache);
        ////    }
        ////    else
        ////    {
        ////        string iframe = context.GetRequestVal("iframe");
        ////        b.Append("<div id='saving" + iframe + "' style=\"display:none; position: fixed;top: 0%;left: 0%;width: 100%;height: 100%;background:#FFF;-moz-opacity: 0.30;opacity: .30;filter: alpha(opacity=30);z-index: 1000;-webkit-animation: animateBackground 1s;animation: animateBackground 1s;\">" +
        ////           Environment.NewLine + "<iframe name='saveTranFrm" + iframe + "' id='saveTranFrm" + iframe + "' style='display:none'></iframe></div>");
        ////        //HTTP_CODE.WriteLogAction("functionName:UIMenu\nUserID:" + UserID + "\nUserName:" + UserName + "\nMenuID:" + MenuID, context);
        ////        StringBuilder bx = new StringBuilder();
        ////        if (MenuOn)
        ////        {
        ////            //json = context.GetSession("Menu_" + UserID);
        ////            //if (json == "")
        ////            GetMenuData(UserID, toolDAO, context, out d);
        ////            /*
        ////            IsCached = context._cache.Get("Menu_" + UserID, out d);
        ////            if (!IsCached)
        ////            {
        ////                d = JObject.Parse("{\"parameterInput\":[" +
        ////                                    "{\"ParamName\":\"SessionUserID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + UserID + "\"}," +
        ////                                    "{\"ParamName\":\"MenuID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + MenuID + "\"}]}");
        ////                toolDAO.ExecuteStore("Menu", "SP_CMS__Functions_GetListByUser", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
        ////                //context.SetSession("Menu_" + UserID, json);
        ////                d = JObject.Parse(json);
        ////                context._cache.Set("Menu_" + UserID, d);
        ////            }     */

        ////            bx.Append(Environment.NewLine + "<div class=\"menuleftvertical\">");// style=\"min-height: 789px;\"
        ////            bx.Append(Environment.NewLine + "<ul class=\"mn\"><li class=\"home\"></li>");
        ////            //bx.Append(spReturn + "SP_CMS__Functions_GetListByUser" + "@UserId;3;1;4;" + UserID);
        ////            long Parent1 = 0; //long Parent2 = 0; long Parent3 = 0;long Parent4 = 0; //long Parent0 = 0; 
        ////            for (var i = 0; i < d.Menu.Items.Count; i++)
        ////            {
        ////                //long.Parse(Tools.GetDataJson(d.Menu.Items[i], "ParentID")) == Parent0
        ////                if (Tools.GetDataJson(d.Menu.Items[i], "IsSearch") == "1")
        ////                    jsonMenuSearch = jsonMenuSearch + ",{\"MenuName\":\"" + Tools.GetDataJson(d.Menu.Items[i], "MenuName") + "\"," +
        ////                        "\"Url\":\"" + Tools.GetDataJson(d.Menu.Items[i], "Url") + "\"}";
        ////                if (IsShowMenu(context, d.Menu.Items[i]) && (Tools.GetDataJson(d.Menu.Items[i], "ParentID", "bigint") == 0 || Tools.GetDataJson(d.Menu.Items[i], "ParentID", "bigint") == -1))
        ////                {
        ////                    int Cnt = Tools.GetDataJson(d.Menu.Items[i], "Cnt", "int");
        ////                    Parent1 = Tools.GetDataJson(d.Menu.Items[i], "MenuID", "bigint");
        ////                    bx.Append(Environment.NewLine + "<li>");
        ////                    bx.Append(Environment.NewLine + "<a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "Url"), Tools.GetDataJson(d.Menu.Items[i], "MenuID")) + "\">");
        ////                    bx.Append(Environment.NewLine + "<span class=\"" + Tools.GetDataJson(d.Menu.Items[i], "CssIcon") + (Tools.GetDataJson(d.Menu.Items[i], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\"></span>" +
        ////                        "<ul class=\"submenu\">");
        ////                    bx.Append(Environment.NewLine + "</a>");
        ////                    bx.Append(Environment.NewLine + "<li>");
        ////                    bx.Append(Environment.NewLine + Tools.GetDataJson(d.Menu.Items[i], "MenuName"));
        ////                    bx.Append(Environment.NewLine + "</li>");
        ////                    /*
        ////                    //-----------------------------------------------------------------------------
        ////                    if (Cnt > 0)
        ////                    {
        ////                        for (var i1 = 0; i1 < d.Menu.Items.Count; i1++)
        ////                        {
        ////                            if (IsShowMenu(context, d.Menu.Items[i1]) && (Tools.GetDataJson(d.Menu.Items[i1], "ParentID", "bigint") == Parent1))
        ////                            {
        ////                                int Cnt1 = Tools.GetDataJson(d.Menu.Items[i1], "Cnt", "int");
        ////                                Parent2 = Tools.GetDataJson(d.Menu.Items[i1], "MenuID", "bigint");
        ////                                bx.Append(Environment.NewLine + "<li>");
        ////                                bx.Append(Environment.NewLine + "<a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i1], "Url"), Tools.GetDataJson(d.Menu.Items[i1], "MenuID")) + "\">");
        ////                                bx.Append(Environment.NewLine + "<span class=\"" + Tools.GetDataJson(d.Menu.Items[i1], "CssIcon") + (Tools.GetDataJson(d.Menu.Items[i1], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\"></span>");
        ////                                bx.Append(Environment.NewLine + Tools.GetDataJson(d.Menu.Items[i1], "MenuName"));
        ////                                bx.Append(Environment.NewLine + "</a>");
        ////                                //-----------------------------------------------------------------------------
        ////                                if (Cnt1 > 0)
        ////                                {
        ////                                    bx.Append(Environment.NewLine + "<ul class=\"submenu\">");
        ////                                    for (var i2 = 0; i2 < d.Menu.Items.Count; i2++)
        ////                                    {
        ////                                        if (IsShowMenu(context, d.Menu.Items[i2]) && (Tools.GetDataJson(d.Menu.Items[i2], "ParentID", "bigint") == Parent2))
        ////                                        {
        ////                                            int Cnt2 = Tools.GetDataJson(d.Menu.Items[i2], "Cnt", "int");
        ////                                            Parent3 = Tools.GetDataJson(d.Menu.Items[i2], "MenuID", "bigint");
        ////                                            bx.Append(Environment.NewLine + "<li>");
        ////                                            bx.Append(Environment.NewLine + "<a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i2], "Url"), Tools.GetDataJson(d.Menu.Items[i2], "MenuID")) + "\">");
        ////                                            bx.Append(Environment.NewLine + "<span class=\"" + Tools.GetDataJson(d.Menu.Items[i2], "CssIcon") + (Tools.GetDataJson(d.Menu.Items[i2], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\"></span>");
        ////                                            bx.Append(Environment.NewLine + Tools.GetDataJson(d.Menu.Items[i2], "MenuName"));
        ////                                            bx.Append(Environment.NewLine + "</a>");
        ////                                            //-----------------------------------------------------------------------------
        ////                                            if (Cnt2 > 0)
        ////                                            {
        ////                                                bx.Append(Environment.NewLine + "<ul class=\"submenu\">");
        ////                                                for (var i3 = 0; i3 < d.Menu.Items.Count; i3++)
        ////                                                {
        ////                                                    if (IsShowMenu(context, d.Menu.Items[i3]) && (Tools.GetDataJson(d.Menu.Items[i3], "ParentID", "bigint") == Parent2))
        ////                                                    {
        ////                                                        int Cnt3 = Tools.GetDataJson(d.Menu.Items[i3], "Cnt", "int");
        ////                                                        Parent4 = Tools.GetDataJson(d.Menu.Items[i3], "MenuID", "bigint");
        ////                                                        bx.Append(Environment.NewLine + "<li>");
        ////                                                        bx.Append(Environment.NewLine + "<a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i3], "Url"), Tools.GetDataJson(d.Menu.Items[i3], "MenuID")) + "\">");
        ////                                                        bx.Append(Environment.NewLine + "<span class=\"" + Tools.GetDataJson(d.Menu.Items[i3], "CssIcon") + (Tools.GetDataJson(d.Menu.Items[i3], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\"></span>");
        ////                                                        bx.Append(Environment.NewLine + Tools.GetDataJson(d.Menu.Items[i3], "MenuName"));
        ////                                                        bx.Append(Environment.NewLine + "</a>");
        ////                                                        //-----------------------------------------------------------------------------
        ////                                                        if (Cnt3 > 0)
        ////                                                        {
        ////                                                            bx.Append(Environment.NewLine + "<ul class=\"submenu\">");
        ////                                                            for (var i4 = 0; i4 < d.Menu.Items.Count; i4++)
        ////                                                            {
        ////                                                                if (IsShowMenu(context, d.Menu.Items[i4]) && (Tools.GetDataJson(d.Menu.Items[i4], "ParentID", "bigint") == Parent2))
        ////                                                                {
        ////                                                                    //int Cnt3 = Tools.GetDataJson(d.Menu.Items[i4], "Cnt", "int");
        ////                                                                    //Parent4 = Tools.GetDataJson(d.Menu.Items[i4], "MenuID", "bigint");
        ////                                                                    bx.Append(Environment.NewLine + "<li>");
        ////                                                                    bx.Append(Environment.NewLine + "<a href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i4], "Url"), Tools.GetDataJson(d.Menu.Items[i4], "MenuID")) + "\">");
        ////                                                                    bx.Append(Environment.NewLine + "<span class=\"" + Tools.GetDataJson(d.Menu.Items[i4], "CssIcon") + (Tools.GetDataJson(d.Menu.Items[i4], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\"></span>");
        ////                                                                    bx.Append(Environment.NewLine + Tools.GetDataJson(d.Menu.Items[i4], "MenuName"));
        ////                                                                    bx.Append(Environment.NewLine + "</a>");
        ////                                                                }
        ////                                                            }
        ////                                                            bx.Append(Environment.NewLine + "</ul>");
        ////                                                        }
        ////                                                        //-----------------------------------------------------------------------------
        ////                                                    }
        ////                                                }
        ////                                                bx.Append(Environment.NewLine + "</ul>");
        ////                                            }
        ////                                            //-----------------------------------------------------------------------------
        ////                                        }
        ////                                    }
        ////                                    bx.Append(Environment.NewLine + "</ul>");
        ////                                }
        ////                                //-----------------------------------------------------------------------------
        ////                                bx.Append(Environment.NewLine + "</li>");
        ////                            }
        ////                        }
        ////                    }
        ////                    //-----------------------------------------------------------------------------
        ////                    */
        ////                    bx.Append(Environment.NewLine + "</ul>");
        ////                    //bx.Append(Environment.NewLine + "</a>");
        ////                    bx.Append(Environment.NewLine + "</li>");
        ////                }
        ////            }
        ////            if (jsonMenuSearch != "")
        ////                jsonMenuSearch = "{\"MenuSearch\":[" + Tools.RemoveFisrtChar(jsonMenuSearch) + "]}";
        ////            else
        ////                jsonMenuSearch = "{\"MenuSearch\":[]}";
        ////            //context.SetSession("jsonMenuSearch_" + UserID, jsonMenuSearch);
        ////            context._cache.Set("jsonMenuSearch_" + UserID + "_" + context.GetSession("language"), jsonMenuSearch, context._cache.CacheByMinute * 5);
        ////            bx.Append(Environment.NewLine + "</ul></div>");
        ////            context._cache.Set("MenuCache_" + UserID + "_" + context.GetSession("language"), bx.ToString(), context._cache.CacheByMinute * 5);
        ////            b.Append(bx.ToString()); bx = null;
        ////        }
        ////    }
            
        ////    string r = b.ToString(); b = null;
        ////    //if (MenuCache == "") context._cache.Set("MenuCache_" + UserID, r); //context.SetSession("MenuCache", r);
        ////    return r;
        ////}
        ////public static string UIMenu_NoUse(ref ToolDAO toolDAO, HRSContext context, bool MenuOn = true, string MenuID = "0")
        ////{
        ////    DateTime localDate = DateTime.Now; StringBuilder b = new StringBuilder();
        ////    string jsonMenuSearch = ""; string json = ""; string parameterOutput = ""; int errorCode = 0; string errorString = ""; dynamic d = null;
        ////    string UserID = context.GetSession("UserID"); string UserName = context.GetSession("UserName"); string ImageID = context.GetSession("ImageID");
        ////    if (MenuID == "0" || MenuID == "") MenuID = context.GetRequestVal("MenuID"); if (MenuID == "") MenuID = "0";

        ////    string iframe = context.GetRequestVal("iframe");
        ////    b.Append("<div id='saving" + iframe + "' style=\"display:none; position: fixed;top: 0%;left: 0%;width: 100%;height: 100%;background:#FFF;-moz-opacity: 0.30;opacity: .30;filter: alpha(opacity=30);z-index: 1000;-webkit-animation: animateBackground 1s;animation: animateBackground 1s;\">" +
        ////           Environment.NewLine + "<iframe name='saveTranFrm" + iframe + "' id='saveTranFrm" + iframe + "' style='display:none'></iframe></div>");
        ////    HTTP_CODE.WriteLogAction("functionName:UIMenu\nUserID:" + UserID + "\nUserName:" + UserName + "\nMenuID:" + MenuID, context);
        ////    if (MenuOn)
        ////    {
        ////        json = context.GetSession("Menu_" + UserID);
        ////        //if (json == "")
        ////        //{
        ////        d = JObject.Parse("{\"parameterInput\":[" +
        ////                            "{\"ParamName\":\"SessionUserID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + UserID + "\"}," +
        ////                            "{\"ParamName\":\"MenuID\", \"ParamType\":\"0\", \"ParamInOut\":\"1\", \"ParamLength\":\"8\", \"InputValue\":\"" + MenuID + "\"}]}");
        ////        toolDAO.ExecuteStore("Menu", "SP_CMS__Functions_GetListByUser", d, ref parameterOutput, ref json, ref errorCode, ref errorString);
        ////        context.SetSession("Menu_" + UserID, json);
        ////        //}
        ////        d = JObject.Parse(json);

        ////        b.Append(Environment.NewLine + "<aside class=\"main-sidebar\">");
        ////        b.Append(Environment.NewLine + "<section class=\"sidebar\">");
        ////        b.Append(Environment.NewLine + "<ul class=\"sidebar-menu\" data-widget=\"tree\">");
        ////        //b.Append(spReturn + "SP_CMS__Functions_GetListByUser" + "@UserId;3;1;4;" + UserID);
        ////        long Parent1 = 0; long Parent2 = 0; long Parent3 = 0;//long Parent4 = 0; long Parent0 = 0; 
        ////        for (var i = 0; i < d.Menu.Items.Count; i++)
        ////        {
        ////            //long.Parse(Tools.GetDataJson(d.Menu.Items[i], "ParentID")) == Parent0
        ////            if (Tools.GetDataJson(d.Menu.Items[i], "IsSearch") == "1")
        ////                jsonMenuSearch = jsonMenuSearch + ",{\"MenuName\":\"" + Tools.GetDataJson(d.Menu.Items[i], "MenuName") + "\"," +
        ////                    "\"Url\":\"" + Tools.GetDataJson(d.Menu.Items[i], "Url") + "\"}";
        ////            if (IsShowMenu(context, d.Menu.Items[i]) && (long.Parse(Tools.GetDataJson(d.Menu.Items[i], "ParentID")) == 0 || long.Parse(Tools.GetDataJson(d.Menu.Items[i], "ParentID")) == -1))
        ////            {
        ////                int Cnt = int.Parse(Tools.GetDataJson(d.Menu.Items[i], "Cnt"));
        ////                Parent1 = long.Parse(Tools.GetDataJson(d.Menu.Items[i], "MenuID"));
        ////                b.Append(Environment.NewLine + "<li class=\"" + /*Tools.GetDataJson(d.Menu.Items[i], "ClassName")*/
        ////                    (Tools.GetDataJson(d.Menu.Items[i], "ListID").IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + (Cnt > 0 ? "treeview" : "") + "\">");
        ////                b.Append(Environment.NewLine + "<a" + (Tools.GetDataJson(d.Menu.Items[i], "WindowName") != "" ? " target=\"" + Tools.GetDataJson(d.Menu.Items[i], "WindowName") + "\"" : "") + " href=\"" + SetUrl(Tools.GetDataJson(d.Menu.Items[i], "Url"), Tools.GetDataJson(d.Menu.Items[i], "MenuID")) + "\">");
        ////                if (Cnt > 0) b.Append(Environment.NewLine + "<i class=\"fa " + Tools.GetDataJson(d.Menu.Items[i], "CssIcon") + "\"></i><span>" + Tools.GetDataJson(d.Menu.Items[i], "MenuName") + "</span>");
        ////                b.Append(Environment.NewLine + "<span class=\"pull-right-container\"><i class=\"fa fa-angle-left pull-right\"></i></span></a>");
        ////                if (Cnt > 0)
        ////                {
        ////                    b.Append(Environment.NewLine + "<ul class=\"treeview-menu\">");
        ////                    for (var i1 = 0; i1 < d.Menu.Items.Count; i1++)
        ////                    {
        ////                        if ((d.Menu.Items[i1].IsDisplay.ToString() == "1") && (long.Parse(d.Menu.Items[i1].ParentID.ToString()) == Parent1))
        ////                        {
        ////                            int Cnt1 = int.Parse(d.Menu.Items[i1].Cnt.ToString());
        ////                            Parent2 = long.Parse(d.Menu.Items[i1].MenuID.ToString());
        ////                            b.Append(Environment.NewLine + "<li class=\"" + /*d.Menu.Items[i1].ClassName.ToString()*/
        ////                                (d.Menu.Items[i1].ListID.ToString().IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + (Cnt1 > 0 ? "treeview" : "") + "\">");
        ////                            b.Append(Environment.NewLine + "<a" + (d.Menu.Items[i1].WindowName.ToString() != "" ? " target=\"" + d.Menu.Items[i1].WindowName.ToString() + "\"" : "") + " href=\"" + SetUrl(d.Menu.Items[i1].Url.ToString(), d.Menu.Items[i1].MenuID.ToString()) + "\">");
        ////                            b.Append(Environment.NewLine + "<i class=\"fa " + d.Menu.Items[i1].CssIcon + "\"></i><span>" + d.Menu.Items[i1].MenuName.ToString() + "</span>");
        ////                            if (Cnt1 > 0) b.Append(Environment.NewLine + "<span class=\"pull-right-container\"><i class=\"fa fa-angle-left pull-right\"></i></span>");
        ////                            b.Append(Environment.NewLine + "</a>");
        ////                            if (Cnt1 > 0)
        ////                            {
        ////                                b.Append(Environment.NewLine + "<ul class=\"treeview-menu\">");
        ////                                for (var i2 = 0; i2 < d.Menu.Items.Count; i2++)
        ////                                {
        ////                                    if ((d.Menu.Items[i2].IsDisplay.ToString() == "1") && (long.Parse(d.Menu.Items[i2].ParentID.ToString()) == Parent2))
        ////                                    {
        ////                                        int Cnt2 = int.Parse(d.Menu.Items[i2].Cnt.ToString());
        ////                                        Parent3 = long.Parse(d.Menu.Items[i2].MenuID.ToString());
        ////                                        b.Append(Environment.NewLine + "<li class=\"" + /*d.Menu.Items[i2].ClassName.ToString()*/
        ////                                            (d.Menu.Items[i2].ListID.ToString().IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + (Cnt2 > 0 ? "treeview" : "") + "\">");
        ////                                        b.Append(Environment.NewLine + "<a" + (d.Menu.Items[i2].WindowName.ToString() != "" ? " target=\"" + d.Menu.Items[i2].WindowName.ToString() + "\"" : "") + " href=\"" + SetUrl(d.Menu.Items[i2].Url.ToString(), d.Menu.Items[i2].MenuID.ToString()) + "\">");
        ////                                        b.Append(Environment.NewLine + "<i class=\"fa " + d.Menu.Items[i2].CssIcon + "\"></i><span>" + d.Menu.Items[i2].MenuName.ToString() + "</span>");
        ////                                        if (Cnt2 > 0) b.Append(Environment.NewLine + "<span class=\"pull-right-container\"><i class=\"fa fa-angle-left pull-right\"></i></span>");
        ////                                        b.Append(Environment.NewLine + "</a>");
        ////                                        if (Cnt2 > 0)
        ////                                        {
        ////                                            b.Append(Environment.NewLine + "<ul class=\"treeview-menu\">");
        ////                                            for (var i3 = 0; i3 < d.Menu.Items.Count; i3++)
        ////                                            {
        ////                                                if ((d.Menu.Items[i3].IsDisplay.ToString() == "1") && (long.Parse(d.Menu.Items[i3].ParentID.ToString()) == Parent3))
        ////                                                {
        ////                                                    b.Append(Environment.NewLine + "<li class=\"" + /*d.Menu.Items[i3].ClassName.ToString()*/
        ////                                                        (d.Menu.Items[i3].ListID.ToString().IndexOf(";" + MenuID + ";") > -1 ? " active " : "") + "\">");
        ////                                                    b.Append(Environment.NewLine + "<a" + (d.Menu.Items[i3].WindowName.ToString() != "" ? " target=\"" + d.Menu.Items[i3].WindowName.ToString() + "\"" : "") + " href=\"" + SetUrl(d.Menu.Items[i3].Url.ToString(), d.Menu.Items[i3].MenuID.ToString()) + "\">");
        ////                                                    b.Append(Environment.NewLine + "<i class=\"fa " + d.Menu.Items[i3].CssIcon + "\"></i><span>" + d.Menu.Items[i3].MenuName.ToString() + "</span>");
        ////                                                    b.Append(Environment.NewLine + "</a>");
        ////                                                    b.Append(Environment.NewLine + "</li>");
        ////                                                }
        ////                                            }
        ////                                            b.Append(Environment.NewLine + "</ul>");
        ////                                        }
        ////                                        b.Append(Environment.NewLine + "</li>");
        ////                                    }
        ////                                }
        ////                                b.Append(Environment.NewLine + "</ul>");
        ////                            }
        ////                            b.Append(Environment.NewLine + "</li>");
        ////                        }
        ////                    }
        ////                    b.Append(Environment.NewLine + "</ul>");
        ////                }
        ////                b.Append(Environment.NewLine + "</li>");
        ////            }
        ////        }
        ////        if (jsonMenuSearch != "")
        ////            jsonMenuSearch = "{\"MenuSearch\":[" + Tools.RemoveFisrtChar(jsonMenuSearch) + "]}";
        ////        else
        ////            jsonMenuSearch = "{\"MenuSearch\":[]}";
        ////        context.SetSession("jsonMenuSearch_" + UserID, jsonMenuSearch);
        ////        b.Append(Environment.NewLine + "</ul></section></aside>");
        ////    }

        ////    string r = b.ToString(); b = null;
        ////    return r;
        ////}
        public static string UIContentTagOpen(ref ToolDAO toolDAO, HRSContext context, bool MenuOn, string MenuID = "0", bool IsSubContent = true)
        {
            string r = "";
            if (MenuOn)
            {
                r = Environment.NewLine + "<!-- container noidungchinh full -->" +
                Environment.NewLine + "<div class=\"container noidungchinh full\">" +
                Environment.NewLine + "<!-- nav aria-label=\"breadcrumb\" -->" +
                Environment.NewLine + "<nav aria-label=\"breadcrumb\">" +
                UIMenuPath(ref toolDAO, context, MenuOn, MenuID) + "</nav>" +
                Environment.NewLine + "<!-- /nav aria-label=\"breadcrumb\" -->" +
                (IsSubContent ?
                    Environment.NewLine + "<!-- container-fluid change_width -->" +
                    Environment.NewLine + "<div class=\"container-fluid change_width\">" +
                    Environment.NewLine + "<!-- change_width -->" +
                    Environment.NewLine + "<div id='change_width' data-changeWidth=\"25%\" >" +
                    Environment.NewLine + "<!-- id='A' -->" +
                    Environment.NewLine + "<div id='A' class=\"w-260\">" :
                    Environment.NewLine + "<!-- container-fluid change_width -->" +
                    Environment.NewLine + "<div class=\"container-fluid change_width\">" +
                    Environment.NewLine + "<!-- change_width -->" +
                    Environment.NewLine + "<div id='change_width' data-changeWidth=\"0%\" class=\"HiddenColB\" >" +
                    Environment.NewLine + "<!-- id='A' -->" +
                    Environment.NewLine + "<div id='A'>"
                ) +
                Environment.NewLine + "<!-- content-change_width -->" +
                Environment.NewLine + "<div class=\"content-change_width\">";
            }
            else
            {
                //r = Environment.NewLine + "<!-- container noidungchinh full -->" +
                //    Environment.NewLine + "<div class=\"container noidungchinh full\">" +
                //    Environment.NewLine + "<!-- container-fluid change_width -->" +
                //    Environment.NewLine + "<div class=\"container-fluid change_width\">" +
                //    Environment.NewLine + "<!-- change_width -->" +
                //    Environment.NewLine + "<div id='change_width' data-changeWidth=\"0%\" class=\"HiddenColB\" >" +
                //    Environment.NewLine + "<!-- id='A' -->" +
                //    Environment.NewLine + "<div id='A'>" +
                //    Environment.NewLine + "<!-- content-change_width -->" +
                //    Environment.NewLine + "<div class=\"content-change_width\">";
            }
                
            return r;
        }
        ////public static string UIContentTagOpen_NoUse(ref ToolDAO toolDAO, HRSContext context, bool MenuOn, string MenuID = "0", string EditLeftForm = "")
        ////{
        ////    string r = Environment.NewLine + "<div" + (MenuOn ? " class=\"content-wrapper\"" : " class=\"content\"") + ">" +
        ////        UIMenuRelated(context, MenuOn, MenuID) + UIMenuPath(ref toolDAO, context, MenuOn, MenuID) +
        ////            Environment.NewLine + "<section class=\"content pd-top-10\">" +
        ////            Environment.NewLine + "<div class=\"row\">" +
        ////            Environment.NewLine + "<div class=\"col-xs-12\">";
        ////            r = r + Environment.NewLine + (EditLeftForm == ""? "<div class=\"box\">": (EditLeftForm == "1" ? "<div class=\"col-right1\">" : EditLeftForm + "<div class=\"col-right\">"));
        ////    return r;
        ////}      
        public static string UIContentTagClose(HRSContext context, bool MenuOn = true, bool IsSubContent = true)
        {
            if (MenuOn)
            {
                return Environment.NewLine + "<!-- /content-change_width -->" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /id='A' -->" +
                    Environment.NewLine + "</div>" +
                    (IsSubContent ?
                    Environment.NewLine + "<div id='B' class=\"w-260\">" +
                    Environment.NewLine + "<div class=\"content-change_width\" id='subContentData'>" +
                    //Environment.NewLine + "<div class=\"scroll-content\" id='subContentData'></div>" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<div id='Z' title-z=\"" + context.GetLanguageLable("ClickToClose") + "\"></div>" : "") +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /change_width -->" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /container-fluid change_width -->" +
                    Environment.NewLine + "</div>" +
                    Environment.NewLine + "<!-- /container noidungchinh full -->";
            }
            else
            {
                return "";
                //Environment.NewLine + "<!-- /content-change_width -->" +
                //    Environment.NewLine + "</div>" +
                //    Environment.NewLine + "<!-- /id='A' -->" +
                //    Environment.NewLine + "</div>" +
                //    Environment.NewLine + "</div>" +
                //    Environment.NewLine + "<!-- /change_width -->" +
                //    Environment.NewLine + "</div>" +
                //    Environment.NewLine + "<!-- /container-fluid change_width -->" +
                //    Environment.NewLine + "</div>" +
                //    Environment.NewLine + "<!-- /container noidungchinh full -->";
            }
        }
        public static string UIHeaderPopup(HRSContext _context, string PageTitle, string PageType = "UploadImage")
        {
            StringBuilder builder = new StringBuilder(); string r = "";
            builder.Append(Environment.NewLine + "<!DOCTYPE html>" +
                Environment.NewLine + "<html>" +
                Environment.NewLine + "<head>" +
                Environment.NewLine + "<meta charset=\"utf-8\">" +
                Environment.NewLine + "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">" +
                Environment.NewLine + "<title>" + PageTitle + "</title>" +
                Environment.NewLine + "<meta content=\"width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no\" name=\"viewport\">" +

                // Css
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/jquery-ui.min.css\"/>  " +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/bootstrap.min.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/jquery.mCustomScrollbar.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/jquery.treetable.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/jquery.treetable.theme.default.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/mdb.min.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/bootstrap-datepicker.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/addons/datatables.min.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/addons/fixedColumns.dataTables.min.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/addons/select2.css\"/>  " +

                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/style.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/style-mobile.css\">" +
                Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/tvc/style.css\">" +

                // Js
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery-30.4.1.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery-ui.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.ui.touch-punch.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.treetable.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.mCustomScrollbar.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/popper.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/bootstrap.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/datatables.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/dataTables.fixedColumns.min.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/addons/select2.full.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/select2totree.js\"></script> " +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/ajSearch.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/Utils.js\"></script>");

            switch (PageType)
            {
                case "ImportExcel":
                    builder.Append(Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jszip.js\"></script>");
                    builder.Append(Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/xlsx.js\"></script>");                    
                    break;
                case "UploadFile":
                    break;
                case "UploadImage":
                    break;
                case "UploadImagePost":
                    builder.Append(Environment.NewLine + "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/jquery.Jcrop.css\" />");
                    builder.Append(Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/jquery.Jcrop.js\"></script>");
                    break;
            }

            builder.Append(Environment.NewLine + "</head>" +
                Environment.NewLine + "<body class=\"hold-transition skin-blue sidebar-mini\" onload=\"resizePopupPage();\">");
            string msg = _context.GetRequestVal("Message");
            if (msg != "")
            {
                string[] arr = (_context.GetRequestVal("Message")).Split(new string[] { "^" }, StringSplitOptions.None);
                int SecondNo = 1; string BoxType = "alert-success"; bool IsWindowClose = false; bool IsParentReload = false;
                if (arr.Length < 2) // Length = 1 msg
                {
                    msg = arr[0];
                    BoxType = "alert-info";
                }
                else if (arr.Length < 3) // Length = 2 SecondNo^msg
                {
                    BoxType = "alert-info";
                    SecondNo = int.Parse(arr[0]);
                    msg = arr[1];
                }
                else if (arr.Length < 4) // Length = 3 BoxType^SecondNo^msg
                {
                    SecondNo = int.Parse(arr[1]);
                    BoxType = arr[0];
                    msg = arr[2];
                }
                else if (arr.Length < 5) // Length = 4 IsWindowClose^BoxType^SecondNo^msg
                {
                    SecondNo = int.Parse(arr[2]);
                    BoxType = arr[1];
                    msg = arr[3];
                    IsWindowClose = (arr[0] == "1");
                }
                else // Length = 5 IsParentReload^IsWindowClose^BoxType^SecondNo^msg
                {
                    IsWindowClose = (arr[1] == "1");
                    IsParentReload = (arr[0] == "1");
                    SecondNo = int.Parse(arr[3]);
                    BoxType = arr[2];
                    msg = arr[4];
                }
                if (SecondNo > 0) builder.Append(UIPopupMsg(_context, msg, SecondNo, BoxType, IsWindowClose, IsParentReload));
            }
            builder.Append("<div id=\"A\"><div class=\"content-change_width\">");//<div class=\"row\"><div class=\"box\">");
            r = builder.ToString(); builder = null;
            return r;
        }
        public static string UIFooterPopup(HRSContext _context, string PageType = "UploadImage")
        {
            StringBuilder builder = new StringBuilder(); string r = "";
            switch (PageType)
            {
                case "ImportExcel":
                    builder.Append("<script type=\"text/javascript\">" +
                    Environment.NewLine + "var arrOption = new Array(); " +
                    Environment.NewLine + "function getData() { " +
                    Environment.NewLine + "     var a = document.getElementById('Sheets'); " +
                    Environment.NewLine + "     if (a)if (a.selectedIndex){" +
                    Environment.NewLine + "         document.getElementById('Excel-Preview').innerHTML=arrOption[a.selectedIndex-1];" +
                    Environment.NewLine + "     }" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "function handleFileSelect(evt) {" +
                    Environment.NewLine + "var files = evt.target.files; // FileList object " +
                    Environment.NewLine + "var xl2json = new ExcelToJSON();" +
                    Environment.NewLine + "xl2json.parseExcel(files[0]);}");
                    builder.Append("" +
                        Environment.NewLine + "var ExcelToJSON = function() {" +
                        Environment.NewLine + "     this.parseExcel = function(file) {" +
                        Environment.NewLine + "         var IsPreview = document.getElementById('chkPrev0').checked; // FileList object " +
                        Environment.NewLine + "         var reader = new FileReader(); " +
                        Environment.NewLine + "         reader.onload = function(e) { " +
                        Environment.NewLine + "             var data = e.target.result; " +
                        Environment.NewLine + "             var workbook = XLSX.read(data, { type: 'binary'});" +
                        Environment.NewLine + "             var strOption = \"<option value=''>" + _context.GetLanguageLable("ChoiceSheets") + "</option>\";" +
                        Environment.NewLine + "             var ij = 0;" +
                        Environment.NewLine + "             workbook.SheetNames.forEach(function(sheetName) {" +
                        Environment.NewLine + "                 // Here is your object" +
                        Environment.NewLine + "                 strOption += \"<option value='\" + sheetName + \"'>\" + sheetName + \"</option>\";" +
                        Environment.NewLine + "                 if(IsPreview){" +
                        Environment.NewLine + "                 var XL_row_object = XLSX.utils.sheet_to_row_object_array(workbook.Sheets[sheetName]);" +
                        Environment.NewLine + "                 var json_object = JSON.parse(JSON.stringify(XL_row_object));" +
                        Environment.NewLine + "txt = \"<table border='1' class='table table-hover table-border flex'>\";" +
                        Environment.NewLine + "var y = json_object[0];" +
                        Environment.NewLine + "txt += \"<tr>\";" +
                        Environment.NewLine + "for (var z in y){" +
                        Environment.NewLine + "txt += \"<td><b>\" + z + \"</b></td>\"; }" +
                        Environment.NewLine + "txt +=\"</tr>\"; " +
                        Environment.NewLine + "for (var i=0; i<json_object.length; i++) {" +
                        Environment.NewLine + "var y = json_object[i];" +
                        Environment.NewLine + "txt += \"<tr>\";" +
                        Environment.NewLine + "for (var z in y){" +
                        Environment.NewLine + "txt += \"<td>\" + y[z] + \"</td>\"; }" +
                        Environment.NewLine + "txt +=\"</tr>\";} " +
                        Environment.NewLine + "txt += \"</table>\"; " +
                        Environment.NewLine + "                 arrOption[ij] = \"<p></p>" + _context.GetLanguageLable("SheetName") + ": <b>\" + sheetName + \"</b><hr>\" + txt; ij++;" +
                        Environment.NewLine + "                 //document.getElementById('Excel-Preview').innerHTML = document.getElementById('Excel-Preview').innerHTML + \"<p></p>sheetName: <b>\" + sheetName + \"</b><hr>\" + txt;" +
                        Environment.NewLine + "                 }else{arrOption[ij] = '';ij++;}" +
                        Environment.NewLine + "             });" +
                        Environment.NewLine + "             document.getElementById('Sheets').innerHTML = strOption;" +
                        Environment.NewLine + "         };" +
                        Environment.NewLine + "         reader.onerror = function(ex) { JsAlert('alert-error', '" + _context.GetLanguageLable("Alert-Error") + "', ex, '', '0');}; " +//alert(ex);
                        Environment.NewLine + "         reader.readAsBinaryString(file);" +
                        Environment.NewLine + "     };" +
                        Environment.NewLine + "};" +
                        Environment.NewLine + "document.getElementById('Upload').addEventListener('change', handleFileSelect, false);");
                    builder.Append("</script>");
                    break;
                case "UploadFile":
                    break;
                case "UploadImage":
                    builder.Append("<script type=\"text/javascript\">");
                    builder.Append("document.frmAtt.Upload.onchange=function (evt) { " +
                        Environment.NewLine + "var Img = document.getElementById(\"Upload-Preview\"); " +
                        Environment.NewLine + "var files = evt.target.files; if (files && files[0])" +
                        Environment.NewLine + "{ var reader = new FileReader(); " +
                        Environment.NewLine + "reader.onload = function(e) {" +
                        Environment.NewLine + "Img.src= e.target.result; Img.style.width = \"200px\"; };  " +
                        Environment.NewLine + "reader.readAsDataURL(files[0]);  } }; ");
                    builder.Append("</script>");
                    break;
                case "UploadImagePost":
                    builder.Append("<script type=\"text/javascript\">");
                    builder.Append("jQuery(document).ready(function() {" +
                        Environment.NewLine + "    jQuery('#imgCrop').Jcrop({" +
                        Environment.NewLine + "      onSelect: storeCoords" +
                        Environment.NewLine + "    }); });" +
                        Environment.NewLine + "function storeCoords(c) {" +
                        Environment.NewLine + "jQuery('#X').val(c.x);" +
                        Environment.NewLine + "jQuery('#Y').val(c.y);" +
                        Environment.NewLine + "jQuery('#W').val(c.w);" +
                        Environment.NewLine + "jQuery('#H').val(c.h);};");
                    builder.Append("</script>");
                    break;
            }
            builder.Append(Environment.NewLine + "</div>" +
                           Environment.NewLine + "</div>" + //content row box
                           //Environment.NewLine + "</div>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/cuttom.js\"></script>" +
                Environment.NewLine + "<script type=\"text/javascript\" src=\"/js/mdb.min.js\"></script>" +
                Environment.NewLine + "</body>" +
                Environment.NewLine + "</html>");
            r = builder.ToString(); builder = null;
            return r;
        }
        public static string UIPopupMsg(HRSContext _context, 
            string msg = "Best check yo self, you're not...", int time = 1, string className = "alert-success", 
            bool IsWindowClose = false, bool IsParentReload = false)
        // alert-success; alert-error; alert-warning; alert-info 
        {
            //string IsMessage = _context.GetCookie("IsMessage");if (IsMessage == null) IsMessage = "";
            //if (IsMessage == "") return "";
            string msgHeader;
            msg = msg.Replace("??", "?")
                    .Replace("'", "")
                    .Replace("%27", "")
                    .Replace("\"", "")
                    .Replace("%22", "")
                    .Replace("\\\\", "")
                    .Replace("\\", "")
                    .Replace("(", "")
                    .Replace("{", "")
                    .Replace(")", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("}", "")
                .Replace("»", ">")
                .Replace("«", "<");
            switch (className)
            {
                case "alert-warning":
                    msgHeader = "Alert-Warning";
                    break;
                case "alert-error":
                    msgHeader = "Alert-Error";
                    break;
                case "alert-info":
                    msgHeader = "Notification";
                    break;
                default:
                    msgHeader = "Alert-Success";
                    break;
            }
            /*
            string r = "<div class=\"alert " + className + "\" style=\"z-index:99995; padding: 10px;\">" +
                    Environment.NewLine + "" +
                    (IsWindowClose? "" : Environment.NewLine + "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">×</button>") +
                    Environment.NewLine + "<script>" +
                    Environment.NewLine + "var CntBar = 100;" +
                    Environment.NewLine + "var iTime = (" + time.ToString() + "000);" +
                    Environment.NewLine + "var myVar = setInterval(myTimer, iTime);" +
                    Environment.NewLine + "function myTimer() {" +
                    Environment.NewLine + "CntBar = CntBar - 10;" +
                    Environment.NewLine + "$('.progress-bar').css('width', CntBar + \"%\");" +
                    Environment.NewLine + "if (CntBar <= 0) {" +
                    Environment.NewLine + "clearInterval(myVar);" +
                    Environment.NewLine + "$('.close').click();";
            if (IsWindowClose)
            {
                if (IsParentReload)
                {
                    r = r + Environment.NewLine + "var parent=window.opener;if (parent==null) parent=dialogArguments;parent.focus();parent.location.reload();";
                }
                r = r + Environment.NewLine + "window.close();";
            }
            r = r + Environment.NewLine + "}" +
                    Environment.NewLine + "}" +
                    Environment.NewLine + "</script>" +
                    Environment.NewLine + "<h4>" + Tools.HtmlEncode(_context.GetLanguageLable(msgHeader)) + "!</h4>" +
                    Environment.NewLine + msg + //Tools.HtmlEncode(msg)
                    Environment.NewLine + "<p><br></p>" +
                    Environment.NewLine + "<div class=\"progress-sm\">" +
                    Environment.NewLine + "<div class=\"progress-bar\" style=\"width: 100%;\"></div></div></div>";
*/
            string r = "<script>" +
                Environment.NewLine + "JsAlert('" + className + "', '" + Tools.HtmlEncode(_context.GetLanguageLable(msgHeader)) + "', '" + msg + "', '', '" +
                (IsParentReload? "4" : (IsWindowClose ? "3" : "0")) +
                "');" + 
                Environment.NewLine + "</script>";
            return r;
        }
        #endregion

        #region UI Tab child form
        public static string UITabForm(HRSContext context, string ChildName, string ChildURL, string DefaultName = "")
        {
            StringBuilder b = new StringBuilder(); string r = "";
            string[] a1; string[] a2;
            string url0 = ""; string urli = "";
            a1 = ChildName.Split(new string[] { "^" }, StringSplitOptions.None);
            a2 = ChildURL.Split(new string[] { "^" }, StringSplitOptions.None);
            if ((ChildURL != "") && (ChildURL != "0"))
            {
                //b.Append("<div class=\"nav-tabs-custom\"><ul class=\"nav nav-tabs\">");
                b.Append(Environment.NewLine + "<div class=\"row\" style=\"margin-top:10px\">" + "" +
                    Environment.NewLine + "<!-- col-sm-10 use-content -->" +
                    Environment.NewLine + "<div class=\"col-sm\">" +
                    Environment.NewLine + "<div class=\"classic-tabs\">" +
                    Environment.NewLine + "<ul class=\"nav\">");
                string aClass = " active ";
                b.Append(Environment.NewLine + "<script language=\"javascript\">" +
                        Environment.NewLine + "var li = new Array();" +
                        Environment.NewLine + "</script>");
                for (var i = 0; i < a2.Length; i++)
                {
                    string[] a0; string[] a01; string[] a02;
                    a0 = a2[i].Split(new string[] { "||" }, StringSplitOptions.None);
                    a01 = a0[1].Split(new string[] { ";" }, StringSplitOptions.None);
                    a02 = a0[2].Split(new string[] { ";" }, StringSplitOptions.None);
                    urli = a0[0] + "?" + a01[0] + "=" + a02[0] + "&iframe=" + i;
                    for (var j = 1; j < a01.Length; j++)
                    {
                        string _a02 = a02[j];
                        if (_a02.Length > 7)
                        {
                            //if (_a02.Substring(0, 7) == "REQUEST") _a02 = context.GetRequestVal(_a02.Substring(7));
                            if (Tools.Left(_a02, 7) == "REQUEST") _a02 = context.GetRequestVal(Tools.Right(_a02, 7, false));
                        }
                        urli = urli + "&" + a01[j] + "=" + _a02;
                    }
                    if (i == 0 && DefaultName == "" || DefaultName == a1[i])
                    {
                        url0 = urli;
                        b.Append(Environment.NewLine + "<li class=\"nav-item\" onclick=\"javascript:onChoiceTab('" + urli + "', 'cl_" + i + "', 'subw');\">" +
                            Environment.NewLine + "<a id=\"cl_" + i + "\" class=\"nav-item nav-link waves-light " + aClass + "\" href=\"javascript:void(0)\">" + context.GetLanguageLable(a1[i]) + "</a></li>");
                    }
                    else
                        b.Append(Environment.NewLine + "<li class=\"nav-item\" onclick=\"javascript:onChoiceTab('" + urli + "', 'cl_" + i + "', 'subw');\">" +
                            Environment.NewLine + "<a id=\"cl_" + i + "\" class=\"nav-item nav-link waves-light\" href=\"javascript:void(0)\">" + context.GetLanguageLable(a1[i]) + "</a></li>");

                    b.Append(Environment.NewLine + "<script language=\"javascript\">" +
                        Environment.NewLine + "li[" + i + "] = 'cl_" + i + "';" +
                        Environment.NewLine + "</script>");
                }
                b.Append("</ul>" +
                    "<div class=\"tab-content\">" +
                    "<iframe name='subw' id='subw' height='400' width='100%' FrameBorder='0' src='" + url0 + "' onload=\"resizeIframe(this)\"></iframe>" +
                    "</div>" +
                    "</div>" +
                    "</div>" +
                    "</div>");
                b.Append(Environment.NewLine + "<script language=\"javascript\">" +
                        Environment.NewLine + "function resizeIframe(obj) {" +
                        Environment.NewLine + "obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';" +
                        //Environment.NewLine + "obj.style.width = obj.contentWindow.document.body.scrollWidth + 'px';" +
                        Environment.NewLine + "}" +
                        Environment.NewLine + "function onChoiceTab(urli, a, b){" +
                        Environment.NewLine + "//alert('urli:' + urli + '\\na:' + a + '\\nb:' + b);" +
                        Environment.NewLine + "for(var i = 0; i < li.length; i++){" +
                        Environment.NewLine + "document.getElementById(li[i]).className='nav-item nav-link waves-light';" +
                        Environment.NewLine + "document.getElementById('subw').src=urli;" +
                        Environment.NewLine + "}" +
                        Environment.NewLine + "var liTag = document.getElementById(a);" +
                        Environment.NewLine + "liTag.className = 'nav-item nav-link waves-light active';" +
                        Environment.NewLine + "}" +
                        Environment.NewLine + "</script>");
            }//style='margin-left:25px' iframe
            //else b.Append("1");
            r= b.ToString(); b = null;
            return r;
        }
        #endregion

    }
}


////private static string Treeview(ToolDAO toolDAO, HRSContext context,
////    string InputName, string ProcName, string ProcParam, string ColumnName, string ColumnType, ref string InputCheck, out string Found,
////    bool IsGrid = false, bool IsClass = false, string InputType = "1", string UITreeFunc = "")
////{
////    StringBuilder r = new StringBuilder(); string r1 = ""; Found = "";
////    dynamic d = null; string parameterOutput = ""; string json = ""; int errorCode = 0; string errorString = ""; 
////    string[] a = ColumnName.Split(new string[] { "," }, StringSplitOptions.None);
////    string[] b = ColumnType.Split(new string[] { "," }, StringSplitOptions.None);
////    if (ProcParam != "") d = JObject.Parse(ProcParam);
////    toolDAO.ExecuteStore("UITree", ProcName, d, ref parameterOutput, ref json, ref errorCode, ref errorString);
////    d = JObject.Parse(json);

////    r.Append(Environment.NewLine + "<table class=\"table treetinhvan table-hover use-icon treetable\" id=\"Tree" + InputName + "\">");
////    r.Append(Environment.NewLine + "<thead class=\"thead-light\"><tr>");
////    for (int i = 1; i < a.Length; i++)
////    {
////        bool IsTd = true;
////        if (i < b.Length) if (b[i] == "-") IsTd = false;
////        if (!IsGrid && i > 1) IsTd = false;
////        if (a[i] == "OrderBy" || a[i] == "ParentOrderBy") IsTd = false;
////        if (IsTd) r.Append(Environment.NewLine + "<th>" + context.GetLanguageLable(a[i]) + "</th>");
////    }
////    r.Append("</tr></thead>");
////    r.Append(Environment.NewLine + "<tbody>");
////    if (d.UITree.Items.Count <= 0)
////    {
////        switch (InputType)
////        {
////            case "1": // Radio                       
////            case "2": // Checkbox
////                r.Append("<input type='hidden' id='" + InputName + "_0' name='" + InputName + "' value='0'>");
////                break;
////        }
////    }
////    string txtValChk = "";
////    for (int i = 0; i < d.UITree.Items.Count; i++)
////    {
////        string IsChecked = "";
////        string a_0 = Tools.GetDataJson(d.UITree.Items[i], a[0]);
////        string a_1 = Tools.GetDataJson(d.UITree.Items[i], a[1]);
////        string a_OrderBy = Tools.GetDataJson(d.UITree.Items[i], "OrderBy");
////        if (("," + InputCheck + ",").IndexOf("," + a_0 + ",") > -1)
////        {
////            Found = Found + "," + a_OrderBy;
////            txtValChk = txtValChk + "," + a_1;
////            IsChecked = " checked";
////        }
////        r.Append(Environment.NewLine + "<tr id=\"trrowid" + i + "\" data-tt-id=\"" + a_OrderBy + "\" " + (Tools.GetDataJson(d.UITree.Items[i], "ParentID", "bigint") > 0 ? "data-tt-parent-id=\"" + Tools.GetDataJson(d.UITree.Items[i], "ParentOrderBy") + "\"" : "") + ">");
////        for (int j = 1; j < a.Length; j++)
////        {
////            bool IsTd = true;
////            if (j < b.Length) if (b[j] == "-") IsTd = false;
////            if (!IsGrid && j > 1) IsTd = false;
////            if (a[j].ToLower() == "orderby" || a[j].ToLower() == "parentorderby") IsTd = false;

////            if (IsTd)
////            {
////                string val = Tools.GetDataJson(d.UITree.Items[i], a[j]); ;
////                if (j == 1)
////                {
////                    string ClassVal = val;// (IsClass? "<span class='" + (Tools.GetDataJson(d.UITree.Items[i], "Cnt", "int") > 0 ? "folder" : "sFile") + "'>" + val + "</span>" : val);
////                    r.Append(Environment.NewLine + "<td>");
////                    switch (InputType) {
////                        case "1": // Radio
////                            ////r.Append("<input " + IsChecked + " type='radio' id='" + a_OrderBy + "' name='" + InputName + "' value='" + a_0 + "' " +
////                            ////    "onclick=\"" + (UITreeFunc != "" ? UITreeFunc + ";" : "") + "SetValTree" + InputName + "('" + a_OrderBy + "', 'txt_" + InputName + "', '" + a_1 + "', '" + a_OrderBy + "', '" + a_0 + "')\">");
////                            ////ClassVal = "<label for='" + a_OrderBy + "'>" + ClassVal + "</label>";
////                            ////ClassVal = (IsClass? "<span class='" + (Tools.GetDataJson(d.UITree.Items[i], "Cnt", "int") > 0 ? "folder" : "sFile") + "'>" + ClassVal + "</span>" : ClassVal);
////                            ////r.Append(ClassVal);
////                            r.Append(Environment.NewLine + "<div class=\"form-check\">" +
////                                Environment.NewLine + " <input " + IsChecked + " type=\"radio\" " +
////                                "id=\"" + a_OrderBy + "\" " +
////                                "name=\"" + InputName + "\" " +
////                                "value=\"" + a_0 + "\" " +
////                                "onclick=\"" + (UITreeFunc != "" ? UITreeFunc + ";" : "") + "SetValTree" + InputName + "('" + a_OrderBy + "', 'txt_" + InputName + "', '" + a_1 + "', '" + a_OrderBy + "', '" + a_0 + "')\"" +
////                                "class=\"form-check-input\">" +
////                                (IsClass ? Environment.NewLine + "<span class='folder'><label class=\"form-check-label\" for=\"" + a_OrderBy + "\">" + ClassVal + "</label></span>" : //" + (Tools.GetDataJson(d.UITree.Items[i], "Cnt", "int") > 0 ? "folder" : "file") + "
////                                    Environment.NewLine + " <label class=\"form-check-label\" for=\"" + a_OrderBy + "\">" + ClassVal + "</label>") +
////                                Environment.NewLine + "</div>");
////                            // JS
////                            r.Append(Environment.NewLine + "<script>");
////                            r.Append(Environment.NewLine + "chkId" + InputName + "[" + i + "] = '" + a_OrderBy + "';");
////                            r.Append(Environment.NewLine + "chkValue" + InputName + "[" + i + "] = '" + a_0 + "';");
////                            r.Append(Environment.NewLine + "chkText" + InputName + "[" + i + "] = '" + a_1 + "';");
////                            r.Append(Environment.NewLine + "</script>");
////                            break;
////                        case "2": // Checkbox
////                            ////r.Append("<input " + IsChecked + "  type='checkbox' id='" + a_OrderBy + "' name='" + InputName + "' value='" + a_0 + "' " +
////                            ////    "onclick=\"" + (UITreeFunc != "" ? UITreeFunc + ";" : "") + "SetValTree" + InputName + "('" + a_OrderBy + "', 'txt_" + InputName + "', '" + a_1 + "', '" + a_OrderBy + "', '" + a_0 + "')\">");
////                            ////ClassVal = "<label for='" + a_OrderBy + "'>" + ClassVal + "</label>";
////                            ////ClassVal = (IsClass ? "<span class='" + (Tools.GetDataJson(d.UITree.Items[i], "Cnt", "int") > 0 ? "folder" : "sFile") + "'>" + ClassVal + "</span>" : ClassVal);
////                            ////r.Append(ClassVal);
////                            r.Append(Environment.NewLine + "<div class=\"form-check\">" +
////                                Environment.NewLine + " <input " + IsChecked + " type=\"checkbox\" " +
////                                "id=\"" + a_OrderBy + "\" " +
////                                "name=\"" + InputName + "\" " +
////                                "value=\"" + a_0 + "\" " +
////                                "onclick=\"" + (UITreeFunc != "" ? UITreeFunc + ";" : "") + "SetValTree" + InputName + "('" + a_OrderBy + "', 'txt_" + InputName + "', '" + a_1 + "', '" + a_OrderBy + "', '" + a_0 + "')\"" +
////                                "class=\"form-check-input\">" +
////                                (IsClass ? Environment.NewLine + "<span class='folder'><label class=\"form-check-label\" for=\"" + a_OrderBy + "\">" + ClassVal + "</label></span>" : //" + (Tools.GetDataJson(d.UITree.Items[i], "Cnt", "int") > 0 ? "folder" : "file") + "
////                                    Environment.NewLine + " <label class=\"form-check-label\" for=\"" + a_OrderBy + "\">" + ClassVal + "</label>") +
////                                Environment.NewLine + "</div>");
////                            // JS
////                            r.Append(Environment.NewLine + "<script>");
////                            r.Append(Environment.NewLine + "chkId" + InputName + "[" + i + "] = '" + a_OrderBy + "';");
////                            r.Append(Environment.NewLine + "chkValue" + InputName + "[" + i + "] = '" + a_0 + "';");
////                            r.Append(Environment.NewLine + "chkText" + InputName + "[" + i + "] = '" + a_1 + "';");
////                            r.Append(Environment.NewLine + "</script>");
////                            break;
////                        default: // URL
////                            if (IsClass)
////                                r.Append("<a href=\"javascript:" + UITreeFunc + "('" + a_0 + "', '" + a_OrderBy + "');\"><span class='" + (Tools.GetDataJson(d.UITree.Items[i], "Cnt", "int") > 0 ? "folder" : "sFile") + "'>" + val + "</span></a>");
////                            else
////                                r.Append("<a href=\"javascript:" + UITreeFunc + "('" + a_0 + "', '" + a_OrderBy + "');\">" + val + "</a>");
////                            break;
////                    }
////                    r.Append("</td>");
////                }
////                else
////                {
////                    switch (b[j].ToLower())
////                    {
////                        case "numeric":
////                            r.Append(Environment.NewLine + "<td class=\"text-right\">" + Tools.FormatNumber(val, 0));
////                            break;
////                        case "date":
////                            DateTime val1;
////                            try
////                            {
////                                val1 = DateTime.Parse(val);
////                                r.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMDate(val1));
////                            }
////                            catch
////                            {
////                                //val1 = DateTime.Now;
////                                r.Append(Environment.NewLine + "<td class=\"text-center\">");// + Tools.HRMDate(val1));
////                            }
////                            break;
////                        case "datetime":
////                            DateTime val2;
////                            try
////                            {
////                                val2 = DateTime.Parse(val);
////                                r.Append(Environment.NewLine + "<td class=\"text-center\">" + Tools.HRMDateTime(val2));
////                            }
////                            catch
////                            {
////                                //val2 = DateTime.Now;
////                                r.Append(Environment.NewLine + "<td class=\"text-center\">");// + Tools.HRMDateTime(val2));
////                            }
////                            break;
////                        default:
////                            r.Append(Environment.NewLine + "<td>" + val);
////                            break;
////                    }
////                    r.Append("</td>");
////                }
////            }
////        }

////        r.Append("</tr>");
////    }
////    if (txtValChk != "")
////    {
////        txtValChk = Tools.RemoveFisrtChar(txtValChk);
////    }
////    InputCheck = txtValChk;
////    r.Append("</tbody>");
////    r.Append(Environment.NewLine + "</table>");
////    if (Found != "") Found = Tools.RemoveFisrtChar(Found);

////    // return data
////    r1 = r.ToString(); r = null;
////    return r1;
////}

////public static string UITreeview(string placeholder, ToolDAO toolDAO, HRSContext context,
////    string InputName, string ProcName, string ProcParam, string ColumnName, string ColumnType, string InputCheck,
////    int Height = 200, int Width = 300, bool IsGrid = false, bool IsClass = false, bool IsInput = true, string InputType = "1", string UITreeFunc = "")
////{
////    StringBuilder r = new StringBuilder();
////    // JS
////    r.Append(Environment.NewLine + "<script>");
////    r.Append(Environment.NewLine + "var isChk" + InputName + " = ('" + InputType + "' == '2');");
////    r.Append(Environment.NewLine + "var chkId" + InputName + " = new Array();");
////    r.Append(Environment.NewLine + "var chkValue" + InputName + " = new Array();");
////    r.Append(Environment.NewLine + "var chkText" + InputName + " = new Array();");
////    r.Append(Environment.NewLine + "</script>");

////    string r1 = ""; string Found;
////    string sTree = Treeview(toolDAO, context, InputName, ProcName, ProcParam, ColumnName, ColumnType, ref InputCheck, out Found, IsGrid, IsClass, InputType, UITreeFunc);
////    if (IsInput)
////    {
////        //if(InputType == "2")r.Append("<input id=\"id_" + InputName + "\" name=\"" + InputName + "\" type=\"hidden\">");

////        r.Append(//Environment.NewLine + "<input id=\"txt_" + InputName + "\" name=\"txt_" + InputName + "\" size=\"25\" value=\"" + InputCheck + "\" readonly>" +
////            Environment.NewLine + "<div class=\"input-group\">" +
////            UITextbox(placeholder, "txt_" + InputName, InputCheck, "readonly", "", "") +
////            Environment.NewLine + "<span class=\"input-group-addon\">" +
////            "<a href=\"javascript:UITreeDiv('" + InputName + "_div');\"><i class=\"fa fa-window-restore\"></i></a></span>" +
////            //"<img src=\"/images/Icon_Select.gif\" Class=\"imggo\" id=\"" + InputName + "img\" onclick=\"UITreeDiv('" + InputName + "_div');\">" +
////            "</div>");// 
////        //style="width:600px; height:400px; display: block; overflow: auto;" class="datepicker datepicker-dropdown dropdown-menu datepicker-orient-left datepicker-orient-bottom"
////        r.Append(Environment.NewLine + "<div " +
////            "class=\"datepicker datepicker-dropdown dropdown-menu datepicker-orient-left datepicker-orient-bottom\" " +
////            "id=\"" + InputName + "_div\" " +
////            "style=\"width:600px; height:400px; display:none;\">");
////        r.Append(sTree);
////        r.Append("</div>");
////    }
////    else
////        r.Append(sTree);
////    // JS
////    r.Append(Environment.NewLine + "<script>");
////    r.Append(Environment.NewLine + "function UITreeDiv(b){var a = document.getElementById(b);if(a)if(a.style.display=='none') a.style.display='block'; else a.style.display='none';}");
////    r.Append(Environment.NewLine + "$(\"#Tree" + InputName + "\").treetable({ expandable: true" +
////    Environment.NewLine + "});");
////    string[] a1 = Found.Split(new string[] { "," }, StringSplitOptions.None);
////    string sUsed = ",,";
////    for (int i = 0; i < a1.Length; i++)
////    {
////        string[] b1 = a1[i].Split(new string[] { "-" }, StringSplitOptions.None);
////        r.Append(Environment.NewLine + "// a1[" + i +"]: " + a1[i]);
////        for (int j = 0; j < b1.Length; j++)
////        {
////            if (sUsed.IndexOf("," + b1[j] + ",") < 0)
////            {
////                sUsed = sUsed + "," + b1[j];
////                r.Append(Environment.NewLine + "// a1[" + i + "]: " + a1[i] + " ==> b1[" + j + "]: " + Tools.GetValId(b1, j));
////                r.Append(Environment.NewLine + "try{$(\"#Tree" + InputName + "\").treetable(\"expandNode\", \"" + Tools.GetValId(b1, j) + "\");}catch (ex) {}");
////            }
////        }
////    }

////    //r.Append(Environment.NewLine + "// ContextMenu");
////    //r.Append(Environment.NewLine + "$(function(){ $('#Tree" + InputName + "').contextMenu({ 	selector: 'tr',  	callback: function(key, options) { 		var m = \"clicked: \" + key + \" on \" + $(this).text(); 		window.console && console.log(m) || alert(m);  	}, 	items: { 		\"edit\": {name: \"Edit\", icon: \"edit\"}, 		\"cut\": {name: \"Cut\", icon: \"cut\"}, 		\"copy\": {name: \"Copy\", icon: \"copy\"}, 		\"paste\": {name: \"Paste\", icon: \"paste\"}, 		\"delete\": {name: \"Delete\", icon: \"delete\"}, 		\"sep1\": \"-------- - \", 		\"quit\": {name: \"Quit\", icon: function($element, key, item){ return 'context-menu-icon context-menu-icon-quit'; }} 	} });});");
////    r.Append(Environment.NewLine + "// Highlight selected row");
////    r.Append(Environment.NewLine + "$(\"#Tree" + InputName + " tbody\").on(\"mousedown\", \"tr\", function() { $(\".selected\").not(this).removeClass(\"selected\"); $(this).toggleClass(\"selected\"); });");
////    r.Append(Environment.NewLine + "// Drag & Drop Example Code");
////    //r.Append(Environment.NewLine + "$(\"#Tree" + InputName + " .sFile, #Tree" + InputName + " .folder\").draggable({ helper: \"clone\", opacity: .75, refreshPositions: true, revert: \"invalid\", revertDuration: 300, scroll: true });");
////    //r.Append(Environment.NewLine + "$(\"#Tree" + InputName + " .folder\").each(function() { $(this).parents(\"#Tree" + InputName + " tr\").droppable({ accept: \".sFile, .folder\", drop: function(e, ui) { var droppedEl = ui.draggable.parents(\"tr\");  $(\"#Tree" + InputName + "\").treetable(\"move\", droppedEl.data(\"ttId\"), $(this).data(\"ttId\")); }, hoverClass: \"accept\", over: function(e, ui) { var droppedEl = ui.draggable.parents(\"tr\"); if(this != droppedEl[0] && !$(this).is(\".expanded\")) { $(\"#Tree" + InputName + "\").treetable(\"expandNode\", $(this).data(\"ttId\")); } } }); });");
////    r.Append(Environment.NewLine + "function SetValTree" + InputName + "(chkId, name, nameval, id, idval){" +
////        Environment.NewLine + " var obj = document.getElementById(chkId); " +
////        Environment.NewLine + " if (!obj) return;" +
////        Environment.NewLine + " var strValId = '';" +
////        Environment.NewLine + " var strValTxt = '';" +
////        Environment.NewLine + " if (isChk" + InputName + "){" +
////        Environment.NewLine + "     for (var i = 0; i < chkId" + InputName + ".length; i++){" +
////        Environment.NewLine + "         if (chkId == chkId" + InputName + "[i] || chkId" + InputName + "[i].substr(0, chkId.length + 1) == chkId + '-'){" +
////        Environment.NewLine + "             if (obj.checked) {" +
////        Environment.NewLine + "                 var chki = document.getElementById(chkId" + InputName + "[i]);" +
////        Environment.NewLine + "                 if (chki) chki.checked = true;" +
////        Environment.NewLine + "             } else {" +
////        Environment.NewLine + "                 var chki = document.getElementById(chkId" + InputName + "[i]);" +
////        Environment.NewLine + "                 if (chki) chki.checked = false;" +
////        Environment.NewLine + "             }" +
////        Environment.NewLine + "         }" +
////        Environment.NewLine + "         var chkj = document.getElementById(chkId" + InputName + "[i]); " +
////        Environment.NewLine + "         if (chkj.checked){ " +
////        Environment.NewLine + "             strValId = strValId + ', ' + chkValue" + InputName + "[i]; " +
////        Environment.NewLine + "             strValTxt = strValTxt + ', ' + chkText" + InputName + "[i]; " +
////        Environment.NewLine + "         }" +
////        Environment.NewLine + "     }" +
////        Environment.NewLine + "     if (strValId.length > 2) strValId = strValId.substr(2); " +
////        Environment.NewLine + "     if (strValTxt.length > 2) strValTxt = strValTxt.substr(2); " +
////        Environment.NewLine + " } else if (obj.checked) {" +
////        Environment.NewLine + "     strValId = idval;" +
////        Environment.NewLine + "     strValTxt = nameval;" +
////        Environment.NewLine + " }" +
////        Environment.NewLine + " var a = document.getElementById(id);" +
////        Environment.NewLine + " if(a) a.value = strValId;" +
////        Environment.NewLine + " var b = document.getElementById(name);" +
////        Environment.NewLine + " if(b) b.value = strValTxt;" +
////        Environment.NewLine + "}");
////    r.Append(Environment.NewLine + "$(\"#Tree" + InputName + "\").treetable({ expandable: true" +
////    Environment.NewLine + "});");
////    r.Append("</script>");

////    // return data
////    r1 = r.ToString(); r = null;
////    return r1;
////}