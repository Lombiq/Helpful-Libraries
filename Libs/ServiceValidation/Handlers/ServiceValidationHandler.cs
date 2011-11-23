//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Orchard.ContentManagement.Handlers;
//using Autofac;
//using Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries;

//namespace Piedone.HelpfulLibraries.ServiceValidation.Handlers
//{
//    public abstract class ServiceValidationHandler : ContentHandler
//    {
//        public ServiceValidationHandler(IComponentContext componentContext)
//        {
//            // This is necessary as generic dependencies are currently not resolved, see issue: http://orchard.codeplex.com/workitem/18141
//            var builder = new ContainerBuilder();
//            builder.RegisterGeneric(typeof(ServiceValidationDictionary<>)).As(typeof(IServiceValidationDictionary<>));

//            builder.Update(componentContext.ComponentRegistry);
//        }
//    }
//}