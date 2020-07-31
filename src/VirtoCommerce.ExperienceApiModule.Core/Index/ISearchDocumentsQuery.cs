namespace VirtoCommerce.ExperienceApiModule.Core.Index
{
    public interface ISearchDocumentsQuery : ISearchQuery, IHaveObjectIds, IHaveSearchParams
    {
    }

    public interface IGetAllDocumentsByIdsQuery : ISearchQuery, IHaveObjectIds
    {
    }

    public interface IGetSingleDocumentQuery : ISearchQuery, IHaveObjectId
    {
    }

    public interface ISearchQuery
    {
    }

    public interface IHaveObjectIds
    {
        string[] ObjectIds { get; set; }
    }

    public interface IHaveObjectId
    {
        string ObjectId { get; set; }
    }

    public interface IHaveSearchParams
    {
        string Query { get; set; }
        bool Fuzzy { get; set; }
        int? FuzzyLevel { get; set; }
        string Filter { get; set; }
        string Facet { get; set; }
        string Sort { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
    }
}
