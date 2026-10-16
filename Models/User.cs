namespace auth35.Models;

public class User
{
    public int Id {get;set;}
    public string? UserName{get;set;}
    public string? Password{get;set;}
    public int Solved{get;set;}
    public int Attempt{get;set;}
}