using System ;
using System.Text.Json.Serialization;

namespace OrderCaller
{

public class Order
{
    [JsonPropertyName("id")] // obs check case of JSON names compared to propertyname !
    public string Id { get;set; }

    [JsonPropertyName("createdTime")]
    public DateTime CreatedTime {get; set;} 

    [JsonPropertyName("orderDescription")]
    public string OrderDescription {get; set;}

    [JsonPropertyName("isCompleted")]
    public bool IsCompleted {get; set;}

}


}

