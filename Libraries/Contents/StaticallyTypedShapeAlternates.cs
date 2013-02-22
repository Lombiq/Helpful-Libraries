using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.DisplayManagement.Implementation;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class StaticallyTypedShapeAlternates : IShapeDisplayEvents
    {
        public void Displaying(ShapeDisplayingContext context)
        {
            var shape = context.Shape;
            if (shape.TemplateName != null)
            {
                var alternates = shape.Metadata.Alternates;
                var fileName = Path.GetFileName((string)shape.TemplateName);

                // E.g. for EditorTemplates/Parts.Title.TitlePart this will create an alternate with the same path.
                var fullPath = shape.Metadata.Type + "s/" + shape.TemplateName;
                alternates.Add(fullPath);
                alternates.Add("DisplayTemplate-" + fileName);

                if (shape.ContentItem != null)
                {
                    var contentType = ((ContentItem)shape.ContentItem).ContentType;
                    alternates.Add(fullPath + "-" + contentType);
                    alternates.Add("DisplayTemplate-" + fileName + "-" + contentType);
                }
            }
        }

        public void Displayed(ShapeDisplayedContext context)
        {
        }
    }
}
