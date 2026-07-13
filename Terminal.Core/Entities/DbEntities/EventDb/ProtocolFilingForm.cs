using Terminal.Core.Enums;

namespace Terminal.Core.Entities.DbEntities.EventDb;

public class ProtocolFilingForm
{
    public int ProtokolFillingFormKey { get; set; }

    public int? SncProjectKey { get; set; }

    public int? LogCode { get; set; }

    public long? PlaceId { get; set; }

    public SnkObjectType? SubjectType { get; set; }

    public int? SubjectId { get; set; }

    public SnkObjectType? ObjectType { get; set; }

    public long? ObjectId { get; set; }

    public int? EventCode { get; set; }

    public long? EventKey { get; set; }

    public double? EventValue { get; set; }

    public string? EventInfo { get; set; }

    public DateTime? EventDatetime { get; set; }

    public double? LatestObjectParameterValue { get; set; }

    public double? CurrentObjectParameterValue { get; set; }

    public string? Hash { get; set; }

    public int? ErrorCode { get; set; }
}
