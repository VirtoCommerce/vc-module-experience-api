using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations
{
    public class LiquidContentRenderer : IContentRenderer
    {
        public async Task<string> RenderAsync(string template, object contextObj)
        {
            var scribanContext = new LiquidTemplateContext()
            {
                EnableRelaxedMemberAccess = true,
                NewLine = Environment.NewLine,
                TemplateLoaderLexerOptions = new LexerOptions
                {
                    Mode = ScriptMode.Liquid
                }
            };
            var scriptObject = new ScriptObject();
            scriptObject.Import(contextObj);
            scribanContext.PushGlobal(scriptObject);

            var parsedTemplate = Template.ParseLiquid(template);
            return await parsedTemplate.RenderAsync(scribanContext);
        }
    }
}
