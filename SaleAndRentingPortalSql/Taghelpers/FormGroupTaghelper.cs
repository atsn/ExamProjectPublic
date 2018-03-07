using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SaleAndRentingPortalSql.Taghelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("FormGroup",Attributes = "Attribute")]
    public class FormGroupTaghelper : TagHelper
    {
        object Attribute { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.PostContent.AppendHtml("<div class='form-group'>");
               output.PostContent.AppendHtml("<label asp-for'"+Attribute+"'></label>'");
               output.PostContent.AppendHtml("<input asp-for='" + Attribute + "' class='form-control'/>");
               output.PostContent.AppendHtml("<span asp-validation-for='" + Attribute + "'class='text-danger'></span>");
             output.PostContent.AppendHtml("</div>");
        }
    }
}
