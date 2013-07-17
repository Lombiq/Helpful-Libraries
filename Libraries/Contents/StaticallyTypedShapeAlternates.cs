// This isn't working yet, see: https://orchard.codeplex.com/workitem/19479
// Commenting out so nobody sees it with Shape Tracing.

//using System.IO;
//using Orchard.ContentManagement;
//using Orchard.DisplayManagement.Implementation;
//using Orchard.Environment.Extensions;

//namespace Piedone.HelpfulLibraries.Contents
//{
//    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
//    public class StaticallyTypedShapeAlternates : IShapeDisplayEvents
//    {
        
//        public void Displaying(ShapeDisplayingContext context)
//        {
//            var shape = context.Shape;
//            if (shape.TemplateName != null)
//            {
//                var alternates = shape.Metadata.Alternates;
//                var fileName = Path.GetFileName((string)shape.TemplateName);

//                // E.g. for EditorTemplates/Parts.Title.TitlePart this will create an alternate with the same path.
//                var fullPath = shape.Metadata.Type + "s/" + shape.TemplateName;
//                alternates.Add(fullPath);
//                alternates.Add("DisplayTemplate-" + fileName);

//                if (shape.ContentItem != null)
//                {
//                    var contentType = ((ContentItem)shape.ContentItem).ContentType;
//                    alternates.Add(fullPath + "-" + contentType);
//                    alternates.Add("DisplayTemplate-" + fileName + "-" + contentType);
//                }
//            }
//        }

//        public void Displayed(ShapeDisplayedContext context)
//        {
//        }
//    }
//}
