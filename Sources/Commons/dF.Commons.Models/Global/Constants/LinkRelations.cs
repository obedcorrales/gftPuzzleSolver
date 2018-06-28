namespace dF.Commons.Models.Global.Constants
{
    public static class LinkRelations
    {
        public static class Collections
        {
            public const string Rel = "Collections";

            public static class Actions
            {
                public const string GoToFirstPage = "goToFirstPage";
                public const string GoToPreviousPage = "goToPreviousPage";
                public const string GoToNextPage = "goToNextPage";
                public const string GoToLastPage = "goToLastPage";
                public const string Search = "search";
            }
        }

        public static class Item
        {
            //public const string Rel = "Item";

            public static class Actions
            {
                public const string GetCollection = "getCollection";
                public const string CreateRelatedItem = "createRelatedItem";
                public const string AddRelatedItem = "addRelatedItem";
                public const string UpdateRelatedItem = "updateRelatedItem";
                public const string RemoveRelatedItem = "removeRelatedItem";
                public const string UpdateItem = "updateItem";
                public const string DeleteItem = "deleteItem";
            }
        }

        public static class Info
        {
            public const string Rel = "Info";

            public static class Actions
            {
                public const string GetHelp = "getHelp";
                public const string GetLicense = "getLicense";
                public const string GetCopyright = "getCopyright";
                public const string GetTermsOfService = "getTermsOfService";
            }
        }

        public static class Navigation
        {
            public const string Rel = "Nav";

            public static class Actions
            {
                public const string Origen = "origen";
                public const string Destination = "destination";
            }
        }

        public static class Workflow
        {
            public const string Rel = "Workflow";

            public static class Actions
            {
                public const string GoToPreviousStep = "goToPreviousStep";
                public const string GoToNextStep = "goToNextStep";
                public const string Restart = "restart";
                public const string Cancel = "cancel";
                public const string Save = "save";
                public const string Terminate = "terminate";
                public const string GoToStep = "goToStep";
            }
        }
    }
}
