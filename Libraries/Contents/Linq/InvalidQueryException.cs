using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents.Linq
{
    [Serializable]
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class InvalidQueryException : Exception
    {
        private string message;

        public InvalidQueryException(string message)
        {
            this.message = message + " ";
        }

        public override string Message
        {
            get
            {
                return "The client query is invalid: " + message;
            }
        }
    }
}
