﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Signum.Web
{
    public static class CalendarHelper
    {
        //jQuery ui DatePicker
        public static string Calendar(this HtmlHelper helper, string elementId)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append(helper.ScriptInclude(helper.CombinedJsUrlPath("Scripts/jqueryui", "ui.core.js", "ui.datepicker.js", "i18n/ui.datepicker-es.js")));
            sb.Append(helper.ScriptInclude("Scripts/jqueryui/ui.core.js",
                "Scripts/jqueryui/ui.datepicker.js",
                "Scripts/jqueryui/i18n/ui.datepicker-es.js"));
            
            //sb.AppendLine(helper.DynamicCssInclude(helper.CombinedCssUrlPath("Scripts/jqueryui", "ui.all.css", "ui.base.css", "ui.core.css", "ui.datepicker.css", "ui.theme.css")));

            sb.AppendLine(helper.DynamicCssInclude("Scripts/jqueryui/ui.all.css",
                "Scripts/jqueryui/ui.base.css",
                "Scripts/jqueryui/ui.core.css",
                "Scripts/jqueryui/ui.datepicker.css",
                "Scripts/jqueryui/ui.theme.css"));

            sb.Append(
                "<script type=\"text/javascript\">\n" + 
                "$(document).ready(function(){\n" +
                "$(\"#" + elementId + "\").datepicker({ changeMonth:true, changeYear:true, firstDay:1, yearRange:'-90:+10', showOn:'button', buttonImageOnly:true, buttonText:'mostrar calendario', buttonImage:'Scripts/jqueryui/images/calendar.png' });\n" + 
                "});\n" + 
                "</script>\n");

            return sb.ToString();
        }

        //Ajax control toolkit calendar
        public static string CalendarAjaxControlToolkit(this HtmlHelper helper, string elementId)
        {
            var sb = new StringBuilder();

            // Add Microsoft Ajax library   
            sb.AppendLine(helper.MicrosoftAjaxLibraryInclude());

            // Add toolkit scripts   
            sb.AppendLine(helper.ToolkitInclude
                (
                    "AjaxControlToolkit.ExtenderBase.BaseScripts.js",
                    "AjaxControlToolkit.Common.Common.js",
                    "AjaxControlToolkit.Common.DateTime.js",
                    "AjaxControlToolkit.Animation.Animations.js",
                    "AjaxControlToolkit.PopupExtender.PopupBehavior.js",
                    "AjaxControlToolkit.Animation.AnimationBehavior.js",
                    "AjaxControlToolkit.Common.Threading.js",
                    "AjaxControlToolkit.Compat.Timer.Timer.js",
                    "AjaxControlToolkit.Calendar.CalendarBehavior.js"
                ));

            // Add Calendar CSS file   
            sb.AppendLine(helper.DynamicToolkitCssInclude("AjaxControlToolkit.Calendar.Calendar.css"));

            // Perform $create   
            sb.AppendLine(helper.Create("AjaxControlToolkit.CalendarBehavior", "{\"format\":\"dd/MM/yyyy\"}", elementId));

            return sb.ToString();
        }
    }
}
