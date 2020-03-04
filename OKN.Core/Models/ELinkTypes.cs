using System.Runtime.Serialization;

namespace OKN.Core.Models
{
    public enum ELinkTypes
    {
        [EnumMember(Value = "other")]
        Other = 0,
        [EnumMember(Value = "oldsaratov")]
        Oldsaratov = 1,
        [EnumMember(Value = "oldsaratov_wiki")]
        OldsaratovWiki = 2,
        [EnumMember(Value = "wikipedia")]
        Wikipedia = 3
    }
}