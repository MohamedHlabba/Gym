using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Gym.Web.TagHelpers
{
    [HtmlTargetElement("booked")]
    public class IconBooked : TagHelper
    {

        public bool IsBooked { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.AddClass("booked", HtmlEncoder.Default);

            var booked = IsBooked;
            var book = "/images/new.png";
            var cancel = "/images/cross.png";


            var result = (booked == true) ? $"<img src='{cancel}'/>" : $"<img src='{book}'/>";

            output.Content.SetHtmlContent(result);

        }
    }
}
