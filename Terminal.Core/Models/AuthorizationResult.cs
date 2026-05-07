using Terminal.Core.Enums;

namespace Terminal.Core.Models;

public class AuthorizationResult
{
    public bool Success { get; set; }
    public AuthorizeFlags Flags { get; set; }
    public PosFlags PosFlags { get; set; }
    public byte[]? Certificate { get; set; }
    public DateTime? ServerTime { get; set; }
    public byte ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}