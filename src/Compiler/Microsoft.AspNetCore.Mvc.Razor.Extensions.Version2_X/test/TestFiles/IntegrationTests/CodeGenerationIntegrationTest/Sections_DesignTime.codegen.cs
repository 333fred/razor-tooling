﻿// <auto-generated/>
#pragma warning disable 1591
namespace AspNetCore
{
    #line hidden
    using TModel = global::System.Object;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    public class TestFiles_IntegrationTests_CodeGenerationIntegrationTest_Sections : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DateTime>
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::InputTestTagHelper __InputTestTagHelper;
        #pragma warning disable 219
        private void __RazorDirectiveTokenHelpers__() {
        ((System.Action)(() => {
#line 1 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Sections.cshtml"
DateTime __typeHelper = default;

#line default
#line hidden
        }
        ))();
        ((System.Action)(() => {
#line 3 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Sections.cshtml"
global::System.Object __typeHelper = "InputTestTagHelper, AppCode";

#line default
#line hidden
        }
        ))();
        ((System.Action)(() => {
#line 11 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Sections.cshtml"
global::System.Object Section1 = null;

#line default
#line hidden
        }
        ))();
        }
        #pragma warning restore 219
        #pragma warning disable 0414
        private static object __o = null;
        #pragma warning restore 0414
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 5 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Sections.cshtml"
   
    Layout = "_SectionTestLayout.cshtml";

#line default
#line hidden
            DefineSection("Section1", async(__razor_section_writer) => {
                __InputTestTagHelper = CreateTagHelper<global::InputTestTagHelper>();
#line 13 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/Sections.cshtml"
__InputTestTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Date);

#line default
#line hidden
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DateTime> Html { get; private set; }
    }
}
#pragma warning restore 1591
