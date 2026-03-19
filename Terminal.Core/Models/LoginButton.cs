using Terminal.Core.Enums;

namespace Terminal.Core.Models;

public record struct LoginButton
{
    public string Content { get; set; }
    
    public bool ContentIsImage { get; set; }
    
    public LoginButtonTypes Type { get; set; }
}